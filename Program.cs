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
            string configPath, logPath;
            Console.Write("Config file path: ");
            configPath = Console.ReadLine();

            Console.Write("Log file path: ");
            logPath = Console.ReadLine();

            Tweeter tweeter = new Tweeter(configPath, logPath);

            while (true)
            {
                Console.Write("I'd like to tweet this: ");
                string msg = Console.ReadLine();
                Console.WriteLine(tweeter.Tweet(msg).Result);
                Console.WriteLine();
            }

            Console.ReadKey(true);
        }
    }
}
