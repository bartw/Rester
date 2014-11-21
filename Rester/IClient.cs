using System.Net.Http;
using System.Threading.Tasks;

namespace BeeWee.Rester
{
    public interface IClient
    {
        Task<HttpResponseMessage> ExecuteAsync(Request request);
    }
}
