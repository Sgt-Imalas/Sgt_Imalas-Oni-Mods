using _3GuBsVisualFixesNTweaks.Defs.Entities;
using _3GuBsVisualFixesNTweaks.Scripts;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static STRINGS.UI.CLUSTERMAP;

namespace _3GuBsVisualFixesNTweaks.Patches
{
	class BuildTool_Patches
	{
		static BuildLocationRule TargetRule;
		static GameObject WallVisualizer = null;
		static List<PlaceWallIndicator> Visualizers = new List<PlaceWallIndicator>();

		static void DeleteVisualizerIfActive()
		{
			if (WallVisualizer != null)
			{
				UnityEngine.Object.Destroy(WallVisualizer);
				WallVisualizer = null;
			}
			if (Visualizers.Any())
			{
				for (int i = Visualizers.Count() - 1; i >= 0; i--)
				{
					if (Visualizers[i] != null)
					{
						UnityEngine.Object.Destroy(Visualizers[i].gameObject);
					}
				}
				Visualizers.Clear();
			}
		}

		static PlaceWallIndicator CreateWallIndicator(BuildingDef def, Vector3 worldPos, CellOffset offset, Orientation visOrientation)
		{		
			var placerGO = GameUtil.KInstantiate(Assets.GetPrefab(PlacerWallPreviewConfig.ID), worldPos, Grid.SceneLayer.FXFront, gameLayer: LayerMask.NameToLayer("Place"));
			placerGO.SetActive(true);
			var placer = placerGO.GetComponent<PlaceWallIndicator>();
			placer.CellOffset = offset;
			placer.InitRotation(visOrientation);
			Visualizers.Add(placer);
			return placer;
		}
		static void CreateVisualizersForDef(BuildingDef def, Vector3 worldPos, Orientation buildingOrientation)
		{
			if (def.BuildingComplete.HasTag(ModAssets.ModTags.PlacementVisualizerExcluded))
				return;

			bool noVerticalWallPreviews = def.BuildingComplete.HasTag(ModAssets.ModTags.PlacementVisualizerExcludedVertical);
			bool noHorizontalWallPreviews = def.BuildingComplete.HasTag(ModAssets.ModTags.PlacementVisualizerExcludedHorizontal);


			TargetRule = def.BuildLocationRule;
			List<CellOffset> targetCellOfssets = new();
			
			switch (TargetRule)
			{
				case BuildLocationRule.OnFoundationRotatable:
				case BuildLocationRule.OnRocketEnvelope:
				case BuildLocationRule.OnFloor:
					foreach (var cellOffset in def.PlacementOffsets)
					{
						if (cellOffset.y == 0 && !noHorizontalWallPreviews)
							CreateWallIndicator(def, worldPos, cellOffset, Orientation.Neutral);
					}
					break;

				case BuildLocationRule.OnWall:
					foreach (var cellOffset in def.PlacementOffsets)
					{
						if (cellOffset.x == 0 && !noVerticalWallPreviews)
							CreateWallIndicator(def, worldPos, cellOffset, Orientation.R270);
					}
					break;

				case BuildLocationRule.OnCeiling:
					foreach (var cellOffset in def.PlacementOffsets)
					{
						if (cellOffset.y == def.HeightInCells-1 && !noHorizontalWallPreviews)
							CreateWallIndicator(def, worldPos, cellOffset, Orientation.R180);
					}
					break;

				case BuildLocationRule.InCorner: //top left corner
					foreach (var cellOffset in def.PlacementOffsets)
					{
						if (cellOffset.y == def.HeightInCells - 1 && !noHorizontalWallPreviews)
							CreateWallIndicator(def, worldPos, cellOffset, Orientation.R180);
					}
					foreach (var cellOffset in def.PlacementOffsets)
					{
						if (cellOffset.x == 0 && !noVerticalWallPreviews)
							CreateWallIndicator(def, worldPos, cellOffset, Orientation.R270);
					}
					break;

				case BuildLocationRule.WallFloor: //bottom corner
					foreach (var cellOffset in def.PlacementOffsets)
					{
						if (cellOffset.y == 0 && !noHorizontalWallPreviews)
							CreateWallIndicator(def, worldPos, cellOffset, Orientation.Neutral);
					}
					foreach (var cellOffset in def.PlacementOffsets)
					{
						if (cellOffset.x == 0 && !noVerticalWallPreviews)
							CreateWallIndicator(def, worldPos, cellOffset, Orientation.R270);
					}
					break;
			}
		}

		static void UpdateRotations(Orientation buildingOrientation)
		{
			if (Visualizers.Any())
			{
				foreach (PlaceWallIndicator vis in Visualizers)
				{
					vis.RefreshRotation(buildingOrientation);
				}
			}
		}
		static void TintVisualizers(Color c)
		{
			if (Visualizers.Any())
			{
				foreach (PlaceWallIndicator vis in Visualizers)
				{
					vis.UpdateTint(c);
				}
			}
		}


		static void MoveVisualizers(Vector3 mousepos, Orientation buildingOrientation)
		{
			if (Visualizers.Any())
			{
				foreach (PlaceWallIndicator vis in Visualizers)
				{
					var rotatedOffset = Rotatable.GetRotatedCellOffset(vis.CellOffset, buildingOrientation);

					var posWithOfset = mousepos + new Vector3(rotatedOffset.x, rotatedOffset.y);
					int cell = Grid.PosToCell(posWithOfset);
					Vector3 posCbc = Grid.CellToPosCBC(cell, Grid.SceneLayer.FXFront);
					vis.transform.SetPosition(posCbc);
					vis.kbac.SetDirty();
				}
			}
		}

		[HarmonyPatch(typeof(BuildTool), nameof(BuildTool.TryRotate))]
		public class BuildTool_TryRotate_Patch
		{
			public static void Postfix(BuildTool __instance)
			{
				UpdateRotations(__instance.buildingOrientation);
			}
		}

		[HarmonyPatch(typeof(BuildTool), nameof(BuildTool.OnActivateTool))]
		public class BuildTool_OnActivateTool_Patch
		{
			public static void Prefix(BuildTool __instance)
			{
				Vector3 worldPos = __instance.ClampPositionToWorld(PlayerController.GetCursorPos(KInputManager.GetMousePos()), ClusterManager.Instance.activeWorld);
				DeleteVisualizerIfActive();
				CreateVisualizersForDef(__instance.def, worldPos, __instance.buildingOrientation);
			}
		}

		[HarmonyPatch(typeof(BuildTool), nameof(BuildTool.OnDeactivateTool))]
		public class BuildTool_OnDeactivateTool_Patch
		{
			public static void Postfix(BuildTool __instance)
			{
				DeleteVisualizerIfActive();
			}
		}

		[HarmonyPatch(typeof(BuildTool), nameof(BuildTool.UpdateVis))]
		public class BuildTool_UpdateVis_Patch
		{
			public static void Postfix(BuildTool __instance, Vector3 pos)
			{
				MoveVisualizers(pos, __instance.buildingOrientation);
			}
		}

		[HarmonyPatch(typeof(BuildTool), nameof(BuildTool.SetColor))]
		public class BuildTool_SetColor_Patch
		{
			public static void Postfix(Color c)
			{
				TintVisualizers(c);
			}
		}
	}
}
