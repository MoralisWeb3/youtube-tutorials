using WalletConnectSharp.Core.Models;

namespace WalletConnectSharp.Core.Events.Response
{
    public class JsonRpcRequestEvent<T> : GenericEvent<T> where T : JsonRpcRequest
    {
    }
}