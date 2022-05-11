
namespace Moralis.Network.Client
{
    public delegate void WebSocketOpenEventHandler();

    public delegate void WebSocketMessageEventHandler(byte[] data);

    public delegate void WebSocketErrorEventHandler(string errorMsg);

    public delegate void WebSocketCloseEventHandler(WebSocketCloseCode closeCode);

}
