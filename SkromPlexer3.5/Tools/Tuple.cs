using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkromPlexer.Tools
{
    public class Tuple<T, U>
    {
        public T Item1;
        public U Item2;

        public Tuple(T i1, U i2)
        {
            Item1 = i1;
            Item2 = i2;
        }
    }
}
