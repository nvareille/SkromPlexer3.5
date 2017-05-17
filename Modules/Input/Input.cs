using System;
using System.Collections.Generic;
using SkromPlexer.ServerCore;
using SkromPlexer.Tools;

namespace DragonDreamServer.Classes.Modules.Input
{
    public delegate void InputCommandDelegate(Core Core, string[] arguments);

    public class InputModule : IModule
    {
        private string Input;
        private Dictionary<string, InputCommandDelegate> Commands;

        public static void CheckParams(int Expected, string[] Arguments, bool mode)
        {
            if ((mode && Expected > Arguments.Length) || (!mode && Expected != Arguments.Length))
                throw new Exception("Bad arguments");
        }

        public void Init(Core core)
        {
            Input = "";
            Commands = new Dictionary<string, InputCommandDelegate>();
        }

        public void AddCommand(Dictionary<string, InputCommandDelegate> c)
        {
            foreach (KeyValuePair<string, InputCommandDelegate> pair in c)
            {
                Commands.Add(pair.Key, pair.Value);
            }
        }

        public void AddCommand(string str, InputCommandDelegate d)
        {
            Commands.Add(str, d);
        }

        public void Start(Core core)
        {
        }

        public void Update(Core core)
        {
            if (Console.KeyAvailable)
            {
                while (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey();

                    if (key.Key == ConsoleKey.Enter)
                        AssemblyCommand(core);
                    else if (key.Key == ConsoleKey.Backspace)
                        Input = Input.Substring(0, Input.Length > 1 ? Input.Length - 1 : 0);
                    else
                        Input += key.KeyChar;
                }
            }
        }

        public void AssemblyCommand(Core core)
        {
            Log.Write("\n");

            try
            {
                while (Input.Contains("  "))
                    Input = Input.Replace("  ", " ");

                var args = Input.Split(' ');
                Commands[args[0]](core, args);
            }
            catch (Exception e)
            {
                Log.Error("{0}\n", e.Message);
                Log.Error("Wrong command\n");
            }

            Input = "";
        }
    }
}