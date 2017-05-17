using System;
using System.IO;
using System.Reflection;
using SkromPlexer.Tools;

namespace SkromPlexer.Configuration
{
    public class AConfigurable
    {
        private Type ConfigType;
        private FieldInfo ConfigVariable;
        private object ConfigObject;

        public AConfigurable()
        {
            ConfigLoader.InitConfig();

            ConfigObject = this;
            ConfigType = GetType();

            Log.Composite(Log.F("Loading config for {0} ... ", ConfigType.Name), "Done !", Log.Info, () =>
            {
                foreach (FieldInfo field in ConfigType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    if (field.FieldType.Name == ConfigType.Name + "Config")
                    {
                        ConfigVariable = field;
                        break;
                    }
                }

                if (ConfigVariable == null)
                    throw new Exception(String.Format("Configurable without config ({0})", ConfigType));

                LoadConfig(false);
            });
        }

        private void LoadConfig(bool verbose = true)
        {
            if (verbose)
                Log.Info("Loading config for {0} ... ", ConfigType.Name);
            string configVar = GetType().Name + "Config";
            string configFile = configVar + ".json";
            
            object o = ConfigLoader.LoadConfigFile(configFile, ConfigVariable.FieldType);
            ConfigVariable.SetValue(ConfigObject, o);
            if (verbose)
                Log.Info("Done !\n");
        }
    }
}
