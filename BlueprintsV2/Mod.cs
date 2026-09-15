
using BlueprintsV2.BlueprintsV2.BlueprintData.PlanningToolMod_Integration;
using BlueprintsV2.ModAPI;
using HarmonyLib;
using KMod;
using ONI_Together_API.Networking;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using System.Collections.Generic;
using UtilLibs;
using static BlueprintsV2.ModAssets;

namespace BlueprintsV2
{
	public class Mod : UserMod2
	{
		public override void OnLoad(Harmony harmony)
		{
			SgtLogger.LogVersion(this, harmony);
			SgtLogger.l("Loading Mod Assets...");
			ModAssets.LoadAssets();
			PUtil.InitLibrary();
			new POptions().RegisterOptions(this, typeof(Config));
			base.OnLoad(harmony);
			ModAssets.RegisterActions();
			BlueprintFileHandling.AttachFileWatcher();
		}
		public override void OnAllModsLoaded(Harmony harmony, IReadOnlyList<KMod.Mod> mods)
		{
			base.OnAllModsLoaded(harmony, mods);
			API_Methods.RegisterExtraData();
			PlanningTool_Integration.Initialize();
			PacketRegistryAPI.AutoRegisterAll();
		}
	}
}
