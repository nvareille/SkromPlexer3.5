# Server Creation

After starting a new project, Let's directly create a server that will just handle connections

---

# C# Project (Stand alone and not Unity3D)
```cs
namespace Project
{
	class Program
	{
		static void Main(string[] args)
		{
			// Instancing the Server component
			Core Core = new Core();
		
			// Initializing it (Useful when using custom modules)
			Core.Init();

			// Starting the server, 'Run' method will run a Loop
			Core.Run();
		}
	}
}
```

### Why isn't this example compatible with Unity3D ?
In this example, We use the ```Core.Run()``` Method.

It will start an infinite loop (Which will block our execution within Unity).

If using a server in Unity, we should call another method instead ```Core.Start()```

---

# C# Project (Unity3D)
```cs
public void Start()
{
	// Instancing the Server component
	Core Core = new Core();
		
	// Initializing it (Useful when using custom modules)
	Core.Init();

	// Starting the server, 'Start' method will not run a Loop
	Core.Start();
}

public void Update()
{
	// Because we used the Method Core.Start(), we have to use manually the Core.Update() Method
	Core.Update();
}
```

---
# Is this over yet ?

No, we still have the configuration to do ...

If the config wasn't created before, it will be by default.

Let's modify it !

### Modifying the config

- The program created some config files & folder at the execution path
- Go into 'ConfigDirectory' and notice several files
- Open PlexerConfig.json
- Modify the 'Port' field to match your desired port (8080 by default)
- The 'IPToConnect' field is only used client side
- Your server is configured !

---

<div style="text-align: right;">Examples.md</div><div style="text-align: right;">Examples.md</div>