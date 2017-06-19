using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using SkromPlexer.Configuration;
using SkromPlexer.Network;
using SkromPlexer.PacketCreator;
using SkromPlexer.ServerCore;

namespace SkromPlexer.Modules.Download
{
    public class DownloadModuleConfig
    {
        public int MaxPacketSize = 1024;
        public string TempFileFolder = "TempFolder/";
    }

    public class DownloadModule : AConfigurable, IModule
    {
        public List<DownloadTask> Tasks;
        private Core Core;
        public DownloadModuleConfig DownloadModuleConfig;

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
            foreach (DownloadTask task in Tasks)
            {
                if (task.Started)
                {
                    try
                    {
                        byte[] b = new byte[DownloadModuleConfig.MaxPacketSize];

                        if (task.Mode == false && !task.Ended)
                        {
                            int i = task.Client.GetSocket().Receive(b, b.Length, SocketFlags.None);

                            task.Downloaded += i;

                            if (i != 0)
                                task.Write(b, i);

                            if (task.Downloaded == task.Size || i == 0)
                                task.Ended = true;
                            
                            if (task.Ended)
                            {
                                task.ToRemove = true;
                                task.FileStream.Close();
                                try
                                {
                                    if (task.Downloaded == task.Size)
                                        Core.GetModule<Plexer>().HandlePackets(Core, task.MainClient, task.Callback);
                                    else
                                        throw new Exception();
                                }
                                catch (Exception)
                                {
                                    task.MainClient.SendingPackets.Add(DownloadPacketCreator.UnvalidateDL(task.Token));
                                }
                            }
                        }
                        else if (!task.Ended)
                        {
                            task.ReadFile();
                            task.Send();
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            Tasks.RemoveAll(i => i.ToRemove && i.Client.SocketDisconnect());
        }

        public DownloadTask CreateDownload(string hash)
        {
            Plexer p = Core.GetModule<Plexer>();
            DownloadTask d = new DownloadTask();

            d.CreateTempFile(DownloadModuleConfig.TempFileFolder, hash);
            Tasks.Add(d);
            return (d);
        }

        public void SendFile(Client client, string path, Packet packet, FileUploadCallback sent = null, FileUploadCallback success = null, FileUploadCallback fail = null)
        {
            client.SendingPackets.Add(DownloadPacketCreator.AskDownload((int)new FileInfo(path).Length, "test", packet));
            Tasks.Add(new DownloadTask()
            {
                MainClient = client,
                FileStream = File.OpenRead(path),
                Hash = "test",
                Sent = sent,
                Success = success,
                Fail = fail
            });
        }

        public DownloadTask CreateTask(Client c, int token)
        {
            DownloadTask t = new DownloadTask();

            t.Client = c;
            t.Token = token;
            t.Mode = false;
            t.Started = false;

            return (null);
        }

        public void BindTask(Client c, Func<DownloadTask, bool> pred, object o, bool mode)
        {
            DownloadTask t = Tasks.First(pred);

            t.Client = c;

            if (o != null)
            {
                if (o is int)
                    t.Token = (int)o;
                else
                    t.Hash = (string)o;
            }

            t.Mode = mode;
            t.Started = true;
            c.IsFileSocket = true;
        }


        public DownloadTask GetCurrentTask()
        {
            return (Tasks.FirstOrDefault(i => i.Ended));
        }
    }
}
