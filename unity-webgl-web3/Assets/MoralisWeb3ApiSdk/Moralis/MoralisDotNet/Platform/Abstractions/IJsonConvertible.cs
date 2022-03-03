using System;
using System.Collections.Generic;
using System.Text;

namespace Moralis.Platform.Abstractions
{
    /// <summary>
    /// Represents an object that can be converted into JSON.
    /// </summary>
    public interface IJsonConvertible
    {
        /// <summary>
        /// Converts the object to a data structure that can be converted to JSON.
        /// </summary>
        /// <returns>An object to be JSONified.</returns>
        IDictionary<string, object> ConvertToJSON();
    }
}
