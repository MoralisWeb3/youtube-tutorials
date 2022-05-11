
namespace Moralis.Platform.Queries.Live
{
    public class UnscubscribeRequest : QueryEventMessage
    {
        public UnscubscribeRequest() => (op, requestId) = (OperationTypes.unsubscribe.ToString(), 0);

        public UnscubscribeRequest(int requestId) => (op, this.requestId) = (OperationTypes.unsubscribe.ToString(), requestId);

        /// <summary>
        /// REQUIRED: Client generated 
        /// </summary>
        public int requestId { get; set; }
    }
}
