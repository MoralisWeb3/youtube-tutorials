//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Moralis.WebGL.Web3Api.Core.Models
//{
//    public class RestRequest
//    {
//        public RestRequest(string resource, Method method)
//        { 
        
//        }

//        public List<Parameter> Parameters { get; }
//        public bool AlwaysMultipartFormData { get; set; }
//        public ISerializer JsonSerializer { get; set; }
//        public IXmlSerializer XmlSerializer { get; set; }
//        public RequestBody? Body { get; set; }
//        public Action<Stream> ResponseWriter { get; set; }
//        public Action<Stream, IHttpResponse> AdvancedResponseWriter { get; set; }
//        public bool UseDefaultCredentials { get; set; }
//        public List<FileParameter> Files { get; }
//        public Action<IRestResponse>? OnBeforeDeserialization { get; set; }
//        public string Resource { get; set; }
//        public DataFormat RequestFormat { get; set; }
//        public IList<DecompressionMethods> AllowedDecompressionMethods { get; }
//        public Action<IHttp>? OnBeforeRequest { get; set; }
//        [Obsolete("Add custom content handler instead. This property will be removed.")]
//        public string DateFormat { get; set; }
//        [Obsolete("Add custom content handler instead. This property will be removed.")]
//        public string XmlNamespace { get; set; }
//        public ICredentials? Credentials { get; set; }
//        public int Timeout { get; set; }
//        public Method Method { get; set; }
//        public int ReadWriteTimeout { get; set; }
//        public int Attempts { get; }
//    }
//}
