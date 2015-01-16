using System.Net.Http;

namespace BeeWee.Rester
{
    public class PutRequest : Request
    {
        public string JSonContent { get; set; }

        public PutRequest(string uri) : base(HttpMethod.Put, uri)
        {
        }
    }
}
