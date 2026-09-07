using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Content.ModDb.ModIntegrations
{
	/// <summary>
	/// Mod by Sanchozz that offers a better, multi-selection capable recipe picker
	/// </summary>
	internal class ChooseIngredient
	{
		private static bool _modEnabled = false;
		internal static void CheckIfEnabled(IReadOnlyList<KMod.Mod> mods)
		{
			_modEnabled = mods.Any(mod => mod.IsEnabledForActiveDlc() && mod.staticID == "ChooseIngredient");
		}

		public static bool ModActive => _modEnabled;
	}
}
