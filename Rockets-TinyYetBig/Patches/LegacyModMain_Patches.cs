using HarmonyLib;
using Klei.AI;
using Rockets_TinyYetBig.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
