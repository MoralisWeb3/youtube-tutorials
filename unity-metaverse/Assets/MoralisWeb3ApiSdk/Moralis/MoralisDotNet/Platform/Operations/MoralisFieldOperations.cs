using System;
using System.Collections.Generic;
using Moralis.Platform.Abstractions;
using Moralis.Platform.Objects;

namespace Moralis.Platform.Operations
{
    public class MoralisObjectIdComparer : IEqualityComparer<object>
    {
        bool IEqualityComparer<object>.Equals(object p1, object p2)
        {
            MoralisObject moralisObj1 = p1 as MoralisObject;
            MoralisObject moralisObj2 = p2 as MoralisObject;
            if (moralisObj1 != null && moralisObj1 != null)
            {
                return Equals(moralisObj1.objectId, moralisObj1.objectId);
            }
            return Equals(p1, p2);
        }

        public int GetHashCode(object p)
        {
            MoralisObject moralisObject = p as MoralisObject;
            if (moralisObject != null)
            {
                return moralisObject.objectId.GetHashCode();
            }
            return p.GetHashCode();
        }
    }

    static class MoralisFieldOperations
    {
        private static MoralisObjectIdComparer comparer;

        public static IMoralisFieldOperation Decode(IDictionary<string, object> json) => throw new NotImplementedException();

        public static IEqualityComparer<object> MoralisObjectComparer
        {
            get
            {
                if (comparer == null)
                {
                    comparer = new MoralisObjectIdComparer();
                }
                return comparer;
            }
        }
    }
}
