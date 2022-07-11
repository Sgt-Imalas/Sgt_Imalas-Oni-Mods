using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilLibs.ModSettings
{
    public class ModSettingsManager<T> where T : IModSettingsInstance, new()
    {
        public T Settings { get; set; }
        public Action<T> OnRead;
        private FileSystemWatcher watcher;
        private string ConfigPath;

        public void InstantiateSettings()
        {
            ConfigPath = Path.Combine(UtilMethods.ModPath, "ModConfig.json");
            Settings = DeserializeJson();
        }

        public void ResetSettings()
        {
            Settings = new T();
            SerializeJson();
        }

        public T DeserializeJson()
        {
            T returnSettings;
            if (!File.Exists(ConfigPath))
            {
                returnSettings = new T(); 
            }
            using (StreamReader r = new StreamReader(ConfigPath))
            {
                string json = r.ReadToEnd();
                returnSettings = JsonConvert.DeserializeObject<T>(json);
            }
            return returnSettings;
        }

        public void SerializeJson()
        {
            var data = JsonConvert.SerializeObject(Settings);
            File.WriteAllText(ConfigPath, data);
        }
    }
}
