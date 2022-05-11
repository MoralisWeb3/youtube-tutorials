using System;
using System.Collections.Generic;

namespace WalletConnectSharp.Core.Events
{
    public class EventFactory
    {
        private static EventFactory _instance;
        
        private Dictionary<Type, IEventProvider> _eventProviders = new Dictionary<Type, IEventProvider>();

        public static EventFactory Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new EventFactory();
                }
                return _instance;
            }
        }

        public void Register<T>(IEventProvider provider)
        {
            Type t = typeof(T);

            if (_eventProviders.ContainsKey(t))
                return;
            
            _eventProviders.Add(t, provider);
        }

        public IEventProvider ProviderFor<T>()
        {
            Type t = typeof(T);
            if (_eventProviders.ContainsKey(t))
                return _eventProviders[t];

            return null;
        }
    }
}