using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace BeeWee.Rester
{
    public class Client
    {
        public async Task<HttpResponseMessage> Get(string uri)
        {
            return await Get(uri, null);
        }

        public async Task<HttpResponseMessage> Get(string uri, Dictionary<string, string> headers)
        {
            var client = new HttpClient();
            var request = CreateRequest(new Uri(uri), HttpMethod.Get, headers);
            return await client.SendAsync(request);
        }

        public async Task<HttpResponseMessage> Get(string uri, Dictionary<string, string> headers, string consumerKey, string consumerSecret)
        {
            return await Get(uri, headers, consumerKey, consumerSecret, null, null, null);
        }

        public async Task<HttpResponseMessage> Get(string uri, Dictionary<string, string> headers, string consumerKey, string consumerSecret, string oauthKey, string oauthSecret)
        {
            return await Get(uri, headers, consumerKey, consumerSecret, oauthKey, oauthSecret, null);
        }

        public async Task<HttpResponseMessage> Get(string uri, Dictionary<string, string> headers, string consumerKey, string consumerSecret, string oauthKey, string oauthSecret, string verifier)
        {
            var method = HttpMethod.Get;

            var oauthHeaders = OAuthHelper.GenerateOAuthHeaders(uri, method.ToString(), consumerKey, consumerSecret, oauthKey, oauthSecret, verifier);

            if (headers == null)
            {
                headers = oauthHeaders;
            }
            else
            {
                headers = Utils.Merge(new Dictionary<string, string>[] { headers, oauthHeaders });
            }

            var client = new HttpClient();
            var request = CreateRequest(new Uri(uri), method, headers);
            return await client.SendAsync(request);
        }

        private HttpRequestMessage CreateRequest(Uri uri, HttpMethod method, Dictionary<string, string> headers)
        {
            var request = new HttpRequestMessage(method, uri);

            if (headers != null && headers.Count > 0)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            return request;
        }
    }
}
