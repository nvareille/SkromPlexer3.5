using System;

namespace SkromPlexer.Tools
{
    public static class ObjectCaster
    {
        public static T Cast<T>(this object o)
        {
            return ((T) o);
        }

        public static T Convert<T>(this object o)
        {
            return ((T) System.Convert.ChangeType(o, typeof(T)));
        }

        public static float Float(this int o)
        {
            return ((float) o);
        }

        public static int Int(this float o)
        {
            return ((int) o);
        }

        public static T CreateInstance<T>(this Type type)
        {
            return ((T)Activator.CreateInstance(type));
        }
    }
}
