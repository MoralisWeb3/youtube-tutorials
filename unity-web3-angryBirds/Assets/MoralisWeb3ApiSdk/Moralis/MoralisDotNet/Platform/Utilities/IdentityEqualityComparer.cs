
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Moralis.Platform.Utilities
{
    /// <summary>
    /// An equality comparer that uses the object identity (i.e. ReferenceEquals)
    /// rather than .Equals, allowing identity to be used for checking equality in
    /// ISets and IDictionaries.
    /// </summary>
    public class IdentityEqualityComparer<T> : IEqualityComparer<T>
    {
        public bool Equals(T x, T y) => ReferenceEquals(x, y);

        public int GetHashCode(T obj) => RuntimeHelpers.GetHashCode(obj);
    }
}
