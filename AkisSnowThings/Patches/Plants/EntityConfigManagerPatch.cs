using AkisSnowThings.Content.Defs.Entities;
using AkisSnowThings.Content.Defs.Plants;
using HarmonyLib;
using TUNING;

namespace AkisSnowThings.Patches.Plants
{
    public class EntityConfigManagerPatch
    {
        [HarmonyPatch(typeof(EntityConfigManager), nameof(EntityConfigManager.LoadGeneratedEntities))]
        public class EntityConfigManager_LoadGeneratedEntities_Patch
        {
            public static void Prefix()
            {
                CROPS.CROP_TYPES.Add(new Crop.CropVal(TreeRemainsConfig.ID, EvergreenTreeConfig.GROWTH_TIME, EvergreenTreeConfig.HARVEST_MASS));
            }
        }
    }
}
