
using BlueprintsV2.BlueprintData;
using BlueprintsV2.BlueprintsV2.BlueprintData.OniTogether_Integration;
using BlueprintsV2.BlueprintsV2.Visualizers.CustomTileRenderer;
using BlueprintsV2.BlueprintsV2.Visualizers.ReplacementVisualizers;
using BlueprintsV2.ModAPI;
using BlueprintsV2.Tools;
using ONI_Together_API;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UtilLibs;
using static BlueprintsV2.BlueprintData.BlueprintState;
using static BlueprintsV2.STRINGS.UI.USEBLUEPRINTSTATECONTAINER.INFOITEMSCONTAINER;

namespace BlueprintsV2.Visualizers
{
	public class BuildingVisual : IVisual
	{
		///store the rotation state of the blueprint without affecting conduits/wires itself; only used by conduits
		protected Orientation BlueprintRotationStateHolder = Orientation.Neutral;

		public GameObject Visualizer { get; protected set; }
		public Vector2I Offset { get; protected set; }

		public PlanScreen.RequirementsState RequirementsState { get; protected set; }

		protected int cell;
		public int CurrentCell => cell;

		protected readonly BuildingConfig buildingConfig;

		public BuildingDef BuildingDef => buildingConfig?.BuildingDef;

		public Orientation RotatedOrientation { get; protected set; }
		public bool FlippedV { get; protected set; }
		public bool FlippedH { get; protected set; }

		public string BuildingID => BuildingDef.PrefabID;
		protected ulong _playerId = BlueprintState.PlayerId_DefaultTilePreviews;
		protected KBatchedAnimController kbac;
		protected bool hasKbac = false;
		protected bool isTile = false;
		//protected Color? _lastColor = null;

		public BuildingVisual(BuildingConfig buildingConfig, int cell, ulong playerId)
		{
			this._playerId = playerId;
			Offset = buildingConfig.Offset;
			RotatedOrientation = buildingConfig.Orientation;
			this.buildingConfig = buildingConfig;
			this.cell = cell;

			Vector3 positionCbc = Grid.CellToPosCBC(cell, buildingConfig.BuildingDef.SceneLayer);
			Visualizer = GameUtil.KInstantiate(buildingConfig.BuildingDef.BuildingPreview, positionCbc, Grid.SceneLayer.Front, "BlueprintModBuildingVisualizer", LayerMask.NameToLayer("Place"));
			Visualizer.transform.SetPosition(positionCbc);
			///has to happen before the visualizer is activated;
			///the kanim batch set a controller ends up in is chosen on registration (which happens on activation),
			///and a controller that is not flagged as always visible at that point lands in a spatially culled batch set of its spawn chunk.
			///always visible controllers never re-register on chunk change, so it would then disappear as soon as that chunk scrolls off screen.
			Visualizer.TryGetComponent<KBatchedAnimController>(out var batchedAnimController);
			if (batchedAnimController != null)
			{
				batchedAnimController.visibilityType = KAnimControllerBase.VisibilityType.Always;
				batchedAnimController.isMovable = true;
				batchedAnimController.Offset = buildingConfig.BuildingDef.GetVisualizerOffset();
			}

			Visualizer.SetActive(true);

			if (Visualizer.TryGetComponent<Rotatable>(out var rotatable))
			{
				rotatable.SetOrientation(RotatedOrientation);
			}
			ModAPI.API_Methods.ApplyAdditionalBuildingData(Visualizer, buildingConfig, _playerId);

			if (batchedAnimController != null)
			{
				//batchedAnimController.TintColour = GetVisualizerColor(cell);

				batchedAnimController.SetLayer(LayerMask.NameToLayer("Place"));
				batchedAnimController.Play("place");
				kbac = batchedAnimController;
			}
			else
			{
				Visualizer.SetLayerRecursively(LayerMask.NameToLayer("Place"));
			}
			ApplyColorIfChanged(cell);
			hasKbac = kbac != null;
			UpdateRequirementsState();
		}

