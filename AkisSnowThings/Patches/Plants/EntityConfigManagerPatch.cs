using AkisSnowThings.Content.Defs.Entities;
using AkisSnowThings.Content.Defs.Plants;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                CROPS.CROP_TYPES.Add(new Crop.CropVal(PineTreeRemainsConfig.ID, PineTreeConfig.GROWTH_TIME, PineTreeConfig.HARVEST_MASS));
            }
        }
    }
}
