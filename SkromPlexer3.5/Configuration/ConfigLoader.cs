using System;
using System.IO;
using Newtonsoft.Json;

namespace SkromPlexer.Configuration
{
    public class ConfigLoaderConfig
    {
        public string Path;
    }

    public class ConfigLoader
    {
        public static ConfigLoaderConfig Config;

        public static T LoadJson<T>(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException(String.Format("Impossible to load the config file ({0})", path));
            return (JsonConvert.DeserializeObject<T>(File.ReadAllText(path)));
        }

        public static object LoadJson(string path, Type obj)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException(String.Format("Impossible to load the config file ({0})", path));
            return (JsonConvert.DeserializeObject(File.ReadAllText(path), obj));
        }

        public static void InitConfig()
        {
            Config = LoadJson<ConfigLoaderConfig>("config.json");
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
