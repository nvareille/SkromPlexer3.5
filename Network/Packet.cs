using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using SkromPlexer.PacketHandlers;

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

    public class CriticPacket : Packet
    {
        public CriticPacket(string content) : base(content)
        {
            IsCriticPacket = true;
        }

        public CriticPacket(string content, params object[] args) : base(content, args)
        {
            IsCriticPacket = true;
        }

        public CriticPacket(MethodBase info, params object[] args) : base(info, args)
        {
            IsCriticPacket = true;
        }
    }

    public class Packet
    {
        public string Content;
        public bool IsCriticPacket;

        public Packet(MethodBase info, params object[] args)
        {
            PacketCreatorFunction p = (PacketCreatorFunction)info.GetCustomAttributes(typeof(PacketCreatorFunction), false)[0];

            Content = p.Packet + ":" + GenArgs(args) + "\n";
        }

        private string GenArgs(object[] args)
        {
            string str = "";
            bool padding = false;

            if (args != null)
            {
                foreach (object arg in args)
                {
                    str += String.Format("{0}{1}", (padding ? " " : ""), arg);
                    padding = true;
                }
            }

            return (str);
        }

        public Packet(string content)
        {
            Content = content;
        }

        public Packet(string content, params object[] args)
        {
            Content = String.Format(content, args);
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
