
using BlueprintsV2.BlueprintData;
using BlueprintsV2.ModAPI;
using BlueprintsV2.Tools;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UtilLibs;
using static STRINGS.UI.SANDBOXTOOLS.SETTINGS;

namespace BlueprintsV2.Visualizers
{
	public class BuildingVisual : IVisual
	{
		public GameObject Visualizer { get; protected set; }
		public Vector2I Offset { get; protected set; }

		public PlanScreen.RequirementsState RequirementsState { get; protected set; }

		protected int cell;

		protected readonly BuildingConfig buildingConfig;

		public BuildingDef BuildingDef => buildingConfig?.BuildingDef;

		public BuildingVisual(BuildingConfig buildingConfig, int cell)
		{
			Offset = buildingConfig.Offset;
			this.buildingConfig = buildingConfig;
			this.cell = cell;

			Vector3 positionCbc = Grid.CellToPosCBC(cell, buildingConfig.BuildingDef.SceneLayer);
			Visualizer = GameUtil.KInstantiate(buildingConfig.BuildingDef.BuildingPreview, positionCbc, Grid.SceneLayer.Front, "BlueprintModBuildingVisualizer", LayerMask.NameToLayer("Place"));
			Visualizer.transform.SetPosition(positionCbc);
			Visualizer.SetActive(true);

			if (Visualizer.GetComponent<Rotatable>() != null)
			{
				Visualizer.GetComponent<Rotatable>().SetOrientation(buildingConfig.Orientation);
			}
			ModAPI.API_Methods.ApplyAdditionalBuildingData(Visualizer, buildingConfig);

			if (Visualizer.TryGetComponent<KBatchedAnimController>(out var batchedAnimController))
			{
				batchedAnimController.visibilityType = KAnimControllerBase.VisibilityType.Always;
				batchedAnimController.isMovable = true;
				batchedAnimController.Offset = buildingConfig.BuildingDef.GetVisualizerOffset();
				batchedAnimController.TintColour = GetVisualizerColor(cell);

				batchedAnimController.SetLayer(LayerMask.NameToLayer("Place"));
				batchedAnimController.Play("place");
			}
			else
			{
				Visualizer.SetLayerRecursively(LayerMask.NameToLayer("Place"));
			}
			UpdateRequirementsState();
		}

		public virtual bool IsPlaceable(int cellParam)
		{
			return ValidCell(cellParam) && HasTech() && AllowedInWorld();
		}
		public virtual void ForceRedraw() => MoveVisualizer(cell, true);
		public virtual void MoveVisualizer(int cellParam, bool forceRedraw = false)
		{
			if (cell != cellParam || forceRedraw)
			{
				Visualizer.transform.SetPosition(Grid.CellToPosCBC(cellParam, buildingConfig.BuildingDef.SceneLayer));

				if (Visualizer.GetComponent<KBatchedAnimController>() != null)
				{
					Visualizer.GetComponent<KBatchedAnimController>().TintColour = GetVisualizerColor(cellParam);
				}
				cell = cellParam;
			}
		}

		private Tag[] GetConstructionElements()
		{
			var ingredients = buildingConfig.BuildingDef.CraftRecipe.Ingredients;
			var elements = new List<Tag>(buildingConfig.SelectedElements.Count);
			for (int i = 0; i < ingredients.Count; ++i)
			{
				var ingredient = ingredients[i];
				Tag selectedElement;
				if (i < buildingConfig.SelectedElements.Count)
				{
					selectedElement = buildingConfig.SelectedElements[i];
				}
				else
				{
					//should never happen, just in case to prevent crash.
					selectedElement = ModAssets.GetFirstAvailableMaterial(ingredient.tag, ingredient.amount);
				}
				var key = BlueprintSelectedMaterial.GetBlueprintSelectedMaterial(selectedElement, ingredient.tag, buildingConfig.BuildingDef.PrefabID);

				if (ModAssets.TryGetReplacementTag(key, out var replacement))
				{
					selectedElement = replacement;
				}
				elements.Add(selectedElement);
			}

			return elements.ToArray();
		}

