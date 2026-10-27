using Klei;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UtilLibs;

namespace Rockets_TinyYetBig.Content.ModDb.BuildingConfigurations
{
    /// <summary>
    /// serializable
    /// </summary>
    class BuildingConfigurationCollection
	{
		static readonly string ConfigFileLocation = FileSystem.Normalize(Path.Combine(KMod.Manager.GetDirectory(), "config", "Rocketry Expanded", "RTB_BuildingConfig.json"));
		public Dictionary<string, BuildingConfigurationEntry> BuildingConfigurations = new ();

		internal static BuildingConfigurationCollection LoadFromFile()
		{
			var file = new FileInfo(ConfigFileLocation);

			SgtLogger.l("Loading Building Configuration from " + file.FullName);
			if (file.Exists && IO_Utils.ReadFromFile<BuildingConfigurationCollection>(file, out var outlines))
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
			IO_Utils.WriteToFile(this, ConfigFileLocation);
		}
	}
}
