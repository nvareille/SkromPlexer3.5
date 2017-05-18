using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkromPlexer.ServerCore;

namespace SkromPlexer.Tools
{
    /// <summary>
    /// A delegate representing a formating function
    /// </summary>
    /// <param name="str">The string format</param>
    public delegate void WriteFunction(string str);

    /// <summary>
    /// A delegate representing an operation function
    /// </summary>
    public delegate void ActionFunction();

    /// <summary>
    /// A class to deal with logging output for Infos, Debugs, Warnings and Errors
    /// Won't display warnings and Debug while in release mode
    /// </summary>
    public class Log
    {
        /// <summary>
        /// A stored reference to the Core
        /// </summary>
        public static Core Core;

        /// <summary>
        /// Formats a Debug message
        /// It won't display while in release
        /// </summary>
        /// <param name="str">The String format</param>
        /// <param name="args">The args to fill the String</param>
        public static void Debug(string str, params object[] args)
        {
            if (!Core.CoreConfig.IsRelease)
                Write(str, args);
        }

        /// <summary>
        /// Formats a Debug message
        /// It won't display while in release
        /// </summary>
        /// <param name="str">The String format</param>
        public static void Debug(string str)
        {
            if (!Core.CoreConfig.IsRelease)
                Write(str);
        }

        /// <summary>
        /// Formats an Info message
        /// </summary>
        /// <param name="str">The String format</param>
        /// <param name="args">The args to fill the String</param>
        public static void Info(string str, params object[] args)
        {
            Write(str, args);
        }

        /// <summary>
        /// Formats an Info message
        /// </summary>
        /// <param name="str">The String format</param>
        public static void Info(string str)
        {
            Write(str);
        }

        /// <summary>
        /// Formats a warning message
        /// </summary>
        /// <param name="str">The String format</param>
        /// <param name="args">The args to fill the String</param>
        public static void Warning(string str, params object[] args)
        {
            Write(str, args);
        }

        /// <summary>
        /// Formats a warning message
        /// </summary>
        /// <param name="str">The String format</param>
        public static void Warning(string str)
        {
            Write(str);
        }

        /// <summary>
        /// Formats a Error message
        /// </summary>
        /// <param name="str">The String format</param>
        /// <param name="args">The args to fill the String</param>
        public static void Error(string str, params object[] args)
        {
            Write(str, args);
        }

        /// <summary>
        /// Formats a Error message
        /// </summary>
        /// <param name="str">The String format</param>
        public static void Error(string str)
        {
            Write(str);
        }

        /// <summary>
        /// A shortcut for the Format function
        /// </summary>
        /// <param name="str">The String format</param>
        /// <param name="args">The args to fill the String</param>
        /// <returns>The formated string</returns>
        public static string F(string str, params object[] args)
        {
            return (String.Format(str, args));
        }

        /// <summary>
        /// A shortcut for the Write function
        /// </summary>
        /// <param name="str">The String format</param>
        /// <param name="args">The args to fill the String</param>
        public static void Write(string str, params object[] args)
        {
            Console.Write(F(str, args));
        }

        /// <summary>
        /// A shortcut for the Write function
        /// </summary>
        /// <param name="str">The String format</param>
        public static void Write(string str)
        {
            Console.Write(str);
        }

        /// <summary>
        /// Will display a new live with the given formating function
        /// </summary>
        /// <param name="fct">The formating function to use</param>
        public static void NewLine(WriteFunction fct)
        {
            fct("\n");
        }

        /// <summary>
        /// Will display a composite message with a delegate function
        /// </summary>
        /// <param name="before">The message to display before the delegate execution</param>
        /// <param name="after">The message to display after the delegate execution</param>
        /// <param name="write">The formating function to use</param>
        /// <param name="fct">The delegate to execute</param>
        public static void Composite(string before, string after, WriteFunction write, ActionFunction fct)
        {
            write.DynamicInvoke(before);
            fct.DynamicInvoke();
            write.DynamicInvoke(after);
            Write("\n");
        }
    }
}
