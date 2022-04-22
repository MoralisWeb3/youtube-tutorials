using System.Collections.Generic;
using System.Runtime.Serialization;
using Moralis.WebGL.Hex.HexTypes;

namespace Moralis.WebGL.Models
{
    [DataContract]
    public class TransactionInput : CallInput
    {
        public TransactionInput()
        {
        }

        public TransactionInput(string data, string addressTo) : base(data, addressTo)
        {
        }

        public TransactionInput(string data, string addressTo, HexBigInteger value) : base(data, addressTo, value)
        {
        }

        public TransactionInput(string data, string addressTo, string addressFrom, HexBigInteger gas,
            HexBigInteger value) : base(data, addressTo, addressFrom, gas, value)
        {
        }

        public TransactionInput(string data, string addressFrom, HexBigInteger gas,
            HexBigInteger value) : base(data, addressFrom, gas, value)
        {
        }

        public TransactionInput(string data, string addressTo, string addressFrom, HexBigInteger gas, HexBigInteger gasPrice,
          HexBigInteger value) : base(data, addressTo, addressFrom, gas, gasPrice, value)
        {
        }

        public TransactionInput(string data, HexBigInteger gas, string addressFrom) : base(data, gas, addressFrom)
        {
        }

        public TransactionInput(HexBigInteger type, string data, string addressTo, string addressFrom, HexBigInteger gas, HexBigInteger value, HexBigInteger maxFeePerGas, HexBigInteger maxPriorityFeePerGas)
            : base(data, addressTo, addressFrom, gas, value, type, maxFeePerGas, maxPriorityFeePerGas)
        {

        }


        /// <summary>
        ///     nonce: QUANTITY - (optional) Integer of a nonce. This allows to overwrite your own pending transactions that use
        ///     the same nonce.
        /// </summary>
        [DataMember(Name = "nonce")]
        public HexBigInteger Nonce { get; set; }
    }

}
