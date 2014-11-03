using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace BeeWee.Rester
{
    public class Client
    {
        private Throttler _throttler;
        private SignatureMethod _signatureMethod;

        public Client() : this(SignatureMethod.PLAINTEXT)
        {
        }

        public Client(SignatureMethod signatureMethod)
        {
            _signatureMethod = signatureMethod;
            _throttler = null;
        }

        public Client(int maxCount, TimeSpan resetSpan) : this(SignatureMethod.PLAINTEXT, maxCount, resetSpan)
        {
        }

        public Client(SignatureMethod signatureMethod, int maxCount, TimeSpan resetSpan)
        {
            _signatureMethod = signatureMethod;

            if (maxCount < 1)
            {
                throw new ArgumentOutOfRangeException("maxCount", "maxCount should be bigger than 0.");
            }

            _throttler = new Throttler(maxCount, resetSpan);
        }

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

            Dictionary<string, string> oauthHeaders;
            
            if (_signatureMethod == SignatureMethod.HMACSHA1)
            {
                oauthHeaders = OAuthHelper.GenerateOAuthHeaders(uri, method.ToString(), consumerKey, consumerSecret, oauthKey, oauthSecret, verifier);
            }
            else
            {
                oauthHeaders = OAuthHelper.GeneratePlainOAuthHeaders(uri, method.ToString(), consumerKey, consumerSecret, oauthKey, oauthSecret, verifier);
            }

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


            if (_throttler != null)
            {
                return await _throttler.RunAsync<HttpRequestMessage, HttpResponseMessage>(client.SendAsync, request);
            }
            else
            {
                return await client.SendAsync(request);
            }            
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