		///relevant for rendering tiles in the multiplayer mod integration
		public ulong GetPlayerId()
		{
			return _playerId;
		}
		public virtual bool IsPlaceable(int cellParam)
		{
			return HasTech() && AllowedInWorld() && ValidCell(cellParam, out bool needsToReplace);
		}
		public virtual void ForceRedraw() => MoveVisualizer(cell, true);
		public virtual void MoveVisualizer(int cellParam, bool forceRedraw = false)
		{
			if (cell != cellParam || forceRedraw)
			{
				Visualizer.transform.SetPosition(Grid.CellToPosCBC(cellParam, buildingConfig.BuildingDef.SceneLayer));
				ApplyColorIfChanged(cellParam);
				cell = cellParam;
			}
		}
		public virtual void RefreshColor()
		{
			ApplyColorIfChanged(cell);
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
		#region replace experiment
		//private bool ViableReplacementCandidate(GameObject toReplace)
		//{
		//	if (toReplace.TryGetComponent<BuildingComplete>(out var component))
		//	{
		//		return (component.Def.Replaceable && buildingConfig.BuildingDef.CanReplace(toReplace) && (component.Def != buildingConfig.BuildingDef || GetConstructionElements()[0] != component.GetComponent<PrimaryElement>().Element.tag));
		//	}
		//	return false;
		//}

		//bool ReplacementLayerOccupied(int cellParam)
		//{
		//	var def = buildingConfig.BuildingDef;
		//	var objOnLayer = Grid.Objects[cellParam, (int)def.ReplacementLayer];

		//	if (objOnLayer != null && objOnLayer != Visualizer)
		//		return true;
		//	if (def.EquivalentReplacementLayers != null)
		//	{
		//		foreach (ObjectLayer replacementLayer in def.EquivalentReplacementLayers)
		//		{
		//			objOnLayer = Grid.Objects[cellParam, (int)replacementLayer];
		//			if (objOnLayer != null && objOnLayer != Visualizer)
		//				return true;
		//		}
		//	}
		//	return false;
		//}
		#endregion
		public virtual void ApplyBuildingData(GameObject building, bool includeTime = true)
		{
			bool isPlanned = building.TryGetComponent<BuildingUnderConstruction>(out var buildingUnderConstruction);
			bool isComplete = building.TryGetComponent<BuildingComplete>(out var buildingComplete);

			var def = buildingConfig.BuildingDef;

			if (isPlanned && buildingUnderConstruction.Def != def)
				return;
			if (isComplete && buildingComplete.Def != def)
				return;

			if (building.TryGetComponent<Rotatable>(out var rotatable))
			{
				rotatable.SetOrientation(RotatedOrientation);
			}
			ModAPI.API_Methods.ApplyAdditionalBuildingData(building, buildingConfig, _playerId);

			if (Visualizer.TryGetComponent<KBatchedAnimController>(out var kbac))
			{
				kbac.TintColour = ModAssets.BLUEPRINTS_COLOR_INVALIDPLACEMENT;
				if (isPlanned)
					kbac.Play("place");
			}

			if (isPlanned && ToolMenu.Instance != null && BlueprintState.CurrentStateInfo(_playerId).UseToolPriority)
			{
				building.FindOrAddComponent<Prioritizable>().SetMasterPriority(ToolMenu.Instance.PriorityScreen.GetLastSelectedPriority());
			}
			if (isComplete && includeTime)
				buildingComplete.SetCreationTime(GameClock.Instance.GetTime());
			UpdateConduitConnectionBits(building);
		}

		public int GetRotatedUtilityConnectionFlags(int plannedFlags)
		{
			int originalRotation = (int)buildingConfig.Orientation; //0-3;
			int rotatedOrientation = (int)BlueprintRotationStateHolder;

			int rotationDiff = originalRotation - rotatedOrientation;

			var shiftable = new List<bool>(4)
			{
				(plannedFlags & (int)UtilityConnections.Left) != 0, //left
				(plannedFlags & (int)UtilityConnections.Right) != 0, //right
				(plannedFlags & (int)UtilityConnections.Up) != 0, //up
				(plannedFlags & (int)UtilityConnections.Down) != 0  //down
			};
			//SgtLogger.l("RotationDiff: " + rotationDiff);

			//SgtLogger.l("Left: " + shiftable[0].ToString());
			//SgtLogger.l("Right: " + shiftable[1].ToString());
			//SgtLogger.l("Up: " + shiftable[2].ToString());
			//SgtLogger.l("Down: " + shiftable[3].ToString());

			if (rotationDiff > 0)
			{
				for (int i = 0; i < rotationDiff; i++)
				{
					//no bit shifting possible because those arent sorted...
					shiftable = [
						shiftable[2],
						shiftable[3],
						shiftable[1],
						shiftable[0],
					];
				}
			}
			else if (rotationDiff < 0)
			{
				for (int i = 0; i < -rotationDiff; ++i)
				{
					shiftable = [
						shiftable[3],
						shiftable[2],
						shiftable[0],
						shiftable[1],
					];
				}
			}
			if (FlippedH)
			{
				bool left = shiftable[0];
				bool right = shiftable[1];
				shiftable[0] = right;
				shiftable[1] = left;
			}
			if (FlippedV)
			{
				bool up = shiftable[2];
				bool down = shiftable[3];
				shiftable[2] = down;
				shiftable[3] = up;
			}

			BitArray bitField = new BitArray(shiftable.ToArray()); //BitArray takes a bool[]
			byte[] bytes = new byte[1];
			bitField.CopyTo(bytes, 0);

			int newRotation = bytes[0];
			//SgtLogger.l("NEW:");

			//SgtLogger.l("Left: " + shiftable[0].ToString());
			//SgtLogger.l("Right: " + shiftable[1].ToString());
			//SgtLogger.l("Up: " + shiftable[2].ToString());
			//SgtLogger.l("Down: " + shiftable[3].ToString());


			//SgtLogger.l($"Original Rotation: {buildingConfig.Orientation}, new Rotation: {RotatedOrientation}, old connection: {plannedFlags} new connection: {newRotation}");
			return newRotation;
		}

		void UpdateConduitConnectionBits(GameObject go)
		{
			if (buildingConfig.BuildingDef.BuildingComplete.GetComponent<IHaveUtilityNetworkMgr>() != null
				&& go.TryGetComponent<KAnimGraphTileVisualizer>(out var vis)
				&& buildingConfig.GetConduitFlags(out var flags))
			{
				var newConnections = (UtilityConnections)GetRotatedUtilityConnectionFlags(flags);
				if (vis.Connections != newConnections)
				{
					UtilityConnections neew = vis.Connections | newConnections;

					vis.UpdateConnections(neew);
					vis.Refresh();
				}
			}
		}

		protected GameObject CreateFinishedBuildingInternal(int cellParam, Vector3 positionCbc)
		{
			var def = buildingConfig.BuildingDef;
			var selectedElements = GetConstructionElements();
			var finishedBuilding = def.Create(positionCbc, null, GetConstructionElements(), def.CraftRecipe, ElementLoader.GetMinMeltingPointAmongElements(selectedElements), def.BuildingComplete);

			if (finishedBuilding == null)
			{
				SgtLogger.warning("failed to place finished building " + def.PrefabID);
				return null;
			}
			ApplyBuildingData(finishedBuilding);

			def.MarkArea(cellParam, RotatedOrientation, def.ObjectLayer, finishedBuilding);
			if (def.IsTilePiece)
			{
				def.MarkArea(cellParam, RotatedOrientation, def.TileLayer, finishedBuilding);
				def.RunOnArea(cellParam, RotatedOrientation, cell0 => TileVisualizer.RefreshCell(cell0, def.TileLayer, def.ReplacementLayer));
			}

			if (finishedBuilding.TryGetComponent<Deconstructable>(out var decon))
			{
				decon.constructionElements = selectedElements;
			}

			finishedBuilding.SetActive(true);
			return finishedBuilding;
		}

		public virtual bool PlaceFinishedBuilding(int cellParam)
		{
			Vector3 positionCbc = Grid.CellToPosCBC(cellParam, buildingConfig.BuildingDef.SceneLayer);
			var def = buildingConfig.BuildingDef;

			GameObject building = null;

			if (CanReplaceExistingBuilding(cellParam, out var replacementCandidate)
				&& BlueprintState.InstantBuild)
			{
				return InstantBuildReplace(cellParam, positionCbc, replacementCandidate);
			}
			else
				building = CreateFinishedBuildingInternal(cellParam, positionCbc);

			if (building == null)
			{
				SgtLogger.warning("failed to place finished building " + def.PrefabID);
				return false;
			}
			ApplyBuildingData(building);
			return true;
		}

		public virtual bool PlacePlannedBuilding(int cellParam)
		{
			var def = buildingConfig.BuildingDef;
			var orientation = RotatedOrientation;
			Vector3 positionCbc = Grid.CellToPosCBC(cellParam, def.SceneLayer);
			GameObject building = null;

			if (CanReplaceExistingBuilding(cellParam, out var replacementCandidate)
				&& !BlueprintState.InstantBuild)
			{
				building = def.TryReplaceTile(Visualizer, positionCbc, orientation, this.GetConstructionElements());
				Grid.Objects[cell, (int)def.ReplacementLayer] = building;
			}
			else
				building = def.Instantiate(positionCbc, orientation, this.GetConstructionElements());

			if (building == null)
			{
				SgtLogger.warning("failed to place planned building " + def.PrefabID);
				return false;
			}
			ApplyBuildingData(building);

			building.SetActive(true);
			return true;
		}
		protected virtual bool InstantBuildReplace(int cell, Vector3 pos, GameObject tile)
		{
			var def = buildingConfig.BuildingDef;
			var buildingOrientation = RotatedOrientation;
			var selectedElements = GetConstructionElements();

			if (def.PlacementOffsets.Length > 1)
				def.RunOnArea(cell, buildingOrientation, (offset_cell =>
				{
					if (offset_cell == cell)
						return;
					GameObject neighborTile = def.GetReplacementCandidate(offset_cell);
					if (neighborTile == null)
						return;
					if (neighborTile.TryGetComponent<SimCellOccupier>(out var sco))
						sco.DestroySelf((() => UnityEngine.Object.Destroy(neighborTile)));
					else
						UnityEngine.Object.Destroy(neighborTile);
				}));
			if (!tile.TryGetComponent<SimCellOccupier>(out var sco))
			{
				UnityEngine.Object.Destroy(tile);
				return CreateFinishedBuildingInternal(cell, pos);
			}
			sco.DestroySelf(() =>
			{
				UnityEngine.Object.Destroy(tile);
				var builtTile = CreateFinishedBuildingInternal(cell, pos);
			});
			return true;
		}

		///this has issues with tiles and conduits; dont use.
		protected virtual bool CanReplaceExistingBuilding(int cell, out GameObject replacementCandidate)
		{

			replacementCandidate = null;
			var def = buildingConfig.BuildingDef;
			bool replacementLayerOccupied = false;
			return false;

			if (ValidCell(cell, out bool isReplacement) && isReplacement)
			{
				replacementCandidate = def.GetReplacementCandidate(cell);
				def.RunOnArea(cell, RotatedOrientation, (offset_cell =>
				{
					if (!def.IsReplacementLayerOccupied(offset_cell))
						return;
					replacementLayerOccupied = true;
				}));
			}
			else
				return false;

			if (replacementLayerOccupied || replacementCandidate == null)
				return false;

			bool allowedToReplace = false;
			if (replacementCandidate.TryGetComponent<BuildingComplete>(out var repBuildingComplete))
			{
				Tag primaryReplaceElement = replacementCandidate.GetComponent<PrimaryElement>().Element.tag;
				if (primaryReplaceElement == SimHashes.StableSnow.CreateTag())
					primaryReplaceElement = SimHashes.Snow.CreateTag();

				allowedToReplace = repBuildingComplete.Def.Replaceable && def.CanReplace(replacementCandidate) && (repBuildingComplete.Def != def || GetConstructionElements()[0] != primaryReplaceElement);
			}

			return allowedToReplace;

		}

		public virtual bool TryForceRebuild(int cellParam)
		{
			var visType = ModAssets.GetVisualizerType(BuildingDef);
			var prefab = visType switch
			{
				VisualizerType.TILE => Assets.GetPrefab(ReplacementVisualizerMultiEntityConfig.TILE_ID),
				VisualizerType.UTILITY => Assets.GetPrefab(ReplacementVisualizerMultiEntityConfig.UTILITY_ID),
				_ => Assets.GetPrefab(ReplacementVisualizerMultiEntityConfig.BUILDING_ID),
			};
			var def = buildingConfig.BuildingDef;
			var orientation = RotatedOrientation;
			Vector3 positionCbc = Grid.CellToPosCBC(cellParam, def.SceneLayer);
			var overrider = Util.KInstantiate(prefab, positionCbc);
			var vis = overrider.GetComponent<ReplacementVis>();

			int flags = -1;
			if (buildingConfig.GetConduitFlags(out var conduitFlags))
				flags = GetRotatedUtilityConnectionFlags(conduitFlags);

			vis.Configure(cellParam, buildingConfig, RotatedOrientation, this.GetConstructionElements(), flags, _playerId);
			vis.gameObject.SetActive(true);
			return true;
		}

		public virtual bool TryReconstructExistingBuilding(int cellParam)
		{
			if (CanRebuildWithMaterial(cellParam, out var reconstructable))
			{
				reconstructable.RequestReconstruct(buildingConfig.SelectedElements[0]);
				ApplyBuildingData(reconstructable.gameObject, false);
				return true;
			}
			else if (reconstructable != null && reconstructable.gameObject != null)
			{
				ApplyBuildingData(reconstructable.gameObject, false);
			}
			return false;
		}

		public virtual bool SameBuildingAlreadyFinishedInPlace(int cellParam, out BuildingComplete bc, bool excludeConduits)
		{
			bc = null;
			var def = buildingConfig.BuildingDef;
			var existingBuilding = Grid.Objects[cellParam, (int)def.ObjectLayer];
			if (existingBuilding != null && existingBuilding.TryGetComponent<BuildingComplete>(out bc))
			{
				//is same def AND the building cell is aligned with the visualizer cell (aka the building is in the exact same spot as the vis.)
				if (bc.Def == def && Grid.PosToCell(existingBuilding) == cellParam)
				{
					if (excludeConduits)
						return !bc.TryGetComponent<IHaveUtilityNetworkMgr>(out _);

					return true;
				}
			}
			return false;
		}
		public virtual bool CanApplyConduitSettings(int cellParam)
		{
			if (!SameBuildingAlreadyFinishedInPlace(cellParam, out var otherConduit, false))
				return false;
			if (otherConduit.TryGetComponent<IHaveUtilityNetworkMgr>(out var mng) && buildingConfig.GetConduitFlags(out var ownFlags))
			{
				var manager = mng.GetNetworkManager();
				var current = (int)manager.GetDisplayConnections(cellParam);
				return current != ownFlags;
			}
			return false;
		}

		public virtual bool CanForceRebuild(int cellParam)
		{
			bool allowed = BlueprintState.CurrentStateInfo(_playerId).ForceBuild && AllowedInWorld() && HasTech();
			if (!allowed)
				return false;

			if (SameBuildingAlreadyFinishedInPlace(cellParam, out var bc, false))
			{
				if (bc.TryGetComponent<PrimaryElement>(out var e) && e.Element.tag == GetConstructionElements()[0])
					return false;
			}
			return allowed;
		}

		public virtual bool CanRebuildWithMaterial(int cellParam, out Reconstructable reconstructable)
		{
			reconstructable = null;
			var def = buildingConfig.BuildingDef;
			if (SameBuildingAlreadyFinishedInPlace(cellParam, out var bc, false))
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
			if (!Grid.IsValidCell(cellParam))
				return false;
			if (BlueprintState.InstantBuild && ValidCell(cellParam, out _) && AllowedInWorld()) //sandbox insta build
			{
				BuildingDef.RunOnArea(cell, RotatedOrientation, offset_cell =>
				{
					if (Grid.IsSolidCell(offset_cell) && !Grid.Foundation[offset_cell])
						SimMessages.Dig(offset_cell, skipEvent: true, backwall: false);
				});

				if (BuildingDef.ObjectLayer == ObjectLayer.Building)
					BuildingDef.RunOnArea(cell, RotatedOrientation, offset_cell =>
					{
						if (!Uprootable.CanUproot(Grid.Objects[offset_cell, (int)this.BuildingDef.ObjectLayer], out Uprootable uprootable))
							return;
						uprootable.CompleteWork((WorkerBase)null);
					});
				else if (BuildingDef.ObjectLayer == ObjectLayer.Backwall)
					BuildingDef.RunOnArea(cell, RotatedOrientation, offset_cell =>
					{
						if (!BackwallManager.HasBackwall(offset_cell))
							return;
						SimMessages.Dig(offset_cell, skipEvent: true, backwall: true);
					});
				return PlaceFinishedBuilding(cellParam);
			}
			else if (IsPlaceable(cellParam)) //regular placing
			{
				return PlacePlannedBuilding(cellParam);
			}
			else if (CanForceRebuild(cellParam)) //force rebuild over existing
			{
				return TryForceRebuild(cellParam);
			}
			//else if (BlueprintState.ForceBuild && CanRebuildWithMaterial(cellParam, out _)) //force rebuild with new materials
			//{
			//	return TryReconstructExistingBuilding(cellParam);
			//}
			else if (CurrentStateInfo(_playerId).ApplySettingsToExistingBuildings && (SameBuildingAlreadyFinishedInPlace(cellParam, out var bc, true) || CanApplyConduitSettings(cellParam))) //apply building settings to existing, does not apply to conduits
			{
				ApplyBuildingData(bc.gameObject, false);
				if (buildingConfig.HasAnyBuildingData)
				{
					PopFXManager.Instance.SpawnFX(ModAssets.BLUEPRINTS_APPLY_SETTINGS_SPRITE, STRINGS.UI.TOOLS.USE_TOOL.SETTINGS_APPLIED, null, offset: Grid.CellToPos(cellParam), Config.Instance.FXTime);
				}
				return true;
			}

			return false;
		}
		//public virtual void ClearTilePreview(int cell)
		//{
		//	var def = buildingConfig.BuildingDef;

		//	if (!Grid.IsValidBuildingCell(cell) || !def.IsTilePiece)
		//		return;
		//	GameObject tileLayerObject = Grid.Objects[cell, (int)def.TileLayer];
		//	if (Visualizer == tileLayerObject)
		//		Grid.Objects[cell, (int)def.TileLayer] = null;
		//	if (!def.isKAnimTile)
		//		return;
		//	GameObject replacementLayerObject = null;
		//	if (def.ReplacementLayer != ObjectLayer.NumLayers)
		//		replacementLayerObject = Grid.Objects[cell, (int)def.ReplacementLayer];
		//	if (tileLayerObject != null && !tileLayerObject.TryGetComponent<Constructable>(out _) || !(replacementLayerObject == null) && !replacementLayerObject != Visualizer)
		//		return;
		//	Grid.Objects[cell, (int)def.ReplacementLayer] = null;

		//	CustomTileRenderer.RemoveTileBlock(GetPlayerId(), def, false, SimHashes.Void, cell);
		//	CustomTileRenderer.RemoveTileBlock(GetPlayerId(), def, true, SimHashes.Void, cell);
		//	CustomTileRenderer.RefreshCell(GetPlayerId(), cell, def.TileLayer, def.ReplacementLayer);
		//}

		/// <summary>
		/// experiment to allow replace building over stuff based on regular build tool, not in use.
		/// </summary>
		/// <param name="cellParam"></param>
		/// <returns></returns>
		//public virtual bool TryBuild(int cellParam)
		//{
		//	ClearTilePreview(cellParam);
		//	Vector3 posCbc = Grid.CellToPosCBC(cellParam, Grid.SceneLayer.Building);
		//	GameObject builtItem = null;
		//	var def = buildingConfig.BuildingDef;
		//	var buildingOrientation = RotatedOrientation;
		//	var selectedElements = GetConstructionElements();
		//	var visualizer = Visualizer;

		//	SgtLogger.l("Visualizer test");
		//	SgtLogger.Assert("Visualizer was null", visualizer);

		//	bool instantBuild = DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive && SandboxToolParameterMenu.instance.settings.InstantBuild;

		//	if (Grid.Objects[cellParam, (int)def.TileLayer] == Visualizer)
		//		Grid.Objects[cellParam, (int)def.TileLayer] = null;

		//	if (Grid.Objects[cellParam, (int)def.ObjectLayer] == Visualizer)
		//		Grid.Objects[cellParam, (int)def.ObjectLayer] = null;

		//	if (Grid.Objects[cellParam, (int)def.ReplacementLayer] == Visualizer)
		//		Grid.Objects[cellParam, (int)def.ReplacementLayer] = null;

		//	if (!instantBuild)
		//	{
		//		builtItem = def.TryPlace(visualizer, posCbc, buildingOrientation, selectedElements, null);
		//	}
		//	else if (def.IsValidBuildLocation(visualizer, posCbc, buildingOrientation) && def.IsValidPlaceLocation(visualizer, posCbc, buildingOrientation, out string _))
		//	{
		//		builtItem = def.Build(cell, buildingOrientation, null, selectedElements, ModAssets.GetSpawnTemperature(def, selectedElements), null, false, GameClock.Instance.GetTime());
		//	}

		//	if (builtItem == null && def.ReplacementLayer != ObjectLayer.NumLayers)
		//	{
		//		GameObject replacementCandidate = def.GetReplacementCandidate(cell);
		//		if (replacementCandidate != null && !def.IsReplacementLayerOccupied(cell))
		//		{
		//			BuildingComplete component = replacementCandidate.GetComponent<BuildingComplete>();
		//			if (component != null && component.Def.Replaceable && def.CanReplace(replacementCandidate) && (component.Def != def
		//						|| selectedElements[0] != replacementCandidate.GetComponent<PrimaryElement>().Element.tag))
		//			{
		//				if (!instantBuild)
		//				{
		//					builtItem = def.TryReplaceTile(visualizer, posCbc, buildingOrientation, selectedElements, null);
		//					Grid.Objects[cell, (int)def.ReplacementLayer] = builtItem;
		//				}
		//				else if (def.IsValidBuildLocation(visualizer, posCbc, buildingOrientation, true) && def.IsValidPlaceLocation(visualizer, posCbc, buildingOrientation, true, out string _))
		//					builtItem = InstantBuildReplace(cell, posCbc, replacementCandidate);
		//			}
		//		}
		//	}

		//	SgtLogger.Assert("builtItem", builtItem);
		//	PostProcessBuild(instantBuild, posCbc, builtItem);
		//	return builtItem != null;
		//}
		//private GameObject InstantBuildReplace(int cell, Vector3 pos, GameObject tile)
		//{
		//	var def = buildingConfig.BuildingDef;
		//	var buildingOrientation = RotatedOrientation;
		//	var selectedElements = GetConstructionElements();

		//	if (!tile.TryGetComponent<SimCellOccupier>(out var SCO))
		//	{
		//		UnityEngine.Object.Destroy(tile);
		//		return def.Build(cell, buildingOrientation, null, selectedElements, ModAssets.GetSpawnTemperature(def, selectedElements), null, false, GameClock.Instance.GetTime());
		//	}
		//	SCO.DestroySelf(() =>
		//	{
		//		UnityEngine.Object.Destroy(tile);
		//		PostProcessBuild(true, pos, def.Build(cell, buildingOrientation, null, selectedElements, ModAssets.GetSpawnTemperature(def, selectedElements), null, false, GameClock.Instance.GetTime()));
		//	});
		//	return null;
		//}

		//private void PostProcessBuild(bool instantBuild, Vector3 pos, GameObject builtItem)
		//{
		//	if (builtItem == null)
		//		return;
		//	if (!instantBuild)
		//	{
		//		Prioritizable component = builtItem.GetComponent<Prioritizable>();
		//		if (component != null)
		//		{
		//			if (ToolMenu.Instance != null)
		//				component.SetMasterPriority(ToolMenu.Instance.PriorityScreen.GetLastSelectedPriority());
		//		}
		//	}
		//	ModAPI.API_Methods.ApplyAdditionalBuildingData(builtItem, buildingConfig);

		//	if (Visualizer.TryGetComponent<KBatchedAnimController>(out var kbac))
		//	{
		//		kbac.TintColour = ModAssets.BLUEPRINTS_COLOR_INVALIDPLACEMENT;
		//		kbac.Play("place");
		//	}
		//	UpdateConduitConnectionBits(builtItem);
		//}

		public virtual bool AllowedInWorld()
		{
			return API_Methods.IsBuildable(buildingConfig.BuildingDef);
		}

		public virtual bool HasTech()
		{
			return BlueprintState.InstantBuild || !Config.Instance.RequireConstructable_Tech || Db.Get().TechItems.IsTechItemComplete(buildingConfig.BuildingDef.PrefabID);
		}
		public virtual bool ValidCell(int cellParam, out bool replacement)
		{
			replacement = false;
			var pos = Grid.CellToPos(cellParam);
			if (Grid.IsValidCellInWorld(cellParam, ClusterManager.Instance.activeWorldId)
				&& Grid.IsVisible(cellParam))
			{
				bool IsValidPlaceLocation = buildingConfig.BuildingDef.IsValidPlaceLocation(Visualizer, cellParam, RotatedOrientation, out string failReason);
				bool IgnorableFailReason =
					   failReason == global::STRINGS.UI.TOOLTIPS.HELP_BUILDLOCATION_WALL
					|| failReason == global::STRINGS.UI.TOOLTIPS.HELP_BUILDLOCATION_CORNER
					|| failReason == global::STRINGS.UI.TOOLTIPS.HELP_BUILDLOCATION_CORNER_FLOOR
					//allow "attach to backwall" buildings to be placed, but not replace already placed ones as that will place a non-cancelable visualizer
					|| (failReason == global::STRINGS.UI.TOOLTIPS.HELP_BUILDLOCATION_BACK_WALL_REQUIRED && BlueprintState.LayerOccupiedAt(this, ObjectLayer.Backwall, cellParam) && !BlueprintState.LayerOccupiedAt(this, BuildingDef.ObjectLayer, cellParam));

				//SgtLogger.l("Fail reason of " + buildingConfig.BuildingDef.name + ": " + faiReason);

				bool validCell = (IsValidPlaceLocation || IgnorableFailReason);


				//replacement = buildingConfig.BuildingDef.IsValidReplaceLocation(pos, RotatedOrientation, buildingConfig.BuildingDef.ReplacementLayer, buildingConfig.BuildingDef.ObjectLayer);
				//if (replacement)
				//	replacement = buildingConfig.BuildingDef.GetReplacementCandidate(cellParam) != null;


				return (validCell || replacement);
			}
			return false;
		}

		public virtual void UpdateRequirementsState()
		{
			API_Methods.BuildableStateValid(buildingConfig.BuildingDef, out var state);
			RequirementsState = state;
		}
		public virtual void ApplyColorIfChanged(int cellParam)
		{
			Color newColor = GetVisualizerColor(cellParam);

			//if (_lastColor.HasValue && newColor == _lastColor.Value)
			//	return;

			//_lastColor = newColor;

			if (hasKbac)
				kbac.TintColour = newColor;
		}

		public Color GetVisualizerColor(int cellParam)
		{
			Color playerColor = default;
			if (!LocalPlayerId(_playerId) && IsMultiplayerVisualizer(_playerId, ref playerColor))
				return playerColor;
			var stateInfo = BlueprintState.CurrentStateInfo(_playerId);
			UpdateRequirementsState();
			if (CanForceRebuild(cellParam))// && CanRebuildWithMaterial(cellParam, out _))
			{
				return ModAssets.BLUEPRINTS_COLOR_VALIDPLACEMENT;
			}
			else if (SameBuildingAlreadyFinishedInPlace(cellParam, out _, false))
			{
				if ((buildingConfig.HasAnyBuildingData || CanApplyConduitSettings(cellParam)) && stateInfo.ApplySettingsToExistingBuildings)
				{
					return ModAssets.BLUEPRINTS_COLOR_CAN_APPLY_SETTINGS;
				}
				else
					return ModAssets.BLUEPRINTS_COLOR_INVISIBLE;
			}
			else if (!ValidCell(cellParam, out _))
			{
				return ModAssets.BLUEPRINTS_COLOR_INVALIDPLACEMENT;
			}
			else if (!HasTech())
			{
				return ModAssets.BLUEPRINTS_COLOR_NOTECH;
			}
			else if (RequirementsState == PlanScreen.RequirementsState.Materials && Config.Instance.RequireConstructable_Material)
				return ModAssets.BLUEPRINTS_COLOR_NOMATERIALS;
			else if (!AllowedInWorld())
				return ModAssets.BLUEPRINTS_COLOR_NOTALLOWEDINWORLD;
			else
			{
				return ModAssets.BLUEPRINTS_COLOR_VALIDPLACEMENT;
			}
		}

		public virtual PermittedRotations GetAllowedRotations()
		{
			var def = buildingConfig.BuildingDef;
			if (def.isKAnimTile)
				return BlueprintTransformationInfo.All;
			else if (def.WidthInCells == 1 && def.HeightInCells == 1 &&
				(def.ObjectLayer == ObjectLayer.Backwall || def.PermittedRotations == PermittedRotations.R360 || def.BuildLocationRule == BuildLocationRule.Anywhere || def.BuildLocationRule == BuildLocationRule.NotInTiles))
				return BlueprintTransformationInfo.All;
			else if (def.WidthInCells % 2 == 1 || def.PermittedRotations == PermittedRotations.FlipH)
				return PermittedRotations.FlipH;
			else if (def.BuildingComplete.TryGetComponent<Door>(out _))
				return PermittedRotations.FlipH;

			return PermittedRotations.Unrotatable;
		}
		public virtual bool AllowedForRotation(Orientation rotation, bool flippedX, bool flippedY)
		{
			var allowed = GetAllowedRotations();
			switch (allowed)
			{
				case BlueprintTransformationInfo.All:
					return true;
				case PermittedRotations.Unrotatable:
					return false;
				case PermittedRotations.FlipH:
					return (rotation == Orientation.Neutral || rotation == Orientation.FlipH) && !flippedY;
			}
			return false;
		}
		public virtual void ApplyRotation(Orientation rotation, bool flippedX, bool flippedY)
		{
			var allowedRotations = GetAnimRotations();
			if (allowedRotations == PermittedRotations.Unrotatable)
				return;

			var def = buildingConfig.BuildingDef;
			Orientation targetRotation = buildingConfig.Orientation;
			if (Visualizer.TryGetComponent<Rotatable>(out var rotatable))
			{
				if (allowedRotations == PermittedRotations.FlipV)
				{
					targetRotation = (targetRotation == Orientation.FlipV ^ flippedY) ? Orientation.FlipV : Orientation.Neutral;
					//ApplyEvenDimensionOffset(flippedX, flippedY, false, buildingConfig.BuildingDef.HeightInCells % 2 == 0);
				}
				else if (allowedRotations == PermittedRotations.FlipH)
				{
					targetRotation = (targetRotation == Orientation.FlipH ^ flippedX) ? Orientation.FlipH : Orientation.Neutral;
					//ApplyEvenDimensionOffset(flippedX, flippedY, buildingConfig.BuildingDef.WidthInCells % 2 == 0, false);
				}
				else if (allowedRotations == PermittedRotations.R360)
				{
					int currentRota = (int)targetRotation;
					int rotationOrientation = (int)rotation;

					currentRota = (currentRota + rotationOrientation) % 4;

					bool widthLarger1 = def.WidthInCells > 1;
					bool heightLarger1 = def.HeightInCells > 1;

					bool rotaFlipX = widthLarger1 && !heightLarger1 && (currentRota % 2 == 0) || heightLarger1 && !widthLarger1 && (currentRota % 2 != 0);
					bool rotaFlipY = heightLarger1 && !widthLarger1 && (currentRota % 2 == 0) || widthLarger1 && !heightLarger1 && (currentRota % 2 != 0);

					if (flippedX && rotaFlipX)
						currentRota += 2;
					if (flippedY && rotaFlipY)
						currentRota += 2;

					currentRota = currentRota % 4;


					///this would be the proper flip logic if drywalls had unified orientation - but they dont
					//int flipModX = 4;
					//if (flippedX)
					//{
					//	if (currentRota % 2 == 0)
					//	{
					//		flipModX += 1;
					//	}
					//	else
					//	{
					//		flipModX -= 1;
					//	}
					//}
					//currentRota = (currentRota+  flipModX) % 4;
					//int flipModY = 4;

					//if (flippedY)
					//{
					//	if (currentRota % 2 == 0)
					//	{
					//		flipModY -= 1;
					//	}
					//	else
					//	{
					//		flipModY += 1;
					//	}
					//}
					//currentRota = (currentRota + flipModY) % 4;

					targetRotation = (Orientation)currentRota;

					//SgtLogger.l(flippedX+"-"+ def.Tag.ToString() + " - r360; old: " + buildingConfig.Orientation + ", rotated: " + targetRotation);
					//ApplyEvenDimensionOffset(flippedX, flippedY, (currentRota % 2 != 0 && def.HeightInCells % 2 == 0), (currentRota % 2 == 0 && def.WidthInCells % 2 == 0));
				}
				//else if (allowedRotations == PermittedRotations.R90)
				//{
				//	bool isRotated = baseOrientation == Orientation.R90;
				//	if (isRotated)
				//	{
				//	}

				//	var rotationOrientation = (int)rotation;
				//	switch (rotation)
				//	{
				//		case Orientation.Neutral:
				//		case Orientation.R90:
				//			rotationOrientation = (int)rotation;
				//			break;
				//		case Orientation.R180:
				//			rotationOrientation = (int)Orientation.Neutral;
				//			flippedY = !flippedY;
				//			break;
				//		case Orientation.R270:
				//			rotationOrientation = (int)Orientation.R90;
				//			flippedY = !flippedY;
				//			flippedX = !flippedX;
				//			break;
				//	}
				//	if (isRotated)
				//		rotationOrientation++;

				//	rotationOrientation = rotationOrientation % 2;
				//	baseOrientation = (Orientation)rotationOrientation;
				//}
				rotatable.SetOrientation(targetRotation);

				if (buildingConfig.BuildingDef.PermittedRotations == PermittedRotations.R90)
				{
					//if the door has an even number of cells, it will need to have its offset adjusted by one, axis depending on the natural state of the door

					//bool evenWidth = def.WidthInCells % 2 == 0 && def.HeightInCells == 1;
					//bool evenHeight = def.HeightInCells % 2 == 0 && def.WidthInCells == 1;

					//bunker doors are rotated in their natural, so they need reversing of the rotation state
					bool isRotatedToHorizontal = def.WidthInCells > 1 ? rotatable.Orientation == Orientation.Neutral : rotatable.Orientation == Orientation.R90;
					bool isRotatedToVertical = !isRotatedToHorizontal;

					//SgtLogger.l(def.PrefabID + ": rotationstate: " + rotatable.orientation + ", ishorizontal: " + isRotatedToHorizontal);
					ApplyEvenDimensionOffset(flippedX, flippedY, isRotatedToHorizontal, isRotatedToVertical);
				}
			}
			FlippedV = flippedY;
			FlippedH = flippedX;
			RotatedOrientation = targetRotation;


			//if (buildingConfig.BuildingDef.WidthInCells % 2 == 0 && flippedX != wasFlippedX)
			//{
			//	wasFlippedX = flippedX;

			//	Offset = new(Offset.X + (flippedX ? -1 : 1), Offset.Y);
			//	//MoveVisualizer(cell, true);
			//}
			//int height = buildingConfig.BuildingDef.HeightInCells;
			//if (height > 1 && flippedY != wasFlippedY)
			//{
			//	wasFlippedY = flippedY;
			//	int offsetCells = height - 1;


			//	Offset = new(Offset.X, Offset.Y + (flippedY ? offsetCells : -offsetCells));
			//	//MoveVisualizer(cell, true);
			//}
		}

		void ApplyEvenDimensionOffset(bool flippedX, bool flippedY, bool isAffectedH, bool isAffectedV)
		{
			int xOffset = 0, yOffset = 0;
			if (FlippedH != flippedX && isAffectedH)
			{
				xOffset = flippedX ? 1 : -1;
			}
			if (FlippedV != flippedY && isAffectedV)
			{
				yOffset = flippedY ? -1 : 1;
			}
			//SgtLogger.l(buildingConfig.BuildingDef.Tag + $": flippedX: {flippedX} flippedY: {flippedY}, offsets: ({xOffset},{yOffset})");
			Offset = new(Offset.X + xOffset, Offset.Y + yOffset);
		}


		public virtual PermittedRotations GetAnimRotations()
		{
			var allowedRotations = buildingConfig.BuildingDef.PermittedRotations;
			if (buildingConfig.BuildingDef.isKAnimTile)
				return PermittedRotations.R360;

			bool higherThan1 = buildingConfig.BuildingDef.HeightInCells > 1,
				  widerThan1 = buildingConfig.BuildingDef.WidthInCells > 1;

			if (higherThan1 && !widerThan1 && allowedRotations == PermittedRotations.Unrotatable)
				return PermittedRotations.FlipH;


			return allowedRotations;
		}

		public void DestroyVisualizer()
		{
			if (Visualizer.TryGetComponent<LogicPorts>(out var ports))
			{
				ports.DestroyVisualizers();
			}
			UnityEngine.Object.Destroy(Visualizer);
		}
		public void SpawnDestroyedByForceTransformFx()
		{
			PopFXManager.Instance.SpawnFX(Assets.GetSprite("icon_action_cancel"), string.Format(FORCETRANSFORMATIONTOGGLE.FX_TEXT,BuildingDef.Name), null, offset: Grid.CellToPos(cell), Config.Instance.FXTime);
		}
	}
}
