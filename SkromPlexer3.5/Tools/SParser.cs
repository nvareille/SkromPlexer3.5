using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkromPlexer.Tools
{
    public class SParser
    {
        public string Str;
        public string Method;
        public string[] Array;
        private int Count;

        public SParser(string str)
        {
            Count = 0;
            Str = str;
        }

        public SParser Remove(string[] r)
        {
            foreach (string s in r)
            {
                Str = Str.Replace(s, "");
            }

            return (this);
        }

        public SParser Split(char c)
        {
            Array = Str.Split(c);

            return (this);
        }

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

        public T Get<T>(int index)
        {
            return ((T)Convert.ChangeType(Array[index], typeof(T)));
        }

        public T Get<T>()
        {
            return (Get<T>(Count++));
        }
    }
}
