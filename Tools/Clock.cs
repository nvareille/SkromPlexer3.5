using System;

namespace SkromPlexer.Tools
{
    /// <summary>
    /// A stopwatch class used to measure time
    /// </summary>
    public class Clock
    {
        private DateTime a;
        private DateTime b;
        private bool Init;

        /// <summary>
        /// Starts the clock
        /// </summary>
        public void Start()
        {
            a = DateTime.Now;
            Init = true;
        }

        /// <summary>
        /// Will return the time elapsed since the Start() or between the Start() and the Stop() if called
        /// </summary>
        /// <returns>The time in milliseconds</returns>
        public double GetElapsed()
        {
            if (Init == false)
                Start();

            TimeSpan t = (b == null ? DateTime.Now - a : b - a);

            return (t.TotalMilliseconds);
        }

        /// <summary>
        /// Stops the clock
        /// </summary>
        /// <returns>The time elapsed in milliseconds</returns>
        public double Stop()
        {
            b = DateTime.Now;

            return (GetElapsed());
        }
    }
}
