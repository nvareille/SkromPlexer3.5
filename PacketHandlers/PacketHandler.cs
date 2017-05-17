using System;
using System.Collections.Generic;
using System.Diagnostics;
using SkromPlexer.Network;
using SkromPlexer.ServerCore;
using SkromPlexer.Tools;

namespace SkromPlexer.PacketHandlers
{
    public delegate List<Packet> PacketHandlerDelegate(Core Core, Client Client, Packet Packet);

    class NotLogguedInException : Exception { };

    public class PacketHandlerManager
    {
        private Dictionary<string, PacketHandlerDelegate> Actions;

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
        }

        public void TreatPacket(Core Core, Client Client, Packet Packet)
        {
            //Console.WriteLine(Packet.Content);

            try
            {
                string a = Packet.PacketAction();

                if (Actions.ContainsKey(a))
                {
                    List<Packet> packets = Actions[a](Core, Client, Packet);

                    if (packets != null)
                        Client.SendingPackets.AddRange(packets);
                }
            }
            catch (NotLogguedInException)
            {
                Client.SendingPackets.Add(new Packet("Error:NotLogguedIn\n"));
            }
            catch (Exception e)
            {
                Log.Error("EXCEPTION: " + e.Message);
                //if (!Core.CoreConfig.IsRelease)
                    throw e;
            }
        }

        public static void CheckConnected(Client c)
        {
            if (!c.Authenticated)
                throw new NotLogguedInException();
        }

        public static void CheckServer(Client c)
        {
            if (!c.Authenticated || c.GetType() != typeof(ServerClient))
                throw new NotLogguedInException();
        }
    }
}
