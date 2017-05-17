using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

using System.Net.Http;
using System.Net.Http.Headers;

namespace RandomThings
{
    class OAuthThing
    {
        HttpClient client;

        string consumerKey;
        string nonce;
        string signature;
        string signatureMethod;
        string timestamp;
        string oauthToken;
        string oauthVersion;

        public OAuthThing()
        {
            client = new HttpClient();

            signatureMethod = "HMAC_SHA1";
            oauthVersion = "1.0";
            nonce = Guid.NewGuid().ToString("N");
            timestamp = (DateTime.Now - new DateTime(1970, 1, 1)).TotalSeconds.ToString();
        }

        void getSignature(string httpMethod)
        {
            HMACSHA1 hmac = new HMACSHA1();
            string key = PercentEncode("key");
            byte[] keyBytes = Encoding.ASCII.GetBytes(key);
            hmac.ComputeHash(Encoding.ASCII.GetBytes(PercentEncode("POST&https://hello.com&text=aaa")));
            //hmac.Key = ...;
        }

        void getSignature(HttpMethod method, string requestUrl)
        {
            client.BaseAddress = new Uri("");
            client.DefaultRequestHeaders.Add("a", "b");

            HttpRequestMessage message = new HttpRequestMessage(method, "request/uri");
        }

        public static string PercentEncode(string something)
        {
            return Uri.EscapeDataString(something);
        }
    }
}
