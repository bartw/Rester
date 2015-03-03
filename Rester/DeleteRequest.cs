using System.Net.Http;

namespace BeeWee.Rester
{
    public class DeleteRequest : Request
    {
        public DeleteRequest(string uri)
            : base(HttpMethod.Delete, uri)
        {
        }
    }
}
