using System;
using SkromPlexer.Tools;

namespace SkromPlexer.Network
{
    /// <summary>
    /// A class defining if the Client is actually a Server
    /// </summary>
    public class ServerClient : Client
    {
        /// <summary>
        /// The class constructor
        /// </summary>
        /// <param name="s">The Client</param>
        public ServerClient(Client s) : base(s.GetSocket())
        {
            UpgradeArgs = s.UpgradeArgs;
        }

        /// <summary>
        /// Class Destructor
        /// </summary>
        ~ServerClient()
        {
            Log.Error("Server at adress {0} was disconnected !!!!\n", UpgradeArgs.Item1);
        }
    }
}
