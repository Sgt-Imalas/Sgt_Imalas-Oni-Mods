using HarmonyLib;

namespace ModOriginInfo.Patches
{
	internal class BuildingConfigManager_Patches
	{

        [HarmonyPatch(typeof(BuildingConfigManager), nameof(BuildingConfigManager.RegisterBuilding))]
        public class BuildingConfigManager_RegisterBuilding_Patch
        {
			static void Postfix(BuildingConfigManager __instance,IBuildingConfig config)
			{
				if(__instance.configTable.TryGetValue(config, out var def))
					ModAssets.RegisterBuildingDef(config, def);

			}
		}
	}
}
