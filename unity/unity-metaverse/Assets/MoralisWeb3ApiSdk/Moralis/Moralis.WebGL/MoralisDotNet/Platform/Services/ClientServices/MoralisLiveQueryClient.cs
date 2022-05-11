#if UNITY_WEBGL
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Moralis.WebGL.Platform.Abstractions;
using Moralis.WebGL.Platform.Objects;
using Moralis.WebGL.Platform.Queries;
using Moralis.WebGL.Platform.Queries.Live;
using Moralis.WebGL.Platform.Utilities;
using static Moralis.WebGL.Platform.Exceptions.MoralisFailureException;
using Cysharp.Threading.Tasks;

namespace Moralis.WebGL.Platform.Services.ClientServices
{
    /// <summary>
    /// Represents a single Live Query subscription.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MoralisLiveQueryClient<T> : ILiveQueryClient where T : MoralisObject
    {
        /// <summary>
        /// Singleton value should beincremented whenever a client is created.
        /// </summary>
        public static int NextRequestId = 1;
        private bool disposedValue;
        private int taskIndex = 0;
        private UniTask subscriptTask;

        private CancellationTokenSource cancelSource = new CancellationTokenSource();
        private CancellationToken cancellationToken = CancellationToken.None;
        private MoralisQuery<T> targetQuery;
        private SubscribableWebSocket liveQueryServerClient = default;

        /// <summary>
        /// Standard constructor
        /// </summary>
        /// <param name="query"></param>
        /// <param name="conncetionData"></param>
        /// <param name="callbacks"></param>
        /// <param name="sessionToken"></param>
        /// <param name="installationId"></param>
        public MoralisLiveQueryClient(MoralisQuery<T> query, IServerConnectionData conncetionData, ILiveQueryCallbacks<T> callbacks, string sessionToken = null, string installationId = null)
        {
            Initialize(query, conncetionData, callbacks, sessionToken, installationId);

            liveQueryServerClient = new SubscribableWebSocket(CreateSubscribeRequest(), ConncetionData, RequestId, InstallationId, sessionToken, query.JsonSerializer);
            liveQueryServerClient.OnEventMessage += HandleEventMessage;
            liveQueryServerClient.OnGeneralMessage += HandleGeneralMessage;

            ClientStatus = LiveQueryClientStatusTypes.New;
        }

        /// <summary>
        /// Constructor accepting SubscribableWebSocket to facilitate testing.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="conncetionData"></param>
        /// <param name="callbacks"></param>
        /// <param name="sessionToken"></param>
        /// <param name="installationId"></param>
        public MoralisLiveQueryClient(MoralisQuery<T> query, IServerConnectionData conncetionData, ILiveQueryCallbacks<T> callbacks, SubscribableWebSocket webSocket, string sessionToken = null, string installationId = null)
        {
            Initialize(query, conncetionData, callbacks, sessionToken, installationId);

            liveQueryServerClient = webSocket;
            liveQueryServerClient.OnEventMessage += HandleEventMessage;

            ClientStatus = LiveQueryClientStatusTypes.New;
        }

        /// <summary>
        /// Called when a connected event is returned from the server.
        /// </summary>
        public event LiveQueryConnectedHandler OnConnected;

        /// <summary>
        /// Called when a subscribed event is returned from the server.
        /// </summary>
        public event LiveQuerySubscribedHandler OnSubscribed;

        /// <summary>
        /// Called when a create event is returned from the server.
        /// </summary>
        public event LiveQueryActionHandler<T> OnCreate;

        /// <summary>
        /// Called when a delete event is returned from the server.
        /// </summary>
        public event LiveQueryActionHandler<T> OnDelete;

        /// <summary>
        /// Called when a enter event is returned from the server.
        /// </summary>
        public event LiveQueryActionHandler<T> OnEnter;

        /// <summary>
        /// Called when a leave event is returned from the server.
        /// </summary>
        public event LiveQueryActionHandler<T> OnLeave;

        /// <summary>
        /// Called when a update event is returned from the server.
        /// </summary>
        public event LiveQueryActionHandler<T> OnUpdate;

        /// <summary>
        /// Called when a ubsubscribed event is received.
        /// </summary>
        public event LiveQueryUnsubscribedHandler OnUnsubscribed;

        /// <summary>
        /// Called when an ErrorMEssage is received.
        /// </summary>
        public event LiveQueryErrorHandler OnError;

        public event LiveQueryGeneralMessageHandler OnGeneralMessage;

        /// <summary>
        /// Indicates the current status of this client.
        /// </summary>
        public LiveQueryClientStatusTypes ClientStatus
        {
            get => liveQueryServerClient.ClientStatus;
            set => liveQueryServerClient.ClientStatus = value;
        }

        /// <summary>
        /// Server connection settings.
        /// </summary>
        public IServerConnectionData ConncetionData { get; set; }

        public string InstallationId { get; set; }

        /// <summary>
        /// Used to lock object resources.
        /// </summary>
        public object Mutex { get; set; } = new object { };

        /// <summary>
        /// Request Id used for this subscription
        /// </summary>
        public int RequestId { get; private set; }

        /// <summary>
        /// User session token.
        /// </summary>
        public string SessionToken { get; set; }

        public IJsonSerializer JsonSerializer { get; set; }

        public void Unsubscribe()
        {
            this.ClientStatus = LiveQueryClientStatusTypes.Closing;
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public void Subscribe()
        {
            lock (Mutex)
            {
                if (!(cancellationToken is { }))
                {
                    cancellationToken = cancelSource.Token;
                }

                this.liveQueryServerClient.Subscribe();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    lock (Mutex)
                    {
                        cancelSource.Cancel();

                        if (subscriptTask is { })
                        {
                            subscriptTask.Forget();
                        }
                    }
                }

                disposedValue = true;
            }
        }

        private void HandleGeneralMessage(string text)
        {
            if (this.OnGeneralMessage != null)
            {
                OnGeneralMessage(text);
            }
        }

        private void HandleEventMessage(byte[] buffer, int bufferSize)
        {
            // Lock mutex to enforce thread safty.
            lock (Mutex)
            {
                if (cancellationToken == CancellationToken.None)
                {
                    cancellationToken = cancelSource.Token;
                }

                // Convert buffer to string - hopefully this is well formed JSON.
                string json = Encoding.UTF8.GetString(buffer);

                // First convert to a basic event message so that
                QueryEventMessage opMsg = (QueryEventMessage)JsonSerializer.Deserialize<QueryEventMessage>(json);

                OperationTypes op = (OperationTypes)Enum.Parse(typeof(OperationTypes), opMsg.op);

                // If response is a response to a command request, set 
                // client status to Waiting.
                if (OperationTypes.connected.Equals(op) ||
                    OperationTypes.subscribed.Equals(op) ||
                    OperationTypes.unsubscribed.Equals(op))
                {
                    ClientStatus = LiveQueryClientStatusTypes.Waiting;
                }

                json = json.AdjustJsonForParseDate();

                try
                {
                    HandleGeneralMessage($"Received message ${op}");
                    // Based on operation indicated by the event, handle message
                    switch (op)
                    { 
                        case OperationTypes.connected:
                            this.ClientStatus = LiveQueryClientStatusTypes.Opening;
                            if (OnConnected != null)
                            {
                                ActionEvent<T> ae = (ActionEvent<T>)JsonSerializer.Deserialize<ActionEvent<T>>(json);
                                OnConnected();
                            }
                            HandleGeneralMessage("Processed connected.");

                            break;
                        case OperationTypes.create:
                            if (OnCreate != null)
                            {
                                ActionEvent<T> ae = (ActionEvent<T>)JsonSerializer.Deserialize<ActionEvent<T>>(json);
                                OnCreate(ae.Object, ae.requestId);
                            }

                            break;
                        case OperationTypes.delete:
                            if (OnDelete != null)
                            {
                                ActionEvent<T> ae = (ActionEvent<T>)JsonSerializer.Deserialize<ActionEvent<T>>(json);
                                OnDelete(ae.Object, ae.requestId);
                            }

                            break;
                        case OperationTypes.enter:
                            if (OnEnter != null)
                            {
                                ActionEvent<T> ae = (ActionEvent<T>)JsonSerializer.Deserialize<ActionEvent<T>>(json);
                                OnEnter(ae.Object, ae.requestId);
                            }

                            break;
                        case OperationTypes.error:
                            if (OnError != null)
                            {
                                ErrorMessage em = (ErrorMessage)JsonSerializer.Deserialize<ErrorMessage>(json);
                                OnError(em);
                            }

                            break;
                        case OperationTypes.leave:
                            if (OnLeave != null)
                            {
                                ActionEvent<T> ae = (ActionEvent<T>)JsonSerializer.Deserialize<ActionEvent<T>>(json);
                                OnLeave(ae.Object, ae.requestId);
                            }

                            break;
                        case OperationTypes.subscribed:
                            this.ClientStatus = LiveQueryClientStatusTypes.Open;
                            if (OnSubscribed != null)
                            {
                                ActionEvent<T> ae = (ActionEvent<T>)JsonSerializer.Deserialize<ActionEvent<T>>(json);
                                OnSubscribed(ae.requestId);
                            }
                            HandleGeneralMessage("Processed subscribed.");

                            break;
                        case OperationTypes.unsubscribed:
                            this.ClientStatus = LiveQueryClientStatusTypes.Closed;
                            if (OnUnsubscribed != null)
                            {
                                ActionEvent<T> ae = (ActionEvent<T>)JsonSerializer.Deserialize<ActionEvent<T>>(json);
                                OnUnsubscribed(ae.requestId);
                            }
                            HandleGeneralMessage("Processed unsubscribed.");

                            break;
                        case OperationTypes.update:
                            if (OnUpdate != null)
                            {
                                ActionEvent<T> ae = (ActionEvent<T>)JsonSerializer.Deserialize<ActionEvent<T>>(json);
                                OnUpdate(ae.Object, ae.requestId);
                            }

                            break;
                    }
                }
                catch (Exception exp)
                {
                    if (OnError != null)
                    {
                        ErrorMessage evt = new ErrorMessage() { 
                            code = (int)ErrorCode.LiveQueryEventHandlingFailed,
                            error = exp.Message
                        };

                        OnError(evt);
                    }
                }

            }
        }

        private void HandleConnectEvent()
        {
            this.ClientStatus = LiveQueryClientStatusTypes.Opening;
            //HandleGeneralMessage($"Subscription Client status changed to: {this.ClientStatus}");
        }

        private void HandleSubscribedEvent(int requestId) 
        {
            this.ClientStatus = LiveQueryClientStatusTypes.Open;
            //HandleGeneralMessage($"Subscription Client status changed to: {this.ClientStatus}");
        }

        private void HandleUnsubscribedEvent(int requestId)
        {
            this.ClientStatus = LiveQueryClientStatusTypes.Closed;
            //HandleGeneralMessage($"Subscription Client status changed to: {this.ClientStatus}");
        }


        private void Initialize(MoralisQuery<T> query, IServerConnectionData conncetionData, ILiveQueryCallbacks<T> callbacks, string sessionToken, string installationId)
        {
            RequestId = NextRequestId++;
            targetQuery = query;
            ConncetionData = conncetionData;
            SessionToken = sessionToken;
            InstallationId = installationId;
            JsonSerializer = query.JsonSerializer;

            this.OnConnected += callbacks.OnConnected;
            this.OnCreate += callbacks.OnCreate;
            this.OnDelete += callbacks.OnDelete;
            this.OnEnter += callbacks.OnEnter;
            this.OnError += callbacks.OnError;
            this.OnLeave += callbacks.OnLeave;
            this.OnSubscribed += callbacks.OnSubscribed;
            this.OnUnsubscribed += callbacks.OnUnsubscribed;
            this.OnUpdate += callbacks.OnUpdate;
            this.OnGeneralMessage += callbacks.OnGeneralMessage;

            // Add internal handlers
            this.OnConnected += this.HandleConnectEvent;
            this.OnSubscribed += this.HandleSubscribedEvent;
            this.OnUnsubscribed += this.HandleUnsubscribedEvent;
        }

        private byte[] CreateSubscribeRequest()
        {
            SubscribeRequest<T> msg = new SubscribeRequest<T>(targetQuery, null, RequestId, null);

            string json = JsonSerializer.Serialize(msg, JsonSerializer.DefaultOptions);

            byte[] bytes = Encoding.UTF8.GetBytes(json);

            return bytes;
        }
    }
}
#endif