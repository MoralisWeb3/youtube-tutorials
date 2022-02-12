using System;
using System.Collections.Generic;
using System.Linq;
using Moralis.Platform.Abstractions;
using Moralis.Platform.Operations;

namespace Moralis.Platform.Queries
{
    public class MutableObjectState : IObjectState
    {
        public bool IsNew { get; set; }
        public IDictionary<string, object> ServerData { get; set; } = new Dictionary<string, object> { };

        public object this[string key] => ServerData[key];

        public bool ContainsKey(string key) => ServerData.ContainsKey(key);

        public void Apply(IDictionary<string, IMoralisFieldOperation> operationSet)
        {
            // Apply operationSet
            foreach (KeyValuePair<string, IMoralisFieldOperation> pair in operationSet)
            {
                ServerData.TryGetValue(pair.Key, out object oldValue);
                object newValue = pair.Value.Apply(oldValue, pair.Key);
                if (newValue != MoralisDeleteOperation.Token)
                    ServerData[pair.Key] = newValue;
                else
                    ServerData.Remove(pair.Key);
            }
        }

        public void Apply(IObjectState other)
        {
            IsNew = other.IsNew;

            foreach (KeyValuePair<string, object> pair in other)
                ServerData[pair.Key] = pair.Value;
        }

        public IObjectState MutatedClone(Action<MutableObjectState> func)
        {
            MutableObjectState clone = MutableClone();
            func(clone);
            return clone;
        }

        protected virtual MutableObjectState MutableClone() => new MutableObjectState
        {
            IsNew = IsNew,
            ServerData = this.ToDictionary(t => t.Key, t => t.Value)
        };

        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator() => ServerData.GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => ((IEnumerable<KeyValuePair<string, object>>)this).GetEnumerator();

    }
}
