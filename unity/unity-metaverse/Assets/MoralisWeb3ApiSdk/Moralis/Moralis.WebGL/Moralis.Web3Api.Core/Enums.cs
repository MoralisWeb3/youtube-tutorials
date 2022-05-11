

namespace Moralis.WebGL.Web3Api.Core
{
    public enum Method
    {
        GET = 0,
        POST = 1,
        PUT = 2,
        DELETE = 3,
        HEAD = 4,
        OPTIONS = 5,
        PATCH = 6,
        MERGE = 7,
        COPY = 8
    }
    public enum ParameterType
    {
        Cookie = 0,
        GetOrPost = 1,
        UrlSegment = 2,
        HttpHeader = 3,
        RequestBody = 4,
        QueryString = 5,
        QueryStringWithoutEncode = 6
    }
    public enum DataFormat
    {
        Json = 0,
        Xml = 1,
        None = 2
    }
}
