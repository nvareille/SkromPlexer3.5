using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using SkromPlexer.Configuration;
using SkromPlexer.PacketHandlers;
using SkromPlexer.ServerCore;

#pragma warning disable 0649

namespace SkromPlexer.Network
{
    /// <summary>
    /// Represents a configuration for the Plexer
    /// </summary>
    public class PlexerConfig
    {
        public int Port = 8080;
        public string IPToConnect = "127.0.0.1";
    }

    /// <summary>
    /// An implementation of a Network multiplexer
    /// </summary>
    public class Plexer : AConfigurable, IModule
    {
        private TcpListener Listener;
        public List<Client> Clients;
        public List<Client> ToAddClients;
        public List<Client> ToUpgrade;
        public PlexerConfig PlexerConfig;
        private PacketHandlerManager PacketHandler;

        private List<ServerClient> ServerClients;
        
        /// <summary>
        /// The class constructon
        /// </summary>
        /// <param name="packetHandlers">PacketHandlers to use</param>
        public Plexer(APacketHandler[] packetHandlers)
        {
            Clients = new List<Client>();
            ToAddClients = new List<Client>();
            ToUpgrade = new List<Client>();
            PacketHandler = new PacketHandlerManager(packetHandlers);
        }

        /// <summary>
        /// The Init function
        /// </summary>
        /// <param name="core">A reference to Core</param>
        public void Init(Core core)
        {
            ServerClients = core.GameServerClients;

            if (core.IsServer)
                Listener = new TcpListener(IPAddress.Any, PlexerConfig.Port);
        }

        /// <summary>
        /// The Start function
        /// </summary>
        /// <param name="core">A reference to Core</param>
        public void Start(Core core)
        {
            if (core.IsServer)
                Listener.Start();
        }

        /// <summary>
        /// Will execute the PacketHandlers
        /// </summary>
        /// <param name="core">A reference to Core</param>
        /// <param name="client">The client sending the Packet</param>
        /// <param name="packet">The Packet that calls the PacketHandler</param>
        public void HandlePackets(Core core, Client client, Packet packet)
        {
            PacketHandler.TreatPacket(core, client, packet);
        }

        /// <summary>
        /// The Update function
        /// </summary>
        /// <param name="core">A reference to Core</param>
        public void Update(Core core)
        {
            Clients.RemoveAll(c => !c.IsConnected() && c.ExecuteDisconnectCallbacks());
            ServerClients.RemoveAll(c => !c.IsConnected() && c.ExecuteDisconnectCallbacks());

            if (core.IsServer && Listener.Pending())
            {
                Clients.Add(new Client(Listener.AcceptSocket(), true));
            }

            foreach (var client in Clients)
            {
                client.TryGetPackets();
                client.TryTreatPacket(core, this);
                client.TrySendPackets();
            }

            foreach (var client in ServerClients)
            {
                client.TryGetPackets();
                client.TryTreatPacket(core, this);
                client.TrySendPackets();
            }

            while (ToUpgrade.Any())
            {
                Clients.Remove(ToUpgrade.First());
                ServerClient c = new ServerClient(ToUpgrade.First());

                ToUpgrade.RemoveAt(0);
                ServerClients.Add(c);
            }

            Clients.AddRange(ToAddClients);
            ToAddClients.Clear();

            ForceDisconnect();
        }

        /// <summary>
        /// Will disconnect every Client that musn't stay connected
        /// </summary>
        public void ForceDisconnect()
        {
            Clients.Where(c => c.MustDisconnect).All(c => c.SocketDisconnect());
            ServerClients.Where(c => c.MustDisconnect).All(c => c.SocketDisconnect());
        }

        /// <summary>
        /// Connect to a server
        /// </summary>
        /// <param name="IPAdress">The IP to connect to</param>
        /// <param name="port">The port to use</param>
        /// <param name="add">Will the client be added to the client list in the Plexer ?</param>
        /// <returns>A client connected to the Server</returns>
        public Client ConnectToServer(string IPAdress, int port, bool add = true, int connectionToken = 0)
        {
            IPAddress[] ip = Dns.GetHostAddresses(IPAdress);

            foreach (IPAddress address in ip)
            {
                try
                {
                    Socket Socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    Socket.Connect(address, port);
                    Socket.Send(BitConverter.GetBytes(connectionToken));

                    Client c = new Client(Socket);
                    if (add)
                        ToAddClients.Add(c);

                    return (c);
                }
                catch(Exception) { }
            }
            
            return (null);
        }

        public Client ConnectToServerTimeout(string IPAdress, int port, bool add = true, int timeout = 1000)
        {
            IPAddress[] ip = Dns.GetHostAddresses(IPAdress);

            Socket Socket = new Socket(ip[0].AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            IAsyncResult result = Socket.BeginConnect(ip[0], port, null, null);
            bool success = result.AsyncWaitHandle.WaitOne(timeout, true);

            if (!success)
            {
                Socket.Close();
                return (null);
            }

            Client c = new Client(Socket);
            if (add)
                ToAddClients.Add(c);

            return (c);
        }
    }
}
