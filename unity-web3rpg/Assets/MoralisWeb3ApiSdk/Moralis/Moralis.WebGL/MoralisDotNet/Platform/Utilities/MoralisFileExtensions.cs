using System;
using Moralis.WebGL.Platform.Objects;

namespace Moralis.WebGL.Platform.Utilities
{
    public class MoralisFileExtensions
    {
        public static MoralisFile Create(string name, Uri uri, string mimeType = null) => new MoralisFile(name, uri, mimeType);
    }
}
