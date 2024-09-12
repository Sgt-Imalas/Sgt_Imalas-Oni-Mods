using HarmonyLib;

namespace OniRetroEdition.ModPatches
{
	internal class LampPatch
	{
		[HarmonyPatch(typeof(LightController), nameof(LightController.InitializeStates))]
		private static class LoopLamp
		{
			public static void Postfix(LightController __instance)
			{
				__instance.on.PlayAnim("on", KAnim.PlayMode.Loop);
			}
		}
	}
}
