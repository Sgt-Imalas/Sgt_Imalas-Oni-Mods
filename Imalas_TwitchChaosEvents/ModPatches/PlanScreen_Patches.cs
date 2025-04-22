using Database;
using HarmonyLib;
using Imalas_TwitchChaosEvents.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace Imalas_TwitchChaosEvents.ModPatches
{
    class PlanScreen_Patches
    {
		[HarmonyPatch(typeof(PlanBuildingToggle), "StandardDisplayFilter")]
		public class PlanBuildingToggle_StandardDisplayFilter_Patch
		{
			public static void Prefix(ref bool ___researchComplete, BuildingDef ___def)
			{
				if (___def.PrefabID == InvertedElectrolyzerConfig.ID)
				{
					bool techUnlocked = Db.Get().Techs?.Get(GameStrings.Technology.Liquids.AirSystems).IsComplete() ?? false;
					___researchComplete = ChaosTwitch_SaveGameStorage.Instance.InvertedWaterGotSpawned && techUnlocked;
				}
				if (___def.PrefabID == InvertedWaterPurifierConfig.ID)
				{
					bool techUnlocked = Db.Get().Techs?.Get(GameStrings.Technology.Liquids.Distillation).IsComplete() ?? false;
					___researchComplete = ChaosTwitch_SaveGameStorage.Instance.InvertedWaterGotSpawned && techUnlocked;
				}
			}
		}

		//[HarmonyPatch(typeof(PlanScreen), nameof(PlanScreen.GetTooltipForRequirementsState))]
		//public class PlanScreen_GetTooltipForRequirementsState_Patch
		//{
		//	public static void Postfix(BuildingDef def, PlanScreen.RequirementsState state, ref string __result)
		//	{
		//		if (def.PrefabID == GravitasBigStorageConfig.ID && state == GravitasBigStorageUnlockManager.needsAnalysis)
		//		{
		//			__result = string.Format(global::STRINGS.UI.PRODUCTINFO_REQUIRESRESEARCHDESC, global::STRINGS.BUILDINGS.PREFABS.LONELYMINIONHOUSE_COMPLETE.NAME);
		//		}
		//	}
		//}

		[HarmonyPatch(typeof(PlanBuildingToggle), "CheckResearch")]
		public class PlanBuildingToggle_CheckResearch_Patch
		{
			public static void Postfix(BuildingDef ___def, ref bool ___researchComplete)
			{
				if (___def.PrefabID == InvertedElectrolyzerConfig.ID)
				{
					bool techUnlocked = Db.Get().Techs?.Get(GameStrings.Technology.Liquids.AirSystems).IsComplete() ?? false;
					___researchComplete = ChaosTwitch_SaveGameStorage.Instance.InvertedWaterGotSpawned && techUnlocked;
				}
				if (___def.PrefabID == InvertedWaterPurifierConfig.ID)
				{
					bool techUnlocked = Db.Get().Techs?.Get(GameStrings.Technology.Liquids.Distillation).IsComplete() ?? false;
					___researchComplete = ChaosTwitch_SaveGameStorage.Instance.InvertedWaterGotSpawned && techUnlocked;
				}
			}
		}
	}
}
