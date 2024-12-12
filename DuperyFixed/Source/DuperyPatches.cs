using Klei;
using KMod;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UtilLibs;

namespace Dupery
{
	internal class DuperyPatches
	{
		public static string ModStaticID;
		public static IReadOnlyList<Mod> Mods;

		public static CheekyLocalizer Localizer;
		public static Dictionary<string, CheekyLocalizer> ModLocalizers;

		public static AccessoryManager AccessoryManager;
		public static PersonalityManager PersonalityManager;

		public static string DirectoryName;



		public static void LoadResources()
		{
			Localizer = new CheekyLocalizer(IO_Utils.ModPath);
			ModLocalizers = new Dictionary<string, CheekyLocalizer>();

			AccessoryManager = new AccessoryManager();
			PersonalityManager = new PersonalityManager();

			Logger.Log("Searching for personalities and accessories provided by other mods.");

			foreach (Mod mod in Mods)
			{
				if (!mod.IsActive())
					continue;

				if (mod.staticID == ModStaticID)
					continue;

				if (mod.content_source == null)
					continue;

				string personalitiesFilePath = Path.Combine(mod.content_source.GetRoot(), PersonalityManager.PERSONALITIES_FILE_NAME);

				if (!File.Exists(personalitiesFilePath))
				{
					personalitiesFilePath = null;

					string[] files = System.IO.Directory.GetFiles(mod.content_source.GetRoot());
					Regex rx = new Regex(@"personalities\.json", RegexOptions.IgnoreCase);

					foreach (string file in files)
					{
						Match curr_rx = rx.Match(Path.GetFileName(file));
						if (curr_rx.Success)
						{
							personalitiesFilePath = file;
							break;
						}
					}
				}

				if (personalitiesFilePath != null && File.Exists(personalitiesFilePath))
				{
					Logger.Log($"Found {PersonalityManager.PERSONALITIES_FILE_NAME} file belonging to mod {mod.title}, attempting to import personalities and accessories...");

					ModLocalizers[mod.staticID] = new CheekyLocalizer(mod.content_source.GetRoot());

					List<string> animNames = GetAnimNames(mod);
					if (animNames != null && animNames.Count > 0)
					{
						Logger.Log($"Found anims belonging to mod {mod.title}, searching for accessories.");

						int totalImported = 0;
						foreach (string animName in animNames)
						{
							Logger.Log($"Checking {animName}...");
							totalImported += AccessoryManager.LoadAccessories(animName, true);
						}

						Logger.Log($"{totalImported} accessories imported successfully.");
					}

					PersonalityManager.TryImportPersonalities(personalitiesFilePath, mod);
				}
			}
		}

		private static List<string> GetAnimNames(Mod mod)
		{
			List<string> animNames = new List<string>();

			string path = FileSystem.Normalize(System.IO.Path.Combine(mod.ContentPath, "anim"));
			if (!System.IO.Directory.Exists(path))
				return null;

			foreach (DirectoryInfo directory1 in new DirectoryInfo(path).GetDirectories())
			{
				foreach (DirectoryInfo directory2 in directory1.GetDirectories())
				{
					string name = directory2.Name + "_kanim";
					foreach (KAnimFile kAnimFile in Assets.ModLoadedKAnims)
					{
						if (kAnimFile.name == name)
						{
							animNames.Add(name);
						}
					}
				}
			}

			return animNames;
		}
	}
}
