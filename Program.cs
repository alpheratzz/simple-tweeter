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
            Tweeter tweeter = new Tweeter("herp", "derp", "durrr", "hurrr");
            Console.WriteLine(tweeter.Tweet("herp derp").Result);

            Console.ReadKey(true);
        }

        public static async Task<HttpResponseMessage> SendMessage(HttpRequestMessage message)
        {
            return await client.SendAsync(message);
        }
    }
}
