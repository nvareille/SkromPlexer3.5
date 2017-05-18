using System;
using System.Collections.Generic;
using System.Threading;
using SkromPlexer.ServerCore;

namespace SkromPlexer.Modules
{
    /// <summary>
    /// A manager for threads
    /// </summary>
    public class ThreadManager : IModule
    {
        private List<Thread> Threads;

        /// <summary>
        /// The Init function
        /// </summary>
        /// <param name="core">a reference to the Core</param>
        public void Init(Core core)
        {
            Threads = new List<Thread>();
        }

        /// <summary>
        /// The Start function
        /// </summary>
        /// <param name="core">a reference to the Core</param>
        public void Start(Core core)
        {
            
        }

        /// <summary>
        /// The Update function
        /// </summary>
        /// <param name="core">a reference to the Core</param>
        public void Update(Core core)
        {
            Threads.RemoveAll(t => !t.IsAlive);
        }

        /// <summary>
        /// Starts a new thread and add it to the pool
        /// </summary>
        /// <param name="fct">The function to execute</param>
        public void StartThread(ThreadStart fct)
        {
            Thread t = new Thread(fct);

            Threads.Add(t);
            t.Start();
        }
    }

    /// <summary>
    /// Extention for the Core class
    /// </summary>
    public static class ThreadManagerExtension
    {
        /// <summary>
        /// Get the ThreamManagerModule
        /// </summary>
        /// <param name="core">The core instance</param>
        /// <returns>The ThreadManager from the Core</returns>
        public static ThreadManager GetThreadManager(this Core core)
        {
            return ((ThreadManager) core.GetModule(typeof(ThreadManager)));
        }
    }
}
