# Modules

Sometimes, we want the server to store data in a dedicated place, so we could process them later.

Maybe we want to automatize the processing, the modules are exactly made for this purpose.

---

# Module Creation

Let's create a module which will manage 'Games' for example

```cs
class GameModule : IModule
{
    public List<Game> Games;

	// This function will be called on Core.Init()
    public void Init(Core core)
    {
		// We create the Game container
        Games = new List<Game>();
    }

	// This function will be called on Core.Start()
    public void Start(Core core)
    {
		// Do stuff
    }

	// This function will be called on Core.Update() or every loop turn when calling Core.Run()
    public void Update(Core core)
    {
        foreach (Game game in Games)
        {
			// core.DeltaTime is the time elapsed between the last Update() call and this one
			// This value is defined in case our processing is time dependant
			// (Character movements, score management ...)
            game.Process(core.DeltaTime);
        }
	}
}
```

Modules alway inherits the IModule interface and implements the ```Init(Core)```, ```Start(Core)``` and ```Update(Core)``` functions

---

# Module Registration

Once we created the module, we have to register it like the PacketHandlers.

Go where you instanced the Core class:

```cs
// The first parameter here is an array of Modules to use
Core Core = new Core(new IModule[]
{
	new GameModule()
}, new APacketHandler[]
{
	new TestPacketHandler()
});
		
// Then we do our usual stuff
Core.Init();
Core.Run();
```