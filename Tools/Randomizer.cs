using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkromPlexer.Tools
{
    /// <summary>
    /// A built in class for random generation
    /// </summary>
    public class Randomizer
    {
        private static Random Random;
        private const string Hex = "0123456789ABCDEF";

        /// <summary>
        /// Initializes the instance
        /// </summary>
        public static void Init()
        {
            Random = new Random();
        }

        /// <summary>
        /// Generates a random Hexadecimal string
        /// </summary>
        /// <param name="size">The size of the string</param>
        /// <returns>A random string in hexadecimal</returns>
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
