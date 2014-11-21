using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace BeeWee.Rester
{
    public class ThrottledClient : Client
    {
        private Throttler _throttler;

        public ThrottledClient(int maxCount, TimeSpan resetSpan)
        {
            if (maxCount < 1)
            {
                throw new ArgumentOutOfRangeException("maxCount", "maxCount should be bigger than 0.");
            }

            _throttler = new Throttler(maxCount, resetSpan);
        }

        public new async Task<HttpResponseMessage> Execute(Request request)
        {
            return await _throttler.RunAsync<Request, HttpResponseMessage>(base.Execute, request);
        }
    }
}
