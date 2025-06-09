using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _SgtsModUpdater
{
    public class Paths
    {
		public static string GameDocumentsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Klei","OxygenNotIncluded");
		public static string ModsFolder = Path.Combine(GameDocumentsFolder, "mods");
		public static string LocalModsFolder = Path.Combine(ModsFolder, "local");
    }
}
