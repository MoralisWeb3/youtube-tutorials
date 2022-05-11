using System;

namespace WalletConnectSharp.Core.Events
{
    public class GenericEvent<T> : IEvent<T>
    {
        public T Response { get; private set; }

        public void SetData(T response)
        {
            if (Response != null)
            {
                throw new ArgumentException("Response was already set");
            }
            
            Response = response;
        }
    }
}