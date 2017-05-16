using System;
using System.IO;
using Newtonsoft.Json;

namespace SkromPlexer.Configuration
{
    public class ConfigLoaderConfig
    {
        public string Path = "ConfigDirectory/";
    }

    public class ConfigLoader
    {
        public static ConfigLoaderConfig Config;

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
                Console.WriteLine(String.Format("Impossible to load the config file ({0}). An empty config file was created", path));
            }
            return (JsonConvert.DeserializeObject<T>(File.ReadAllText(path)));
        }

        public static object LoadJson(string path, Type obj)
        {
            if (!File.Exists(path))
            {
                File.WriteAllText(path, JsonConvert.SerializeObject(Activator.CreateInstance(obj), Formatting.Indented));
                throw new FileNotFoundException(String.Format("Impossible to load the config file ({0})", path));
            }
            return (JsonConvert.DeserializeObject(File.ReadAllText(path), obj));
        }

        public static void InitConfig()
        {
            Config = LoadJson<ConfigLoaderConfig>("config.json", true);
        }

        public static T LoadConfigFile<T>(string file)
        {
            return (LoadJson<T>(Config.Path + file));
        }

        public static object LoadConfigFile(string file, Type obj)
        {
            return (LoadJson(Config.Path + file, obj));
        }
    }
}
