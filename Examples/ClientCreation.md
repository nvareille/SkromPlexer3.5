# Client Creation

We have our server, let's connect a client to it

---

## Connecting to Server
```cs
// Instance of the module
Core = new Core();

// Initialisation, false if this is a Client
Core.Init(false);

// Registration of an object that we can get later
Core.SetData(this);

// Start() or Run() depending on what type of program we are doing
Core.Start();

// Get the Plexer module, the module that handles the connections
Plexer PInstance = Core.GetModule<Plexer>();

// Creation of a Client and connection to the server (Using the IPToConnect & Port from config file)
Client client = PInstance.ConnectToServer(Plexer.PlexerConfig.IPToConnect, Plexer.PlexerConfig.Port);

// We are connected to the server !!
```

---

## Sending Data to server

```cs
// client is an Instance of the Client class
// SendingPackets is a List of Packet (Our data container for the library)

// Sending a Packet "Ping" to the client always put a ':' and a "\n" inside, these are delimiters
client.SendingPackets.Add(new Packet("Ping:\n"));

```

The Packets and Data will always be treated on ```Core.Update()``` if available

---

## [<< Server Creation](ServerCreation.md) | [PacketCreator & Best Practices >>](PacketCreator.md)