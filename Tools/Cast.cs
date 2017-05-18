using System;

namespace SkromPlexer.Tools
{
    /// <summary>
    /// A class extension to cast easily the values
    /// </summary>
    public static class ObjectCaster
    {
        /// <summary>
        /// Casts an element to type
        /// Same as (T) variable
        /// </summary>
        /// <typeparam name="T">The type to cast to</typeparam>
        /// <param name="o">The element to cast</param>
        /// <returns>The cast value</returns>
        public static T Cast<T>(this object o)
        {
            return ((T) o);
        }

        /// <summary>
        /// Convert an element to the given type
        /// </summary>
        /// <typeparam name="T">The type to convert to</typeparam>
        /// <param name="o">The element to convert</param>
        /// <returns>The converted value</returns>
        public static T Convert<T>(this object o)
        {
            return ((T) System.Convert.ChangeType(o, typeof(T)));
        }

        /// <summary>
        /// Convert to float
        /// </summary>
        /// <param name="o">The element to convert</param>
        /// <returns>The converted value</returns>
        public static float Float(this int o)
        {
            return ((float) o);
        }

        /// <summary>
        /// convert to int
        /// </summary>
        /// <param name="o">The element to convert</param>
        /// <returns>The converted element</returns>
        public static int Int(this float o)
        {
            return ((int) o);
        }

        /// <summary>
        /// Will instantiate a type
        /// </summary>
        /// <typeparam name="T">The type to cast to</typeparam>
        /// <param name="type">The type to instantiate</param>
        /// <returns>The instanced value</returns>
        public static T CreateInstance<T>(this Type type)
        {
            return ((T)Activator.CreateInstance(type));
        }
    }
}