		private bool ViableReplacementCandidate(GameObject toReplace)
		{
			if (toReplace.TryGetComponent<BuildingComplete>(out var component))
			{
				return (component.Def.Replaceable && buildingConfig.BuildingDef.CanReplace(toReplace) && (component.Def != buildingConfig.BuildingDef || GetConstructionElements()[0] != component.GetComponent<PrimaryElement>().Element.tag));
			}
			return false;
		}

		bool ReplacementLayerOccupied(int cellParam)
		{
			var def = buildingConfig.BuildingDef;
			var objOnLayer = Grid.Objects[cellParam, (int)def.ReplacementLayer];

			if (objOnLayer != null && objOnLayer != Visualizer)
				return true;
			if (def.EquivalentReplacementLayers != null)
			{
				foreach (ObjectLayer replacementLayer in def.EquivalentReplacementLayers)
				{
					objOnLayer = Grid.Objects[cellParam, (int)replacementLayer];
					if (objOnLayer != null && objOnLayer != Visualizer)
						return true;
				}
			}
			return false;
		}
		public virtual void ApplyBuildingData(GameObject building)
		{
			bool isPlanned = building.TryGetComponent<BuildingUnderConstruction>(out var buildingUnderConstruction);
			bool isCOmplete = building.TryGetComponent<BuildingComplete>(out var buildingComplete);

			var def = buildingConfig.BuildingDef;
			var orientation = buildingConfig.Orientation;

			if (building.TryGetComponent<Rotatable>(out var rotatable))
			{
				rotatable.SetOrientation(orientation);
			}
			ModAPI.API_Methods.ApplyAdditionalBuildingData(building, buildingConfig);

			if (Visualizer.TryGetComponent<KBatchedAnimController>(out var kbac))
			{
				kbac.TintColour = ModAssets.BLUEPRINTS_COLOR_INVALIDPLACEMENT;
				if (isPlanned)
					kbac.Play("place");
			}

			if (buildingConfig.BuildingDef.BuildingComplete.GetComponent<IHaveUtilityNetworkMgr>() != null && building.TryGetComponent<KAnimGraphTileVisualizer>(out var vis) && buildingConfig.GetConduitFlags(out var flags))
			{
				vis.UpdateConnections((UtilityConnections)flags);
			}

			if (isPlanned && ToolMenu.Instance != null)
			{
				building.FindOrAddComponent<Prioritizable>().SetMasterPriority(ToolMenu.Instance.PriorityScreen.GetLastSelectedPriority());
			}
		}
		public virtual bool PlaceFinishedBuilding(int cellParam)
		{
			Vector3 positionCbc = Grid.CellToPosCBC(cellParam, buildingConfig.BuildingDef.SceneLayer);
			GameObject building = buildingConfig.BuildingDef.Create(positionCbc, null, selected_elements: GetConstructionElements(), buildingConfig.BuildingDef.CraftRecipe, 293.15f, buildingConfig.BuildingDef.BuildingComplete);
			if (building == null)
			{
				return false;
			}

			buildingConfig.BuildingDef.MarkArea(cellParam, buildingConfig.Orientation, buildingConfig.BuildingDef.ObjectLayer, building);
			if (buildingConfig.BuildingDef.IsFoundation)
				buildingConfig.BuildingDef.RunOnArea(cellParam, buildingConfig.Orientation, cell0 => TileVisualizer.RefreshCell(cell0, buildingConfig.BuildingDef.TileLayer, buildingConfig.BuildingDef.ReplacementLayer));

			if (building.GetComponent<Deconstructable>() != null)
			{
				building.GetComponent<Deconstructable>().constructionElements = this.GetConstructionElements();
			}

			ApplyBuildingData(building);

			building.SetActive(true);
			return true;
		}
		public virtual bool PlacePlannedBuilding(int cellParam)
		{
			var def = buildingConfig.BuildingDef;
			var orientation = buildingConfig.Orientation;
			Vector3 positionCbc = Grid.CellToPosCBC(cellParam, def.SceneLayer);
			GameObject building = def.Instantiate(positionCbc, orientation, this.GetConstructionElements());
			if (building == null)
			{
				return false;
			}
			ApplyBuildingData(building);

			building.SetActive(true);
			return true;
		}
		public virtual bool TryReconstructExistingBuilding(int cellParam)
		{
			if (CanRebuildWithMaterial(cellParam, out var reconstructable))
			{
				reconstructable.RequestReconstruct(buildingConfig.SelectedElements[0]);
				ApplyBuildingData(reconstructable.gameObject);
				return true;
			}
			else if (reconstructable != null && reconstructable.gameObject != null)
			{
				ApplyBuildingData(reconstructable.gameObject);
			}
			return false;
		}

