using DupeModelAccessPermissions.Content.Scripts;
using HarmonyLib;

namespace DupeModelAccessPermissions.Patches
{
	internal class AccessControl_Patches
	{
		[HarmonyPatch(typeof(AccessControl), nameof(AccessControl.OnPrefabInit))]
		public class AccessControl_OnPrefabInit_Patch
		{
			public static void Postfix(AccessControl __instance)
			{
				__instance.gameObject.AddOrGet<AccessControl_Extension>();
			}
		}
	}
}
