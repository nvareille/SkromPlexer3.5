using System;
using System.Collections.Generic;
using System.Reflection;
using SkromPlexer.Network;

namespace SkromPlexer.PacketHandlers
{
    public class PacketHandler : Attribute { }

    public class PacketCreator : Attribute { }

    public class PacketHandlerFunction : Attribute
    {
        public Type[] Args;

        public PacketHandlerFunction(Type[] args = null)
        {
            Args = args;
        }
    }

    public class PacketCreatorFunction : Attribute
    {
        public string Packet;
        public Type[] Args;

        public PacketCreatorFunction(string packet, Type[] args = null)
        {
            Packet = packet;
            Args = args;
        }
    }



    public class APacketHandler
    {
        public virtual void Init(Dictionary<string, PacketHandlerDelegate> Actions)
        {
            RegisterActions(Actions);
        }

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
