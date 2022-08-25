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


        public bool DoesColonyExist(string colonyName)
        {
            GetAllStoredModlists();
            var result = Modlists.Find(list => list.ReferencedColonySaveName == colonyName) != null;
            Debug.Log("ModList found for this savegame");
            return result;
        }
        public bool DoesModlistExistForThisSave(string path)
        {
            GetAllStoredModlists();
            var result = Modlists.Find(list => list.SavePoints.Find(sp => sp.referencedSavePath == path) != null) != null;
            return result;
        }

        public void InstantiateModView(string colonyName, string referencedPath)
        {
            Debug.Log("Tried Loading Modlist, but the feature doesnt exist yet! Ö ");
            Debug.Log("Parameters are: Colony Name: "+colonyName + ": Savepath: "+ referencedPath);
        }

        public void GetAllStoredModlists()
        {
            Modlists.Clear();
            var files = Directory.GetFiles(ModAssets.ModPath);
            foreach(var modlist in files)
            {
                try
                {
                   //Debug.Log("Trying to load: " + modlist);
                   var list = SaveGameModList.ReadModlistListFromFile(modlist);
                    Modlists.Add(list);
                }
                catch(Exception e)
                {
                    Debug.LogError("Couln't load modlist from: " + modlist + ", Error: "+e);
                }
            }
            //Debug.Log("Found Mod Configs for " + files.Count() + " Colonies");
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
