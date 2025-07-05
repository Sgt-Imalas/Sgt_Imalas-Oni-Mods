using HarmonyLib;
using KMod;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using UtilLibs;
using UtilLibs.BuildingPortUtils;

namespace RonivansLegacy_ChemicalProcessing
{
	public class Mod : UserMod2
	{
		public static Harmony HarmonyInstance;
		public override void OnLoad(Harmony harmony)
		{
			//Patches.Reactor_Patches.PatchInnerMeltdownLoop.FindReactorMeltdownLoop();

			ModAssets.LoadAssets();
			PUtil.InitLibrary(false);
			new POptions().RegisterOptions(this, typeof(Config));

			BuildingDatabase.RegisterBuildings();
			HarmonyInstance = harmony;
			base.OnLoad(harmony);
			SgtLogger.LogVersion(this, harmony);
			ConduitDisplayPortPatching.PatchAll(harmony);
			BuildingDatabase.RegisterAdditionalBuildingElements();
			AdditionalRecipes.RegisterTags();
		}
	}
}
