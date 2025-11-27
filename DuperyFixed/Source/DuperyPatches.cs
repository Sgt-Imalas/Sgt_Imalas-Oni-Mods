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

			SgtLogger.l("Loading old hair swap kanim accessories");

			///these have broken batch files; load from older file without broken states
			Db.Get().AccessorySlots.Hair.accessories.RemoveAll(a => a.Id == "hair_033");
			Db.Get().AccessorySlots.HatHair.accessories.RemoveAll(a => a.Id == "hat_hair_033");
			///load old hair file for now removed hairs
			AccessoryManager.LoadAccessories("old_hair_swap_kanim", true, false);
			//AccessoryManager.LoadAccessories("old_head_swap_kanim", true, false);

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
						Logger.Log($"Found {animNames.Count} anims belonging to mod {mod.title}, searching for accessories.");

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
			SgtLogger.l("getting anims from mod: " + mod.label.title);
			string animDirectory = FileSystem.Normalize(System.IO.Path.Combine(mod.ContentPath, "anim"));

			string dreamIconDicrectory = FileSystem.Normalize(System.IO.Path.Combine(mod.ContentPath, "dreamicons"));
			if (System.IO.Directory.Exists(dreamIconDicrectory))
			{
				foreach(var file in System.IO.Directory.GetFiles(dreamIconDicrectory))
				{
					var fileInfo = new FileInfo(file);
					if(fileInfo.Extension == ".png")
					{
						SgtLogger.l("Adding custom dream icon to assets: " + fileInfo.Name);
						AssetUtils.AddSpriteToAssets(fileInfo);
					}
				}
			}
			else
			{
				SgtLogger.l("no dream icon directory found under: " + dreamIconDicrectory);
			}


			if (!System.IO.Directory.Exists(animDirectory))
			{
				SgtLogger.l("no anim directory found under: "+ animDirectory);
                return null;
            }

			foreach (DirectoryInfo directory1 in new DirectoryInfo(animDirectory).GetDirectories())
			{
				foreach (DirectoryInfo directory2 in directory1.GetDirectories())
				{
					string name = directory2.Name + "_kanim";
					SgtLogger.l("Kanim in mod: " + name);
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
