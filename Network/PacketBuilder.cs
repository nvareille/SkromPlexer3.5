using System.Collections.Generic;
using System.Text;

namespace SkromPlexer.Network
{
    /// <summary>
    /// A class to build Packets upon reception
    /// </summary>
    class PacketBuilder
    {
        private string Content;
        public bool HaveCompletePackets;

        /// <summary>
        /// Append data to the builder
        /// </summary>
        /// <param name="data">Data to append</param>
        public void BuildPacket(byte[] data)
        {
            Content += Encoding.Default.GetString(data);
            CheckPackets();
        }

        /// <summary>
        /// Checks if a Packet is available
        /// </summary>
        private void CheckPackets()
        {
            while (Content.Contains("\n\n"))
                Content = Content.Replace("\n\n", "\n");
            while (Content.Contains("\0"))
                Content = Content.Replace("\0", "");
            HaveCompletePackets = Content.Contains("\n");
        }

        /// <summary>
        /// Extract a Packet built
        /// </summary>
        /// <returns>Packet or null if unaviable</returns>
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

        /// <summary>
        /// Get a List of Packets if available
        /// </summary>
        /// <returns>List of Packet or null if unaviable</returns>
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
