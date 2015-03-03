using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BeeWee.Rester
{
    public class Client : IClient
    {
        public async Task<HttpResponseMessage> ExecuteRawAsync(Request request, IAuthenticator authenticator = null)
        {
            var client = new HttpClient();
            return await client.SendAsync(CreateHttpRequest(request, authenticator));
        }

        private HttpRequestMessage CreateHttpRequest(Request request, IAuthenticator authenticator)
        {
            var httpRequest = new HttpRequestMessage(request.Method, new Uri(request.Uri));

            if (request.Method == HttpMethod.Post || request.Method == HttpMethod.Put)
            {
                var contentRequest = request as ContentRequest;

                if (contentRequest != null && !string.IsNullOrEmpty(contentRequest.Content) && contentRequest.Encoding != null && !string.IsNullOrEmpty(contentRequest.MediaType))
                {
                    httpRequest.Content = new StringContent(contentRequest.Content, contentRequest.Encoding, contentRequest.MediaType);
                }
            }

            var headers = MergeHeaders(request.Headers, authenticator != null ? authenticator.GetHeaders(request) : null);

            if (headers != null && headers.Count > 0)
            {
                foreach (var header in headers)
                {
                    httpRequest.Headers.Add(header.Key, header.Value);
                }
            }

            return httpRequest;
        }

        private Dictionary<string, string> MergeHeaders(Dictionary<string, string> requestHeaders, Dictionary<string, string> authenticationHeaders)
        {
            if (requestHeaders == null && authenticationHeaders == null)
            {
                return null;
            }
            else if (requestHeaders == null)
            {
                return authenticationHeaders;
            }
            else if (authenticationHeaders == null)
            {
                return requestHeaders;
            }
            else
            {
                return Utils.Merge(new Dictionary<string, string>[] { requestHeaders, authenticationHeaders });
            }
        }
    }
}
