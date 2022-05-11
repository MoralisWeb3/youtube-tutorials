
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace Moralis.Platform.Utilities
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
        public static Task<T> Safe<T>(this Task<T> task) => task ?? Task.FromResult(default(T));

        /// <summary>
        /// Ensures a task (even null) is awaitable.
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public static Task Safe(this Task task) => task ?? Task.FromResult<object>(null);

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

        public static Task<TResult> OnSuccess<TIn, TResult>(this Task<TIn> task, Func<Task<TIn>, TResult> continuation) => ((Task) task).OnSuccess(t => continuation((Task<TIn>) t));

        public static Task OnSuccess<TIn>(this Task<TIn> task, Action<Task<TIn>> continuation) => task.OnSuccess((Func<Task<TIn>, object>) (t =>
                                                                                                             {
                                                                                                                 continuation(t);
                                                                                                                 return null;
                                                                                                             }));

        public static Task<TResult> OnSuccess<TResult>(this Task task, Func<Task, TResult> continuation) => task.ContinueWith(t =>
                                                           {
                                                               if (t.IsFaulted)
                                                               {
                                                                   AggregateException ex = t.Exception.Flatten();
                                                                   if (ex.InnerExceptions.Count == 1)
                                                                       ExceptionDispatchInfo.Capture(ex.InnerExceptions[0]).Throw();
                                                                   else
                                                                       ExceptionDispatchInfo.Capture(ex).Throw();
                                                                   // Unreachable
                                                                   return Task.FromResult(default(TResult));
                                                               }
                                                               else if (t.IsCanceled)
                                                               {
                                                                   TaskCompletionSource<TResult> tcs = new TaskCompletionSource<TResult>();
                                                                   tcs.SetCanceled();
                                                                   return tcs.Task;
                                                               }
                                                               else
                                                                   return Task.FromResult(continuation(t));
                                                           }).Unwrap();

        public static Task OnSuccess(this Task task, Action<Task> continuation) => task.OnSuccess((Func<Task, object>) (t =>
        {
            continuation(t);
            return null;
        }));

        public static Task WhileAsync(Func<Task<bool>> predicate, Func<Task> body)
        {
            Func<Task> iterate = null;
            iterate = () => predicate().OnSuccess(t =>
                {
                    if (!t.Result)
                        return Task.FromResult(0);
                    return body().OnSuccess(_ => iterate()).Unwrap();
                }).Unwrap();
            return iterate();
        }
    }
}
