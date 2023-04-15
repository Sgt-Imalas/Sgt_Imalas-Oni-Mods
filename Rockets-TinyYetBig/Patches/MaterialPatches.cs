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
    public class MaterialPatches
    {
        [HarmonyPatch(typeof(ElementLoader))]
        [HarmonyPatch(nameof(ElementLoader.Load))]
        public static class Patch_ElementLoader_Load
        {
            public static void Postfix()
            {
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
