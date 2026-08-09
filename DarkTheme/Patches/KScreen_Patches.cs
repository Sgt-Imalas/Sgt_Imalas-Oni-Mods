using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using UtilLibs;

namespace DarkTheme.Patches
{
	internal class KScreen_Patches
	{

		[HarmonyPatch(typeof(ReportScreen), nameof(ReportScreen.OnPrefabInit))]
		public class ReportScreen_OnPrefabInit_Patch
		{
			public static void Prefix(ReportScreen __instance)
			{
				ModAssets.DarkenBackgrounds(__instance);
				ModAssets.DarkenBackgrounds(__instance.lineItem);
				ModAssets.DarkenBackgrounds(__instance.lineItemHeader);
				ModAssets.DarkenBackgrounds(__instance.lineItemSpacer);
				ModAssets.DarkenBackgrounds(__instance.contentFolder);
			}
		}

		[HarmonyPatch(typeof(AllDiagnosticsScreen), nameof(AllDiagnosticsScreen.OnPrefabInit))]
		public class AllDiagnosticsScreen_OnPrefabInit_Patch
		{
			public static void Prefix(AllDiagnosticsScreen __instance)
			{
				ModAssets.DarkenBackgrounds(__instance);
				ModAssets.DarkenBackgrounds(__instance.rootListContainer);
				ModAssets.DarkenBackgrounds(__instance.diagnosticLinePrefab);
				ModAssets.DarkenBackgrounds(__instance.subDiagnosticLinePrefab);
			}
		}

		[HarmonyPatch(typeof(TableScreen), nameof(TableScreen.OnPrefabInit))]
		public class TableScreen_OnPrefabInit_Patch
		{
			public static void Prefix(TableScreen __instance)
			{
				ModAssets.DarkenBackgrounds(__instance);
				ModAssets.DarkenBackgrounds(__instance.prefab_row_empty.transform, true, 0);
				ModAssets.DarkenBackgrounds(__instance.prefab_row_header.transform, true, 0);
				ModAssets.DarkenBackgrounds(__instance.prefab_world_divider.transform, true, 0);
			}
		}

		[HarmonyPatch(typeof(DetailsScreen), nameof(DetailsScreen.OnPrefabInit))]
		public class DetailsScreen_OnPrefabInit_Patch
		{
			public static void Prefix(DetailsScreen __instance)
			{
				ModAssets.DarkenBackgrounds(__instance);
			}
		}

		[HarmonyPatch(typeof(KScreen), nameof(KScreen.OnPrefabInit))]
		public class KScreen_OnPrefabInit_Patch
		{
			public static void Prefix(KScreen __instance)
			{
				var instanceType = __instance.GetType();
				if(instanceType == typeof(ComplexFabricatorSideScreen) 
				|| instanceType == typeof(SelectedRecipeQueueScreen)
					)
				{
					ModAssets.DarkenBackgrounds(__instance);
				}
			}
		}

		[HarmonyPatch(typeof(ComplexRecipeManager), nameof(ComplexRecipeManager.PostProcess))]
		public class ComplexRecipeManager_PostProcess_Patch
		{
			public static void Prefix(ComplexRecipeManager __instance)
			{
				SgtLogger.l("DUMPING RECIPES: ");
				foreach(var recipe in __instance.preProcessRecipes)
				{
					Console.WriteLine(recipe.id + "," + recipe.recipeCategoryID + ";");
				}
			}
		}
		//[HarmonyPatch(typeof(TableRow), nameof(TableRow.OnPrefabInit))]
		//public class TableRow_OnPrefabInit_Patch
		//{
		//	public static void Prefix(TableRow __instance)
		//	{
		//		LocText reference = __instance.GetComponent<HierarchyReferences>().GetReference<LocText>("NameLabel");
		//		reference.color = Color.white;
		//	}
		//}

		[HarmonyPatch(typeof(ScheduleScreen), nameof(ScheduleScreen.OnPrefabInit))]
		public class ScheduleScreen_OnPrefabInit_Patch
		{
			public static void Prefix(ScheduleScreen __instance)
			{
				ModAssets.DarkenBackgrounds(__instance);
				ModAssets.DarkenBackgrounds(__instance.scheduleEntryPrefab);
			}
		}


		//[HarmonyPatch(typeof(Db), nameof(Db.Initialize))]
		//public class Db_Initialize_Patch
		//{
		//	internal static IEnumerable<MethodBase> Methods()
		//	{
		//		const string name = nameof(KScreen.OnPrefabInit);
		//		yield return AccessTools.Method(typeof(ReportScreen), name);
		//		yield return AccessTools.Method(typeof(AllDiagnosticsScreen), name);
		//		yield return AccessTools.Method(typeof(TableScreen), name);
		//		yield return AccessTools.Method(typeof(ScheduleScreen), name);
		//	}
		//	public static void ScreenDarkeningPrefix(KScreen __instance)
		//	{
		//		bool darkened = ModAssets.DarkenBackgrounds(__instance);
		//		SgtLogger.l("Darkened "+__instance.GetType().Name+"; white screen found: "+darkened);
		//		foreach(var field in  __instance.GetType().GetFields(AccessTools.all))
		//		{
		//			//SgtLogger.l(__instance.name + " has field: " + field.Name+ " of type "+field.FieldType.Name);
		//			if(field.FieldType == typeof(GameObject) && field.Name.ToUpperInvariant().Contains("PREFAB"))
		//			{
		//				//SgtLogger.l($"Found prefab field {field.Name} on {__instance.name}");
		//				var go = Traverse.Create(__instance).Field<GameObject>(field.Name).Value;
		//				if(go != null)
		//					ModAssets.DarkenBackgrounds(go.transform, darkened, 5);
		//			}
		//		}
		//	}
		//	public static void Postfix(Db __instance)
		//	{
		//		var patch = AccessTools.Method(typeof(Db_Initialize_Patch), nameof(ScreenDarkeningPrefix));

		//		foreach(var method in Methods())
		//		{
		//			if(method != null)
		//			{
		//				Mod.Harmony.Patch(method, new HarmonyMethod(patch));
		//			}
		//		}
		//	}
		//}
	}
}
