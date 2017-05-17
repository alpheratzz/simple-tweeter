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
            //Tweeter tweeter = new Tweeter("herp", "derp", "durrr", "hurrr");
            //tweeter.Tweet("herp derp");
            Dictionary<string, string> d = new Dictionary<string, string>();
            d["1"] = "a";
            Console.WriteLine(d["1"]);

            Console.ReadKey(true);
        }

        public static async Task<HttpResponseMessage> SendMessage(HttpRequestMessage message)
        {
            return await client.SendAsync(message);
        }
    }
}
