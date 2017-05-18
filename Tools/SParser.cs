using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkromPlexer.Tools
{
    /// <summary>
    /// A Built in Parser to cut and mmanage patterns in strings
    /// </summary>
    public class SParser
    {
        private string Str;
        private string Method;
        private string[] Array;
        private int Count;

        /// <summary>
        /// Initialisation of the instance
        /// </summary>
        /// <param name="str">The string to cut and manage</param>
        public SParser(string str)
        {
            Count = 0;
            Str = str;
        }

        /// <summary>
        /// Removes Needles from a Haystack
        /// </summary>
        /// <param name="r">The needles to remove</param>
        /// <returns>A SParser instance</returns>
        public SParser Remove(string[] r)
        {
            foreach (string s in r)
            {
                Str = Str.Replace(s, "");
            }

            return (this);
        }

        /// <summary>
        /// Split the Haystack with the given tokens
        /// </summary>
        /// <param name="c">Splitter Token</param>
        /// <returns>A SParser instance</returns>
        public SParser Split(char c)
        {
            Array = Str.Split(c);

            return (this);
        }

        /// <summary>
        /// Will extract a method name from the Haystack
        /// Example: HelloWorld(); would give HelloWorld
        /// </summary>
        /// <returns>A SParser instance</returns>
        public SParser GetMethod()
        {
            Method = "";

            while (Str[0] != '(')
            {
                Method += Str[0];
                Str = Str.Substring(1);
            }
            return (this);
        }

        /// <summary>
        /// After a split, will return an element form the array and will convert it
        /// </summary>
        /// <typeparam name="T">The instance type to get</typeparam>
        /// <param name="index">The array index to get</param>
        /// <returns>The element of the array converted and cast</returns>
        public T Get<T>(int index)
        {
            return ((T)Convert.ChangeType(Array[index], typeof(T)));
        }

        /// <summary>
        /// After a split, will return an element form the actual array index and will convert it
        /// The index will be incremented
        /// </summary>
        /// <typeparam name="T">The instance type to get</typeparam>
        /// <returns>The element of the array converted and cast</returns>
        public T Get<T>()
        {
            return (Get<T>(Count++));
        }
    }
}
