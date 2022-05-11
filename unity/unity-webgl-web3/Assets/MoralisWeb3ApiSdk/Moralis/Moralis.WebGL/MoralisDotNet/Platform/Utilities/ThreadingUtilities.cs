using System;

namespace Moralis.WebGL.Platform.Utilities
{
    internal static class ThreadingUtilities
    {
        public static void Lock(ref object @lock, Action operationToLock)
        {
            lock (@lock)
                operationToLock();
        }

        public static TResult Lock<TResult>(ref object @lock, Func<TResult> operationToLock)
        {
            TResult result = default;
            lock (@lock)
                result = operationToLock();

            return result;
        }
    }
}
