using HarmonyLib;
using KMod;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using System.Collections.Generic;
using UtilLibs;
using static CrittersShedFurOnBrush.ModIntegrations;

namespace CrittersShedFurOnBrush
{
	public class Mod : UserMod2
	{
		public override void OnLoad(Harmony harmony)
		{
			PUtil.InitLibrary(false);
			new POptions().RegisterOptions(this, typeof(Config));
			ModAssets.InitSheddables();
			SgtLogger.LogVersion(this, harmony);
			base.OnLoad(harmony);
		}
		public override void OnAllModsLoaded(Harmony harmony, IReadOnlyList<KMod.Mod> mods)
		{
			base.OnAllModsLoaded(harmony, mods);
			CompatibilityNotifications.FixBrokenTimeout(harmony);
			UpgradableCritters_Integration.Init();

		}
	}
}
