namespace WalletConnectSharp.Core.Models
{
    public class Address
    {
        public string _address;

        public Address(string address)
        {
            this._address = address;
        }

        public static implicit operator string(Address address) => address._address;
        public static explicit operator Address(string address) => new Address(address);

        public override string ToString()
        {
            return _address;
        }
    }
}