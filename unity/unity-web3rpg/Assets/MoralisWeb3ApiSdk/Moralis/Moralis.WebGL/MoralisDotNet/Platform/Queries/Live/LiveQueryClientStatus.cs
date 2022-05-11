using System;
using System.Collections.Generic;
using System.Text;

namespace Moralis.WebGL.Platform.Queries.Live
{
    public enum LiveQueryClientStatusTypes
    {
        /// <summary>
        /// The client is new and not running
        /// </summary>
        New,
        /// <summary>
        /// A subscribe request was sent
        /// </summary>
        Opening,
        /// <summary>
        /// The client is subscribed and listening for server events.
        /// </summary>
        Open,
        /// <summary>
        /// An unscubscribe message has bee sent to the server.
        /// </summary>
        Closing,
        /// <summary>
        /// The client has been successfully unscubscribed from the server and can be disposed of.
        /// </summary>
        Closed,
        /// <summary>
        /// The client is waiting on a request result to finish processing.
        /// </summary>
        Waiting
    }
}
