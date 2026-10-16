using HarmonyLib;
using Rockets_TinyYetBig.Elements;

namespace Rockets_TinyYetBig.Patches.ElementPatches
{
	internal class LegacyModMain_Patches
	{
		[HarmonyPatch(typeof(LegacyModMain), nameof(LegacyModMain.ConfigElements))]
		public static class Add_NeutroniumAlloy_Effects
		{
			public static void Postfix() => ModElements.RegisterElementEffects();
		}
	}
}
