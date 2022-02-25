using Moralis.Platform.Abstractions;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Moralis.Platform.Queries.Live
{
    public class MoralisClientWebSocket : IClientWebSocket
    {
        private ClientWebSocket webSocket = new ClientWebSocket();

        //
        // Summary:
        //     Gets the reason why the close handshake was initiated on System.Net.WebSockets.ClientWebSocket
        //     instance.
        //
        // Returns:
        //     Returns System.Net.WebSockets.WebSocketCloseStatus. The reason why the close
        //     handshake was initiated.
        public WebSocketCloseStatus? CloseStatus => webSocket.CloseStatus;
        //
        // Summary:
        //     Gets a description of the reason why the System.Net.WebSockets.ClientWebSocket
        //     instance was closed.
        //
        // Returns:
        //     Returns System.String. The description of the reason why the System.Net.WebSockets.ClientWebSocket
        //     instance was closed.
        public string CloseStatusDescription => webSocket.CloseStatusDescription;
        //
        // Summary:
        //     Gets the WebSocket options for the System.Net.WebSockets.ClientWebSocket instance.
        //
        // Returns:
        //     Returns System.Net.WebSockets.ClientWebSocketOptions. The WebSocket options for
        //     the System.Net.WebSockets.ClientWebSocket instance.
        public ClientWebSocketOptions Options => webSocket.Options;
        //
        // Summary:
        //     Get the WebSocket state of the System.Net.WebSockets.ClientWebSocket instance.
        //
        // Returns:
        //     Returns System.Net.WebSockets.WebSocketState. The WebSocket state of the System.Net.WebSockets.ClientWebSocket
        //     instance.
        public WebSocketState State => webSocket.State;
        //
        // Summary:
        //     Gets the supported WebSocket sub-protocol for the System.Net.WebSockets.ClientWebSocket
        //     instance.
        //
        // Returns:
        //     Returns System.String. The supported WebSocket sub-protocol.
        public string SubProtocol => webSocket.SubProtocol;

        //
        // Summary:
        //     Aborts the connection and cancels any pending IO operations.
        public void Abort() => webSocket.Abort();
        //
        // Summary:
        //     Close the System.Net.WebSockets.ClientWebSocket instance as an asynchronous operation.
        //
        // Parameters:
        //   closeStatus:
        //     The WebSocket close status.
        //
        //   statusDescription:
        //     A description of the close status.
        //
        //   cancellationToken:
        //     A cancellation token used to propagate notification that this operation should
        //     be canceled.
        //
        // Returns:
        //     Returns System.Threading.Tasks.Task. The task object representing the asynchronous
        //     operation.
        public Task CloseAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken)
             => webSocket.CloseAsync(closeStatus, statusDescription, cancellationToken);
        //
        // Summary:
        //     Close the output for the System.Net.WebSockets.ClientWebSocket instance as an
        //     asynchronous operation.
        //
        // Parameters:
        //   closeStatus:
        //     The WebSocket close status.
        //
        //   statusDescription:
        //     A description of the close status.
        //
        //   cancellationToken:
        //     A cancellation token used to propagate notification that this operation should
        //     be canceled.
        //
        // Returns:
        //     Returns System.Threading.Tasks.Task. The task object representing the asynchronous
        //     operation.
        public Task CloseOutputAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken)
             => webSocket.CloseOutputAsync(closeStatus, statusDescription, cancellationToken);
        //
        // Summary:
        //     Connect to a WebSocket server as an asynchronous operation.
        //
        // Parameters:
        //   uri:
        //     The URI of the WebSocket server to connect to.
        //
        //   cancellationToken:
        //     A cancellation token used to propagate notification that the operation should
        //     be canceled.
        //
        // Returns:
        //     Returns System.Threading.Tasks.Task. The task object representing the asynchronous
        //     operation.
        public Task ConnectAsync(Uri uri, CancellationToken cancellationToken)
             => webSocket.ConnectAsync(uri, cancellationToken);

        //
        // Summary:
        //     Receive data on System.Net.WebSockets.ClientWebSocket as an asynchronous operation.
        //
        // Parameters:
        //   buffer:
        //     The buffer to receive the response.
        //
        //   cancellationToken:
        //     A cancellation token used to propagate notification that this operation should
        //     be canceled.
        //
        // Returns:
        //     Returns System.Threading.Tasks.Task`1. The task object representing the asynchronous
        //     operation.
        public Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken)
             => webSocket.ReceiveAsync(buffer, cancellationToken);
        //
        // Summary:
        //     Send data on System.Net.WebSockets.ClientWebSocket as an asynchronous operation.
        //
        // Parameters:
        //   buffer:
        //     The buffer containing the message to be sent.
        //
        //   messageType:
        //     Specifies whether the buffer is clear text or in a binary format.
        //
        //   endOfMessage:
        //     Specifies whether this is the final asynchronous send. Set to true if this is
        //     the final send; false otherwise.
        //
        //   cancellationToken:
        //     A cancellation token used to propagate notification that this operation should
        //     be canceled.
        //
        // Returns:
        //     The task object representing the asynchronous operation.
        public Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken)
             => webSocket.SendAsync(buffer, messageType, endOfMessage, cancellationToken);

        public void Dispose() => webSocket.Dispose();
    }
}
