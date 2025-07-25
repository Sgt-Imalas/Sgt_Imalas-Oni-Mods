using ElementUtilNamespace;
using HarmonyLib;
using Rockets_TinyYetBig.Elements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rockets_TinyYetBig.Patches.ElementPatches
{
	internal class ElementLoader_Patches
	{
		[HarmonyPatch(typeof(ElementLoader), nameof(ElementLoader.Load))]
		public class ElementLoader_Load_Patch
		{
			/// <summary>
			/// Adds the new mod elements to the substance list
			/// </summary>
			public static void Prefix(Dictionary<string, SubstanceTable> substanceTablesByDlc, ref Hashtable substanceList)
			{
				// Add my new elements
				var list = substanceTablesByDlc[DlcManager.VANILLA_ID].GetList();
				ModElements.RegisterSubstances(list);
			}

			/// <summary>
			/// Add additionally needed mod tags to certain elements
			/// </summary>
			public static List<SimHashes> RadShieldingElements = new List<SimHashes>()
			{
				SimHashes.Lead,
				SimHashes.Gold,
				SimHashes.DepletedUranium,
				SimHashes.Tungsten,
				SimHashes.TempConductorSolid
			};

			[HarmonyPriority(Priority.Low)]
			public static void Postfix()
			{
				foreach (var element in ElementLoader.elements)
				{
					if (element.HasTag(GameTags.CombustibleLiquid))
					{
						element.oreTags = element.oreTags.Append(ModAssets.Tags.RocketFuelTag);
					}
				}
				AddTagToElement(SimHashes.LiquidHydrogen, ModAssets.Tags.RocketFuelTag);

				foreach (var radShieldElemnt in RadShieldingElements)
				{
					AddTagToElement(radShieldElemnt, ModAssets.Tags.RadiationShieldingRocketConstructionMaterial);
				}

				AddTagsToElement(SimHashes.Chlorine, [ModAssets.Tags.CorrosiveOxidizer, ModAssets.Tags.OxidizerEfficiency_3]);
				AddTagsToElement(SimHashes.LiquidOxygen, [ModAssets.Tags.LOXTankOxidizer, ModAssets.Tags.OxidizerEfficiency_4]);

				//not entirely required, since those entries come included in the vanilla game, but if there is a custom solid oxidizer, it should be added like this:
				AddTagsToElement(SimHashes.Fertilizer, [ModAssets.Tags.RocketSolidOxidizerTag, ModAssets.Tags.OxidizerEfficiency_1]);
				AddTagsToElement(SimHashes.OxyRock, [ModAssets.Tags.RocketSolidOxidizerTag, ModAssets.Tags.OxidizerEfficiency_2]);
			}
			static void AddTagToElement(SimHashes element, Tag tag) => AddTagsToElement(element, [tag]);
			static void AddTagsToElement(SimHashes element, Tag[] tags)
			{
				var elem = ElementLoader.GetElement(element.CreateTag());
				if(elem == null)
				{
					Debug.LogError($"Element {element} not found in ElementLoader.");
					return;
				}

				if (elem.oreTags is null)
				{
					elem.oreTags = [];
				}
				foreach (var tag in tags)
				{
					if (!elem.oreTags.Contains(tag))
					{
						elem.oreTags = elem.oreTags.Append(tag);
					}
				}
			}
		}

		/// <summary>
		/// make neutronium fully block radiation since it is used in the plated nosecone walls
		/// </summary>
		[HarmonyPatch(typeof(ElementLoader), nameof(ElementLoader.CollectElementsFromYAML))]
		public class MakeNeutroniumFullyRadBlocking_Patch
		{
			public static void Postfix(ref List<ElementLoader.ElementEntry> __result)
			{
				foreach (var elem in __result)
				{
					if (elem.elementId == SimHashes.Unobtanium.ToString())
					{
						elem.radiationAbsorptionFactor = 1.26f;
					}
				}
			}
		}
	}
}
