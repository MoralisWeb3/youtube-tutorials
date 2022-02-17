using System.Collections.Generic;
using System.Threading.Tasks;
using Nethereum.JsonRpc.Client;
using Nethereum.JsonRpc.Client.RpcMessages;
using WalletConnectSharp.Core;

namespace WalletConnectSharp.NEthereum.Client
{
    public class WalletConnectClient : ClientBase
    {
        private long _id;
        public WalletConnectProtocol Provider { get; }

        public WalletConnectClient(WalletConnectProtocol provider)
        {
            this.Provider = provider;
        }

        protected override async Task<RpcResponseMessage> SendAsync(RpcRequestMessage message, string route = null)
        {
            _id += 1;
            var mapParameters = message.RawParameters as Dictionary<string, object>;
            var arrayParameters = message.RawParameters as object[];
            var rawParameters = message.RawParameters;

            RpcRequestMessage rpcRequestMessage;
            if (mapParameters != null) 
                rpcRequestMessage = new RpcRequestMessage(_id, message.Method, mapParameters);
            else if (arrayParameters != null)
                rpcRequestMessage = new RpcRequestMessage(_id, message.Method, arrayParameters);
            else
                rpcRequestMessage = new RpcRequestMessage(_id, message.Method, rawParameters);
            
            TaskCompletionSource<RpcResponseMessage> eventCompleted = new TaskCompletionSource<RpcResponseMessage>(TaskCreationOptions.None);
            
            Provider.Events.ListenForGenericResponse<RpcResponseMessage>(rpcRequestMessage.Id, (sender, args) =>
            {
                eventCompleted.SetResult(args.Response);
            });
            
            await Provider.SendRequest(rpcRequestMessage);

            return await eventCompleted.Task;
        }
    }
}