using System;
using WalletConnectSharp.Core.Events;

namespace WalletConnectSharp.Core.Network
{
    public class TransportFactory
    {
        private static TransportFactory _instance;

        public static TransportFactory Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new TransportFactory();
                return _instance;
            }
        }
        
        private TransportFactory() {}

        private Func<EventDelegator, ITransport> _defaultBuilder;

        public ITransport BuildDefaultTransport(EventDelegator eventDelegator)
        {
            if (_defaultBuilder != null)
            {
                return _defaultBuilder(eventDelegator);
            }

            return null;
        }
        
        public void RegisterDefaultTransport(Func<EventDelegator, ITransport> builder)
        {
            if (this._defaultBuilder != null)
            {
                throw new ArgumentException("A Default Transport has already been defined!");
            }
            
            this._defaultBuilder = builder;
        }
    }
}