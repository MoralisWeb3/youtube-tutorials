
using System;
using Moralis.Platform.Abstractions;

namespace Moralis.Platform.Services.Models
{
    /// <summary>
    /// Represents upload progress.
    /// </summary>
    public class DataTransferLevel : EventArgs, IDataTransferLevel
    {
        public DataTransferLevel() { }
        /// <summary>
        /// Gets the progress (a number between 0.0 and 1.0) of an upload or download.
        /// </summary>
        public double Amount { get; set; }
    }
}
