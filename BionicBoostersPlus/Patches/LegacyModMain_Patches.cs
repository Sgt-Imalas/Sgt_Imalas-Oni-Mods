using BionicBoostersPlus.Content.ModDb;
using HarmonyLib;

namespace BionicBoostersPlus.Patches
{
	internal class LegacyModMain_Patches
	{

		/// <summary>
		/// patch here to have the food entities initialized
		/// </summary>
		[HarmonyPatch(typeof(LegacyModMain), nameof(LegacyModMain.LoadEntities))]
		public class LegacyModMain_LoadEntities_Patch
		{
			public static void Postfix()
			{
				BB_Recipes.RegisterRecipes_PostLoadEntities();
			}
		}
	}
}
