using HarmonyLib;
using Rockets_TinyYetBig.Content.Scripts.UI;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UtilLibs.UIcmp;

namespace Rockets_TinyYetBig.Patches
{
	internal class ConditionListSideScreen_Patches
	{
		/// <summary>
		/// not usable, the method is not called directly in SO
		/// </summary>
		//[HarmonyPatch(typeof(ConditionListSideScreen), nameof(ConditionListSideScreen.BuildRows))]
		//public class ConditionListSideScreen_BuildRows_Patch
		//{
		//	public static void Postfix(ConditionListSideScreen __instance)
		//	{
		//		foreach(var rowcondition in __instance.rows)
		//		{
		//			SgtLogger.l("RowCondition: "+rowcondition.Key.GetType().Name);
		//			if (rowcondition.Key.GetType() != typeof(ConditionFlightPathIsClear))
		//				continue;

		//			SgtLogger.l("adding GoToBlockingLocationHandler");
		//			rowcondition.Value.AddOrGet<GoToBlockingLocationHandler>();
		//		}
		//	}
		//}

		[HarmonyPatch(typeof(ConditionListSideScreen), nameof(ConditionListSideScreen.SetRowState))]
		public class ConditionListSideScreen_SetRowState_Patch
		{
			public static void Postfix(ConditionListSideScreen __instance, GameObject row, ProcessCondition condition)
			{
				if (!row.TryGetComponent<HierarchyReferences>(out var refs))
					return;

				if (condition.GetType() != typeof(ConditionFlightPathIsClear))
					return;

				//set in the method, only active if condition is fulfilled
				bool conditionFulfilled = refs.GetReference<Image>("Check").gameObject.activeSelf;
				if (conditionFulfilled)
					return;

				row.AddOrGet<GoToBlockingLocationHandler>().FlightMode = true;
			}
		}
	}
}
