using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
    class ResourceTreeLoader_Patches
	{
		/// <summary>
		/// add research card to research screen
		/// </summary>
		[HarmonyPatch(typeof(ResourceTreeLoader<ResourceTreeNode>), MethodType.Constructor, typeof(TextAsset))]
		public class ResourceTreeLoader_Load_Patch
		{
			public static void Postfix(ResourceTreeLoader<ResourceTreeNode> __instance, TextAsset file)
			{
				ModTechs.RegisterTechCards(__instance);
			}
		}
	}
}
