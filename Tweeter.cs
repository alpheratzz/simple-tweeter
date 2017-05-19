using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Http;
using System.Security.Cryptography;
using System.IO;

namespace RandomThings
{
    class Tweeter
    {
        HttpClient client;
        Dictionary<string, string> postParams;

        //not sure if i even need to store them too
        readonly string oauthToken;
        readonly string oauthConsumerKey;

        const string tweetApiAddress = @"https://api.twitter.com/1.1/";
        const string uploadApiAddress = @"https://upload.twitter.com/1.1/";

        string logPath;

        HMACSHA1 hmac;

        public Tweeter(string oauthConfigPath, string logPath)
        {
            this.logPath = logPath;

            try
            {
                using (StreamReader reader = new StreamReader(oauthConfigPath))
                {
                    oauthToken = reader.ReadLine();
                    oauthConsumerKey = reader.ReadLine();
                    string tokenSecret = reader.ReadLine();
                    string consumerSecret = reader.ReadLine();

                    Setup(tokenSecret, consumerSecret);
                }
            }
            catch (FileNotFoundException ex)
            {
                throw new FileNotFoundException($"Config file {ex.FileName} was not found");
            }
        }

        void Setup(string oauthTokenSecret, string oauthConsumerSecret)
        {
            hmac = new HMACSHA1(Encoding.ASCII.GetBytes($"{PercentEncode(oauthConsumerSecret)}&{PercentEncode(oauthTokenSecret)}"));

            client = new HttpClient();
            client.BaseAddress = new Uri(tweetApiAddress);

            postParams = new Dictionary<string, string>();
            
            using (StreamWriter logger = new StreamWriter(logPath, true))
            {
                logger.WriteLine("\n" + DateTime.Now.ToString() + "\n");
            }
        }

        public async Task<string> Tweet(string text)
        {
            SetDefaultPostParameters();

            //values that may or must change
            postParams["status"] = text;
            postParams["trim_user"] = "true";

            AuthorizeRequest();
            
            var content = new FormUrlEncodedContent(postParams.Where(pair => !pair.Key.StartsWith("oauth_")));

            HttpResponseMessage response = await client.PostAsync(@"statuses/update.json", content);
            using (StreamWriter logger = new StreamWriter(logPath, true))
            {
                logger.WriteLine(response.Content.ReadAsStringAsync().Result + "\n");
            }
            return response.StatusCode.ToString();
        }

        //setting values that are going to be the same for any post request
        void SetDefaultPostParameters()
        {
            postParams.Clear();

            postParams.Add("oauth_token", oauthToken);
            postParams.Add("oauth_consumer_key", oauthConsumerKey);
            postParams.Add("oauth_signature_method", "HMAC-SHA1");
            postParams.Add("oauth_version", "1.0");
        }

        void AuthorizeRequest()
        {
            //setting unique oauth parameters
            postParams["oauth_nonce"] = GenerateNonce();
            postParams["oauth_timestamp"] = GetUnixTimestamp(DateTime.UtcNow).ToString();

            //creating signature            
            postParams["oauth_signature"] = GetSignature(postParams);

            //setting the Authorization header
            var tmp = postParams.Where(pair => pair.Key.StartsWith("oauth_"))
                .Select(pair => $"{PercentEncode(pair.Key)}=\"{PercentEncode(pair.Value)}\"");
            string auth_header = "OAuth " + string.Join(", ", tmp);
            client.DefaultRequestHeaders.Remove("Authorization");
            client.DefaultRequestHeaders.Add("Authorization", auth_header);
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

            return System.Convert.ToBase64String(hmac.ComputeHash(Encoding.ASCII.GetBytes(forSigning)));
        }

        //some utility one-line methods lol

        static int GetUnixTimestamp(DateTime utcDate)
        {
            return (int)(utcDate - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        }

        static string PercentEncode(string text)
        {
            return Uri.EscapeDataString(text);
        }

        static string GenerateNonce()
        {
            return Guid.NewGuid().ToString("N");
        }
    }    
}
