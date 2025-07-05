using Klei;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Content.ModDb.BuildingConfigurations
{
    /// <summary>
    /// serializable
    /// </summary>
    class BuildingConfigurationCollection
	{
		static string ConfigFileLocation = FileSystem.Normalize(Path.Combine(KMod.Manager.GetDirectory(), "config", "RonivansAIO_BuildingConfig.json"));
        public Dictionary<string, BuildingConfigurationEntry> BuildingConfigurations = new ();

		internal static BuildingConfigurationCollection LoadFromFile()
		{
			var file = new FileInfo(ConfigFileLocation);
			if (file.Exists && IO_Utils.ReadFromFile<BuildingConfigurationCollection>(file, out var outlines, converterSettings: new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto }))
			{
				SgtLogger.l("loaded building config file with " + outlines.BuildingConfigurations.Count + " entries");
				return outlines;
			}
			else
			{
				return new();
			}
		}

		internal void WriteToFile()
		{
			IO_Utils.WriteToFile(this, ConfigFileLocation, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto });
		}
	}
}
