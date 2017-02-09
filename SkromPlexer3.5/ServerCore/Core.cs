using System;
using System.Collections.Generic;
using System.Linq;
using DragonDreamServer.Classes.Modules.Input;
using SkromPlexer.BDDORM;
using SkromPlexer.Configuration;
using SkromPlexer.Network;
using SkromPlexer.PacketHandlers;
using SkromPlexer.Tools;

namespace SkromPlexer.ServerCore
{
    public class CoreConfig
    {
        public int TicksPerSecond;
        public string GameServerPassword;
        public string[] ORMIndexes;
        public bool IsRelease;
    }

    public class Core : AConfigurable
    {
        public CoreConfig CoreConfig;

        private Data Data;

        private List<AConfigurable> Configurables;
        private List<IModule> Modules;

        public Plexer Plexer;

        //public List<Client> Clients;

        public List<ServerClient> GameServerClients;

        public bool Running;

        public Core(IModule[] modules = null, APacketHandler[] packetHandlers = null, AConfigurable[] configurables = null)
        {
            Data = new Data();

            foreach (string id in CoreConfig.ORMIndexes)
            {
                new MysqlBDD(id);
            }

            Plexer = new Plexer(packetHandlers);
            Configurables = configurables == null ? new List<AConfigurable>() : new List<AConfigurable>(configurables);
            Modules = new List<IModule>();

            Configurables.Add(Plexer);
            Modules.Add(Plexer);
            Modules.Add(new InputModule());

            if (modules != null)
            {
                foreach (IModule module in modules)
                {
                    Modules.Add(module);
                }
            }

            GameServerClients = new List<ServerClient>();
        }

        public void Init()
        {
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

        public void Run()
        {
            Running = true;

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
            while (Running)
            {
                foreach (IModule module in Modules)
                {
                    module.Update(this);
                }
            }
        }

        public IModule GetModule(Type t)
        {
            return (Modules.First(c => c.GetType() == t));
        }

        public InputModule GetInputModule()
        {
            return ((InputModule) GetModule(typeof(InputModule)));
        }
    }
}
