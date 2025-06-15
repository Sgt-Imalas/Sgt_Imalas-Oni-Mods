using HarmonyLib;
using KMod;
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
			HarmonyInstance = harmony;
			base.OnLoad(harmony);
			SgtLogger.LogVersion(this, harmony);
			ConduitDisplayPortPatching.PatchAll(harmony);
			BuildingInjection.RegisterAdditionalBuildingElements();
			AdditionalRecipes.RegisterTags();
		}
	}
}
