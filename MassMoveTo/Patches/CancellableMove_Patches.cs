using FMOD;
using HarmonyLib;
using MassMoveTo.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Text;

namespace MassMoveTo.Patches
{
	internal class CancellableMove_Patches
	{

		[HarmonyPatch(typeof(CancellableMove), nameof(CancellableMove.OnCancel), [typeof(Movable)])]
		public class CancellableMove_OnCancel_Patch
		{
			public static bool Prefix(CancellableMove __instance, Movable cancel_movable)
			{
				if (__instance is MultiFetch_CancellableMove multiFetchMove)
				{
					multiFetchMove.MultiChore_OnCancel(cancel_movable);
					return false;
				}
				return true;
			}
		}

		[HarmonyPatch(typeof(CancellableMove), nameof(CancellableMove.SetMovable))]
		public class CancellableMove_SetMovable_Patch
		{
			public static bool Prefix(CancellableMove __instance, Movable movable)
			{
				if (__instance is MultiFetch_CancellableMove multiFetchMove)
				{
					multiFetchMove.MultiChore_SetMovable(movable);
					return false;
				}
				return true;
			}
		}


		[HarmonyPatch(typeof(CancellableMove), nameof(CancellableMove.IsDeliveryComplete))]
		public class CancellableMove_IsDeliveryComplete_Patch
		{
			public static bool Prefix(CancellableMove __instance, ref bool __result)
			{
				if (__instance is MultiFetch_CancellableMove multiFetchMove)
				{
					__result = true;
					return false;
				}
				return true;
			}
		}
		//[HarmonyPatch(typeof(CancellableMove), nameof(CancellableMove.RemoveMovable))]
		//public class CancellableMove_RemoveMovable_Patch
		//{
		//	public static bool Post(CancellableMove __instance, Movable moved)
		//	{
		//		if (__instance is MultiFetch_CancellableMove multiFetchMove)
		//		{
		//			multiFetchMove.MultiChore_RemoveMovable(moved);
		//			return false;
		//		}
		//		return true;
		//	}
		//}
	}
}
