using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig.Patches
{
	internal class PlaceSpaceAvailable_Patches
	{
		static SelectModuleCondition.SelectionContext _lastContext;
		[HarmonyPatch(typeof(PlaceSpaceAvailable), nameof(PlaceSpaceAvailable.EvaluateCondition))]
		public class PlaceSpaceAvailable_EvaluateCondition_Patch
		{
			public static void Postfix(SelectModuleCondition.SelectionContext selectionContext) => _lastContext = selectionContext;
		}

		[HarmonyPatch(typeof(PlaceSpaceAvailable), nameof(PlaceSpaceAvailable.GetStatusTooltip))]
		public class PlaceSpaceAvailable_GetStatusTooltip_Patch
		{
			public static void Postfix(bool ready, GameObject moduleBase, BuildingDef selectedPart, ref string __result)
			{
				ModAssets.ClearBlockedBy_Construction();

				if (ready 
					|| !moduleBase.TryGetComponent<BuildingAttachPoint>(out var ap) 
					|| !moduleBase.TryGetComponent<Building>(out var existingBuilding))
					return;


				HashSet<int> obstructingCells = [];
				GameObject moduleAbove = GetAttachedModule(ap);

				//logic mirrors the patched methods checks, but with dedicated collection of values so they can be written into the text
				switch (_lastContext)
				{
					case SelectModuleCondition.SelectionContext.AddModuleAbove:
						// modules above must be able to move up by the new part's height
						if (moduleAbove != null)
							CollectStackBlockers(moduleAbove, selectedPart.HeightInCells, null, obstructingCells);
						CollectBlockers(Grid.OffsetCell(Grid.PosToCell(moduleBase), 0, existingBuilding.Def.HeightInCells), selectedPart, moduleBase, null, obstructingCells);
						break;

					case SelectModuleCondition.SelectionContext.AddModuleBelow:
						// existing module and everything above it must be able to move up
						CollectStackBlockers(moduleBase, selectedPart.HeightInCells, null, obstructingCells);
						CollectBlockers(Grid.PosToCell(moduleBase), selectedPart, moduleBase, null, obstructingCells);
						break;

					case SelectModuleCondition.SelectionContext.ReplaceModule:
						int moveAmount = selectedPart.HeightInCells - existingBuilding.Def.HeightInCells;
						if (moduleAbove != null)
							CollectStackBlockers(moduleAbove, moveAmount, moduleBase, obstructingCells);

						CollectBlockers(Grid.PosToCell(moduleBase), selectedPart, moduleBase, null, obstructingCells);
						break;

					default:
						return;
				}

				if (obstructingCells.Count == 0)
					return;



				ModAssets.SetBlocker(obstructingCells.First(), true, out string buildingName);

				string blockedBy = STRINGS.UI_MOD.RTB_CONSTRUCTIONBLOCKEDBY + " " + UIUtils.StripAllFormatting(buildingName) + " "+ STRINGS.UI_MOD.RTB_CLICKTOSELECT;
				//string blockedBy = STRINGS.UI_MOD.RTB_CONSTRUCTIONBLOCKEDBY+="\n";
				////skip double mentions
				//foreach (var cell in obstructingCells)
				//{
				//	var building = GetBlockingBuildingAt(cell);

				//	if (building != null)
				//	{
				//		if (!ModAssets.LastBlockingBuildings.Contains(building))
				//		{
				//			ModAssets.LastBlockingBuildings.Add(building);
				//			blockedBy += UIUtils.StripAllFormatting(building.GetProperName());
				//			blockedBy += ", ";
				//		}
				//	}
				//	else
				//	{
				//		ModAssets.LastBlockingCells.Add(cell);
				//		blockedBy += Grid.Element[cell].name;
				//		blockedBy += ", ";
				//	}
				//}
				//blockedBy = blockedBy.TrimEnd(',');
				//blockedBy += "\n"+STRINGS.UI_MOD.RTB_CLICKTOSELECT;
				__result += blockedBy;
			}

			static GameObject GetBlockingBuildingAt(int cell)
			{
				var obj = Grid.Objects[cell, (int)ObjectLayer.Building];
				if (obj == null) //gantries have their own layer
					obj = Grid.Objects[cell, (int)ObjectLayer.Gantry];
				return obj;
			}

			static GameObject GetAttachedModule(BuildingAttachPoint ap)
			{
				var attached = ap.points[0].attachedBuilding;
				if (attached != null && attached.HasTag(GameTags.RocketModule))
					return attached.gameObject;
				return null;
			}

			/// <summary>
			/// Mirrors ReorderableBuilding.CanMoveVertically, but also collects the cells
			/// </summary>
			static void CollectStackBlockers(GameObject first, int moduleHeight, GameObject ignoreModule, HashSet<int> blockedCells)
			{
				var chain = new List<GameObject>();
				if (ignoreModule != null)
					chain.Add(ignoreModule);

				GameObject current = first;
				while (current != null && !chain.Contains(current))
				{
					chain.Add(current);
					current = current.TryGetComponent<BuildingAttachPoint>(out var ap) ? GetAttachedModule(ap) : null;
				}

				foreach (var module in chain)
				{
					if (module == ignoreModule || !module.TryGetComponent<Building>(out var building))
						continue;

					int shiftedCell = Grid.OffsetCell(Grid.PosToCell(module), 0, moduleHeight);
					CollectBlockers(shiftedCell, building.Def, module, chain, blockedCells);

					//remove if ever going back to multiple items in select
					if (blockedCells.Count > 0)
						return;
				}
			}

			/// <summary>
			/// Second half of vertical move check cell collection
			/// </summary>
			static void CollectBlockers(int originCell, BuildingDef def, GameObject ignore, List<GameObject> chain, HashSet<int> blockedCells)
			{
				//remove if ever going back to multiple items in select
				if (blockedCells.Count > 0)
					return;

				foreach (CellOffset offset in def.PlacementOffsets)
				{
					int cell = Grid.OffsetCell(originCell, offset);
					if (!Grid.IsValidCell(cell))
						continue;
					if (ReorderableBuilding.CheckCellClear(cell, ignore))
						continue;

					//own stack members that will move away are not blockers
					var buildingAtCell = GetBlockingBuildingAt(cell);
					if (buildingAtCell != null && chain != null && chain.Contains(buildingAtCell))
						continue;

					blockedCells.Add(cell);
				}
			}
		}
	}
}
