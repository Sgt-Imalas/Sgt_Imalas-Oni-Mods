using HarmonyLib;
using KMod;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using System.Collections.Generic;
using UtilLibs;

namespace PaintYourPipes
{
	public class Mod : UserMod2
	{
		public static Harmony Harmony;
		public override void OnLoad(Harmony harmony)
		{
			Harmony = harmony;
			PUtil.InitLibrary(false);
			new POptions().RegisterOptions(this, typeof(Config));
			base.OnLoad(harmony);
			SgtLogger.LogVersion(this, harmony);
			ModAssets.HotKeys.Register();
		}
		public override void OnAllModsLoaded(Harmony harmony, IReadOnlyList<KMod.Mod> mods)
		{
			base.OnAllModsLoaded(harmony, mods);
			Patches.AddColorComponentToFinishedBuildings.ExecutePatch(harmony);
			CompatibilityPatches.Reverse_Bridges_Compatibility.ExecutePatch(harmony);
			if (!Config.Instance.OverlayOnly)
			{
				CompatibilityPatches.Material_Colored_Tiles_Compatibility.ExecutePatch(harmony);
				//CompatibilityPatches.MaterialColour_Compatibility.ExecutePatch(harmony);
			}
		}
	}
}
