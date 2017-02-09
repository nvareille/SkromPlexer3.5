using System;
using System.Collections.Generic;
using System.Threading;
using SkromPlexer.ServerCore;

namespace SkromPlexer.Modules
{
    public class ThreadManager : IModule
    {
        private List<Thread> Threads;

        public void Init(Core core)
        {
            Threads = new List<Thread>();
        }

        public void Start(Core core)
        {
            
        }

        public void Update(Core core)
        {
            Threads.RemoveAll(t => !t.IsAlive);
        }

        public void StartThread(ThreadStart fct)
        {
            Thread t = new Thread(fct);

            Threads.Add(t);
            t.Start();
        }
    }

    public static class ThreadManagerExtension
    {
        public static ThreadManager GetThreadManager(this Core core)
        {
            return ((ThreadManager) core.GetModule(typeof(ThreadManager)));
        }
    }
}
