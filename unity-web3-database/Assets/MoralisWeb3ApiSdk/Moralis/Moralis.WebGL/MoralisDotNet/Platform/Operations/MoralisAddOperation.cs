using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Moralis.WebGL.Platform.Abstractions;
using Moralis.WebGL.Platform.Utilities;

namespace Moralis.WebGL.Platform.Operations
{
    class MoralisAddOperation : IMoralisFieldOperation
    {
        ReadOnlyCollection<object> Data { get; }

        public string __op { get { return "Add"; } }

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
