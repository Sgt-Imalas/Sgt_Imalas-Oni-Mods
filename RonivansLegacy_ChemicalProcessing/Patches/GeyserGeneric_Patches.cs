using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
    class GeyserGeneric_Patches
    {

        [HarmonyPatch(typeof(GeyserGenericConfig), nameof(GeyserGenericConfig.GenerateConfigs))]
        public class GeyserGenericConfig_GenerateConfigs_Patch
        {
            public static void Postfix(List<GeyserGenericConfig.GeyserPrefabParams> __result) => ModGeysers.RegisterGeysers(__result);            
        }


        [HarmonyPatch(typeof(GeyserGenericConfig), nameof(GeyserGenericConfig.CreatePrefabs))]
        public class GeyserGenericConfig_CreatePrefabs_Patch
        {
            public static void Postfix(List<GameObject> __result)
            {
				ModGeysers.MarkInvis(__result);

			}
        }
    }
}
