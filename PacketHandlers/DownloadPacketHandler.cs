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
                typeof(string),
                typeof(string)
            });

            DownloadModule module = core.GetModule<DownloadModule>();
            DownloadTask t = module.CreateDownload(args.Get<string>(1));

            /*if (core.GetModule<Plexer>().PacketMayBeTreated(args.Get<string>(2)) == false)
                throw new Exception("Impossible to treat the packet " + args.Get<string>(2));*/

            t.Size = args.Get<int>(0);
            t.Callback = new Packet(packet.ExtractAfter(' ', 2));
            t.MainClient = client;
            return (DownloadPacketCreator.DownloadAccept(t.Token, args.Get<string>(1)).ToList());
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
            Client c = p.ConnectToServer(end.Address.ToString(), end.Port, true, args.Get<int>(0));

            int token = args.Get<int>(0);
            string hash = args.Get<string>(1);

            module.BindTask(c, (i => i.Hash == hash), token, true);

            return (null);
        }

        public static List<Packet> ValidateDL(Core core, Client client, Packet packet)
        {
            PacketArg args = packet.GetArguments(new[]
            {
                typeof(int)
            });

            int id = args.Get<int>(0);
            DownloadModule a = core.GetModule<DownloadModule>();
            DownloadTask t = a.Tasks.FirstOrDefault(i => i.Token == id);

            t.ToRemove = true;
            t.Success?.Invoke();
            return (null);
        }

        public static List<Packet> UnvalidateDL(Core core, Client client, Packet packet)
        {
            PacketArg args = packet.GetArguments(new[]
            {
                typeof(int)
            });

            int id = args.Get<int>(0);
            DownloadModule a = core.GetModule<DownloadModule>();
            DownloadTask t = a.Tasks.FirstOrDefault(i => i.Token == id);

            t.ToRemove = true;
            t.Fail?.Invoke();
            return (null);
        }
    }
}
