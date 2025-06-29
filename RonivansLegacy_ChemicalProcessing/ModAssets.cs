using Klei.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing
{
    internal class ModAssets
    {
        public class Tags
		{
			///group tag for metal sands
			public static Tag RandomSand = TagManager.Create("ChemicalProcessing_RandomSand");

            ///since biodiesel is now vanilla, retire the modded element, but allow both the modded and the vanilla element in recipes via this tag
			public static Tag Biodiesel_Composition = TagManager.Create("ChemicalProcessing_Biodiesel_Composition");
			///since phyto oil is now vanilla, retire the modded element, but allow both the modded and the vanilla element in recipes via this tag
			public static Tag BioOil_Composition = TagManager.Create("ChemicalProcessing_BioOil_Composition");

			///group tag for guidance units
			public static Tag MineralProcessing_GuidanceUnit = TagManager.Create("MineralProcessing_GuidanceUnit");

			///Prevents free material cheesing from drill by destroying the drillbit on cancellation of the recipe
			public static Tag RandomRecipeIngredient_DestroyOnCancel = TagManager.Create("RandomRecipeIngredient_DestroyOnCancel");
		}       
    }
}
