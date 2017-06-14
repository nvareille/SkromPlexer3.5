using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using SkromPlexer.Modules.Download;
using SkromPlexer.Network;
using SkromPlexer.PacketCreator;
using SkromPlexer.ServerCore;

namespace SkromPlexer.PacketHandlers
{
    public class DownloadPacketHandler : APacketHandler
    {
        public static List<Packet> AskDownload(Core core, Client client, Packet packet)
        {
            PacketArg args = packet.GetArguments(new[]
            {
                typeof(int),
                typeof(string)
            });

            DownloadModule module = core.GetModule<DownloadModule>();

            Console.WriteLine("ASK");

            int t = module.CreateDownload(args.Get<string>(1));

            return (DownloadPacketCreator.DownloadAccept(t, args.Get<string>(1)).ToList());
        }

        public static List<Packet> DownloadAccept(Core core, Client client, Packet packet)
        {
            PacketArg args = packet.GetArguments(new[]
            {
                typeof(int),
                typeof(string)
            });

            DownloadModule module = core.GetModule<DownloadModule>();
            Plexer p = core.GetModule<Plexer>();
            IPEndPoint end = (IPEndPoint)client.GetSocket().RemoteEndPoint;

            module.BindTask(args.Get<int>(0), args.Get<string>(1));
            p.ConnectToServer(end.Address.ToString(), end.Port, true, args.Get<int>(0));
            
            return (null);
        }
    }
}
