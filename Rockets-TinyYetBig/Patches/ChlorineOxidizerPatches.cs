using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Core.Tokens;
using static Rockets_TinyYetBig.Patches.AnimationFixes;

namespace Rockets_TinyYetBig.Patches
{
    internal class ChlorineOxidizerPatches
    {
        /// <summary>
        /// Calls the carbon field animation fix.
        /// </summary>
        [HarmonyPatch(typeof(Db))]
        [HarmonyPatch("Initialize")]
        public static class Db_Init_Patch
        {
            public static void Postfix()
            {
                var chlorineTag = SimHashes.Chlorine.CreateTag();
                if (!Clustercraft.dlc1OxidizerEfficiencies.ContainsKey(chlorineTag))
                {
                    Clustercraft.dlc1OxidizerEfficiencies.Add(chlorineTag, 3);
                }
            }
        }
        [HarmonyPatch(typeof(OxidizerTank),nameof(OxidizerTank.GetOxidizersAvailable))]
        public static class OxidizerAddition
        {
            public static void Postfix(OxidizerTank __instance, Dictionary<Tag, float> __result )
            {
                var chlorineTag = SimHashes.Chlorine.CreateTag();
                if (!__result.ContainsKey(chlorineTag))
                {
                    __result.Add(chlorineTag, __instance.storage.GetAmountAvailable(chlorineTag));
                }
            }
        }
        [HarmonyPatch(typeof(OxidizerTank), nameof(OxidizerTank.GetTotalOxidizerAvailable))]
        public static class OxidizerAddition2
        {
            public static void Postfix(OxidizerTank __instance,ref float __result)
            {
                var chlorineTag = SimHashes.Chlorine.CreateTag();
                    __result += __instance.storage.GetAmountAvailable(chlorineTag);
            }
        }
    }
}
