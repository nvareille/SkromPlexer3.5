using System;
using SkromPlexer.Configuration;
using SkromPlexer.ServerCore;
using SkromPlexer.Tools;

namespace SkromPlexer.Modules.Routine
{
    public class RoutineConfig
    {
        public float Interval;
    }

    public abstract class RoutineModule : AConfigurable, IModule
    {
        private Clock Clock;
        protected float Interval;

        public abstract void Init(Core core);

        public void Start(Core core)
        {
            Clock = new Clock();

            Clock.Start();
            Log.Info(Log.F("\n\t\t{0} initialized to {1} ms\n", GetType().Name, Interval));
        }

        public void Update(Core core)
        {
            if (Clock.GetElapsed() >= Interval)
            {
                Clock.Start();
                Log.NewLine(Log.Debug);
                Log.Debug(GetType().Name + " Starting.\n");
                Action(core);
                Log.NewLine(Log.Debug);
            }
        }

        public abstract void Action(Core core);
    }
}
