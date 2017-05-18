using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkromPlexer.Tools
{
    /// <summary>
    /// A basic implementation of the Tuple class for .NET 3.5
    /// </summary>
    /// <typeparam name="T">The first type of the Tuple</typeparam>
    /// <typeparam name="U">The second type of the Tuple</typeparam>
    public class Tuple<T, U>
    {
        /// <summary>
        /// A templated instance of type T
        /// </summary>
        public T Item1;

        /// <summary>
        /// A templated instance of type U
        /// </summary>
        public U Item2;

        /// <summary>
        /// Generation of the Tuple, it will assign the values in the instance
        /// </summary>
        /// <param name="i1">First item</param>
        /// <param name="i2">Second item</param>
        public Tuple(T i1, U i2)
        {
            Item1 = i1;
            Item2 = i2;
        }
    }
}
