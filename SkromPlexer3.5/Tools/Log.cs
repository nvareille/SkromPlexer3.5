using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkromPlexer.ServerCore;

namespace SkromPlexer.Tools
{
    public delegate void WriteFunction(string str);

    public delegate void ActionFunction();

    public class Log
    {
        public static Core Core;

        public static void Debug(string str, params object[] args)
        {
            if (!Core.CoreConfig.IsRelease)
                Write(str, args);
        }

        public static void Debug(string str)
        {
            if (!Core.CoreConfig.IsRelease)
                Write(str);
        }

        public static void Info(string str, params object[] args)
        {
            Write(str, args);
        }

        public static void Info(string str)
        {
            Write(str);
        }

        public static void Warning(string str, params object[] args)
        {
            Write(str, args);
        }

        public static void Warning(string str)
        {
            Write(str);
        }

        public static void Error(string str, params object[] args)
        {
            Write(str, args);
        }

        public static void Error(string str)
        {
            Write(str);
        }

        public static string F(string str, params object[] args)
        {
            return (String.Format(str, args));
        }

        public static void Write(string str, params object[] args)
        {
            Console.Write(F(str, args));
        }

        public static void Write(string str)
        {
            Console.Write(str);
        }

        public static void NewLine(WriteFunction fct)
        {
            fct("\n");
        }

        public static void Composite(string before, string after, WriteFunction write, ActionFunction fct)
        {
            write.DynamicInvoke(before);
            fct.DynamicInvoke();
            write.DynamicInvoke(after);
            Write("\n");
        }
    }
}
