using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Http;
using System.Net.Http.Headers;

namespace RandomThings
{
    class Program
    {
        static HttpClient client;

        static void Main(string[] args)
        {
            string token, consumerKey, tokenSecret, consumerSecret;

            Console.Write("Your OAuth token: ");
            token = Console.ReadLine();
            Console.Write("Your OAuth consumer key: ");
            consumerKey = Console.ReadLine();
            Console.Write("Your OAuth token secret: ");
            tokenSecret = Console.ReadLine();
            Console.Write("Your OAuth consumer secret: ");
            consumerSecret = Console.ReadLine();

            Tweeter tweeter = new Tweeter(token, consumerKey, tokenSecret, consumerSecret);

            while (true)
            {
                string msg = Console.ReadLine();
                Console.WriteLine(tweeter.Tweet(msg).Result);
                Console.WriteLine();
            }

            Console.ReadKey(true);
        }

        public static async Task<HttpResponseMessage> SendMessage(HttpRequestMessage message)
        {
            return await client.SendAsync(message);
        }
    }
}
