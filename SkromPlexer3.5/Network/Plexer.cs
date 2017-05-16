using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using SkromPlexer.Configuration;
using SkromPlexer.PacketHandlers;
using SkromPlexer.ServerCore;

#pragma warning disable 0649

namespace SkromPlexer.Network
{
    public class PlexerConfig
    {
        public int Port;
        public string IPToConnect;
    }

    public class Plexer : AConfigurable, IModule
    {
        private TcpListener Listener;
        public List<Client> Clients;
        public List<Client> ToUpgrade;
        public PlexerConfig PlexerConfig;
        private PacketHandler PacketHandler;

        private List<ServerClient> ServerClients;
        
        public Plexer(APacketHandler[] packetHandlers)
        {
            Clients = new List<Client>();
            ToUpgrade = new List<Client>();
            PacketHandler = new PacketHandler(packetHandlers);
        }

        public void Init(Core core)
        {
            ServerClients = core.GameServerClients;

            if (core.IsServer)
                Listener = new TcpListener(IPAddress.Any, PlexerConfig.Port);
        }

        public void Start(Core core)
        {
            if (core.IsServer)
                Listener.Start();
        }

        public void HandlePackets(Core core, Client client, Packet packet)
        {
            PacketHandler.TreatPacket(core, client, packet);
        }

        public void Update(Core core)
        {
            Clients.RemoveAll(c => !c.IsConnected() && c.ExecuteDisconnectCallbacks());
            ServerClients.RemoveAll(c => !c.IsConnected() && c.ExecuteDisconnectCallbacks());

            if (core.IsServer && Listener.Pending())
            {
                Clients.Add(new Client(Listener.AcceptSocket()));
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

            ForceDisconnect();
        }

        public void ForceDisconnect()
        {
            Clients.Where(c => c.MustDisconnect).All(c => c.SocketDisconnect());
            ServerClients.Where(c => c.MustDisconnect).All(c => c.SocketDisconnect());
        }

        public Client ConnectToServer(string IPAdress, int port, bool add = false)
        {
            IPAddress[] ip = Dns.GetHostAddresses(IPAdress);

            Socket Socket = new Socket(ip[0].AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            Socket.Connect(ip[0], port);

            Client c = new Client(Socket);
            if (add)
                Clients.Add(c);

            return (c);
        }
    }
}
