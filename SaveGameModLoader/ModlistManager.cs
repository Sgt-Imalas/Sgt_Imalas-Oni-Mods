using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveGameModLoader
{
    public class ModlistManager
    {
        public List<SaveGameModList> Modlists = new();
        private static readonly Lazy<ModlistManager> _instance = new Lazy<ModlistManager>(() => new ModlistManager());

        public static ModlistManager Instance { get { return _instance.Value; } }
        
        public ModlistManager()
        {
            //GetAllStoredModlists();
        }
        public void GetAllStoredModlists()
        {
            Modlists.Clear();
            var files = Directory.GetFiles(ModAssets.ModPath);
            foreach(var modlist in files)
            {
                try
                {
                   var list = SaveGameModList.ReadModlistListFromFile(modlist);
                    Modlists.Add(list);
                }
                catch
                {
                    Debug.LogError("Couln't load modlist from: " + modlist);
                }
            }
            Debug.Log("Found Mod Configs for " + files.Count() + " Colonies");
        }

        public bool CreateOrAddToModLists(string savePath, string colonyGuid ,List<KMod.Label> list)
        {
            //var savePath = SaveGameModList.StripAllPaths(savePathPreStripped);
            bool Initialized=false;
            Debug.Log("GUID: " + colonyGuid.ToString());
            Debug.Log(savePath);

            var colonyModSave = Modlists.Find(entry => entry.ReferencedColonySaveName  == SaveGameModList.GetModListFileName(savePath));

            if (colonyModSave == null)
            {
                Initialized = true;
                colonyModSave = new SaveGameModList(savePath, colonyGuid);
                Modlists.Add(colonyModSave);
            }
            bool subEntryExisting = colonyModSave.AddOrUpdateEntryToModList(savePath, list);
            return Initialized || subEntryExisting;
        }
    }
}
