using System;
using System.Collections.Generic;
using System.Linq;
using SkromPlexer.Configuration;
using SkromPlexer.Modules.Download;
using SkromPlexer.Modules.Input;
using SkromPlexer.Network;
using SkromPlexer.PacketHandlers;
using SkromPlexer.Tools;

namespace SkromPlexer.ServerCore
{
    public class CoreConfig
    {
        /// <summary>
        /// Is the Core in release mode ?
        /// </summary>
        public bool IsRelease;
    }

    /// <summary>
    /// The main class for the library, it will handle everything
    /// </summary>
    public class Core : AConfigurable
    {
        public CoreConfig CoreConfig;

        public object Data;

        private List<AConfigurable> Configurables;
        private List<IModule> Modules;
        private Plexer Plexer;

        /// <summary>
        /// A list of clients that are servers
        /// </summary>
        public List<ServerClient> GameServerClients;

        private Clock Clock;
        public double DeltaTime;
        
        /// <summary>
        /// Is this Core a Server ?
        /// </summary>
        public bool IsServer;

        /// <summary>
        /// Is the Core running ?
        /// </summary>
        public bool Running;

        /// <summary>
        /// Recover a stored instance
        /// </summary>
        /// <typeparam name="T">The instance Class</typeparam>
        /// <returns>The stored instance</returns>
        public T GetData<T>()
        {
            return (Data.Cast<T>());
        }

        /// <summary>
        /// Stores an instance that can be recovered later
        /// </summary>
        /// <param name="d">The instance to store</param>
        public void SetData(object d)
        {
            Data = d;
        }

        /// <summary>
        /// The class constructor
        /// </summary>
        /// <param name="modules">Modules to use</param>
        /// <param name="packetHandlers">PacketHandlers to use</param>
        /// <param name="configurables">The configurables ready to hot reload</param>
        public Core(IModule[] modules = null, APacketHandler[] packetHandlers = null, AConfigurable[] configurables = null)
        {
            Plexer = new Plexer(packetHandlers);
            Configurables = configurables == null ? new List<AConfigurable>() : new List<AConfigurable>(configurables);
            Modules = new List<IModule>();

            Configurables.Add(Plexer);
            Modules.Add(Plexer);
            Modules.Add(new DownloadModule());
            Modules.Add(new InputModule());

            if (modules != null)
            {
                foreach (IModule module in modules)
                {
                    Modules.Add(module);
                }
            }

            GameServerClients = new List<ServerClient>();
            Clock = new Clock();
        }

        /// <summary>
        /// Will initialize the Core
        /// </summary>
        /// <param name="server">true if server, false or nothing if client</param>
        public void Init(bool server = true)
        {
            IsServer = server;
            Log.Core = this;
            
            Log.Info("\nInitializing modules ...\n");
            foreach (IModule module in Modules)
            {
                Log.Composite(Log.F("\tInit {0} ... ", module.GetType().Name), "Done !", Log.Info, () =>
                {
                    module.Init(this);
                });
            }
            Log.Info("Modules Initialized !\n");
        }

        /// <summary>
        /// Starts every modules
        /// </summary>
        public void Start()
        {
            Log.Info("\nStarting modules ...\n");
            foreach (IModule module in Modules)
            {
                Log.Composite(Log.F("\tStarting {0} ... ", module.GetType().Name), "Done !", Log.Info, () =>
                {
                    module.Start(this);
                });
            }
            Log.Info("Modules Started !\n");

            Log.Info("\nServer is running !\n");
        }

        /// <summary>
        /// Do a server frame, updating every modules
        /// </summary>
        public void Update()
        {
            DeltaTime = Clock.Stop();
            Clock.Start();
            foreach (IModule module in Modules)
            {
                module.Update(this);
            }
        }

        /// <summary>
        /// Runs the server in a single loop
        /// </summary>
        public void Run()
        {
            Running = true;

            Start();

            while (Running)
            {
                Update();
            }
        }

        /// <summary>
        /// Get a module from the core
        /// </summary>
        /// <param name="t">Module type to get</param>
        /// <returns>The selected module</returns>
        public IModule GetModule(Type t)
        {
            return (Modules.First(c => c.GetType() == t));
        }

        /// <summary>
        /// Get a module from the core
        /// </summary>
        /// <typeparam name="T">The instance class to get</typeparam>
        /// <returns>The selected module</returns>
        public T GetModule<T>()
        {
            return ((T)Modules.First(c => c.GetType() == typeof(T)));
        }

        /// <summary>
        /// Get the InputModule
        /// </summary>
        /// <returns>The InputModule instance</returns>
        public InputModule GetInputModule()
        {
            return ((InputModule) GetModule(typeof(InputModule)));
        }
    }
}
