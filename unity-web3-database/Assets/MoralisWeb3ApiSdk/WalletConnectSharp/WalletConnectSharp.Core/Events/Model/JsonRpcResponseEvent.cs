using WalletConnectSharp.Core.Models;

namespace WalletConnectSharp.Core.Events.Request
{
    public class JsonRpcResponseEvent<T> : GenericEvent<T> where T : JsonRpcResponse
    {
    }
}