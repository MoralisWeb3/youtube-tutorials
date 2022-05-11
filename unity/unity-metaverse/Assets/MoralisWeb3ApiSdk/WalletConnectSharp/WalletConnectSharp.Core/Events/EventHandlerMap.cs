using System;
using System.Collections.Generic;

namespace WalletConnectSharp.Core.Events
{
    public class EventHandlerMap<TEventArgs>
    {
        private Dictionary<string, EventHandler<TEventArgs>> mapping =
            new Dictionary<string, EventHandler<TEventArgs>>();

        private EventHandler<TEventArgs> BeforeEventExecuted;

        public EventHandlerMap(EventHandler<TEventArgs> callbackBeforeExecuted)
        {
            if (callbackBeforeExecuted == null)
            {
                callbackBeforeExecuted = CallbackBeforeExecuted;
            }

            this.BeforeEventExecuted = callbackBeforeExecuted;
        }

        private void CallbackBeforeExecuted(object sender, TEventArgs e)
        {
        }

        public EventHandler<TEventArgs> this[string topic]
        {
            get
            {
                if (!mapping.ContainsKey(topic))
                {
                    mapping.Add(topic, BeforeEventExecuted);
                }
                
                return mapping[topic];
            }
            set
            {
                if (mapping.ContainsKey(topic))
                {
                    mapping.Remove(topic);
                }
                
                mapping.Add(topic, value);
            }
        }

        public bool Contains(string topic)
        {
            return mapping.ContainsKey(topic);
        }
    }
}