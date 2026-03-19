using HarmonyLib;
using Rockets_TinyYetBig.SpaceStations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rockets_TinyYetBig.Patches
{
    class PlanScreen_Patches
    {
		public static PlanScreen.RequirementsState SpaceStationInteriorRequired = (PlanScreen.RequirementsState)nameof(SpaceStationInteriorRequired).GetHashCode();
		public static PlanScreen.RequirementsState SpaceStationInteriorForbidden = (PlanScreen.RequirementsState)nameof(SpaceStationInteriorForbidden).GetHashCode();

		[HarmonyPatch(typeof(PlanScreen), nameof(PlanScreen.GetTooltipForRequirementsState))]
        public class PlanScreen_GetTooltipForRequirementsState_Patch
        {
            public static void Postfix(PlanScreen __instance, BuildingDef def, PlanScreen.RequirementsState state, ref string __result)
            {
				if (state == SpaceStationInteriorForbidden)
					__result = STRINGS.UI.PRODUCTINFO_SPACE_STATION_NOT_INTERIOR;
				else if (state == SpaceStationInteriorRequired)
					__result = STRINGS.UI.PRODUCTINFO_SPACE_STATION_INTERIOR;
			}
        }

		/// <summary>
		/// Allows for "Space Station Only Buildings"
		/// </summary>
		[HarmonyPatch(typeof(PlanScreen), nameof(PlanScreen.GetBuildableStateForDef))]
		public static class AllowCertainBuildingsInSpaceStations
		{
			/// <summary>
			/// GameTags.RocketInteriorBuilding means rocket and space station
			/// GameTags.NonRocketInteriorBuilding means none
			/// 
			/// </summary>
			public static void Postfix(PlanScreen __instance,BuildingDef def, ref PlanScreen.RequirementsState __result)
			{
				bool worldIsRocket = ClusterUtil.ActiveWorldIsRocketInterior();
				bool worldIsSpaceStation = SpaceStationManager.ActiveWorldIsSpaceStationInterior();

				var building = def.BuildingComplete;

				bool requiresInRocket = building.HasTag(GameTags.RocketInteriorBuilding);

				bool requiresInSpaceStation = building.HasTag(ModAssets.Tags.SpaceStationOnlyInteriorBuilding);
				bool probibitsInSpaceStation = building.HasTag(ModAssets.Tags.RocketInteriorOnlyBuilding);

				if (requiresInRocket && worldIsSpaceStation && !probibitsInSpaceStation)
					__result = ReevaluateMaterialState(def);
				else if (worldIsSpaceStation && probibitsInSpaceStation)
					__result = SpaceStationInteriorForbidden;
				else if(requiresInSpaceStation && !worldIsSpaceStation)
					__result = SpaceStationInteriorRequired;
			}

			static PlanScreen.RequirementsState ReevaluateMaterialState(BuildingDef def)
			{
				if (!DebugHandler.InstantBuildMode && !Game.Instance.SandboxModeActive && !ProductInfoScreen.MaterialsMet(def.CraftRecipe))
					return PlanScreen.RequirementsState.Materials;
				else
					return PlanScreen.RequirementsState.Complete;
			}
		}
	}
}
    