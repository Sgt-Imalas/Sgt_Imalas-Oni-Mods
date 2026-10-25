using HarmonyLib;
using Rockets_TinyYetBig.Content.ModDb;
using UnityEngine;

namespace Rockets_TinyYetBig.Patches.ResearchPatches
{
	internal class ResearchTypes_Patch
	{
		[HarmonyPatch(typeof(ResearchTypes), MethodType.Constructor, [])]
		public class ResearchTypes_Constructor_Patch
		{
			/// <summary>
			/// adds the deep space research type
			/// </summary>
			/// <param name="__instance"></param>
			public static void Postfix(ResearchTypes __instance)
			{
				ModTechsDB.RegisterCustomTechTypes(__instance);
			}
		}
	}
}
