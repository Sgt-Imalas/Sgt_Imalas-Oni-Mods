using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;


namespace RonivansLegacy_ChemicalProcessing.Patches
{
    class LegacyModMain_Patches
    {

        [HarmonyPatch(typeof(LegacyModMain), nameof(LegacyModMain.ConfigElements))]
        public class LegacyModMain_ConfigElements_Patch
        {
            public static void Postfix()
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
			public static void Postfix()
			{
				AdditionalRecipes.RegisterRecipes_PostLoadEntities();
			}
		}
    }
}
