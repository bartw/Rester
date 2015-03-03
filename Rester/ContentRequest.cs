using System.Net.Http;
using System.Text;

namespace BeeWee.Rester
{
    public abstract class ContentRequest : Request
    {
        public string Content { get; private set; }
        public Encoding Encoding { get; private set; }
        public string MediaType { get; private set; }

        public ContentRequest(HttpMethod method, string uri, string content, Encoding encoding, string mediaType)
            : base(method, uri)
        {
            Content = content;
            Encoding = encoding;
            MediaType = mediaType;
        }
    }
}
