using HarmonyLib;
using ItemDropPrevention.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ItemDropPrevention.Patches
{
	internal class BaseMinionConfig_Patches
	{

		[HarmonyPatch(typeof(BaseMinionConfig), nameof(BaseMinionConfig.BaseMinion))]
		public class BaseMinionConfig_BaseMinion_Patch
		{
			public static void Postfix(GameObject __result)
			{
				__result.AddOrGet<DroppablesHolder>();
			}
		}
	}
}
