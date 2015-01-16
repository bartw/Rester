using System.Net.Http;

namespace BeeWee.Rester
{
    public class GetRequest : Request
    {
        public GetRequest(string uri) : base(HttpMethod.Get, uri)
        {

        }
    }
}
