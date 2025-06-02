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
				if (def.BuildingComplete.HasTag(ModAssets.Tags.SpaceStationOnlyInteriorBuilding) 
					&& SpaceStationManager.ActiveWorldIsSpaceStationInterior()
					&& def.BuildingComplete.HasTag(GameTags.NotRocketInteriorBuilding))
				{
					{
						__result = ReevaluateMaterialState(def);
					}
				}
				else if(def.BuildingComplete.HasTag(ModAssets.Tags.SpaceStationOnlyInteriorBuilding)
					&& !SpaceStationManager.ActiveWorldIsSpaceStationInterior()
					&& __result == PlanScreen.RequirementsState.RocketInteriorOnly)
				{
					__result = SpaceStationInteriorRequired;
				}

				else if (def.BuildingComplete.HasTag(ModAssets.Tags.RocketInteriorOnlyBuilding)
					&& SpaceStationManager.ActiveWorldIsSpaceStationInterior())
				{
					__result = SpaceStationInteriorForbidden;
				}
				else if (def.BuildingComplete.HasTag(ModAssets.Tags.SpaceStationOnlyInteriorBuilding)
					&& SpaceStationManager.ActiveWorldIsRocketInterior())
				{
					__result = PlanScreen.RequirementsState.RocketInteriorForbidden;
				}
				return;



				//bool isSpaceStationInterior = SpaceStationManager.ActiveWorldIsSpaceStationInterior();
				//bool isRocketInterior = SpaceStationManager.ActiveWorldIsRocketInterior();

				//bool hasRocketRequirement = def.BuildingComplete.HasTag(GameTags.RocketInteriorBuilding);
				//bool hasRocketForbiddenRequirement = def.BuildingComplete.HasTag(GameTags.NotRocketInteriorBuilding);

				//bool hasRocketOnlyRequirement = def.BuildingComplete.HasTag(ModAssets.Tags.RocketInteriorOnlyBuilding);
				//bool hasStationInteriorOnlyRequirement = def.BuildingComplete.HasTag(ModAssets.Tags.RocketInteriorOnlyBuilding);

				//if (hasStationInteriorOnlyRequirement && isRocketInterior)
				//	__result = SpaceStationInteriorRequired;
				//else if(!hasStationInteriorOnlyRequirement && isRocketInterior)
				//	__result = SpaceStationInteriorForbidden;
				//else if(hasStationInteriorOnlyRequirement && isSpaceStationInterior)
				//	__result = ReevaluateMaterialState(def);


				//return;



				if (def.BuildingComplete.HasTag(ModAssets.Tags.SpaceStationOnlyInteriorBuilding) && SpaceStationManager.ActiveWorldIsSpaceStationInterior())
				{
					if (def.BuildingComplete.HasTag(GameTags.NotRocketInteriorBuilding) && def.BuildingComplete.HasTag(ModAssets.Tags.SpaceStationOnlyInteriorBuilding))
					{
						if (!DebugHandler.InstantBuildMode && !Game.Instance.SandboxModeActive && !ProductInfoScreen.MaterialsMet(def.CraftRecipe))
							__result = PlanScreen.RequirementsState.Materials;
						else
							__result = PlanScreen.RequirementsState.Complete;
					}
				}
				if (def.BuildingComplete.HasTag(ModAssets.Tags.RocketInteriorOnlyBuilding) && !SpaceStationManager.ActiveWorldIsRocketInterior())
				{
					__result = SpaceStationInteriorForbidden;
				}
				else if (def.BuildingComplete.HasTag(ModAssets.Tags.SpaceStationOnlyInteriorBuilding) && !SpaceStationManager.ActiveWorldIsRocketInterior())
				{
					__result = SpaceStationInteriorRequired;
				}
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
    