using AquaticMinnowMinion.Content.ModDb;
using HarmonyLib;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace AquaticMinnowMinion.Patches.MoisturizingWorkablePatches
{
	internal class Watercooler_Patches
	{

        [HarmonyPatch(typeof(WaterCooler.StatesInstance), nameof(WaterCooler.StatesInstance.Drink))]
        public class WaterCooler_StatesInstance_Drink_Patch
        {
            public static void Postfix(GameObject __0) => ModAssets.TriggerDrinkMoisturization(__0);
        }
	}
}
