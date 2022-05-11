using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Moralis.Platform.Abstractions;
using Moralis.Platform.Objects;
using Moralis.Platform.Utilities;
namespace Moralis.Platform.Operations
{
    class MoralisUniqueAddOperation : IMoralisFieldOperation
    {
        ReadOnlyCollection<object> Data { get; }

        public string __op { get { return "AddUnique"; } }

        public IEnumerable<object> objects => Data;
        public MoralisUniqueAddOperation(IEnumerable<object> objects) => Data = new ReadOnlyCollection<object>(objects.ToList());

        public IMoralisFieldOperation MergeWithPrevious(IMoralisFieldOperation previous) => previous switch
        {
            null => this,
            MoralisDeleteOperation { } => new MoralisSetOperation(Data.ToList()),
            MoralisSetOperation { } setOp => new MoralisSetOperation(Conversion.To<IList<object>>(setOp.Value).Concat(Data).ToList()),
            MoralisAddOperation { } addition => new MoralisAddOperation(addition.objects.Concat(Data)),
            _ => throw new InvalidOperationException("Operation is invalid after previous operation.")
        };

        public object Apply(object oldValue, string key)
        {
            if (oldValue == null)
            {
                return Data.ToList();
            }

            List<object> result = Conversion.To<IList<object>>(oldValue).ToList();
            IEqualityComparer<object> comparer = MoralisFieldOperations.MoralisObjectComparer;

            foreach (object target in Data)
            {
                if (target is MoralisObject)
                {
                    if (!(result.FirstOrDefault(reference => comparer.Equals(target, reference)) is { } matched))
                    {
                        result.Add(target);
                    }
                    else
                    {
                        result[result.IndexOf(matched)] = target;
                    }
                }
                else if (!result.Contains(target, comparer))
                {
                    result.Add(target);
                }
            }

            return result;
        }
    }
}
