using System.Collections.Generic;
using System.Net.Http;

namespace BeeWee.Rester
{
    public class Request
    {
        public HttpMethod Method { get; set; }
        public string Uri { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public SignatureMethod SignatureMethod { get; set; }
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
        public string OAuthKey { get; set; }
        public string OAuthSecret { get; set; }
        public string Verifier { get; set; }
    }
}
