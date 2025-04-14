using _3GuBsVisualFixesNTweaks.Scripts;
using HarmonyLib;
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
		public static class ModTags
		{
			//completely disable wall visualizers for the building (use if those are fully included in the building anim)
			public static Tag PlacementVisualizerExcluded = TagManager.Create("VFNT_PlacementVisualizerExcluded");
			//disable wall visualizers for the building, but only for vertical walls
			public static Tag PlacementVisualizerExcludedVertical = TagManager.Create("VFNT_PlacementVisualizerExcludedVertical");
			//disable wall visualizers for the building, but only for horizontal walls
			public static Tag PlacementVisualizerExcludedHorizontal = TagManager.Create("VFNT_PlacementVisualizerExcludedHorizontal");
		}

		static Dictionary<SimHashes, Color> CachedColors = new Dictionary<SimHashes, Color>();

		static Dictionary<GameObject, KBatchedAnimController> CachedKBACs = new();
		static Dictionary<GameObject, KBatchedAnimController> CachedFGKBACs = new();

		public static Color GetElementColor(SimHashes simhash)
		{
			if (!CachedColors.TryGetValue(simhash, out var color))
			{
				var element = ElementLoader.GetElement(simhash.CreateTag());
				color = element.substance.conduitColour;
				color.a = 1f;
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

		public static void TryApplyConduitTint(ConduitType type, int conduitCell, KBatchedAnimController kbac, KBatchedAnimController kbac2, bool doForceElementColor = false, Color ForceElementColor = default, bool cleanupPrev = false)
		{
			if (doForceElementColor)
			{
				kbac.SetSymbolTint("tint", ForceElementColor);
				kbac2?.SetSymbolTint("tint_fg", ForceElementColor);
				return;
			}


			ConduitFlow flowManager = Conduit.GetFlowManager(type);
			ConduitFlow.Conduit conduit = flowManager.GetConduit(conduitCell);
			if (!flowManager.HasConduit(conduitCell))
			{
				if (cleanupPrev)
				{
					kbac.SetSymbolTint("tint", Color.clear);
					kbac2?.SetSymbolTint("tint", Color.clear);
				}
				return;
			}
			ConduitFlow.ConduitContents contents = conduit.GetContents(flowManager);

			if (contents.mass > 0f)
			{
				kbac.SetSymbolTint("tint", ModAssets.GetElementColor(contents.element));
				kbac2?.SetSymbolTint("tint_fg", ModAssets.GetElementColor(contents.element));
			}
			else
			{

				if (cleanupPrev)
				{
					kbac.SetSymbolTint("tint", Color.clear);
					kbac2?.SetSymbolTint("tint", Color.clear);
				}
			}
		}

		public static bool TryGetCachedKbacs(GameObject key, out KBatchedAnimController kbac, out KBatchedAnimController fg)
		{

			kbac = null;
			fg = null;

			if (key == null)
				return false;

			if (!CachedKBACs.TryGetValue(key, out kbac))
			{
				if (key.TryGetComponent<KBatchedAnimController>(out kbac))
				{
					CachedKBACs.Add(key, kbac);
					if (kbac.layering?.foregroundController is KBatchedAnimController kbac2)
					{
						CachedFGKBACs.Add(key, kbac2);
						fg = kbac2;
					}
					else
						SgtLogger.l("no fg kbac found for " + key.GetProperName());
				}
			}
			if (kbac == null)
				return false;

			CachedFGKBACs.TryGetValue(key, out fg);
			return true;

		}

		public static ContentTintable AddGeneratorTint(GameObject go)
		{
			var gen = go.GetComponent<EnergyGenerator>();
			var generatorConsumable = gen.formula.inputs?.FirstOrDefault();
			Tag tintLimiter = null;
			if (generatorConsumable != null && generatorConsumable.HasValue)
			{
				tintLimiter = generatorConsumable.Value.tag;
			}
			SgtLogger.l("Generator Tint tag: " + tintLimiter,go.GetProperName());
			var tint = go.AddOrGet<ContentTintable>();
			tint.TintTag = tintLimiter;
			tint.TintGeneratorMeter = true;
			return tint;
		}
		public static ContentTintable AddElementConverterTint(GameObject go)
		{
			var gen = go.GetComponent<ElementConverter>();
			var inputConsumable = gen.consumedElements?.FirstOrDefault();
			Tag tintLimiter = null;
			if (inputConsumable.HasValue)
			{
				tintLimiter = inputConsumable.Value.Tag;
			}
			SgtLogger.l("ElementConverter Tint tag: " + tintLimiter, go.GetProperName());
			var tint = go.AddOrGet<ContentTintable>();
			tint.TintTag = tintLimiter;
			return tint;
		}
	}
}
