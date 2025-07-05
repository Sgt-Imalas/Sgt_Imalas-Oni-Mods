//using HarmonyLib;
//using RonivansLegacy_ChemicalProcessing.Content.ModDb;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace RonivansLegacy_ChemicalProcessing.Patches
//{
//	class MinionStartingStats_Patches
//	{
//		[HarmonyPatch(typeof(MinionStartingStats), nameof(MinionStartingStats.GenerateAptitudes))]
//		public class MinionStartingStats_GenerateAptitudes_Patch
//		{
//			[HarmonyPriority(Priority.VeryHigh)]
//			public static void Prefix(MinionStartingStats __instance, ref string guaranteedAptitudeID)
//			{
//				ModPersonalities.OnAptitudeRoll(__instance, ref guaranteedAptitudeID);
//			}
//			//public static void Postfix(MinionStartingStats __instance)
//			//{
//			//	ModPersonalities.OnPostAptitudeRoll(__instance);
//			//}
//		}
//	}
//}
