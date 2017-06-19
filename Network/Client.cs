using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using SkromPlexer.Modules.Download;
using SkromPlexer.ServerCore;
using SkromPlexer.Tools;

namespace SkromPlexer.Network
{
    public delegate void ClientDisconnectDelegate();

    /// <summary>
    /// A class for handling client connection
    /// </summary>
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

        public bool IsFileSocket;
        public int DLToken;

        public Tuple<string, string> UpgradeArgs;

        /// <summary>
        /// The class constructor
        /// </summary>
        /// <param name="socket">A socket from this client</param>
        public Client(Socket socket, bool serverSide, DownloadModule dl = null)
        {
            Socket = socket;
            PacketBuilder = new PacketBuilder();
            ReceivedPackets = new List<Packet>();
            SendingPackets = new List<Packet>();
            DisconnectCallbacks = new List<ClientDisconnectDelegate>();

            if (serverSide)
            {
                byte[] a = new byte[4];
                int i = Socket.Receive(a, 4, SocketFlags.None);
                DLToken = BitConverter.ToInt32(a, 0);

                if (DLToken != 0)
                {
                    IsFileSocket = true;
                    dl.BindTask(this, (elem => elem.Token == DLToken), null, false);
                }
            }
        }

        /// <summary>
        /// Get the socket from this client
        /// </summary>
        /// <returns>The client's socket</returns>
        public Socket GetSocket()
        {
            return (Socket);
        }

        /// <summary>
        /// Checks if the client is still connected
        /// </summary>
        /// <returns>true if connected, false otherwise</returns>
        public bool IsConnected()
        {
            if (Socket == null || !Socket.Connected)
                return (false);

            try
            {
                bool part1 = Socket.Poll(0, SelectMode.SelectRead);
                bool part2 = (Socket.Available == 0);

                return ((!part1 || !part2));
            }
            catch (Exception)
            {
                return (false);
            }
        }

        /// <summary>
        /// Will extract data to create Packets
        /// </summary>
        public void GetPackets()
        {
            byte[] buffer = new byte[BufferSize];
            int read = Socket.Receive(buffer);

            PacketBuilder.BuildPacket(buffer);
            if (PacketBuilder.HaveCompletePackets)
                ReceivedPackets.AddRange(PacketBuilder.ExtractPackets());
        }

        /// <summary>
        /// Tries to send the available Packets
        /// </summary>
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

        /// <summary>
        /// Check if data is available on the socket and extract packets
        /// </summary>
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

        /// <summary>
        /// Check if packets could be sent and sends them
        /// </summary>
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

        /// <summary>
        /// Handles the packets and call the PacketHandler
        /// </summary>
        /// <param name="core">A reference to Core</param>
        /// <param name="plexer">A reference to Plexer</param>
        public void TreatPackets(Core core, Plexer plexer)
        {
            while (ReceivedPackets.Any())
            {
                Packet Packet = ReceivedPackets.First();

                ReceivedPackets.Remove(Packet);
                plexer.HandlePackets(core, this, Packet);
            }
        }

        /// <summary>
        /// Check if we received data and try to call the corresponding packet handlers
        /// </summary>
        /// <param name="core">A reference to Core</param>
        /// <param name="plexer">A reference to Plexer</param>
        public void TryTreatPacket(Core core, Plexer plexer)
        {
            if (ReceivedPackets.Any())
                TreatPackets(core, plexer);
        }

        /// <summary>
        /// Disonnects the client's Socket while sending a Packet
        /// </summary>
        /// <param name="packet">Packet to send</param>
        /// <returns>true</returns>
        public bool Disconnect(Packet packet)
        {
            MustDisconnect = true;
            SendingPackets.Add(packet);
            return (true);
        }

        /// <summary>
        /// Immediatly disconnects a socket
        /// </summary>
        /// <returns>true</returns>
        public bool SocketDisconnect()
        {
            Socket.Shutdown(SocketShutdown.Both);
            Socket.Close();
            MustDisconnect = true;
            return (true);
        }

        /// <summary>
        /// Adds a packet to the SendingPackets
        /// </summary>
        /// <param name="p">PAcket to send</param>
        /// <returns>true</returns>
        public bool AddPacket(Packet p)
        {
            SendingPackets.Add(p);
            return (true);
        }

        /// <summary>
        /// Will execute the disconnection callbacks
        /// </summary>
        /// <returns>true</returns>
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