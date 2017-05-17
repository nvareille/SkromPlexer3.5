using System;
using System.Collections.Generic;
using System.Globalization;

namespace SkromPlexer.Network
{
    public class PacketArg
    {
        private int Count;
        public object[] Args;

        protected PacketArg(object[] args)
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

        public static PacketArg CreatePacketArg(object[] args)
        {
            return (new PacketArg(args));
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
            CultureInfo c = new CultureInfo("en-us");
            List<object> arguments = new List<object>();
            string[] args = Content.Split(':')[1].Split(' ');
            int count = 0;

            foreach (string arg in args)
            {
                if (count >= types.Length)
                    return (PacketArg.CreatePacketArg(arguments.ToArray()));

                if (types[count] == typeof(bool))
                    arguments.Add(Convert.ToBoolean(arg));
                else if (types[count] == typeof(string))
                    arguments.Add(arg);
                else if (types[count] == typeof(int))
                    arguments.Add(Convert.ToInt32(arg));
                else if (types[count] == typeof(float))
                    arguments.Add(float.Parse(arg.Replace(',', '.'), c));
                ++count;
            }

            return (PacketArg.CreatePacketArg(arguments.ToArray()));
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

        public static List<Packet> operator +(Packet p, List<Packet> list)
        {
            List<Packet> l = new List<Packet>();

            l.Add(p);
            l.AddRange(list);
            return (l);
        }
    }

    public static class PacketExtend
    {
        public static List<Packet> Append(this List<Packet> l1, List<Packet> l2)
        {
            l1.AddRange(l2);
            return (l1);
        }
    }
}
