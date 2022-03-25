using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace WalletConnectSharp.Core.Events
{
    public class EventManager<T, TEventArgs> : IEventProvider where TEventArgs : IEvent<T>, new()
    {
        private static EventManager<T, TEventArgs> _instance;

        public EventHandlerMap<TEventArgs> EventTriggers;

        public static EventManager<T, TEventArgs> Instance
        {
            get 
            {
                if (_instance == null)
                {
                    _instance = new EventManager<T, TEventArgs>();
                }
                
                return _instance; 
            }
        }

        private EventManager()
        {
            EventTriggers = new EventHandlerMap<TEventArgs>(CallbackBeforeExecuted);
            
            EventFactory.Instance.Register<T>(this);
        }
        
        private void CallbackBeforeExecuted(object sender, TEventArgs e)
        {
        }

        public void PropagateEvent(string topic, string responseJson)
        {
            if (EventTriggers.Contains(topic))
            {
                var eventTrigger = EventTriggers[topic];

                if (eventTrigger != null)
                {
                    var response = JsonConvert.DeserializeObject<T>(responseJson);
                    var eventArgs = new TEventArgs();
                    eventArgs.SetData(response);
                    eventTrigger(this, eventArgs);
                }
            }
        }
    }
}