
using HarmonyLib;
using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static AmogusMorb.ModAssets;
using static KAnim;

namespace AmogusMorb
{
    internal class Patches
    {
        [HarmonyPatch(typeof(GlomConfig))]
        [HarmonyPatch(nameof(GlomConfig.CreatePrefab))]
        public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {

            public static void Postfix(GameObject __result)
            {
                KBatchedAnimController kBatchedAnimController = __result.AddOrGet<KBatchedAnimController>();
                kBatchedAnimController.AnimFiles = new KAnimFile[1] { Assets.GetAnim("amorbus_sus_kanim") };
            }
        }
        /// <summary>
        /// Init. auto translation
        /// </summary>
        [HarmonyPatch(typeof(Localization), "Initialize")]
        public static class Localization_Initialize_Patch
        {
            public static void Prefix()
            {
                CREATURES.FAMILY.GLOM = (LocString)UI.FormatAsLink("Amorbus", "GLOMSPECIES");
                CREATURES.FAMILY_PLURAL.GLOMSPECIES = (LocString)UI.FormatAsLink("Amorbi", nameof(CREATURES.FAMILY_PLURAL.GLOMSPECIES));
                CREATURES.SPECIES.GLOM.NAME = (LocString)UI.FormatAsLink("Amorbus", nameof(CREATURES.SPECIES.GLOM));
                CREATURES.SPECIES.GLOM.DESC = "When the Imposter is Sus?!?\n\n" + CREATURES.SPECIES.GLOM.DESC;
            }
        }
    }
}