		public virtual bool SameBuildingAlreadyInPlace(int cellParam, out BuildingComplete bc)
		{
			bc = null;
			var def = buildingConfig.BuildingDef;
			var existingBuilding = Grid.Objects[cellParam, (int)def.ObjectLayer];
			if (existingBuilding != null && existingBuilding.TryGetComponent<BuildingComplete>(out bc))
			{
				//is same def AND the building cell is alligned with the visualizer cell (aka the building is in the exact same spot as the vis.)
				if (bc.Def == def && Grid.PosToCell(existingBuilding) == cellParam)
				{
					return true;
				}
			}
			return false;
		}

		public virtual bool CanRebuildWithMaterial(int cellParam, out Reconstructable reconstructable)
		{
			reconstructable = null;
			var def = buildingConfig.BuildingDef;
			if (SameBuildingAlreadyInPlace(cellParam, out var bc))
			{
				if (bc.Def == def
					&& bc.TryGetComponent<Reconstructable>(out reconstructable)
					&& reconstructable.AllowReconstruct
					&& bc.TryGetComponent<PrimaryElement>(out var primaryElement)
					&& primaryElement.Element.tag != GetConstructionElements()[0])
				{
					return true;
				}
			}
			return false;
		}

		public virtual bool TryUse(int cellParam)
		{
			if (BlueprintState.InstantBuild && ValidCell(cellParam) && AllowedInWorld())
			{
				for (int index = 0; index < buildingConfig.BuildingDef.PlacementOffsets.Length; ++index)
				{
					CellOffset rotatedCellOffset = Rotatable.GetRotatedCellOffset(buildingConfig.BuildingDef.PlacementOffsets[index], buildingConfig.Orientation);
					int offsetCell = Grid.OffsetCell(cellParam, rotatedCellOffset);
					if (!Grid.Objects[offsetCell, (int)ObjectLayer.FoundationTile])
						WorldDamage.Instance.DestroyCell(offsetCell);
				}
				return PlaceFinishedBuilding(cellParam);
			}
			else if (IsPlaceable(cellParam))
			{
				return PlacePlannedBuilding(cellParam);
			}
			else if (BlueprintState.ForceMaterialChange && CanRebuildWithMaterial(cellParam,out _))
			{
				return TryReconstructExistingBuilding(cellParam);
			}
			else if (SameBuildingAlreadyInPlace(cellParam, out var bc))
			{
				ApplyBuildingData(bc.gameObject);
				if(buildingConfig.HasAnyBuildingData)
					PopFXManager.Instance.SpawnFX(ModAssets.BLUEPRINTS_APPLY_SETTINGS_SPRITE, STRINGS.UI.TOOLS.USE_TOOL.SETTINGS_APPLIED, null, offset: PlayerController.GetCursorPos(KInputManager.GetMousePos()), Config.Instance.FXTime);

				return true;
			}

			return false;
		}
		public virtual void ClearTilePreview(int cell)
		{
			var def = buildingConfig.BuildingDef;

			if (!Grid.IsValidBuildingCell(cell) || !def.IsTilePiece)
				return;
			GameObject tileLayerObject = Grid.Objects[cell, (int)def.TileLayer];
			if (Visualizer == tileLayerObject)
				Grid.Objects[cell, (int)def.TileLayer] = null;
			if (!def.isKAnimTile)
				return;
			GameObject replacementLayerObject = null;
			if (def.ReplacementLayer != ObjectLayer.NumLayers)
				replacementLayerObject = Grid.Objects[cell, (int)def.ReplacementLayer];
			if (tileLayerObject != null && !tileLayerObject.TryGetComponent<Constructable>(out _) || !(replacementLayerObject == null) && !replacementLayerObject != Visualizer)
				return;
			Grid.Objects[cell, (int)def.ReplacementLayer] = null;
			World.Instance.blockTileRenderer.RemoveBlock(def, false, SimHashes.Void, cell);
			World.Instance.blockTileRenderer.RemoveBlock(def, true, SimHashes.Void, cell);
			TileVisualizer.RefreshCell(cell, def.TileLayer, def.ReplacementLayer);
		}

