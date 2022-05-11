using Moralis.Platform.Abstractions;
using Moralis.Platform.Objects;
using Moralis.Platform.Queries.Live;
using System;
using System.Collections.Generic;
using System.Text;

namespace Moralis
{
    public class MoralisLiveQueryCallbacks<T> : ILiveQueryCallbacks<T> where T : MoralisObject
    {

        /// <summary>
        /// Called when a connected event is returned from the server.
        /// </summary>
        public event LiveQueryConnectedHandler OnConnectedEvent;

        /// <summary>
        /// Called when a subscribed event is returned from the server.
        /// </summary>
        public event LiveQuerySubscribedHandler OnSubscribedEvent;

        /// <summary>
        /// Called when a create event is returned from the server.
        /// </summary>
        public event LiveQueryActionHandler<T> OnCreateEvent;

        /// <summary>
        /// Called when a delete event is returned from the server.
        /// </summary>
        public event LiveQueryActionHandler<T> OnDeleteEvent;

        /// <summary>
        /// Called when a enter event is returned from the server.
        /// </summary>
        public event LiveQueryActionHandler<T> OnEnterEvent;

        /// <summary>
        /// Called when a leave event is returned from the server.
        /// </summary>
        public event LiveQueryActionHandler<T> OnLeaveEvent;

        /// <summary>
        /// Called when a update event is returned from the server.
        /// </summary>
        public event LiveQueryActionHandler<T> OnUpdateEvent;

        /// <summary>
        /// Called when a ubsubscribed event is received.
        /// </summary>
        public event LiveQueryUnsubscribedHandler OnUnsubscribedEvent;

        /// <summary>
        /// Called when an ErrorMEssage is received.
        /// </summary>
        public event LiveQueryErrorHandler OnErrorEvent;

        public event LiveQueryGeneralMessageHandler OnGeneralMessageEvent;

        public void OnConnected()
        {
            if (OnConnectedEvent != null)
            {
                OnConnectedEvent();
            }
        }

        public void OnCreate(T item, int requestId)
        {
            if (OnCreateEvent != null) 
            {
                OnCreateEvent(item, requestId);
            }
        }

        public void OnDelete(T item, int requestId)
        {
            if (OnDeleteEvent != null)
            {
                OnDeleteEvent(item, requestId);
            }
        }

        public void OnEnter(T item, int requestId)
        {
            if (OnEnterEvent != null)
            {
                OnEnterEvent(item, requestId);
            }
        }

        public void OnError(ErrorMessage em)
        {
            if (OnErrorEvent != null)
            {
                OnErrorEvent(em);
            }
        }

        public void OnLeave(T item, int requestId)
        {
            if (OnLeaveEvent != null)
            {
                OnLeaveEvent(item, requestId);
            }
        }

        public void OnSubscribed(int requestId)
        {
            if (OnSubscribedEvent != null)
            {
                OnSubscribedEvent(requestId);
            }
        }

        public void OnUnsubscribed(int requestId)
        {
            if (OnUnsubscribedEvent != null)
            {
                OnUnsubscribedEvent(requestId);
            }
        }

        public void OnUpdate(T item, int requestId)
        {
            if (OnUpdateEvent != null)
            {
                OnUpdateEvent(item, requestId);
            }
        }

        public void OnGeneralMessage(string text)
        {
            if (OnGeneralMessageEvent != null)
            {
                OnGeneralMessageEvent(text);
            }
        }
    }
}
