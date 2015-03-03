using System.Net.Http;
using System.Text;

namespace BeeWee.Rester
{
    public class PostRequest : ContentRequest
    {
        public PostRequest(string uri, string content, Encoding encoding, string mediaType)
            : base(HttpMethod.Post, uri, content, encoding, mediaType)
        {
        }
    }
}
