using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace BeeWee.Rester
{
    public class OAuth1Authenticator : IAuthenticator
    {
        public SignatureMethod Method { get; private set; }
        public string ConsumerKey { get; private set; }
        public string ConsumerSecret { get; private set; }
        public string TokenKey { get; private set; }
        public string TokenSecret { get; private set; }
        public string Verifier { get; private set; }

        public OAuth1Authenticator(SignatureMethod method, string consumerKey, string consumerSecret, string tokenKey = null, string tokenSecret = null, string verifier = null)
        {
            Method = method;
            ConsumerKey = consumerKey;
            ConsumerSecret = consumerSecret;
            TokenKey = tokenKey;
            TokenSecret = tokenSecret;
            Verifier = verifier;
        }

        public Dictionary<string, string> GetHeaders(Request request)
        {
            switch (Method)
            {
                case SignatureMethod.PLAINTEXT:
                    return GeneratePlainOAuthHeaders(request);
                case SignatureMethod.HMACSHA1:
                    return GenerateHmacOAuthHeaders(request);
                default:
                    return null;
            }
        }

        private Dictionary<string, string> GeneratePlainOAuthHeaders(Request request)
        {
            var random = new Random();
            var headers = new Dictionary<string, string>();

            StringBuilder sb = new StringBuilder();

            sb.Append("OAuth ");

            sb.AppendFormat("{0}=\"{1}\",", "oauth_consumer_key", ConsumerKey);
            sb.AppendFormat("{0}=\"{1}\",", "oauth_nonce", random.Next().ToString());

            if (!string.IsNullOrEmpty(TokenKey))
            {
                sb.AppendFormat("{0}=\"{1}\",", "oauth_token", TokenKey);
            }

            sb.AppendFormat("{0}=\"{1}{2}", "oauth_signature", ConsumerSecret, "&");

            if (!string.IsNullOrEmpty(TokenSecret))
            {
                sb.AppendFormat("{0}", TokenSecret);
            }

            sb.Append("\",");

            sb.AppendFormat("{0}=\"{1}\",", "oauth_signature_method", "PLAINTEXT");
            sb.AppendFormat("{0}=\"{1}\",", "oauth_timestamp", DateTime.UtcNow.ToUnixTime().ToString());

            if (!string.IsNullOrEmpty(Verifier))
            {
                sb.AppendFormat("{0}=\"{1}\",", "oauth_verifier", Verifier);
            }

            sb.AppendFormat("{0}=\"{1}\"", "oauth_version", "1.0");


            headers.Add(HttpRequestHeader.Authorization.ToString(), sb.ToString());

            return headers;
        }

        private Dictionary<string, string> GenerateHmacOAuthHeaders(Request request)
        {
            var random = new Random();
            var headers = new Dictionary<string, string>();

            headers.Add("oauth_consumer_key", ConsumerKey);
            headers.Add("oauth_nonce", random.Next().ToString());
            headers.Add("oauth_timestamp", DateTime.UtcNow.ToUnixTime().ToString());
            headers.Add("oauth_signature_method", "HMAC-SHA1");
            headers.Add("oauth_version", "1.0");

            if (!string.IsNullOrEmpty(TokenKey))
            {
                headers.Add("oauth_token", TokenKey);
            }

            if (!string.IsNullOrEmpty(Verifier))
            {
                headers.Add("oauth_verifier", Verifier);
            }

            StringBuilder sb = new StringBuilder();

            sb.Append("OAuth ");

            foreach (var header in headers)
            {
                sb.Append(string.Format("{0}=\"{1}\",", header.Key, header.Value));
            }

            sb.Append(string.Format("oauth_signature=\"{0}\"", GenerateSignature(request.Uri, request.Method.ToString(), ConsumerSecret, TokenKey, TokenSecret, headers)));

            headers.Add(HttpRequestHeader.Authorization.ToString(), sb.ToString());

            return headers;
        }

        private static string GenerateSignature(string uri, string method, string consumerSecret, string tokenKey, string tokenSecret, Dictionary<string, string> headers)
        {
            var hmacKeyBase = consumerSecret.UrlEncode() + "&" + ((tokenSecret == null) ? "" : tokenSecret).UrlEncode();

            var orderedHeaders = headers.OrderBy(p => p.Key).ThenBy(p => p.Value);

            StringBuilder stringParameter = new StringBuilder();
            foreach (var header in orderedHeaders)
            {
                stringParameter.Append(string.Format("{0}={1}&", header.Key.UrlEncode(), header.Value.UrlEncode()));
            }

            stringParameter.Remove(stringParameter.Length - 1, 1);

            string signatureBase = string.Format("{0}&{1}&{2}",
                method.ToUpper(),
                new Uri(uri).GetComponents(UriComponents.SchemeAndServer | UriComponents.Path, UriFormat.Unescaped).UrlEncode(),
                stringParameter.ToString().UrlEncode());

            var hash = HmacSha1Sign(Encoding.UTF8.GetBytes(hmacKeyBase), signatureBase);

            return Convert.ToBase64String(hash).UrlEncode();
        }

        private static byte[] HmacSha1Sign(byte[] keyBytes, string message)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message);
            MacAlgorithmProvider objMacProv = MacAlgorithmProvider.OpenAlgorithm("HMAC_SHA1");
            CryptographicKey hmacKey = objMacProv.CreateKey(keyBytes.AsBuffer());
            IBuffer buffHMAC = CryptographicEngine.Sign(hmacKey, messageBytes.AsBuffer());
            return buffHMAC.ToArray();

        }
    }
}
