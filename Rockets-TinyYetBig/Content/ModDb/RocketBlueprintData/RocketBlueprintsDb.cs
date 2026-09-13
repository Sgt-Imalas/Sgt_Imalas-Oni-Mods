using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UtilLibs;

namespace Rockets_TinyYetBig.Content.ModDb.RocketBlueprintData
{
	internal class RocketBlueprintsDb
	{
		static string RocketBPDir => Path.Combine(IO_Utils.ModConfigFolder, "RocketBlueprints");

		static List<RocketBlueprint> RocketBlueprints = new List<RocketBlueprint>();

		public static void InitDirectory()
		{
			var dir = Directory.CreateDirectory(RocketBPDir);
			int counter = 0;
			foreach (var item in dir.GetFiles())
			{
				if (IO_Utils.ReadFromFile<RocketBlueprint>(item, out var rocketBp))
				{
					AddNew(rocketBp);
					counter++;
				}
			}
			SgtLogger.l($"Loaded {counter} rocket templates");
		}

		internal static void AddNew(RocketBlueprint newBP, bool writeToFile = false)
		{
			RocketBlueprints.Add(newBP);
			if (writeToFile)
			{
				IO_Utils.WriteToFile<RocketBlueprint>(newBP, Path.Combine(RocketBPDir, SanitationUtils.SanitizeName(newBP.FriendlyName) + ".json"));
			}
		}
		public static List<RocketBlueprint> GetBlueprints() => RocketBlueprints;
	}
}
