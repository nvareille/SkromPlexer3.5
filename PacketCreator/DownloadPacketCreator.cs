using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using SkromPlexer.Network;
using SkromPlexer.PacketHandlers;

namespace SkromPlexer.PacketCreator
{
    [PacketHandlers.PacketCreator]
    public static class DownloadPacketCreator
    {
        [PacketCreatorFunction("AskDownload", new []
         {
             "int", "File Size",
             "string", "Hash",
             "string", "Callback"
         })]
        public static Packet AskDownload(int size, string hash, Packet callback)
        {
            return (new Packet(MethodBase.GetCurrentMethod(), size, hash, callback.Content));
        }

        [PacketCreatorFunction("DownloadAccept", new []
         {
             "int", "Token",
             "string", "Hash"
         })]
        public static Packet DownloadAccept(int token, string hash)
        {
            return new Packet(MethodBase.GetCurrentMethod(), token, hash);
        }

        [PacketCreatorFunction("ValidateDL", new []
         {
             "int", "DL Token"
         })]
        public static Packet ValidateDL(int token)
        {
            return (new Packet(MethodBase.GetCurrentMethod(), token));
        }

        [PacketCreatorFunction("UnvalidateDL", new []
         {
             "int", "DL Token"
         })]
        public static Packet UnvalidateDL(int token)
        {
            return (new Packet(MethodBase.GetCurrentMethod(), token));
        }
    }
}
