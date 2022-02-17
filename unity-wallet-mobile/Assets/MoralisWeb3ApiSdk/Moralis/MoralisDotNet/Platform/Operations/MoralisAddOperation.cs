using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Moralis.Platform.Abstractions;
using Moralis.Platform.Utilities;

namespace Moralis.Platform.Operations
{
    class MoralisAddOperation : IMoralisFieldOperation
    {
        ReadOnlyCollection<object> Data { get; }

        //[JsonProperty("__op")]
        public string __op { get { return "Add"; } }

        //[JsonProperty("objects")]
        public IEnumerable<object> objects => Data;
        public MoralisAddOperation(IEnumerable<object> objects) => Data = new ReadOnlyCollection<object>(objects.ToList());

        public IMoralisFieldOperation MergeWithPrevious(IMoralisFieldOperation previous) => previous switch
        {
            null => this,
            MoralisDeleteOperation { } => new MoralisSetOperation(Data.ToList()),
            MoralisSetOperation { } setOp => new MoralisSetOperation(Conversion.To<IList<object>>(setOp.Value).Concat(Data).ToList()),
            MoralisAddOperation { } addition => new MoralisAddOperation(addition.objects.Concat(Data)),
            _ => throw new InvalidOperationException("Operation is invalid after previous operation.")
        };

        public object Apply(object oldValue, string key) => oldValue is { } ? Conversion.To<IList<object>>(oldValue).Concat(Data).ToList() : Data.ToList();

        
    }
}
