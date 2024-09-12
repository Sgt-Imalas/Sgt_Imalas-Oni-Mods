using HarmonyLib;

namespace ReactorMeterFix
{
	internal class Patches
	{
		/// <summary>
		/// add buildings to plan screen
		/// </summary>
		[HarmonyPatch(typeof(Reactor))]
		[HarmonyPatch(nameof(Reactor.OnSpawn))]
		public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
		{

			public static void Prefix()
			{
				Reactor.meterFrameScaleHack = 1;
			}
		}
	}
}
