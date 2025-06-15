using Database;
using Dupes_Industrial_Overhaul.Chemical_Processing.Space;
using HarmonyLib;
using Klei.AI;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
    class GameplayEvents_Patches
    {

        [HarmonyPatch(typeof(GameplayEvents), MethodType.Constructor, [typeof(ResourceSet)])]
        public class GameplayEvents_Constructor_Patch
		{
            public static void Postfix(GameplayEvents __instance)
            {
                MeteorShowerAdjustments.AddMeteorOreComet(__instance);
			}
        }
    }
}
