using HarmonyLib;
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
