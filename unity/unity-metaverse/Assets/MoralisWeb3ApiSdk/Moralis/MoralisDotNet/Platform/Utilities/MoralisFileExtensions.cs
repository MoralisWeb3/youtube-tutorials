using System;
using Moralis.Platform.Objects;

namespace Moralis.Platform.Utilities
{
    public class MoralisFileExtensions
    {
        public static MoralisFile Create(string name, Uri uri, string mimeType = null) => new MoralisFile(name, uri, mimeType);
    }
}
