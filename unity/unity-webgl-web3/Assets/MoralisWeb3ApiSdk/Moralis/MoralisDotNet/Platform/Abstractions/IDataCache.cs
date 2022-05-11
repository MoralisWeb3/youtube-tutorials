using System.Collections.Generic;
using System.Threading.Tasks;

namespace Moralis.Platform.Abstractions
{
    /// <summary>
    /// An interface for a dictionary that is persisted to disk asynchronously.
    /// </summary>
    /// <typeparam name="TKey">They key type of the dictionary.</typeparam>
    /// <typeparam name="TValue">The value type of the dictionary.</typeparam>
    public interface IDataCache<TKey, TValue> : IDictionary<TKey, TValue>
    {
        /// <summary>
        /// Adds a key to this dictionary, and saves it asynchronously.
        /// </summary>
        /// <param name="key">The key to insert.</param>
        /// <param name="value">The value to insert.</param>
        /// <returns></returns>
        Task AddAsync(TKey key, TValue value);

        /// <summary>
        /// Removes a key from this dictionary, and saves it asynchronously.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task RemoveAsync(TKey key);
    }

}
