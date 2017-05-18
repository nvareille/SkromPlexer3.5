using System;

namespace SkromPlexer.Tools
{
    /// <summary>
    /// A class provided to get timestamps and time operations
    /// </summary>
    public class Time
    {
        /// <summary>
        /// Generate a timestamp since Epoch
        /// </summary>
        /// <returns>The number of seconds since Epoch</returns>
        public static int Timestamp()
        {
            return ((int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
        }
    }
}