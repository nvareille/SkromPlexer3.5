using System;
using System.Collections.Generic;
using SkromPlexer.ServerCore;
using SkromPlexer.Tools;

namespace SkromPlexer.Modules.Input
{
    public delegate void InputCommandDelegate(Core Core, string[] arguments);

    /// <summary>
    /// The module that handles the user input on terminal
    /// </summary>
    public class InputModule : IModule
    {
        private string Input;
        private Dictionary<string, InputCommandDelegate> Commands;

        /// <summary>
        /// Will check if the command command is respected
        /// </summary>
        /// <param name="Expected">Number of arguments</param>
        /// <param name="Arguments">The arguments</param>
        /// <param name="mode">Is infinite args</param>
        public static void CheckParams(int Expected, string[] Arguments, bool mode)
        {
            if ((mode && Expected > Arguments.Length) || (!mode && Expected != Arguments.Length))
                throw new Exception("Bad arguments");
        }

        /// <summary>
        /// Module initialisation
        /// </summary>
        /// <param name="core"></param>
        public void Init(Core core)
        {
            Input = "";
            Commands = new Dictionary<string, InputCommandDelegate>();
        }

        /// <summary>
        /// Adds a command to the module
        /// </summary>
        /// <param name="c">The commands into a dictionnary</param>
        public void AddCommand(Dictionary<string, InputCommandDelegate> c)
        {
            foreach (KeyValuePair<string, InputCommandDelegate> pair in c)
            {
                Commands.Add(pair.Key, pair.Value);
            }
        }

        /// <summary>
        /// Adds a single command
        /// </summary>
        /// <param name="str">The command string</param>
        /// <param name="d">The command's method</param>
        public void AddCommand(string str, InputCommandDelegate d)
        {
            Commands.Add(str, d);
        }

        /// <summary>
        /// The server starting function
        /// </summary>
        /// <param name="core">A reference to the Core</param>
        public void Start(Core core)
        {
        }

        /// <summary>
        /// The server Update function
        /// </summary>
        /// <param name="core">A reference to the Core</param>
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

        /// <summary>
        /// Computes the command and try to execute it
        /// </summary>
        /// <param name="core">A reference to the Core</param>
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