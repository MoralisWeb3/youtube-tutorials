
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using Cysharp.Threading.Tasks;

namespace Moralis.WebGL.Platform.Utilities
{
    /// <summary>
    /// Provides helper methods that allow us to use terser code elsewhere.
    /// </summary>
    public static class InternalExtensions
    {
        /// <summary>
        /// Ensures a task (even null) is awaitable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <returns></returns>
        public static UniTask<T> Safe<T>(this UniTask<T> task)
        {
            if (task is { })
                return task;
            else
                return UniTask.FromResult(default(T));
        }

        /// <summary>
        /// Ensures a task (even null) is awaitable.
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public static UniTask Safe(this UniTask task)
        {

            if (task is { })
                return task;
            else
                return UniTask.FromResult<object>(null);
        }

        public delegate void PartialAccessor<T>(ref T arg);

        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> self,
            TKey key,
            TValue defaultValue)
        {
            if (self.TryGetValue(key, out TValue value))
                return value;
            return defaultValue;
        }

        public static bool CollectionsEqual<T>(this IEnumerable<T> a, IEnumerable<T> b) => Equals(a, b) ||
                   a != null && b != null &&
                   a.SequenceEqual(b);

       // public static UniTask<TResult> OnSuccess<TIn, TResult>(this UniTask<TIn> task, Func<UniTask<TIn>, TResult> continuation) => ((UniTask) task).OnSuccess(t => continuation((UniTask<TIn>) t));
        //public static UniTask<TResult> OnSuccess<TIn, TResult>(this UniTask<TIn> task, Func<UniTask<TIn>, TResult> continuation) => task.OnSuccess(t => continuation((UniTask<TIn>)t));

        //public static UniTask OnSuccess<TIn>(this UniTask<TIn> task, Action<UniTask<TIn>> continuation) => task.OnSuccess((Func<UniTask<TIn>, object>) (t =>
        //                                                                                                     {
        //                                                                                                         continuation(t);
        //                                                                                                         return null;
        //                                                                                                     }));

        //public static UniTask<TResult> OnSuccess<TResult>(this UniTask task, Func<UniTask, TResult> continuation) //=> task.ContinueWith(t =>
        //{
        //    if (task.Status.Equals(UniTaskStatus.Faulted))
        //    {
        //        //AggregateException ex = t.Exception.Flatten();
        //        //if (ex.InnerExceptions.Count == 1)
        //        //    ExceptionDispatchInfo.Capture(ex.InnerExceptions[0]).Throw();
        //        //else
        //        //    ExceptionDispatchInfo.Capture(ex).Throw();
        //        //// Unreachable
        //        return UniTask.FromResult(default(TResult));
        //    }
        //    else if (task.Status.Equals(UniTaskStatus.Canceled))
        //    {
        //        UniTaskCompletionSource<TResult> tcs = new UniTaskCompletionSource<TResult>();
        //        tcs.TrySetCanceled();
        //        return tcs.Task;
        //    }
        //    else
        //        return UniTask.FromResult(continuation(task));
        //}

 

        //public static UniTask OnSuccess(this UniTask task, Action<UniTask> continuation) => task.OnSuccess((Func<UniTask, object>) (t =>
        //{
        //    continuation(t);
        //    return null;
        //}));

        //public static UniTask WhileAsync(Func<UniTask<bool>> predicate, Func<UniTask> body)
        //{
        //    Func<UniTask> iterate = null;

        //    iterate = () => predicate().ContinueWith( t1 =>
        //    {
        //        if (!t1)
        //        {
        //            return UniTask.FromResult(0);
        //        }
        //        else
        //        {
        //            return body().ContinueWith()
        //        }
        //    }
        //    //iterate = () => predicate().OnSuccess(t =>
        //    //    {
        //    //        if (!t.GetAwaiter().GetResult())
        //    //            return UniTask.FromResult(0);
        //    //        return body().OnSuccess(_ => iterate()).Unwrap();
        //    //    }).Unwrap();
        //    return iterate();
        //}

        public static async void Wait<T>(this UniTask<T> task) //=> task.ContinueWith(t =>
        {
            if (UniTaskStatus.Pending.Equals(task.Status))
            {
                await task;
            }
        }

        public static async UniTask<T> Result<T>(this UniTask<T> task)
        {
            UniTask<T> result = default;

            if (UniTaskStatus.Pending.Equals(task.Status))
                await task;

            if (!task.Status.Equals(UniTaskStatus.Faulted) && !task.Status.Equals(UniTaskStatus.Canceled))
            {
                result = task;
            }

            return result.GetAwaiter().GetResult();
        }
    }
}
