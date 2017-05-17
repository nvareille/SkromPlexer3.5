# Packet Creator & Best Practices

One of the best way to create packets and format them is to create a PacketCreator class

---

## Examples
```cs
// This Packet will contain data. For test purposes, it may be convenient to create a test packet:
new Packet("Test:\n");
```

However, in case we use multiple times this packet, we could create a PacketCreator

```cs
public static class TestPacketCreator
{
	public static Packet TestPacket()
	{
		return (new Packet("Test:\n"));
	}
	
	public static Packet TestIdPacket(int id)
	{
		return (new Packet("TestId:{0}\n", id));
	}
}

...

// We can call our packet this way:
client.SendingPackets.Add(TestPacketCreator.TestPacket());
client.SendingPackets.Add(TestPacketCreator.TestIdPacket(42));

```

---
## What's the best practice ?

The library uses a tool to generate the corresponding PacketHandlers from a given PacketCreator

```cs
// We are using attribute PacketCreator on the PacketCreator Class
[PacketCreator]
public static class TestPacketCreator
{
	// We are unsing attribute PacketCreatorFunction on the corresponding method
	// The first argument corresponds to the data sent
	[PacketCreatorFunction("Test")]
	public static Packet TestPacket()
	{
		// MethodBase.GetCurrentMethod() will fetch for this method and will access to PacketCreatorFunction arguments
		return (new Packet(MethodBase.GetCurrentMethod()));
	}
	
	// The second argument corresponds to the requested types for formating the packet
	[PacketCreatorFunction("TestId", new[]
	{
		typeof(int)
	})]
	public static Packet TestIdPacket(int id)
	{
		return (new Packet(MethodBase.GetCurrentMethod(), id));
	}
}
```

---

# [<< Client Creation](ClientCreation.md) | [PacketHandlers >>](PacketHandlers.md)