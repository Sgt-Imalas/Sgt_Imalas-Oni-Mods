using HarmonyLib;
using MassMoveTo.Content.Defs.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using UtilLibs;

namespace MassMoveTo.Patches
{
	internal class Movable_Patches
	{

		[HarmonyPatch(typeof(Movable), nameof(Movable.CreateStorageProxy))]
		public class Movable_CreateStorageProxy_Patch
		{
			[HarmonyPrepare]
			static bool Prepare() => Config.Instance.MultiDeliveryTargets;

			public static IEnumerable<CodeInstruction> Transpiler(ILGenerator _, IEnumerable<CodeInstruction> orig)
			{
				var vanillaIdField = AccessTools.Field(typeof(MovePickupablePlacerConfig), nameof(MovePickupablePlacerConfig.ID));
				var modIdFIeld = AccessTools.Field(typeof(MMT_MultiMovePickupablePlacerConfig), nameof(MMT_MultiMovePickupablePlacerConfig.ID));

				foreach (var ci in orig)
				{
					if (ci.LoadsField(vanillaIdField))
						yield return new CodeInstruction(OpCodes.Ldsfld, modIdFIeld);
					else
						yield return ci;
				}
			}
		}

		//[HarmonyPatch(typeof(Movable), nameof(Movable.CleanupMove))]
		//public class Movable_CleanupMove_Patch
		//{	
		//	public static void Prefix(Movable __instance)
		//	{
		//		SgtLogger.l(__instance + " OnCleanupMove"+", has Proxy: "+__instance.StorageProxy);
		//	}
		//}
	}
}
