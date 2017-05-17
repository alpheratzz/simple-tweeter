using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Http;
using System.Security.Cryptography;

namespace RandomThings
{
    class Tweeter
    {
        HttpClient client;
        Dictionary<string, string> authParams;

        readonly string oauthToken;
        readonly string oauthConsumerKey;
        readonly string oauthTokenSecret;
        readonly string oauthConsumerSecret;

        public Tweeter(string oauthToken, string oauthConsumerKey, string oauthTokenSecret, string oauthConsumerKeySecret)
        {
            this.oauthToken = oauthToken;
            this.oauthConsumerKey = oauthConsumerKey;
            this.oauthTokenSecret = oauthTokenSecret;
            this.oauthConsumerSecret = oauthConsumerKeySecret;

            client = new HttpClient();
            client.BaseAddress = new Uri(@"https://api.twitter.com/1.1/");

            authParams = new Dictionary<string, string>();
        }

        public void Tweet(string text)
        {
            authParams.Clear();

            authParams.Add("oauth_token", oauthToken);
            authParams.Add("oauth_cosumer_key", oauthConsumerKey);
            authParams.Add("oauth_signature_method", "HMAC-SHA1");
            authParams.Add("oauth_version", "1.0");
            authParams.Add("oauth_nonce", GetNonce());
            authParams.Add("oauth_timestamp", GetUnixTimestamp(DateTime.UtcNow).ToString());

            //this is a bit awkward..
            authParams.Add("status", text);
            authParams.Add("oauth_signature", GetSignature(authParams));

            authParams.Remove("status");  
            
                      

            //relative uri is 
            //statuses/update.json
        }

        string GetSignature(Dictionary<string, string> parameters)
        {
            //twitter requires to sort params by the encoded key
            //that's why this looks a bit messy
            var tmp = parameters.Select(pair => new KeyValuePair<string, string>(PercentEncode(pair.Key), PercentEncode(pair.Value)))
                    .OrderBy(pair => pair.Key).Select(pair => $"{pair.Key}={pair.Value}");
            string param_string = string.Join("&", tmp);

            //seriously, they want me to do so much encoding
            string forSigning = $"POST&{PercentEncode(client.BaseAddress.ToString() + @"statuses/update.json")}&{PercentEncode(param_string)}";

            //shouldn't really store the key
            //although i have it's parts stored lol
            HMACSHA1 hmac = new HMACSHA1(Encoding.ASCII.GetBytes($"{PercentEncode(oauthConsumerSecret)}&{PercentEncode(oauthTokenSecret)}"));

            return System.Convert.ToBase64String(hmac.ComputeHash(Encoding.ASCII.GetBytes(forSigning)));
        }

        static int GetUnixTimestamp(DateTime utcDate)
        {
            return (int)(utcDate - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        }

        public static string PercentEncode(string text)
        {
            return Uri.EscapeDataString(text);
        }

        static string GetNonce()
        {
            return Guid.NewGuid().ToString("N");
        }
    }    
}
