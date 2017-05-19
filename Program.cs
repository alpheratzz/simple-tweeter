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
            string img;

            try
            {                
                tweeter = new Tweeter(oauthConfigPath, logPath);
                while (true)
                {
                    img = null;
                    Console.Write("I'd like to tweet this: ");
                    string msg = Console.ReadLine();
                    List<string> imgs = new List<string>();
                    while (true)
                    {
                        Console.WriteLine("Do you want to post a picture? (y/n)");
                        if (imgs.Count >= 4)
                            Console.WriteLine("Warning: only maximum of 4 pictures can be posted");
                        if (Console.ReadKey(true).Key == ConsoleKey.Y)
                        {
                            Console.Write("Image path: ");
                            img = Console.ReadLine();
                            imgs.Add(img);
                        }
                        else break;
                    }
                    Console.WriteLine(tweeter.Tweet(msg, imgs.ToArray()).Result);
                    Console.WriteLine();                    
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
