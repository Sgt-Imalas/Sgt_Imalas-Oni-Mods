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

				var hydrogen = ElementLoader.GetElement(SimHashes.LiquidHydrogen.CreateTag());
				if (hydrogen.oreTags is null)
				{
					hydrogen.oreTags = [];
				}
				hydrogen.oreTags = hydrogen.oreTags.Append(ModAssets.Tags.RocketFuelTag);


				foreach (var radShieldElemnt in RadShieldingElements)
				{
					var element = ElementLoader.GetElement(radShieldElemnt.CreateTag());
					if (element.oreTags is null)
					{
						element.oreTags = [];
					}
					element.oreTags = element.oreTags.Append(ModAssets.Tags.RadiationShieldingRocketConstructionMaterial);
				}

				var LiquidChlorine = ElementLoader.GetElement(SimHashes.Chlorine.CreateTag());
				if (LiquidChlorine.oreTags is null)
				{
					LiquidChlorine.oreTags = [];
				}
				LiquidChlorine.oreTags = LiquidChlorine.oreTags.Append(ModAssets.Tags.CorrosiveOxidizer);
				LiquidChlorine.oreTags = LiquidChlorine.oreTags.Append(ModAssets.Tags.OxidizerEfficiency_3);


				var liquidOxygen = ElementLoader.GetElement(SimHashes.LiquidOxygen.CreateTag());
				if (liquidOxygen.oreTags is null)
				{
					liquidOxygen.oreTags = [];
				}
				liquidOxygen.oreTags = liquidOxygen.oreTags.Append(ModAssets.Tags.LOXTankOxidizer);
				liquidOxygen.oreTags = liquidOxygen.oreTags.Append(ModAssets.Tags.OxidizerEfficiency_4);
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
