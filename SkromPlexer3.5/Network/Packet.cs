using System;
using System.Collections.Generic;

namespace SkromPlexer.Network
{
    public class PacketArg
    {
        private int Count;
        private object[] Args;

        public PacketArg(object[] args)
        {
            Count = 0;
            Args = args;
        }

        public T Get<T>(int id)
        {
            return ((T) Args[id]);
        }

        public T Get<T>()
        {
            return ((T)Args[Count++]);
        }
    }

    public class Packet
    {
        public string Content;
        public bool IsCriticPacket;

        public Packet(string content, bool c = false)
        {
            Content = content;
            IsCriticPacket = c;
        }

        public Packet(string content, bool c, params object[] args)
        {
            Content = String.Format(content, args);
            IsCriticPacket = c;
        }

        public string PacketAction()
        {
            return (Content.Split(':')[0]);
        }

        public PacketArg GetArguments(Type[] types)
        {
            List<object> arguments = new List<object>();
            string[] args = Content.Split(':')[1].Split(' ');
            int count = 0;

            foreach (string arg in args)
            {
                if (count >= types.Length)
                    return (new PacketArg(arguments.ToArray()));

                if (types[count] == typeof(bool))
                    arguments.Add(Convert.ToBoolean(arg));
                else if (types[count] == typeof(string))
                    arguments.Add(arg);
                else if (types[count] == typeof(int))
                    arguments.Add(Convert.ToInt32(arg));
                ++count;
            }

            return (new PacketArg(arguments.ToArray()));
        }

        public List<Packet> ToList()
        {
            List<Packet> p = new List<Packet>();

            p.Add(this);
            return (p);
        }

        public static List<Packet> operator +(List<Packet> list, Packet p)
        {
            list.Add(p);
            return (list);
        }

        public static List<Packet> operator +(Packet list, Packet p)
        {
            List<Packet> l = list.ToList();

            l.Add(p);
            return (l);
        }
    }
}
