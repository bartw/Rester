using System.Collections.Generic;
using System.Net.Http;

namespace BeeWee.Rester
{
    public abstract class Request
    {
        public HttpMethod Method { get; private set; }
        public string Uri { get; private set; }
        public Dictionary<string, string> Headers { get; private set; }

        public Request(HttpMethod method, string uri)
        {
            Method = method;
            Uri = uri;
            Headers = new Dictionary<string, string>();
        }
    }
}
