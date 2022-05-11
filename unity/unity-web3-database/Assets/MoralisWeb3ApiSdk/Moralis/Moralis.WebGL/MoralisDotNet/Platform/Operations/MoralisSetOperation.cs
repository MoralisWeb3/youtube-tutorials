using Moralis.WebGL.Platform.Abstractions;

namespace Moralis.WebGL.Platform.Operations
{
    class MoralisSetOperation : IMoralisFieldOperation
    {
        public object Value { get; private set; }

        public MoralisSetOperation(object value) => Value = value;

        public IMoralisFieldOperation MergeWithPrevious(IMoralisFieldOperation previous) => this;

        public object Apply(object oldValue, string key) => Value;


    }
}
