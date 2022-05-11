using System.Threading.Tasks;
using UnityEngine;

namespace DefaultNamespace
{
    public class WaitForTaskResult<T> : CustomYieldInstruction
    {
        private T Result
        {
            get { return source.Result; }
        }
        private Task<T> source;

        public Task<T> Source
        {
            get { return source; }
        }

        public override bool keepWaiting
        {
            get { return !source.IsCompleted && !source.IsFaulted && !source.IsCanceled; }
        }
        
        public WaitForTaskResult(Task<T> task)
        {
            this.source = task;
        }
    }
}