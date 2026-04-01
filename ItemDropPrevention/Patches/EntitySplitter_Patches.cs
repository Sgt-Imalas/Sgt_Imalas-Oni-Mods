using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace ItemDropPrevention.Patches
{
	internal class EntitySplitter_Patches
	{

		[HarmonyPatch(typeof(EntitySplitter), nameof(EntitySplitter.CanFirstAbsorbSecond))]
		public class EntitySplitter_CanFirstAbsorbSecond_Patch
		{
			public static void Postfix(Pickupable pickupable, Pickupable other, ref bool __result)
			{
				if (!__result)
					return;

				if (pickupable != null && pickupable.KPrefabID.HasTag(ModAssets.BlockedFromDoingStuff) || other != null && other.KPrefabID.HasTag(ModAssets.BlockedFromDoingStuff))
				{
					__result = false;
				}
			}
		}
	}
}
