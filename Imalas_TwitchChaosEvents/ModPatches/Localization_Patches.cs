using HarmonyLib;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UtilLibs;

namespace Imalas_TwitchChaosEvents.ModPatches
{
    class Localization_Patches
    {
		[HarmonyPatch(typeof(Localization), "Initialize")]
		public static class Localization_Initialize_Patch
		{
			public static void Postfix()
			{
				LocalisationUtil.Translate(typeof(STRINGS), true);
				ModAssets.ModTraits.RegisterTraits();
			}
		}
	}
}
