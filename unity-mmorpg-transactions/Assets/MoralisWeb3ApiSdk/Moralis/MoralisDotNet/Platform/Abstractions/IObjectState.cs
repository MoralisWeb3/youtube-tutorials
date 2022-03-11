using System;
using System.Collections.Generic;
using System.Text;
using Moralis.Platform.Queries;

namespace Moralis.Platform.Abstractions
{
    public interface IObjectState : IEnumerable<KeyValuePair<string, object>>
    {
        bool IsNew { get; }
        object this[string key] { get; }

        bool ContainsKey(string key);

        IObjectState MutatedClone(Action<MutableObjectState> func);
    }
}
