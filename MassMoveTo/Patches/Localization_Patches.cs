using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UtilLibs;

namespace MassMoveTo.Patches
{
	internal class Localization_Patches
	{
		/// <summary>
		/// Init. auto translation
		/// </summary>yPatch(typeof(Localization), nameof(Localization.Initialize))]
        public class Localization_Initialize_Patch
		{
			public static void Postfix()
			{
				LocalisationUtil.Translate(typeof(STRINGS), true);
			}
		}
	}
}
