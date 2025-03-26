using Klei.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static STRINGS.UI.ELEMENTAL;

namespace _3GuBsVisualFixesNTweaks
{
    internal class ModAssets
	{
		static Dictionary<SimHashes, Color> CachedColors = new Dictionary<SimHashes, Color>();

		public static Color GetElementColor(SimHashes simhash)
		{
			if (!CachedColors.TryGetValue(simhash, out var color))
			{
				var element = ElementLoader.GetElement(simhash.CreateTag());
				color = element.substance.conduitColour;
				CachedColors[simhash] = color;
			}
			return color;

		}
		public static ModHashes OnRefineryAnimPlayed = new("VFNT_OnRefineryAnimPlayed");
		public static void RefreshOutputTracker(SymbolOverrideController soc, GameObject item)
		{
			soc.TryRemoveSymbolOverride("output_tracker");
			if (item == null)
				return;
			var build = item.GetComponent<KBatchedAnimController>().AnimFiles[0].GetData().build;
			KAnim.Build.Symbol symbol = build.GetSymbol((KAnimHashedString)build.name);
			if (symbol == null && build.symbols.Any()) //klei forgot to name symbols properly, defaulting to the first symbol
			{
				symbol = build.symbols[0];
			}
			if (symbol == null)
			{
				Debug.LogWarning((build.name + " is missing symbol " + build.name));
			}
			else
				soc.AddSymbolOverride("output_tracker", symbol);
		}
	}
}
