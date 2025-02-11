using HarmonyLib;
using System;
using UtilLibs;

namespace PaintYourPipes
{
	internal class CompatibilityPatches
	{
		public class Material_Colored_Tiles_Compatibility
		{
			public static void ExecutePatch(Harmony harmony)
			{

				var m_TargetType = AccessTools.TypeByName("MaterialColoredTilesAndMore.Patches");

				var m_Prefix = AccessTools.Method(typeof(Material_Colored_Tiles_Compatibility), "Prefix");
				if (m_TargetType != null)
				{
					var m_TargetMethod = AccessTools.Method(m_TargetType, "ChangeBuildingColor", new Type[] { typeof(BuildingComplete) });
					if (m_TargetMethod == null)
					{
						SgtLogger.warning("MaterialColoredTilesAndMore mod target method ChangeBuildingColor not found on type MaterialColoredTilesAndMore.Patches");
						return;
					}
					harmony.Patch(m_TargetMethod, new HarmonyMethod(m_Prefix));
				}
				else
				{
					SgtLogger.l("MaterialColoredTilesAndMore mod target type Painter not found.");
				}
			}
			public static bool Prefix(BuildingComplete building)
			{
				return !building.TryGetComponent<ColorableConduit>(out _);
			}
		}
		//public class MaterialColour_Compatibility
		//{
		//	public static void ExecutePatch(Harmony harmony)
		//	{
		//		var m_TargetType = AccessTools.TypeByName("MaterialColor.Painter");

		//		var m_Prefix = AccessTools.Method(typeof(MaterialColour_Compatibility), "Prefix");
		//		if (m_TargetType != null)
		//		{
		//			var m_TargetMethod = AccessTools.Method(m_TargetType, "UpdateBuildingColor", new Type[] { typeof(BuildingComplete) });
		//			if (m_TargetMethod == null)
		//			{
		//				SgtLogger.warning("MaterialColor mod target method UpdateBuildingColor not found on type MaterialColor.Painter");
		//				return;
		//			}
		//			harmony.Patch(m_TargetMethod, new HarmonyMethod(m_Prefix));
		//		}
		//		else
		//		{
		//			SgtLogger.l("MaterialColor mod target type Painter not found.");
		//		}
		//	}
		//	public static bool Prefix(BuildingComplete building)
		//	{
		//		return !building.TryGetComponent<ColorableConduit>(out _);
		//	}
		//}
		public class Reverse_Bridges_Compatibility
		{
			public static void ExecutePatch(Harmony harmony)
			{
				var m_TargetType = AccessTools.TypeByName("ReverseBridgeButton");

				var m_Postfix = AccessTools.Method(typeof(Reverse_Bridges_Compatibility), "Postfix");
				if (m_TargetType != null)
				{
					var m_TargetMethod = AccessTools.Method(m_TargetType, "MakeNewBuilding");
					if (m_TargetMethod == null)
					{
						SgtLogger.warning("Reverse Bridges mod target method MakeNewBuilding not found on type ReverseBridgeButton");
						return;
					}
					harmony.Patch(m_TargetMethod, null, new HarmonyMethod(m_Postfix), null);
				}
				else
				{
					SgtLogger.l("Reverse Bridges mod target type ReverseBridgeButton not found.");
				}
			}
			public static void Postfix(BuildingComplete oldBuilding, ref string __state)
			{
				var newBuilding = SelectTool.Instance.selected.gameObject;
				if (oldBuilding.TryGetComponent<ColorableConduit_UnderConstruction>(out var @old_uc)
					&& newBuilding.TryGetComponent<ColorableConduit_UnderConstruction>(out var @new_uc))
				{
					new_uc.ColorHex = @old_uc.ColorHex;
				}
				else if (oldBuilding.TryGetComponent<ColorableConduit>(out var @old)
					&& newBuilding.TryGetComponent<ColorableConduit>(out var @new))
				{
					@new.SetColor(@old.GetColor());
				}
			}

		}
	}
}
