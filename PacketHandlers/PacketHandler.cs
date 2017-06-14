using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using SkromPlexer.Network;
using SkromPlexer.ServerCore;
using SkromPlexer.Tools;

namespace SkromPlexer.PacketHandlers
{
    public delegate List<Packet> PacketHandlerDelegate(Core Core, Client Client, Packet Packet);

    /// <summary>
    /// The exception throw if the client isn't connected
    /// </summary>
    class NotLogguedInException : Exception { };

    /// <summary>
    /// The class that will use the PacketHandler callbacks
    /// </summary>
    public class PacketHandlerManager
    {
        private Dictionary<string, PacketHandlerDelegate> Actions;

        /// <summary>
        /// The class constructor
        /// </summary>
        /// <param name="packetHandlers">The PacketHandlers to use</param>
        public PacketHandlerManager(APacketHandler[] packetHandlers)
        {
            Actions = new Dictionary<string, PacketHandlerDelegate>();

            if (packetHandlers != null)
            {
                foreach (APacketHandler packetHandler in packetHandlers)
                {
                    packetHandler.Init(Actions);
                }
            }

            new DownloadPacketHandler().Init(Actions);
        }

        /// <summary>
        /// Will treat the Packets for the given Client
        /// </summary>
        /// <param name="Core">A reference to Core</param>
        /// <param name="Client">A reference to the Client</param>
        /// <param name="Packet">The Packet to treat</param>
        public void TreatPacket(Core Core, Client Client, Packet Packet)
        {
            try
            {
                string a = Packet.PacketAction();

                if (Actions.ContainsKey(a))
                {
                    Console.WriteLine(Packet.Content);

                    List<Packet> packets = Actions[a](Core, Client, Packet);

                    if (packets != null)
                        Client.SendingPackets.AddRange(packets);
                }
                else
                {
                    Console.WriteLine("Warning: Packet {0} isn't registered !", a);
                }
            }
            catch (NotLogguedInException)
            {
                Client.SendingPackets.Add(new Packet("Error:NotLogguedIn\n"));
            }
            catch (Exception e)
            {
                Log.Error("EXCEPTION: " + e.Message);
                throw e;
            }
        }

        /// <summary>
        /// Checks if the client is Authenticated
        /// </summary>
        /// <param name="c">The client to check</param>
        public static void CheckAuthenticated(Client c)
        {
            if (!c.Authenticated)
                throw new NotLogguedInException();
        }

        /// <summary>
        /// Check if the client is a Server
        /// </summary>
        /// <param name="c"></param>
        public static void CheckServer(Client c)
        {
            if (!c.Authenticated || c.GetType() != typeof(ServerClient))
                throw new NotLogguedInException();
        }
    }
}
