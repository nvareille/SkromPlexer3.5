using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Mime;
using System.Reflection;
using SkromPlexer.PacketHandlers;

namespace SkromPlexer.Network
{
    /// <summary>
    /// Represents arguments received within a Packet
    /// </summary>
    public class PacketArg
    {
        private int Count;
        public object[] Args;

        /// <summary>
        /// The class constructor
        /// </summary>
        /// <param name="args">The arguments within the packet</param>
        protected PacketArg(object[] args)
        {
            Count = 0;
            Args = args;
        }

        /// <summary>
        /// Get an argument from the Packet
        /// </summary>
        /// <typeparam name="T">The type to cast to</typeparam>
        /// <param name="id">The index of the argument</param>
        /// <returns>The argument of the packet</returns>
        public T Get<T>(int id)
        {
            return ((T) Args[id]);
        }

        /// <summary>
        /// Get an argument from the Packet at the current index
        /// </summary>
        /// <returns>The argument of the packet</returns>
        public T Get<T>()
        {
            return ((T)Args[Count++]);
        }

        /// <summary>
        /// Will call to the class constructor
        /// </summary>
        /// <param name="args">The arguments of the packet</param>
        /// <returns>A PacketArg instance</returns>
        public static PacketArg CreatePacketArg(object[] args)
        {
            return (new PacketArg(args));
        }
    }

    /// <summary>
    /// Packet that will shut down the socket upon sending
    /// </summary>
    public class CriticPacket : Packet
    {
        /// <summary>
        /// The class constructor
        /// </summary>
        /// <param name="content">the Packet content</param>
        public CriticPacket(string content) : base(content)
        {
            IsCriticPacket = true;
        }

        /// <summary>
        /// The class constructor
        /// </summary>
        /// <param name="content">The Packet content</param>
        /// <param name="args">The Packet arguments</param>
        public CriticPacket(string content, params object[] args) : base(content, args)
        {
            IsCriticPacket = true;
        }

        /// <summary>
        /// The class constructor
        /// </summary>
        /// <param name="info">The calling method info for getting the PacketCreatorFunction attribute</param>
        /// <param name="args">The Packet arguments</param>
        public CriticPacket(MethodBase info, params object[] args) : base(info, args)
        {
            IsCriticPacket = true;
        }
    }

    /// <summary>
    /// A class for containing data to be sent or received
    /// </summary>
    public class Packet
    {
        public string Content;
        public bool IsCriticPacket;

        /// <summary>
        /// The class constructor
        /// </summary>
        /// <param name="info">The calling method info for getting the PacketCreatorFunction attribute</param>
        /// <param name="args">The Packet arguments</param>
        public Packet(MethodBase info, params object[] args)
        {
            PacketCreatorFunction p = (PacketCreatorFunction)info.GetCustomAttributes(typeof(PacketCreatorFunction), false)[0];

            Content = p.Packet + ":" + GenArgs(args) + "\n";
        }

        /// <summary>
        /// Will generate arguments to append in the packet
        /// </summary>
        /// <param name="args">The packet Arguments</param>
        /// <returns>The string append in the packet</returns>
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

        /// <summary>
        /// The class constructor
        /// </summary>
        /// <param name="content">The Packet content</param>
        public Packet(string content)
        {
            Content = content;
        }

        /// <summary>
        /// The class constructor
        /// </summary>
        /// <param name="content">The packet content</param>
        /// <param name="args">The Packet arguments</param>
        public Packet(string content, params object[] args)
        {
            Content = String.Format(content, args);
        }

        /// <summary>
        /// Generate the packet action
        /// </summary>
        /// <returns>A string with the action name to use</returns>
        public string PacketAction()
        {
            return (Content.Split(':')[0]);
        }

        /// <summary>
        /// Will extract arguments from the packet string
        /// </summary>
        /// <param name="types">The expected types</param>
        /// <returns>A PacketArg containing the arguments</returns>
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

        /// <summary>
        /// Transforms a Packent into a List
        /// </summary>
        /// <returns>A List of Packets containing the Packet</returns>
        public List<Packet> ToList()
        {
            List<Packet> p = new List<Packet>();

            p.Add(this);
            return (p);
        }

        /// <summary>
        /// Append a Packet to a list
        /// </summary>
        /// <param name="list">The list to append to</param>
        /// <param name="p">the Packet to append</param>
        /// <returns>The filled list</returns>
        public static List<Packet> operator +(List<Packet> list, Packet p)
        {
            list.Add(p);
            return (list);
        }

        /// <summary>
        /// Creates a list form 2 Packets
        /// </summary>
        /// <param name="list">First Packet</param>
        /// <param name="p">Second Packet</param>
        /// <returns>A filled list with the 2 Packets</returns>
        public static List<Packet> operator +(Packet list, Packet p)
        {
            List<Packet> l = list.ToList();

            l.Add(p);
            return (l);
        }

        /// <summary>
        /// Appends a Packet to a list
        /// </summary>
        /// <param name="p">The Packet to append</param>
        /// <param name="list">The list to append to</param>
        /// <returns></returns>
        public static List<Packet> operator +(Packet p, List<Packet> list)
        {
            List<Packet> l = new List<Packet>();

            l.Add(p);
            l.AddRange(list);
            return (l);
        }

        public string ExtractAfter(char c, int occurence)
        {
            int count = 0;
            string extract = "";

            while (count < Content.Length)
            {
                if (occurence <= 0)
                    extract += Content[count];
                if (Content[count] == c)
                    --occurence;
                ++count;
            }

            return (extract);
        }
    }

    /// <summary>
    /// A Class extension for lists of Packets
    /// </summary>
    public static class PacketExtend
    {
        /// <summary>
        /// Merge 2 lists of Packets
        /// </summary>
        /// <param name="l1">First list</param>
        /// <param name="l2">Second list</param>
        /// <returns></returns>
        public static List<Packet> Append(this List<Packet> l1, List<Packet> l2)
        {
            l1.AddRange(l2);
            return (l1);
        }
    }
}
