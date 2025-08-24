using HarmonyLib;
using Klei.AI;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;
using static RonivansLegacy_ChemicalProcessing.Content.ModDb.ModElements;


namespace RonivansLegacy_ChemicalProcessing.Patches
{
    class LegacyModMain_Patches
    {

        [HarmonyPatch(typeof(LegacyModMain), nameof(LegacyModMain.ConfigElements))]
        public class LegacyModMain_ConfigElements_Patch
        {
            public static void Postfix(LegacyModMain __instance)
			{
				ModElements.ConfigureElements();
			}
        }


		/// <summary>
		/// patch here to have the food entities initialized
		/// </summary>
		[HarmonyPatch(typeof(LegacyModMain), nameof(LegacyModMain.LoadEntities))]
		public class LegacyModMain_LoadEntities_Patch
		{
			public static void Postfix(LegacyModMain __instance)
			{
				AdditionalRecipes.RegisterRecipes_PostLoadEntities();
			}
		}
    }
}
