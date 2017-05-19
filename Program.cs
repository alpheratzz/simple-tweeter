using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Http;
using System.IO;

namespace RandomThings
{
    class Program
    {
        static void Main(string[] args)
        {
            string oauthConfigPath, logPath;
            Console.Write("OAuth config file path: ");
            oauthConfigPath = Console.ReadLine();

            Console.Write("Log file path: ");
            logPath = Console.ReadLine();

            Tweeter tweeter;

            try
            {
                tweeter = new Tweeter(oauthConfigPath, logPath);
                while (true)
                {
                    //Console.Write("I'd like to tweet this: ");
                    //string msg = Console.ReadLine();
                    //Console.WriteLine(tweeter.Tweet(msg).Result);
                    //Console.WriteLine();

                    Console.Write("Image path: ");
                    string img = Console.ReadLine();
                    HttpResponseMessage response = tweeter.UploadImage(img).Result;
                    Console.WriteLine(response?.StatusCode);
                }
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                Console.WriteLine("See you later!");
            }            

            Console.ReadKey(true);
        }
    }
}
