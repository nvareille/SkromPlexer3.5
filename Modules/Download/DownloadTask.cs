using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SkromPlexer.Modules.Download
{
    public class DownloadTask
    {
        public bool Asked;
        public bool Mode;
        public int Token;
        public string Hash;
        public FileStream FileStream;

        public void CreateTempFile(string hash)
        {
            FileStream = File.Create("random");
            Token = 4242;
            Hash = hash;
        }
    }
}
