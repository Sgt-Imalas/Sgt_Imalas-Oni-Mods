using HarmonyLib;
using UndigYourself.Content.Scripts;

namespace UndigYourself.Patches
{
	internal class Pickupable_Patches
	{

        [HarmonyPatch(typeof(Pickupable), nameof(Pickupable.OnPrefabInit))]
        public class Pickupable_OnPrefabInit_Patch
        {
            public static void Postfix(Pickupable __instance)
            {
                __instance.gameObject.AddOrGet<NeutroniumMole>();
            }
        }
	}
}
