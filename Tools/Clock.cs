using System;

namespace SkromPlexer.Tools
{
    public class Clock
    {
        private DateTime a;
        private DateTime b;

        public void Start()
        {
            a = DateTime.Now;
        }

        public double GetElapsed()
        {
            TimeSpan t = (b == null ? DateTime.Now - a : b - a);

            return (t.TotalMilliseconds);
        }

        public double Stop()
        {
            b = DateTime.Now;

            return (GetElapsed());
        }
    }
}
