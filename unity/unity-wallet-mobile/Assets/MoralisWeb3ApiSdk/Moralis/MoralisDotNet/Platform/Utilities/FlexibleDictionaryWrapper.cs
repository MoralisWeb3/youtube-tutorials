
using System.Collections.Generic;
using System.Linq;

namespace Moralis.Platform.Utilities
{
    /// <summary>
    /// Provides a Dictionary implementation that can delegate to any other
    /// dictionary, regardless of its value type. Used for coercion of
    /// dictionaries when returning them to users.
    /// </summary>
    /// <typeparam name="TOut">The resulting type of value in the dictionary.</typeparam>
    /// <typeparam name="TIn">The original type of value in the dictionary.</typeparam>
    [Preserve(AllMembers = true, Conditional = false)]
    public class FlexibleDictionaryWrapper<TOut, TIn> : IDictionary<string, TOut>
    {
        private readonly IDictionary<string, TIn> toWrap;
        public FlexibleDictionaryWrapper(IDictionary<string, TIn> toWrap) => this.toWrap = toWrap;

        public void Add(string key, TOut value) => toWrap.Add(key, (TIn) Conversion.ConvertTo<TIn>(value));

        public bool ContainsKey(string key) => toWrap.ContainsKey(key);

        public ICollection<string> Keys => toWrap.Keys;

        public bool Remove(string key) => toWrap.Remove(key);

        public bool TryGetValue(string key, out TOut value)
        {
            bool result = toWrap.TryGetValue(key, out TIn outValue);
            value = (TOut) Conversion.ConvertTo<TOut>(outValue);
            return result;
        }

        public ICollection<TOut> Values => toWrap.Values
                    .Select(item => (TOut) Conversion.ConvertTo<TOut>(item)).ToList();

        public TOut this[string key]
        {
            get => (TOut) Conversion.ConvertTo<TOut>(toWrap[key]);
            set => toWrap[key] = (TIn) Conversion.ConvertTo<TIn>(value);
        }

        public void Add(KeyValuePair<string, TOut> item) => toWrap.Add(new KeyValuePair<string, TIn>(item.Key,
                (TIn) Conversion.ConvertTo<TIn>(item.Value)));

        public void Clear() => toWrap.Clear();

        public bool Contains(KeyValuePair<string, TOut> item) => toWrap.Contains(new KeyValuePair<string, TIn>(item.Key,
                (TIn) Conversion.ConvertTo<TIn>(item.Value)));

        public void CopyTo(KeyValuePair<string, TOut>[] array, int arrayIndex)
        {
            IEnumerable<KeyValuePair<string, TOut>> converted = from pair in toWrap
                                                                select new KeyValuePair<string, TOut>(pair.Key,
                                                                    (TOut) Conversion.ConvertTo<TOut>(pair.Value));
            converted.ToList().CopyTo(array, arrayIndex);
        }

        public int Count => toWrap.Count;

        public bool IsReadOnly => toWrap.IsReadOnly;

        public bool Remove(KeyValuePair<string, TOut> item) => toWrap.Remove(new KeyValuePair<string, TIn>(item.Key,
                (TIn) Conversion.ConvertTo<TIn>(item.Value)));

        public IEnumerator<KeyValuePair<string, TOut>> GetEnumerator()
        {
            foreach (KeyValuePair<string, TIn> pair in toWrap)
                yield return new KeyValuePair<string, TOut>(pair.Key,
                    (TOut) Conversion.ConvertTo<TOut>(pair.Value));
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
