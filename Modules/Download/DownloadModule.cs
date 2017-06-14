using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkromPlexer.Network;
using SkromPlexer.PacketCreator;
using SkromPlexer.ServerCore;

namespace SkromPlexer.Modules.Download
{
    public class DownloadModule : IModule
    {
        private List<DownloadTask> Tasks;
        private Core Core;

        public void Init(Core core)
        {
            Tasks = new List<DownloadTask>();
            Core = core;
        }

        public void Start(Core core)
        {
        }

        public void Update(Core core)
        {
        }

        public int CreateDownload(string hash)
        {
            Plexer p = Core.GetModule<Plexer>();
            DownloadTask d = new DownloadTask();

            d.CreateTempFile(hash);
            Tasks.Add(d);
            return (d.Token);
        }

        public void SendFile(Client client, string path)
        {
            Tasks.Add(new DownloadTask() {Hash = "test"});
            client.SendingPackets.Add(DownloadPacketCreator.AskDownload(42, "test"));
        }

        public void BindTask(int token, string hash)
        {
            Tasks.First(i => i.Hash == hash).Token = token;
        }
    }
}
