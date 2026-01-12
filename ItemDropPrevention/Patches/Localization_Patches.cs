using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemDropPrevention.Patches
{
	internal class Localization_Patches
	{
		[HarmonyPatch(typeof(Localization), "Initialize")]
		public class Localization_Initialize_Patch
		{
			public static void Postfix()
			{
				UtilLibs.LocalisationUtil.Translate(typeof(STRINGS), true);
			}
		}
	}
}
