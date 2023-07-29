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

namespace Rockets_TinyYetBig.Patches
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
            [HarmonyPriority(Priority.Low)]
            public static void Postfix()
            {
                foreach(var element in ElementLoader.elements)
                {
                    if(element.HasTag(GameTags.CombustibleLiquid) )
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

                var uran = ElementLoader.GetElement(SimHashes.DepletedUranium.CreateTag());
                if (uran.oreTags is null)
                {
                    uran.oreTags = new Tag[] { };
                }
                uran.oreTags = uran.oreTags.Append(ModAssets.Tags.RadiationShielding);

                var lead = ElementLoader.GetElement(SimHashes.Lead.CreateTag());
                if (lead.oreTags is null)
                {
                    lead.oreTags = new Tag[] { };
                }
                lead.oreTags = lead.oreTags.Append(ModAssets.Tags.RadiationShielding);



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
