using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkromPlexer.Tools
{
    public class Randomizer
    {
        private static Random Random;
        private const string Hex = "0123456789ABCDEF";

        public static void Init()
        {
            Random = new Random();
        }

        public static string RandomHexString(int size)
        {
            if (Random == null)
                Init();

            string str = "";
            int count = 0;

            while (count < size)
            {
                str += Hex[Random.Next(16)];
                ++count;
            }

            return (str);
        }
    }
}
