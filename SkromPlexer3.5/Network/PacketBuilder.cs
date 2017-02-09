using System.Collections.Generic;
using System.Text;

namespace SkromPlexer.Network
{
    class PacketBuilder
    {
        private string Content;
        public bool HaveCompletePackets;

        public void BuildPacket(byte[] data)
        {
            Content += Encoding.Default.GetString(data);
            CheckPackets();
        }

        private void CheckPackets()
        {
            while (Content.Contains("\n\n"))
                Content = Content.Replace("\n\n", "\n");
            while (Content.Contains("\0"))
                Content = Content.Replace("\0", "");
            HaveCompletePackets = Content.Contains("\n");
        }

        private Packet GetPacket()
        {
            if (Content.Contains("\n"))
            {
                string[] content = Content.Split('\n');

                Content = Content.Substring(content[0].Length + 1);
                CheckPackets();
                return (new Packet(content[0]));
            }
            return (null);
        }

        public List<Packet> ExtractPackets()
        {
            List<Packet> packets = new List<Packet>();

            while (HaveCompletePackets)
            {
                Packet packet = GetPacket();

                if (packet != null)
                    packets.Add(packet);
            }
            return (packets);
        }
    }
}
