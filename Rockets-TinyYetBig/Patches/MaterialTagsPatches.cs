using HarmonyLib;
using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;

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
                    if(element.oreTags!=null && element.oreTags.Contains(GameTags.CombustibleLiquid) )
                    {
                        element.oreTags.AddToArray(ModAssets.Tags.RocketFuelTag);
                    }
                }

                var hydrogen = ElementLoader.GetElement(SimHashes.Hydrogen.CreateTag());
                if (hydrogen.oreTags is null)
                {
                    hydrogen.oreTags = new Tag[] { };
                }
                hydrogen.oreTags = hydrogen.oreTags.AddToArray(ModAssets.Tags.RocketFuelTag);


                var uran = ElementLoader.GetElement(SimHashes.DepletedUranium.CreateTag());
                if (uran.oreTags is null)
                {
                    uran.oreTags = new Tag[] { };
                }
                uran.oreTags = uran.oreTags.AddToArray(ModAssets.Tags.RadiationShielding);

                var lead = ElementLoader.GetElement(SimHashes.Lead.CreateTag());
                if (lead.oreTags is null)
                {
                    lead.oreTags = new Tag[] { };
                }
                lead.oreTags = lead.oreTags.AddToArray(ModAssets.Tags.RadiationShielding);
            }
        }
    }
}
