using Moralis.Platform.Abstractions;

namespace Moralis.Platform.Operations
{
    public class MoralisDeleteOperation : IMoralisFieldOperation
    {
        public string __op { get { return "Delete";  } }
        internal static object Token { get; } = new object { };

        public static MoralisDeleteOperation Instance { get; } = new MoralisDeleteOperation { };

        private MoralisDeleteOperation() { }

        public IMoralisFieldOperation MergeWithPrevious(IMoralisFieldOperation previous) => this;

        public object Apply(object oldValue, string key) => Token;
    }
}
