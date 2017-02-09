using System;

namespace SkromPlexer.Tools
{
    public class Time
    {
        public static int Timestamp()
        {
            return ((int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
        }
    }
}