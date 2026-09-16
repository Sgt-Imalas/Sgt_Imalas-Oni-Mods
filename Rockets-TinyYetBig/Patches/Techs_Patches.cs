using HarmonyLib;
using Rockets_TinyYetBig.Content.ModDb;

namespace Rockets_TinyYetBig.Patches.ResearchPatches
{
	internal class Techs_Patches
	{

		/// <summary>
		/// Add research node to tree
		/// </summary>
		[HarmonyPatch(typeof(Database.Techs), "Init")]
		public class Techs_TargetMethod_Patch
		{
			public static void Postfix(Database.Techs __instance) => ModTechsDB.RegisterTechs(__instance);		
		}
	}
}
