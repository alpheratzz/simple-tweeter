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

            //nonce
            //Console.WriteLine(Guid.NewGuid().ToString("N"));

            client = new HttpClient();
            client.BaseAddress = new Uri(@"https://jsonplaceholder.typicode.com");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Get, "posts/1");
            HttpResponseMessage response = SendMessage(msg).Result;
            //Console.WriteLine(response.RequestMessage + "\n");
            //Console.WriteLine(response.Content.ReadAsStringAsync().Result);
            //Console.WriteLine(client.BaseAddress.ToString());

            Dictionary<string, string> d = new Dictionary<string, string>();
            d.Add("a", "a");
            d.Add("abb", "a");
            d.Add("aba", "a");
            d.Add("ab", "a");
            d.Add("aa", "a");            
            d.Add("aac", "a");

            var tmp = d.OrderBy(pair => pair.Key).Select(pair => Tweeter.PercentEncode(pair.Key) + "=" + Tweeter.PercentEncode(pair.Value));

            foreach (var pair in tmp)
                Console.WriteLine(pair);

            Console.ReadKey(true);
        }

        public static async Task<HttpResponseMessage> SendMessage(HttpRequestMessage message)
        {
            return await client.SendAsync(message);
        }
    }
}
