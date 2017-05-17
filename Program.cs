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
            Tweeter tweeter = new Tweeter("491632305-l41LEyIUBg0G81VHnnRNSOJq1DArQ6EMVIrpUS2y",
                "9EbXB5SZmWUETdsgAVnLPINCM",
                "zHt9rC9if1SGiZt3XgVJ1UXtYGFhPktinpVjryNoecZ9X",
                "AASY6dyccwjfMDnAlHoXxId46fssDsvYqRrbWn8XSW1SfSTxyX");

            //while (true)
            //{
                string msg = Console.ReadLine();
                Console.WriteLine(tweeter.Tweet(msg).Result);
                Console.WriteLine();
            //}

            Console.ReadKey(true);
        }

        public static async Task<HttpResponseMessage> SendMessage(HttpRequestMessage message)
        {
            return await client.SendAsync(message);
        }
    }
}
