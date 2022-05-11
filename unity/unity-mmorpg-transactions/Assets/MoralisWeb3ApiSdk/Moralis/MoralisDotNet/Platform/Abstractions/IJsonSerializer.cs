using System.Collections.Generic;


namespace Moralis.Platform.Abstractions
{
    /// <summary>
    /// Defines an interface for common json serialization functions.
    /// </summary>
    public interface IJsonSerializer
    {
        IDictionary<string, object> DefaultOptions { get; set; }

        /// <summary>
        /// Performs deserialization of json into object of type T
        /// </summary>
        /// <typeparam name="T">Type of opbject</typeparam>
        /// <param name="json">Json serialized object</param>
        /// <param name="options">Serialization objects. Implementation must map relevant options type.</param>
        /// <returns>Type</returns>
        T Deserialize<T>(string json, IDictionary<string, object> options = null);

        /// <summary>
        /// Json serializes target using provided options.
        /// </summary>
        /// <param name="target">Object to serialize</param>
        /// <param name="option">Serialization objects. Implementation must map relevant options type.</param>
        /// <returns>Json string</returns>
        string Serialize(object target, IDictionary<string, object> options = null);
    }
}
