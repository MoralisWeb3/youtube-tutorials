using System;

namespace Moralis.Network.Client
{
    public class WebSocketInvalidArgumentException : WebSocketException
    {
        public WebSocketInvalidArgumentException()
        {
        }

        public WebSocketInvalidArgumentException(string message) : base(message)
        {
        }

        public WebSocketInvalidArgumentException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
