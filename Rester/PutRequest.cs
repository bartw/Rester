using System.Net.Http;
using System.Text;

namespace BeeWee.Rester
{
    public class PutRequest : ContentRequest
    {
        public PutRequest(string uri, string content, Encoding encoding, string mediaType) : base(HttpMethod.Put, uri, content, encoding, mediaType)
        {
        }
    }
}
