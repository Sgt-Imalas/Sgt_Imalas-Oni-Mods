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
        private static ModlistManager _instance;
        public static ModlistManager Instance
        {
            get
            {
                return _instance == null ? new ModlistManager() : _instance;
            }
        }
        
        public ModlistManager()
        {
            GetAllStoredModlists();
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
        }

        public Get
        public void AddNewModlist(SaveGameModList list)
        {
            Modlists.Add(list);
            list.WriteModlistToFile();
        }
    }
}
