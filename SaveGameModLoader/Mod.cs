using HarmonyLib;
using KMod;
using System;

namespace SaveGameModLoader
{
    public class Mod : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            ModAssets.ModPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)+ "\\SaveGames\\";
            System.IO.Directory.CreateDirectory(ModAssets.ModPath);
            Debug.Log("[ModLists per Savegame]: Initialized file paths.");
            ModlistManager.Instance.GetAllStoredModlists();
            base.OnLoad(harmony);
        }
    }
}
