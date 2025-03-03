using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rockets_TinyYetBig.Patches.ResearchPatches
{
	internal class ResearchTypes_Patch
	{
		[HarmonyPatch(typeof(ResearchTypes), MethodType.Constructor, [])]
		public class ResearchTypes_Constructor_Patch
		{
			public static void Postfix(ResearchTypes __instance)
			{

				__instance.Types.Add(new ResearchType(
					ModAssets.DeepSpaceScienceID,
					STRINGS.DEEPSPACERESEARCH.NAME,
					STRINGS.DEEPSPACERESEARCH.NAME,
					Assets.GetSprite((HashedString)"research_type_deep_space_icon"),
					new Color32(100, 100, 100, byte.MaxValue),
					null,
					2400f,
					(HashedString)"research_center_kanim",
					[],
					STRINGS.DEEPSPACERESEARCH.RECIPEDESC
				));
			}
		}
	}
}
