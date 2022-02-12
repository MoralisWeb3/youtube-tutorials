using System;
using System.Threading;
using System.Threading.Tasks;

namespace Moralis.Platform.Utilities
{
    /// <summary>
    /// A helper class for enqueuing tasks
    /// </summary>
    public class TaskQueue
    {
        /// <summary>
        /// We only need to keep the tail of the queue. Cancelled tasks will
        /// just complete normally/immediately when their turn arrives.
        /// </summary>
        Task Tail { get; set; }

        /// <summary>
        /// Gets a cancellable task that can be safely awaited and is dependent
        /// on the current tail of the queue. This essentially gives us a proxy
        /// for the tail end of the queue whose awaiting can be cancelled.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that cancels
        /// the task even if the task is still in the queue. This allows the
        /// running task to return immediately without breaking the dependency
        /// chain. It also ensures that errors do not propagate.</param>
        /// <returns>A new task that should be awaited by enqueued tasks.</returns>
        private Task GetTaskToAwait(CancellationToken cancellationToken)
        {
            lock (Mutex)
            {
                return (Tail ?? Task.FromResult(true)).ContinueWith(task => { }, cancellationToken);
            }
        }

        /// <summary>
        /// Enqueues a task created by <paramref name="taskStart"/>. If the task is
        /// cancellable (or should be able to be cancelled while it is waiting in the
        /// queue), pass a cancellationToken.
        /// </summary>
        /// <typeparam name="T">The type of task.</typeparam>
        /// <param name="taskStart">A function given a task to await once state is
        /// snapshotted (e.g. after capturing session tokens at the time of the save call).
        /// Awaiting this task will wait for the created task's turn in the queue.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to
        /// cancel waiting in the queue.</param>
        /// <returns>The task created by the taskStart function.</returns>
        public T Enqueue<T>(Func<Task, T> taskStart, CancellationToken cancellationToken = default) where T : Task
        {
            Task oldTail;
            T task;

            lock (Mutex)
            {
                oldTail = Tail ?? Task.FromResult(true);

                // The task created by taskStart is responsible for waiting the
                // task passed to it before doing its work (this gives it an opportunity
                // to do startup work or save state before waiting for its turn in the queue
                task = taskStart(GetTaskToAwait(cancellationToken));

                // The tail task should be dependent on the old tail as well as the newly-created
                // task. This prevents cancellation of the new task from causing the queue to run
                // out of order.
                Tail = Task.WhenAll(oldTail, task);
            }
            return task;
        }

        public object Mutex { get; } = new object { };
    }
}
