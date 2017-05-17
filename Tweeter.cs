﻿using System;
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

        HMACSHA1 hmac;

        public Tweeter(string oauthToken, string oauthConsumerKey, string oauthTokenSecret, string oauthConsumerSecret)
        {
            this.oauthToken = oauthToken;
            this.oauthConsumerKey = oauthConsumerKey;

            hmac = new HMACSHA1(Encoding.ASCII.GetBytes($"{PercentEncode(oauthConsumerSecret)}&{PercentEncode(oauthTokenSecret)}"));

            client = new HttpClient();
            client.BaseAddress = new Uri(@"https://api.twitter.com/1.1/");

            authParams = new Dictionary<string, string>();
        }

        public async void Tweet(string text)
        {
            authParams.Clear();

            authParams.Add("oauth_token", oauthToken);
            authParams.Add("oauth_consumer_key", oauthConsumerKey);
            authParams.Add("oauth_signature_method", "HMAC-SHA1");
            authParams.Add("oauth_version", "1.0");
            authParams.Add("oauth_nonce", GetNonce());
            authParams.Add("oauth_timestamp", GetUnixTimestamp(DateTime.UtcNow).ToString());

            //this is a bit awkward..
            authParams.Add("status", text);
            authParams.Add("oauth_signature", GetSignature(authParams));

            authParams.Remove("status");

            var tmp = authParams.Select(pair => $"{PercentEncode(pair.Key)}=\"{PercentEncode(pair.Value)}\"");
            string auth_header = "OAuth " + string.Join(", ", tmp);

            

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
