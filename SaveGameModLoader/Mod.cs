using HarmonyLib;
using KMod;
using System.IO;

namespace SaveGameModLoader
{
    public class Mod : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            ModAssets.ModPath = Directory.GetParent(Directory.GetParent(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)).FullName) + "\\[ModSync]StoredModConfigs\\";
            ModAssets.ModPacksPath = ModAssets.ModPath + "\\[StandAloneModLists]\\";
            System.IO.Directory.CreateDirectory(ModAssets.ModPath);
            System.IO.Directory.CreateDirectory(ModAssets.ModPacksPath);
            ModAssets.ModID = this.mod.label.id;
            //Debug.Log(ModAssets.ModID+"");
            Debug.Log("[Synchronize Mods]: Initialized file paths.");
            ModlistManager.Instance.GetAllStoredModlists();
            base.OnLoad(harmony);
        }
    }
}
