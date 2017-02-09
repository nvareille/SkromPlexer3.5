using System;

namespace SkromPlexer.Tools
{
    public class Clock
    {
        private DateTime a;

        public void Start()
        {
            a = DateTime.Now;
        }

        public double GetElapsed()
        {
            TimeSpan b = DateTime.Now - a;

            return (b.TotalMilliseconds);
        }
    }
}
