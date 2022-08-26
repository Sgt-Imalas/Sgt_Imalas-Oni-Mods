using HarmonyLib;
using KMod;
using System.IO;

namespace SaveGameModLoader
{
    public class Mod : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            ModAssets.ModPath = Directory.GetParent(Directory.GetParent(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)).FullName)  + "\\[ModSync]StoredModConfigs\\";
            System.IO.Directory.CreateDirectory(ModAssets.ModPath);
            Debug.Log("[ModLists per Savegame]: Initialized file paths.");
            ModlistManager.Instance.GetAllStoredModlists();
            base.OnLoad(harmony);
        }
    }
}
