using Moralis.WebGL.Hex.HexConvertors.Extensions;

namespace Moralis.WebGL.Hex.HexConvertors
{
    public class HexUTF8StringConvertor : IHexConvertor<string>
    {
        public string ConvertToHex(string value)
        {
            return value.ToHexUTF8();
        }

        public string ConvertFromHex(string hex)
        {
            return hex.HexToUTF8String();
        }
    }
}