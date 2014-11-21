using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace BeeWee.Rester
{
    public class Client : IClient
    {
        public async Task<HttpResponseMessage> ExecuteAsync(Request request)
        {
            var client = new HttpClient();
            return await client.SendAsync(CreateHttpRequest(request));
        }

        private HttpRequestMessage CreateHttpRequest(Request request)
        {
            var httpRequest = new HttpRequestMessage(request.Method, new Uri(request.Uri));
            var headers = MergeHeaders(request);

            if (headers != null && headers.Count > 0)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            return httpRequest;
        }

        private Dictionary<string, string> MergeHeaders(Request request)
        {
            Dictionary<string, string> oauthHeaders;

            if (request.SignatureMethod == SignatureMethod.HMACSHA1)
            {
                oauthHeaders = OAuthHelper.GenerateOAuthHeaders(request);
            }
            else
            {
                oauthHeaders = OAuthHelper.GeneratePlainOAuthHeaders(request);
            }

            if (request.Headers == null)
            {
                return oauthHeaders;
            }
            else
            {
                return Utils.Merge(new Dictionary<string, string>[] { request.Headers, oauthHeaders });
            }
        }
    }
}
