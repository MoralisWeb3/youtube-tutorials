using System;
using System.Threading.Tasks;
using Nethereum.JsonRpc.Client;

namespace MoralisWeb3ApiSdk
{
    /// <summary>
    /// 
    /// </summary>
    public class DeadRpcReadClient : IClient
    {
        private Action<string> messageHandler;
        public DeadRpcReadClient(Action<string> msgHandler)
        {
            messageHandler = msgHandler;
        }

        public RequestInterceptor OverridingRequestInterceptor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Task<T> SendRequestAsync<T>(RpcRequest request, string route = null)
        {
            if (messageHandler != null)
            {
                messageHandler($"Method {request.Method} is not supported by this RPC handler");
            }

            return null;
        }

        public Task<T> SendRequestAsync<T>(string method, string route = null, params object[] paramList)
        {
            if (messageHandler != null)
            {
                messageHandler($"Method {method} is not supported by this RPC handler");
            }

            return null;
        }

        public Task SendRequestAsync(RpcRequest request, string route = null)
        {
            if (messageHandler != null)
            {
                messageHandler($"Method {request.Method} is not supported by this RPC handler");
            }

            return null;
        }

        public Task SendRequestAsync(string method, string route = null, params object[] paramList)
        {
            if (messageHandler != null)
            {
                messageHandler($"Method {method} is not supported by this RPC handler");
            }

            return null;
        }
    }
}
