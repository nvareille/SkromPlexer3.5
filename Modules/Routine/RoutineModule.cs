using System;
using SkromPlexer.Configuration;
using SkromPlexer.ServerCore;
using SkromPlexer.Tools;

namespace SkromPlexer.Modules.Routine
{
    /// <summary>
    /// A routine config
    /// </summary>
    public class RoutineConfig
    {
        public float Interval;
    }

    /// <summary>
    /// A class that handles routines, better inherited
    /// </summary>
    public abstract class RoutineModule : AConfigurable, IModule
    {
        private Clock Clock;
        protected float Interval;

        public abstract void Init(Core core);

        /// <summary>
        /// Start function of the routine
        /// </summary>
        /// <param name="core">A reference to the Core</param>
        public void Start(Core core)
        {
            Clock = new Clock();

            Clock.Start();
            Log.Info(Log.F("\n\t\t{0} initialized to {1} ms\n", GetType().Name, Interval));
        }

        /// <summary>
        /// Update function of the routine
        /// </summary>
        /// <param name="core">A reference to the Core</param>
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

        /// <summary>
        /// The function that will be execute upon routine Exec
        /// </summary>
        /// <param name="core">A reference to the Core</param>
        public abstract void Action(Core core);
    }
}
