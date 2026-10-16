using System.IO;
using Newtonsoft.Json;

namespace DebugButton
{
    public class Config
    {
        public bool DisableWhenSandboxOff { get; set; } = false;

        public static Config Instance { get; private set; } = new Config();

        // 最初は空にしておき、Mod読み込み時にゲーム側からパスをもらって上書きする
        public static string ConfigPath { get; set; }

        public static void Load()
        {
            try
            {
                // パスが正常に設定されており、かつファイルが存在する場合のみ読み込む
                if (!string.IsNullOrEmpty(ConfigPath) && File.Exists(ConfigPath))
                {
                    string json = File.ReadAllText(ConfigPath);
                    Instance = JsonConvert.DeserializeObject<Config>(json) ?? new Config();
                }
                else
                {
                    Save();
                }
            }
            catch
            {
                Instance = new Config();
            }
        }

        public static void Save()
        {
            try
            {
                if (!string.IsNullOrEmpty(ConfigPath))
                {
                    string json = JsonConvert.SerializeObject(Instance, Formatting.Indented);
                    File.WriteAllText(ConfigPath, json);
                }
            }
            catch
            {
            }
        }
    }
}