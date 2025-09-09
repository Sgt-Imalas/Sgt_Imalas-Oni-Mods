using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _SgtsModUpdater.Model.LocalMods
{
    public class LocalMod
    {
        public string FolderPath;
        public string Version => !string.IsNullOrWhiteSpace(DllVersion) ? DllVersion : ModInfoYaml.version;
        public ModInfoYaml ModInfoYaml { get; private set; }
		public ModYaml ModYaml { get; private set; }

        public string ModType { get; private set; }

        public string DllVersion = string.Empty;
        public bool Deletable => ModType.ToUpperInvariant() == "LOCAL"; //no steam mod deletion!

		public LocalMod(ModYaml modYaml, ModInfoYaml modInfoYaml, string path)
        {
            FolderPath = path;
			ModInfoYaml = modInfoYaml;
            ModYaml = modYaml;
            ModType = new DirectoryInfo(path).Parent.Name;
        }
    }
}
