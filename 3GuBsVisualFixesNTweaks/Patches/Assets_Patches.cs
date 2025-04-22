using HarmonyLib;
using Klei;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace _3GuBsVisualFixesNTweaks.Patches
{
	class Assets_Patches
	{
		[HarmonyPatch(typeof(Assets), "OnPrefabInit")]
		public class Assets_OnPrefabInit_Patch
		{
			public static void Prefix(Assets __instance)
			{
				var path = Path.Combine(UtilMethods.ModPath, "assets", "replacement_textures");

				SgtLogger.l(path, "PATH for imports");
				var directory = new DirectoryInfo(path);
				if (!directory.Exists)
				{
					SgtLogger.logError("Directory does not exist: " + path);
					return;
				}

				var files = directory.GetFiles();

				SgtLogger.l(files.Count().ToString(), "Files to import and override");
				foreach (var fileInfo in files)
				{
					if (fileInfo.Exists)
					{
						try
						{
							SgtLogger.l("importing texture: " + fileInfo.Name);
							AssetUtils.OverrideSpriteTextures(__instance, fileInfo);
						}
						catch (Exception e)
						{
							SgtLogger.logError("Failed at importing sprite: " + fileInfo.FullName + ",\nError: " + e);
						}
					}
				}

			}
		}
	}
}