		/// <summary>
		/// experiment to allow replace building over stuff based on regular build tool, not in use.
		/// </summary>
		/// <param name="cellParam"></param>
		/// <returns></returns>
		public virtual bool TryBuild(int cellParam)
		{
			ClearTilePreview(cellParam);
			Vector3 posCbc = Grid.CellToPosCBC(cellParam, Grid.SceneLayer.Building);
			GameObject builtItem = null;
			var def = buildingConfig.BuildingDef;
			var buildingOrientation = buildingConfig.Orientation;
			var selectedElements = GetConstructionElements();
			var visualizer = Visualizer;

			SgtLogger.l("Visualizer test");
			SgtLogger.Assert("Visualizer was null", visualizer);

			bool instantBuild = DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive && SandboxToolParameterMenu.instance.settings.InstantBuild;

			if (Grid.Objects[cellParam, (int)def.TileLayer] == Visualizer)
				Grid.Objects[cellParam, (int)def.TileLayer] = null;

			if (Grid.Objects[cellParam, (int)def.ObjectLayer] == Visualizer)
				Grid.Objects[cellParam, (int)def.ObjectLayer] = null;

			if (Grid.Objects[cellParam, (int)def.ReplacementLayer] == Visualizer)
				Grid.Objects[cellParam, (int)def.ReplacementLayer] = null;

			if (!instantBuild)
			{
				builtItem = def.TryPlace(visualizer, posCbc, buildingOrientation, selectedElements, null);
			}
			else if (def.IsValidBuildLocation(visualizer, posCbc, buildingOrientation) && def.IsValidPlaceLocation(visualizer, posCbc, buildingOrientation, out string _))
			{
				builtItem = def.Build(cell, buildingOrientation, null, selectedElements, 293.15f, null, false, GameClock.Instance.GetTime());
			}

			if (builtItem == null && def.ReplacementLayer != ObjectLayer.NumLayers)
			{
				GameObject replacementCandidate = def.GetReplacementCandidate(cell);
				if (replacementCandidate != null && !def.IsReplacementLayerOccupied(cell))
				{
					BuildingComplete component = replacementCandidate.GetComponent<BuildingComplete>();
					if (component != null && component.Def.Replaceable && def.CanReplace(replacementCandidate) && (component.Def != def
								|| selectedElements[0] != replacementCandidate.GetComponent<PrimaryElement>().Element.tag))
					{
						if (!instantBuild)
						{
							builtItem = def.TryReplaceTile(visualizer, posCbc, buildingOrientation, selectedElements, null);
							Grid.Objects[cell, (int)def.ReplacementLayer] = builtItem;
						}
						else if (def.IsValidBuildLocation(visualizer, posCbc, buildingOrientation, true) && def.IsValidPlaceLocation(visualizer, posCbc, buildingOrientation, true, out string _))
							builtItem = InstantBuildReplace(cell, posCbc, replacementCandidate);
					}
				}
			}

			SgtLogger.Assert("builtItem", builtItem);
			PostProcessBuild(instantBuild, posCbc, builtItem);
			return builtItem != null;
		}
		private GameObject InstantBuildReplace(int cell, Vector3 pos, GameObject tile)
		{
			var def = buildingConfig.BuildingDef;
			var buildingOrientation = buildingConfig.Orientation;
			var selectedElements = GetConstructionElements();

			if (!tile.TryGetComponent<SimCellOccupier>(out var SCO))
			{
				UnityEngine.Object.Destroy(tile);
				return def.Build(cell, buildingOrientation, null, selectedElements, 293.15f, null, false, GameClock.Instance.GetTime());
			}
			SCO.DestroySelf(() =>
			{
				UnityEngine.Object.Destroy(tile);
				PostProcessBuild(true, pos, def.Build(cell, buildingOrientation, null, selectedElements, 293.15f, null, false, GameClock.Instance.GetTime()));
			});
			return null;
		}

