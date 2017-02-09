using System;
using System.Collections.Generic;
using System.Reflection;
using SkromPlexer.Network;

namespace SkromPlexer.PacketHandlers
{
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
