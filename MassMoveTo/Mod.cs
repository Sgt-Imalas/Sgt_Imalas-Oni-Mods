using HarmonyLib;
using KMod;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using System.Collections.Generic;
using UtilLibs;

namespace MassMoveTo
{
	public class Mod : UserMod2
	{
		public override void OnLoad(Harmony harmony)
		{
			PUtil.InitLibrary(false);
			new POptions().RegisterOptions(this, typeof(Config));
			//base.OnLoad(harmony);
			SgtLogger.LogVersion(this, harmony);
			ModAssets.RegisterActions();
		}

		public override void OnAllModsLoaded(Harmony harmony, IReadOnlyList<KMod.Mod> mods)
		{
			base.OnAllModsLoaded(harmony, mods);
			ModIntegration_ChainTool.TryInit(harmony);
			if (ModIntegration_ChainTool.ChainToolActive)
				SgtLogger.warning("Chain Tool detected, disabling multi delivery for mass move tool as that mod breaks it");
			base.OnLoad(harmony);
		}
	}
}
