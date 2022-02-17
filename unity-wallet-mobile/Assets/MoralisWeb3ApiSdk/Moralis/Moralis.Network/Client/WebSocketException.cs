using System;

namespace Moralis.Network.Client
{
    public class WebSocketException : Exception
    {
        public WebSocketException()
        {
        }

        public WebSocketException(string message) : base(message)
        {
        }

        public WebSocketException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
