using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Moralis.Platform.Utilities
{
    /// <summary>
    /// Represents an event handler that calls back from the synchronization context
    /// that subscribed.
    /// <typeparam name="T">Should look like an EventArgs, but may not inherit EventArgs if T is implemented by the Windows team.</typeparam>
    /// </summary>
    public class SynchronizedEventHandler<T>
    {
        LinkedList<Tuple<Delegate, TaskFactory>> Callbacks { get; } = new LinkedList<Tuple<Delegate, TaskFactory>> { };

        public void Add(Delegate target)
        {
            lock (Callbacks)
            {
                TaskFactory factory = SynchronizationContext.Current is { } ? new TaskFactory(CancellationToken.None, TaskCreationOptions.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.FromCurrentSynchronizationContext()) : Task.Factory;

                foreach (Delegate invocation in target.GetInvocationList())
                {
                    Callbacks.AddLast(new Tuple<Delegate, TaskFactory>(invocation, factory));
                }
            }
        }

        public void Remove(Delegate target)
        {
            lock (Callbacks)
            {
                if (Callbacks.Count == 0)
                {
                    return;
                }

                foreach (Delegate invocation in target.GetInvocationList())
                {
                    LinkedListNode<Tuple<Delegate, TaskFactory>> node = Callbacks.First;

                    while (node != null)
                    {
                        if (node.Value.Item1 == invocation)
                        {
                            Callbacks.Remove(node);
                            break;
                        }
                        node = node.Next;
                    }
                }
            }
        }

        public Task Invoke(object sender, T args)
        {
            IEnumerable<Tuple<Delegate, TaskFactory>> toInvoke;
            Task<int>[] toContinue = new[] { Task.FromResult(0) };

            lock (Callbacks)
            {
                toInvoke = Callbacks.ToList();
            }

            List<Task<object>> invocations = toInvoke.Select(callback => callback.Item2.ContinueWhenAll(toContinue, _ => callback.Item1.DynamicInvoke(sender, args))).ToList();
            return Task.WhenAll(invocations);
        }
    }
}
