using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SkromPlexer.Network;
using SkromPlexer.Tools;

namespace SkromPlexer.Modules.Download
{
    public delegate void FileUploadCallback();

    public class DownloadTask
    {
        public bool Asked;
        public bool Mode;
        public bool Started;
        public bool Ended;
        public bool ToRemove;
        public int Token;
        public int Size;
        public int Downloaded;
        public string Hash;
        public string FileName;
        public Packet Callback;
        public FileStream FileStream;
        public Client MainClient;
        public Client Client;
        public Queue<Tuple<byte[], int, int>> ReadBuff;
        public FileUploadCallback Sent;
        public FileUploadCallback Success;
        public FileUploadCallback Fail;

        public DownloadTask()
        {
            ReadBuff = new Queue<Tuple<byte[], int, int>>();
        }

        public void CreateTempFile(string path, string hash)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            FileName = path + Randomizer.RandomHexString(16);
            FileStream = new FileStream(FileName, FileMode.Create);
            Token = 4242;
            Hash = hash;
        }

        public void Write(byte[] buffer, int length)
        {
            FileStream.Write(buffer, 0, length);
        }

        public void ReadFile()
        {
            byte[] b = new byte[1024];
            int i = FileStream.Read(b, 0, b.Length);

            if (i != 0)
                ReadBuff.Enqueue(new Tuple<byte[], int, int>(b, i, 0));
        }

        public void Send()
        {
            if (ReadBuff.Count == 0)
            {
                Ended = true;
                if (Sent != null)
                    Sent();
                return;
            }

            var t = ReadBuff.Peek();
            byte[] buff = new byte[t.Item2 - t.Item3];

            Buffer.BlockCopy(t.Item1, t.Item3, buff, 0, t.Item2 - t.Item3);
            int i = Client.GetSocket().Send(buff);

            t.Item3 += i;

            if (t.Item2 <= t.Item3)
                ReadBuff.Dequeue();
        }
    }
}
