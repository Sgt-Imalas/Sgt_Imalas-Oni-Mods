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

		static List<RocketBlueprint> RocketBlueprints = null;

		public static void InitDirectory()
		{
			var dir = Directory.CreateDirectory(RocketBPDir);
			int counter = 0;
			RocketBlueprints = new List<RocketBlueprint>();
			foreach (var item in dir.GetFiles())
			{
				if (IO_Utils.ReadFromFile<RocketBlueprint>(item, out var rocketBp))
				{
					rocketBp.RefreshValidity();
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
				IO_Utils.WriteToFile<RocketBlueprint>(newBP, GetBlueprintPath(newBP));
			}
		}
		static string GetBlueprintPath(RocketBlueprint bp) => Path.Combine(RocketBPDir, SanitationUtils.SanitizeName(bp.FriendlyName) + ".json");
		public static List<RocketBlueprint> GetBlueprints()
		{
			if (RocketBlueprints == null)
				InitDirectory();
			return RocketBlueprints;
		}

		internal static void DeleteBlueprint(RocketBlueprint bp)
		{
			var bpPath = GetBlueprintPath(bp);
			RocketBlueprints.Remove(bp);
			if (File.Exists(bpPath))
			{
				try
				{
					File.Delete(bpPath);
				}
				catch (Exception e)
				{
					SgtLogger.error("Could not delete blueprint " + bp + ", error:\n" + e.Message);
				}
			}
		}
	}
}
