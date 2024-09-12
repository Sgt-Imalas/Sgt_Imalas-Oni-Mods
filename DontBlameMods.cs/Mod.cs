using HarmonyLib;
using KMod;
using System.Collections.Generic;
using UtilLibs;
using UtilLibs.ModSyncing;

namespace DontBlameMods.cs
{
	public class Mod : UserMod2
	{
		public override void OnLoad(Harmony harmony)
		{
			base.OnLoad(harmony);
			SgtLogger.debuglog("Initialized");
			SgtLogger.LogVersion(this, harmony);
			ModSyncUtils.RegisterModAsSyncMod(this.mod);
		}
		public override void OnAllModsLoaded(Harmony harmony, IReadOnlyList<KMod.Mod> mods)
		{
			base.OnAllModsLoaded(harmony, mods);
		}
	}
}
