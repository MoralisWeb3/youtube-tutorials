using System.Collections.Generic;
using Moralis.WebGL.Platform.Objects;

namespace Moralis.WebGL.Platform.Queries.Live
{
    public class SubscribeRequest<T> : QueryEventMessage where T : MoralisObject
    {
        private MoralisQuery<T> _query;

        /// <summary>
        /// REQUIRED: Moralis Application Id
        /// </summary>
        public string applicationId { get; set; }

        /// <summary>
        /// REQUIRED: Client generated 
        /// </summary>
        public int requestId { get; set; }

        /// <summary>
        /// OPTIONAL: Moralis current user session token.
        /// </summary>
        public string sessionToken { get; set; }

        /// <summary>
        /// Query parameter values sent to 
        /// </summary>
        public IDictionary<string, object> query { get; private set; }

        internal MoralisQuery<T> OriginalQuery
        { 
            get { return _query; }
            set
            {
                _query = value;

                query = _query.BuildParameters(true);
            }
        }

        public SubscribeRequest() => op = OperationTypes.subscribe.ToString();

        public SubscribeRequest(MoralisQuery<T> targetQuery, string applicationId, int requestId, string sessionToken = null) => 
            (this.OriginalQuery, this.applicationId, this.requestId, op, this.sessionToken) = 
            (targetQuery, applicationId, requestId, OperationTypes.subscribe.ToString(), sessionToken);
        
    }
}
