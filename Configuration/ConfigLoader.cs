using System;
using System.IO;
using Newtonsoft.Json;

namespace SkromPlexer.Configuration
{
    /// <summary>
    /// The config for the ConfigLoader class
    /// </summary>
    public class ConfigLoaderConfig
    {
        public string Path = "ConfigDirectory/";
    }

    /// <summary>
    /// The class used to define config loading
    /// </summary>
    public class ConfigLoader
    {
        public static ConfigLoaderConfig Config;

        /// <summary>
        /// Load a json from a given path
        /// </summary>
        /// <typeparam name="T">The type to cast to</typeparam>
        /// <param name="path">The path toi load from</param>
        /// <param name="isPathFile">Is it the path file (Where the path for all config is written)</param>
        /// <returns>The instance of the config</returns>
        public static T LoadJson<T>(string path, bool isPathFile = false)
        {
            if (!File.Exists(path))
            {
                if (isPathFile)
                {
                    Config = new ConfigLoaderConfig();
                    Directory.CreateDirectory(Config.Path);
                    File.WriteAllText("Config.json", JsonConvert.SerializeObject(Config, Formatting.Indented));
                }
                Console.WriteLine(
                    String.Format("Impossible to load the config file ({0}). An empty config file was created", path));
            }
            return (JsonConvert.DeserializeObject<T>(File.ReadAllText(path)));
        }

        /// <summary>
        /// Loads a json from the given type
        /// </summary>
        /// <param name="path">The path to file</param>
        /// <param name="obj">The type to load</param>
        /// <returns></returns>
        public static object LoadJson(string path, Type obj)
        {
            if (!File.Exists(path))
            {
                File.WriteAllText(path, JsonConvert.SerializeObject(Activator.CreateInstance(obj), Formatting.Indented));
                throw new FileNotFoundException(String.Format("Impossible to load the config file ({0}), Creating it", path));
            }
            return (JsonConvert.DeserializeObject(File.ReadAllText(path), obj));
        }

        /// <summary>
        /// Initializes the class
        /// </summary>
        public static void InitConfig()
        {
            Config = LoadJson<ConfigLoaderConfig>("config.json", true);
        }

        /// <summary>
        /// Load a config file
        /// </summary>
        /// <typeparam name="T">The type to cast to</typeparam>
        /// <param name="file">the file's path</param>
        /// <returns>The config instance</returns>
        public static T LoadConfigFile<T>(string file)
        {
            return (LoadJson<T>(Config.Path + file));
        }

        /// <summary>
        /// Try to load a config file, it will create it if the file doesn't exist
        /// </summary>
        /// <param name="file">The file's path</param>
        /// <param name="obj">the type to cast to</param>
        /// <returns></returns>
        public static object LoadConfigFile(string file, Type obj)
        {
            try
            {
                return (LoadJson(Config.Path + file, obj));
            }
            catch (Exception)
            {
                return (LoadJson(Config.Path + file, obj));
            }
        }
    }
}
