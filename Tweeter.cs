using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomThings
{
    class Tweeter
    {
        public Tweeter()
        { }

        public void Tweet(string text)
        { }

        static int GetUnixTimestamp(DateTime utcDate)
        {
            return (int)(utcDate - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        }

        static string PercentEncode(string text)
        {
            return Uri.EscapeDataString(text);
        }
    }    
}
