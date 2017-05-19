using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Http;
using System.Security.Cryptography;
using System.IO;
using System.Drawing;

namespace RandomThings
{
    class Tweeter
    {
        HttpClient client;
        Dictionary<string, string> postParams;

        //not sure if i even need to store them too
        readonly string oauthToken;
        readonly string oauthConsumerKey;

        readonly Uri tweetApiAddress = new Uri(@"https://api.twitter.com/1.1/");
        readonly Uri uploadApiAddress = new Uri(@"https://upload.twitter.com/1.1/");

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
            client.BaseAddress = tweetApiAddress;

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

            AuthorizeRequest(false);
            
            var content = new FormUrlEncodedContent(postParams.Where(pair => !pair.Key.StartsWith("oauth_")));

            client.BaseAddress = tweetApiAddress;
            HttpResponseMessage response = await client.PostAsync(@"statuses/update.json", content);
            using (StreamWriter logger = new StreamWriter(logPath, true))
            {
                logger.WriteLine(response.Content.ReadAsStringAsync().Result + "\n");
            }
            return response.StatusCode.ToString();
        }

        public async Task<HttpResponseMessage> UploadImage(string path)
        {
            client.BaseAddress = uploadApiAddress;

            SetDefaultPostParameters();
            AuthorizeRequest(true);

            if (!IsValidImage(path))
                return null;
            byte[] data = File.ReadAllBytes(path);
            //postParams["media"] = string.Empty;

            var content = new MultipartFormDataContent();
            content.Add(new ByteArrayContent(data), "media");            
            return await client.PostAsync(@"media/upload.json", content);
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

        void AuthorizeRequest(bool mediaUpload)
        {
            //setting unique oauth parameters
            postParams["oauth_nonce"] = GenerateNonce();
            postParams["oauth_timestamp"] = GetUnixTimestamp(DateTime.UtcNow).ToString();

            //creating signature            
            postParams["oauth_signature"] = GetSignature(postParams, mediaUpload);

            //setting the Authorization header
            var tmp = postParams.Where(pair => pair.Key.StartsWith("oauth_"))
                .Select(pair => $"{PercentEncode(pair.Key)}=\"{PercentEncode(pair.Value)}\"");
            string auth_header = "OAuth " + string.Join(", ", tmp);
            client.DefaultRequestHeaders.Remove("Authorization");
            client.DefaultRequestHeaders.Add("Authorization", auth_header);
        }

        string GetSignature(Dictionary<string, string> parameters, bool mediaUpload)
        {
            //twitter requires to sort params by the encoded key
            //that's why this looks a bit messy
            var tmp = (mediaUpload ? parameters.Where(pair => pair.Key.StartsWith("oauth_")) : parameters)
                    .Select(pair => new KeyValuePair<string, string>(PercentEncode(pair.Key), PercentEncode(pair.Value)))
                    .OrderBy(pair => pair.Key).Select(pair => $"{pair.Key}={pair.Value}");
            string param_string = string.Join("&", tmp);

            string baseString;
            //seriously, they want me to do so much encoding
            if (!mediaUpload)
                baseString = $"POST&{PercentEncode(client.BaseAddress.ToString() + @"statuses/update.json")}&{PercentEncode(param_string)}";
            else
                baseString = $"POST&{PercentEncode(client.BaseAddress.ToString() + @"media/upload.json")}&{PercentEncode(param_string)}";

            return System.Convert.ToBase64String(hmac.ComputeHash(Encoding.ASCII.GetBytes(baseString)));
        }

        //checking if the file with specified path is really an image file
        bool IsValidImage(string path)
        {            
            try
            {
                using (Image img = Image.FromFile(path))
                { }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
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
