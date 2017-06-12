using System;
using System.Collections.Generic;
using System.Reflection;
using SkromPlexer.Network;

namespace SkromPlexer.PacketHandlers
{
    /// <summary>
    /// An attribute used by the generation tool
    /// </summary>
    public class PacketHandler : Attribute { }

    /// <summary>
    /// An attribute used by the generation tool
    /// Will generate a PacketHandler file if it is attributed to a class
    /// </summary>
    public class PacketCreator : Attribute { }

    /// <summary>
    /// An attribute used by the generation tool
    /// It will generate a PacketCreator callback if associed to a function
    /// </summary>
    public class PacketHandlerFunction : Attribute { }

    /// <summary>
    /// An attribute used by the generation tool
    /// It will generate a PacketHandler callback if associed to a function
    /// </summary>
    public class PacketCreatorFunction : Attribute
    {
        public string Packet;
        public Type[] Args;
        public string[] StringArgs;

        /// <summary>
        /// The attribute's constructor
        /// </summary>
        /// <param name="packet">The Packet content</param>
        /// <param name="args">The Packets arguments types</param>
        public PacketCreatorFunction(string packet, Type[] args = null)
        {
            Packet = packet;
            Args = args;
        }

        public PacketCreatorFunction(string packet, string[] args)
        {
            Packet = packet;
            StringArgs = args;
        }
    }

    /// <summary>
    /// Basis of a PacketHandler, best inherited
    /// </summary>
    public class APacketHandler
    {
        /// <summary>
        /// The Init function called from the Core
        /// </summary>
        /// <param name="Actions">The Dictionnary of actions to fill</param>
        public virtual void Init(Dictionary<string, PacketHandlerDelegate> Actions)
        {
            RegisterActions(Actions);
        }

        /// <summary>
        /// Will register the callbacks from the PacketHandler
        /// </summary>
        /// <param name="Actions">The Dictionanry containing the callbacks</param>
        public virtual void RegisterActions(Dictionary<string, PacketHandlerDelegate> Actions)
        {
            foreach (MethodInfo method in GetType().GetMethods())
            {
                if (method.ReturnParameter.ParameterType == typeof(List<Packet>))
                    Actions.Add(method.Name, (PacketHandlerDelegate)Delegate.CreateDelegate(typeof(PacketHandlerDelegate), method));
            }
        }
    }
}
