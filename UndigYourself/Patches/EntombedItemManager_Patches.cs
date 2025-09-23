using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UndigYourself.Content.Scripts;

namespace UndigYourself.Patches
{
	internal class EntombedItemManager_Patches
	{

        [HarmonyPatch(typeof(EntombedItemManager), nameof(EntombedItemManager.OnDeserialized))]
        public class EntombedItemManager_OnDeserialized_Patch
        {
            public static void Postfix(EntombedItemManager __instance)
            {
                __instance.gameObject.AddOrGet<EntombedItemManagerNeutroniumChecker>();
            }
        }
	}
}
