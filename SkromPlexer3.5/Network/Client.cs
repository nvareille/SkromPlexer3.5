using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using SkromPlexer.ServerCore;
using SkromPlexer.Tools;

namespace SkromPlexer.Network
{
    public delegate void ClientDisconnectDelegate();

    public class Client
    {
        private const int BufferSize = 1024;

        private Socket Socket;
        private PacketBuilder PacketBuilder;
        public List<Packet> ReceivedPackets;
        public List<Packet> SendingPackets;
        public bool MustDisconnect;
        public bool Authenticated;
        public List<ClientDisconnectDelegate> DisconnectCallbacks;
        public object UserData;

        public Tuple<string, string> UpgradeArgs;

        public Client(Socket socket)
        {
            Socket = socket;
            PacketBuilder = new PacketBuilder();
            ReceivedPackets = new List<Packet>();
            SendingPackets = new List<Packet>();
            DisconnectCallbacks = new List<ClientDisconnectDelegate>();
        }

        public Socket GetSocket()
        {
            return (Socket);
        }

        public bool IsConnected()
        {
            bool part1 = Socket.Poll(0, SelectMode.SelectRead);
            bool part2 = (Socket.Available == 0);

            return ((!part1 || !part2) && Socket.Connected);
        }

        public void GetPackets()
        {
            byte[] buffer = new byte[BufferSize];
            int read = Socket.Receive(buffer);

            PacketBuilder.BuildPacket(buffer);
            if (PacketBuilder.HaveCompletePackets)
                ReceivedPackets.AddRange(PacketBuilder.ExtractPackets());
        }

        public void SendPackets()
        {
            bool Error = false;

            while (!Error && SendingPackets.Count > 0)
            {
                Packet Packet = SendingPackets.First();
                byte[] content = Encoding.ASCII.GetBytes(Packet.Content);

                int length = Socket.Send(content);

                if (Packet.IsCriticPacket)
                    MustDisconnect = true;

                if (length < 0)
                    Error = true;
                else if (length < Packet.Content.Length)
                {
                    Error = true;
                    Packet.Content = Packet.Content.Substring(length);
                }
                else
                    SendingPackets.Remove(Packet);
            }
        }

        public void TryGetPackets()
        {
            try
            {
                if (Socket.Poll(0, SelectMode.SelectRead))
                    GetPackets();
            }
            catch (Exception)
            {
                MustDisconnect = true;
            }
        }

        public void TrySendPackets()
        {
            try
            {
                if (SendingPackets.Count > 0 && Socket.Poll(0, SelectMode.SelectWrite))
                    SendPackets();
            }
            catch (Exception)
            {
                MustDisconnect = true;
            }
            
        }

        public void TreatPackets(Core core, Plexer plexer)
        {
            while (ReceivedPackets.Any())
            {
                Packet Packet = ReceivedPackets.First();

                ReceivedPackets.Remove(Packet);
                plexer.HandlePackets(core, this, Packet);
            }
        }

        public void TryTreatPacket(Core core, Plexer plexer)
        {
            if (ReceivedPackets.Any())
                TreatPackets(core, plexer);
        }

        public bool Disconnect(Packet packet)
        {
            MustDisconnect = true;
            SendingPackets.Add(packet);
            return (true);
        }

        public bool SocketDisconnect()
        {
            Socket.Disconnect(false);
            return (true);
        }

        public bool AddPacket(Packet p)
        {
            SendingPackets.Add(p);
            return (true);
        }

        public bool ExecuteDisconnectCallbacks()
        {
            foreach (var disconnectCallback in DisconnectCallbacks)
            {
                try
                {
                    disconnectCallback.DynamicInvoke();
                }
                catch (Exception e)
                {
                }
            }

            return (true);
        }
    }
}