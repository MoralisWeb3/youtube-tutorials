using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using Moralis.Platform.Exceptions;
using static Moralis.Platform.Exceptions.MoralisFailureException;
using System.Collections.Generic;
using Moralis.Platform.Abstractions;

namespace Moralis.Platform.Queries.Live
{
    public class SubscribableWebSocket
    {
        private byte[] subscriptionRequest = null;
        private bool cocnectionSent = false;
        private bool subscriptionSent = false;
        private bool unsubscribeSent = false;

        /// <summary>
        /// Event raised when a message is received from the server.
        /// </summary>
        public event LiveQueryMessageHandler OnEventMessage;

        public event LiveQueryGeneralMessageHandler OnGeneralMessage;

        /// <summary>
        /// Indicates the current status of this client.
        /// </summary>
        public LiveQueryClientStatusTypes ClientStatus { get; set; }

        /// <summary>
        /// Server connection settings.
        /// </summary>
        public IServerConnectionData ConncetionData { get; set; }

        /// <summary>
        /// Provides Serialization / Deserialization services.
        /// </summary>
        public IJsonSerializer JsonSerializer { get; set; }

        public string InstallationId { get; set; }

        /// <summary>
        /// Request Id used for this subscription
        /// </summary>
        public int RequestId { get; private set; }

        /// <summary>
        /// User session token.
        /// </summary>
        public string SessionToken { get; set; }

        private IClientWebSocket webSocket;

        public SubscribableWebSocket(byte[] subRequest, IServerConnectionData connectionData, int requestId, string installationId, string sessionToken, IJsonSerializer jsonSerializer )
        {
            subscriptionRequest = subRequest;
            ConncetionData = connectionData;
            InstallationId = installationId;
            RequestId = requestId;
            SessionToken = sessionToken;
            JsonSerializer = jsonSerializer;
        }

        /// <summary>
        /// Creates a live query subscription. If established, listens until either 
        /// the app or the server closes the connection.
        /// </summary>
        public async void Subscribe()
        {
            if (String.IsNullOrWhiteSpace(ConncetionData.LiveQueryServerURI))
            {
                throw new MoralisFailureException(ErrorCode.ServerUrlNullOrEmtpy, "");
            }

            ClientStatus = LiveQueryClientStatusTypes.New;
            bool receiving = false;
            List<byte> messageBytes = new List<byte>();

            using (IClientWebSocket ws = webSocket switch 
            {
                { } => webSocket,
                _ => new MoralisClientWebSocket()
            })
            {
                await ws.ConnectAsync(new Uri(ConncetionData.LiveQueryServerURI), CancellationToken.None);
                byte[] buffer = new byte[256];
                bool msgSent = false;

                // Establish a connection and listen and process messages until closed.
                while (ws.State == WebSocketState.Open && ClientStatus != LiveQueryClientStatusTypes.Closed)
                {
                    if (!receiving)
                    {
                        // If in new status create and send a connection request.
                        if (!cocnectionSent && LiveQueryClientStatusTypes.New.Equals(ClientStatus))
                        {
                            SendGeneralMessage("Sending connection request.");
                            byte[] bytes = CreateConnectRequest();
                            ArraySegment<byte> seg = new ArraySegment<byte>(bytes);
                            await ws.SendAsync(seg, WebSocketMessageType.Text, true, CancellationToken.None);
                            cocnectionSent = true;
                            msgSent = true;
                        }
                        // If in openning status, create and send a subscription request.
                        else if (!subscriptionSent && LiveQueryClientStatusTypes.Opening.Equals(ClientStatus))
                        {
                            SendGeneralMessage("Sending subscription request.");
                            ArraySegment<byte> seg = new ArraySegment<byte>(subscriptionRequest);
                            await ws.SendAsync(seg, WebSocketMessageType.Text, true, CancellationToken.None);
                            subscriptionSent = true;
                            msgSent = true;
                        }
                        // If in closing status, create and send an unsubscribe request.
                        else if (!unsubscribeSent &&  LiveQueryClientStatusTypes.Closing.Equals(ClientStatus))
                        {
                            SendGeneralMessage("Sending unsubscribe request.");
                            byte[] bytes = CreateUnsubscribeRequest();
                            ArraySegment<byte> seg = new ArraySegment<byte>(bytes);
                            await ws.SendAsync(seg, WebSocketMessageType.Text, true, CancellationToken.None);
                            unsubscribeSent = true;
                            msgSent = true;
                        }
                    }

                    // Only listen if the client is not processing the response for 
                    // a request (such as connect, subscribe, unsubscribe).
                    if (receiving || msgSent || LiveQueryClientStatusTypes.Open.Equals(ClientStatus))
                    {
                        SendGeneralMessage($"Listening, status: {ClientStatus}");
                        // Listen for next response from server.
                        var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                        SendGeneralMessage("Result received.");
                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                            ClientStatus = LiveQueryClientStatusTypes.Closed;
                            receiving = false;
                            msgSent = false;
                        }
                        else
                        {
                            if (!receiving)
                            {
                                messageBytes.Clear();
                                receiving = true;
                            }

                            messageBytes.AddRange(PackBuffer(buffer, result.Count));

                            if (result.EndOfMessage)
                            {
                                receiving = false;
                                msgSent = false;
                                // Handle normal processing message.
                                OnEventMessage(messageBytes.ToArray(), messageBytes.Count);
                            }
                        }
                    }
                }

                Console.WriteLine("Left subscription loop.");
            }
        }

        internal void SetState(LiveQueryClientStatusTypes newState) => ClientStatus = newState;

        /// <summary>
        /// Should only be set for testing scenarios.
        /// </summary>
        /// <param name="ws"></param>
        public void SetWebsocket(IClientWebSocket ws) => webSocket = ws;
        
        private byte[] CreateConnectRequest()
        {
            ConnectRequest msg = new ConnectRequest()
            {
                applicationId = ConncetionData.ApplicationID,
                //InstallationId = InstallationId,
                //SessionToken = SessionToken
            };

            string json = JsonSerializer.Serialize(msg, JsonSerializer.DefaultOptions);

            byte[] bytes = Encoding.UTF8.GetBytes(json);

            return bytes;
        }

        private byte[] CreateUnsubscribeRequest()
        {
            UnscubscribeRequest msg = new UnscubscribeRequest(RequestId);

            string json = JsonSerializer.Serialize(msg, JsonSerializer.DefaultOptions);

            byte[] bytes = Encoding.UTF8.GetBytes(json);

            return bytes;
        }

        private byte[] PackBuffer(byte[] buffer, int size)
        {
            List<byte> resp = new List<byte>();
            int index = 0;
            foreach (byte b in buffer)
            {

                if (b == 0)
                    break;

                resp.Add(b);
                index++;

                if (index >= size)
                    break;
            }

            return resp.ToArray();
        }

        private void SendGeneralMessage(string msg)
        {
            if (OnGeneralMessage != null)
            {
                OnGeneralMessage(msg);
            }
        }
    }
}
