using System;
using SkromPlexer.Tools;

namespace SkromPlexer.Network
{
    public class ServerClient : Client
    {
        public ServerClient(Client s) : base(s.GetSocket())
        {
            UpgradeArgs = s.UpgradeArgs;
        }

        ~ServerClient()
        {
            Log.Error("Server at adress {0} was disconnected !!!!\n", UpgradeArgs.Item1);
        }
    }
}
