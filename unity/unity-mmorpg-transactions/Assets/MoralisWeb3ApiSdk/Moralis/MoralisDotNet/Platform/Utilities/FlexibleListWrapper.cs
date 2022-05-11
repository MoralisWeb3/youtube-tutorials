
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Moralis.Platform.Utilities
{
    /// <summary>
    /// Provides a List implementation that can delegate to any other
    /// list, regardless of its value type. Used for coercion of
    /// lists when returning them to users.
    /// </summary>
    /// <typeparam name="TOut">The resulting type of value in the list.</typeparam>
    /// <typeparam name="TIn">The original type of value in the list.</typeparam>
    [Preserve(AllMembers = true, Conditional = false)]
    public class FlexibleListWrapper<TOut, TIn> : IList<TOut>
    {
        private IList<TIn> toWrap;
        public FlexibleListWrapper(IList<TIn> toWrap) => this.toWrap = toWrap;

        public int IndexOf(TOut item) => toWrap.IndexOf((TIn) Conversion.ConvertTo<TIn>(item));

        public void Insert(int index, TOut item) => toWrap.Insert(index, (TIn) Conversion.ConvertTo<TIn>(item));

        public void RemoveAt(int index) => toWrap.RemoveAt(index);

        public TOut this[int index]
        {
            get => (TOut) Conversion.ConvertTo<TOut>(toWrap[index]);
            set => toWrap[index] = (TIn) Conversion.ConvertTo<TIn>(value);
        }

        public void Add(TOut item) => toWrap.Add((TIn) Conversion.ConvertTo<TIn>(item));

        public void Clear() => toWrap.Clear();

        public bool Contains(TOut item) => toWrap.Contains((TIn) Conversion.ConvertTo<TIn>(item));

        public void CopyTo(TOut[] array, int arrayIndex) => toWrap.Select(item => (TOut) Conversion.ConvertTo<TOut>(item))
                .ToList().CopyTo(array, arrayIndex);

        public int Count => toWrap.Count;

        public bool IsReadOnly => toWrap.IsReadOnly;

        public bool Remove(TOut item) => toWrap.Remove((TIn) Conversion.ConvertTo<TIn>(item));

        public IEnumerator<TOut> GetEnumerator()
        {
            foreach (object item in (IEnumerable) toWrap)
                yield return (TOut) Conversion.ConvertTo<TOut>(item);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
