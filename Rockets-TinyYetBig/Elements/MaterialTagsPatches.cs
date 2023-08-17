using HarmonyLib;
using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UtilLibs;
using static STRINGS.ELEMENTS;

namespace Rockets_TinyYetBig.Elements
{
    public class MaterialTagsPatches
    {
        /// <summary>
        /// Add additionally needed mod tags to certain elements
        /// </summary>
        [HarmonyPatch(typeof(ElementLoader))]
        [HarmonyPatch(nameof(ElementLoader.Load))]
        public static class Patch_ElementLoader_Load
        {
            public static List<SimHashes> RadShieldingElements = new List<SimHashes>()
            {
                SimHashes.Lead,
                SimHashes.Gold,
                SimHashes.DepletedUranium,
                SimHashes.Tungsten,
                SimHashes.TempConductorSolid
            };


            [HarmonyPriority(Priority.Low)]
            public static void Postfix()
            {
                foreach (var element in ElementLoader.elements)
                {
                    if (element.HasTag(GameTags.CombustibleLiquid))
                    {
                        element.oreTags = element.oreTags.Append(ModAssets.Tags.RocketFuelTag);
                    }
                }

                var hydrogen = ElementLoader.GetElement(SimHashes.LiquidHydrogen.CreateTag());
                if (hydrogen.oreTags is null)
                {
                    hydrogen.oreTags = new Tag[] { };
                }
                hydrogen.oreTags = hydrogen.oreTags.Append(ModAssets.Tags.RocketFuelTag);


                foreach (var radShieldElemnt in RadShieldingElements)
                {
                    var element = ElementLoader.GetElement(radShieldElemnt.CreateTag());
                    if (element.oreTags is null)
                    {
                        element.oreTags = new Tag[] { };
                    }
                    element.oreTags = element.oreTags.Append(ModAssets.Tags.RadiationShieldingRocketConstructionMaterial);
                }

                var LiquidChlorine = ElementLoader.GetElement(SimHashes.Chlorine.CreateTag());
                if (LiquidChlorine.oreTags is null)
                {
                    LiquidChlorine.oreTags = new Tag[] { };
                }
                LiquidChlorine.oreTags = LiquidChlorine.oreTags.Append(ModAssets.Tags.CorrosiveOxidizer);
                LiquidChlorine.oreTags = LiquidChlorine.oreTags.Append(ModAssets.Tags.OxidizerEfficiency_3);


                var liquidOxygen = ElementLoader.GetElement(SimHashes.LiquidOxygen.CreateTag());
                if (liquidOxygen.oreTags is null)
                {
                    liquidOxygen.oreTags = new Tag[] { };
                }
                liquidOxygen.oreTags = liquidOxygen.oreTags.Append(ModAssets.Tags.LOXTankOxidizer);
                liquidOxygen.oreTags = liquidOxygen.oreTags.Append(ModAssets.Tags.OxidizerEfficiency_4);
            }
        }
    }
}
