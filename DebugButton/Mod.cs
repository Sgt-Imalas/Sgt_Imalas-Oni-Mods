using System.IO;
using HarmonyLib;
using KMod;
using UtilLibs;
using Directory = System.IO.Directory;

namespace DebugButton
{
    public class Mod : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            string baseModPath = Manager.GetDirectory();

            if (!string.IsNullOrEmpty(baseModPath) && this.mod != null)
            {
                string baseConfigPath = Path.Combine(baseModPath, "config");

                string modConfigDir = Path.Combine(baseConfigPath, this.mod.staticID);

                if (!Directory.Exists(modConfigDir))
                {
                    Directory.CreateDirectory(modConfigDir);
                }

                Config.ConfigPath = Path.Combine(modConfigDir, "config.json");
            }

            Config.Load();

            base.OnLoad(harmony);
            SgtLogger.LogVersion(this, harmony);
        }
    }
}