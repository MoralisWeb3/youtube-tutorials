using System;
using Moralis.Network.Client;
using Cysharp.Threading.Tasks;

namespace Moralis.WebGL.Platform.Abstractions
{
    public interface IClientWebSocket : IDisposable
    {
        WebSocketCloseCode CloseStatus { get; }
        string CloseReasonsDescription { get; }
        WebSocketState State { get; }
        UniTask CloseAsync(WebSocketCloseCode closeCode, string reason);
        UniTask ConnectAsync();
        UniTask SendAsync(byte[] buffer);
        UniTask SendTextAsync(string msg);
        void Abort();
    }
}
