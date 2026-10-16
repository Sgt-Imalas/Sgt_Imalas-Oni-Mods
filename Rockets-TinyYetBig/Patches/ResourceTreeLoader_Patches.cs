using HarmonyLib;
using Rockets_TinyYetBig.Content.ModDb;
using UnityEngine;

namespace Rockets_TinyYetBig.Patches.ResearchPatches
{
	internal class ResourceTreeLoader_Patches
	{
		/// <summary>
		/// add research card to research screen
		/// </summary>
		[HarmonyPatch(typeof(ResourceTreeLoader<ResourceTreeNode>), MethodType.Constructor, typeof(TextAsset))]
		public class ResourceTreeLoader_Load_Patch
		{
			public static void Postfix(ResourceTreeLoader<ResourceTreeNode> __instance) => ModTechsDB.RegisterTechCards(__instance);
		}
	}
}