		private void PostProcessBuild(bool instantBuild, Vector3 pos, GameObject builtItem)
		{
			if (builtItem == null)
				return;
			if (!instantBuild)
			{
				Prioritizable component = builtItem.GetComponent<Prioritizable>();
				if (component != null)
				{
					if (ToolMenu.Instance != null)
						component.SetMasterPriority(ToolMenu.Instance.PriorityScreen.GetLastSelectedPriority());
				}
			}
			ModAPI.API_Methods.ApplyAdditionalBuildingData(builtItem, buildingConfig);

			if (Visualizer.TryGetComponent<KBatchedAnimController>(out var kbac))
			{
				kbac.TintColour = ModAssets.BLUEPRINTS_COLOR_INVALIDPLACEMENT;
				kbac.Play("place");
			}
			if (buildingConfig.BuildingDef.BuildingComplete.GetComponent<IHaveUtilityNetworkMgr>() != null && builtItem.TryGetComponent<KAnimGraphTileVisualizer>(out var vis) && buildingConfig.GetConduitFlags(out var flags))
			{
				vis.UpdateConnections((UtilityConnections)flags);
			}
		}
		public virtual bool AllowedInWorld()
		{
			return API_Methods.IsBuildable(buildingConfig.BuildingDef);
		}

		public virtual bool HasTech()
		{
			return (BlueprintState.InstantBuild || !Config.Instance.RequireConstructable || Db.Get().TechItems.IsTechItemComplete(buildingConfig.BuildingDef.PrefabID));
		}
		public virtual bool ValidCell(int cellParam)
		{
			var pos = Grid.CellToPos(cellParam);
			if (Grid.IsValidCell(cellParam)
				&& Grid.IsVisible(cellParam))
			{
				bool IsValidPlaceLocation = buildingConfig.BuildingDef.IsValidPlaceLocation(Visualizer, cellParam, buildingConfig.Orientation, out string faiReason);
				bool IgnorableFailReason =
					faiReason == global::STRINGS.UI.TOOLTIPS.HELP_BUILDLOCATION_WALL
					|| faiReason == global::STRINGS.UI.TOOLTIPS.HELP_BUILDLOCATION_CORNER
					|| faiReason == global::STRINGS.UI.TOOLTIPS.HELP_BUILDLOCATION_CORNER_FLOOR;


				bool validCell = (IsValidPlaceLocation || IgnorableFailReason);

				bool replacement = false;
				//BlueprintState.InstantBuild ? false : buildingConfig.BuildingDef.IsValidReplaceLocation(pos, buildingConfig.Orientation, buildingConfig.BuildingDef.ReplacementLayer, buildingConfig.BuildingDef.ObjectLayer);

				return (validCell || replacement);
			}

			return false;
		}

		public virtual void UpdateRequirementsState()
		{
			API_Methods.BuildableStateValid(buildingConfig.BuildingDef, out var state);
			RequirementsState = state;
		}

		public virtual Color GetVisualizerColor(int cellParam)
		{
			UpdateRequirementsState();
			if (!BlueprintState.InstantBuild && BlueprintState.ForceMaterialChange && CanRebuildWithMaterial(cellParam, out _))
			{
				return ModAssets.BLUEPRINTS_COLOR_VALIDPLACEMENT;
			}
			else if(SameBuildingAlreadyInPlace(cellParam,out _) && buildingConfig.HasAnyBuildingData)
			{
				return ModAssets.BLUEPRINTS_COLOR_CAN_APPLY_SETTINGS;
			}
			else if (!ValidCell(cellParam))
			{
				return ModAssets.BLUEPRINTS_COLOR_INVALIDPLACEMENT;
			}
			else if (!HasTech())
			{
				return ModAssets.BLUEPRINTS_COLOR_NOTECH;
			}
			else if (RequirementsState == PlanScreen.RequirementsState.Materials && Config.Instance.RequireConstructable)
				return ModAssets.BLUEPRINTS_COLOR_NOMATERIALS;
			else if (!AllowedInWorld())
				return ModAssets.BLUEPRINTS_COLOR_NOTALLOWEDINWORLD;
			else
			{
				return ModAssets.BLUEPRINTS_COLOR_VALIDPLACEMENT;
			}
		}
	}
}
