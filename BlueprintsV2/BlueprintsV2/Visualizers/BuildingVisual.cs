
using BlueprintsV2.BlueprintData;
using BlueprintsV2.BlueprintsV2.Visualizers.ReplacementVisualizers;
using BlueprintsV2.ModAPI;
using System.Collections;
using System.Collections.Generic;
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

		protected readonly BuildingConfig _buildingConfig;

		public readonly BuildingDef _def;

		public Orientation RotatedOrientation { get; protected set; }
		public bool FlippedV { get; protected set; }
		public bool FlippedH { get; protected set; }

		public string BuildingID => _def.PrefabID;
		protected ulong _playerId = BlueprintState.PlayerId_DefaultTilePreviews;
		protected KBatchedAnimController kbac;
		protected bool hasKbac = false;
		protected bool isTile = false;
		protected IUtilityNetworkMgr _networkMgr = null;
		//protected Color? _lastColor = null;

		public BuildingVisual(BuildingConfig buildingConfig, int cell, ulong playerId)
		{
			this._playerId = playerId;
			Offset = buildingConfig.Offset;
			RotatedOrientation = buildingConfig.Orientation;
			this._buildingConfig = buildingConfig;
			this._def = buildingConfig.BuildingDef;
			this.cell = cell;
			isTile = _def.isKAnimTile && _def.BlockTileAtlas;

			CreateVisualizer();
			SetLayer();
			ApplyColorIfChanged(cell);
			UpdateRequirementsState();
		}

		protected virtual void CreateVisualizer()
		{
			Vector3 positionCbc = Grid.CellToPosCBC(cell, _def.SceneLayer);
			Visualizer = GameUtil.KInstantiate(_def.BuildingPreview, positionCbc, Grid.SceneLayer.Front, "BlueprintModBuildingVisualizer", LayerMask.NameToLayer("Place"));
			Visualizer.transform.SetPosition(positionCbc);
			hasKbac = Visualizer.TryGetComponent<KBatchedAnimController>(out kbac);
			if (hasKbac)
			{
				//set visibility type before activating GO, otherwise it will get culled when its original chunk goes offscreen.
				kbac.visibilityType = KAnimControllerBase.VisibilityType.Always;
				kbac.isMovable = true;
				kbac.Offset = _def.GetVisualizerOffset();
			}
			Visualizer.SetActive(true);
			if (Visualizer.TryGetComponent<Rotatable>(out var rotatable))
			{
				rotatable.SetOrientation(RotatedOrientation);
			}
			ModAPI.API_Methods.ApplyAdditionalBuildingData(Visualizer, _buildingConfig, _playerId);
		}
		protected void SetLayer()
		{
			if (hasKbac)
			{
				kbac.SetLayer(LayerMask.NameToLayer("Place"));
				kbac.Play("place");
			}
			else
			{
				Visualizer.SetLayerRecursively(LayerMask.NameToLayer("Place"));
			}
		}

		///relevant for rendering visualizers in the multiplayer mod integration
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
				Visualizer.transform.SetPosition(Grid.CellToPosCBC(cellParam, _def.SceneLayer));
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
			var ingredients = _def.CraftRecipe.Ingredients;
			var elements = new List<Tag>(_buildingConfig.SelectedElements.Count);
			for (int i = 0; i < ingredients.Count; ++i)
			{
				var ingredient = ingredients[i];
				Tag selectedElement;
				if (i < _buildingConfig.SelectedElements.Count)
				{
					selectedElement = _buildingConfig.SelectedElements[i];
				}
				else
				{
					//should never happen, just in case to prevent crash.
					selectedElement = ModAssets.GetFirstAvailableMaterial(ingredient.tag, ingredient.amount);
				}
				var key = BlueprintSelectedMaterial.GetBlueprintSelectedMaterial(selectedElement, ingredient.tag, _def.PrefabID);

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
		//		return (component.Def.Replaceable &&_buildingDef.CanReplace(toReplace) && (component.Def !=_buildingDef || GetConstructionElements()[0] != component.GetComponent<PrimaryElement>().Element.tag));
		//	}
		//	return false;
		//}

		//bool ReplacementLayerOccupied(int cellParam)
		//{
		//	var def =_buildingDef;
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

			var def = _def;

			if (isPlanned && buildingUnderConstruction.Def != def)
				return;
			if (isComplete && buildingComplete.Def != def)
				return;

			if (building.TryGetComponent<Rotatable>(out var rotatable) && rotatable.Orientation != RotatedOrientation)
			{
				rotatable.SetOrientation(RotatedOrientation);
			}
			ModAPI.API_Methods.ApplyAdditionalBuildingData(building, _buildingConfig, _playerId);

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
			int originalRotation = (int)_buildingConfig.Orientation; //0-3;
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
			if (_networkMgr != null
				&& go.TryGetComponent<KAnimGraphTileVisualizer>(out var vis)
				&& _buildingConfig.GetConduitFlags(out var flags))
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
			var def = _def;
			var selectedElements = GetConstructionElements();
			var finishedBuilding = def.Create(positionCbc, null, GetConstructionElements(), def.CraftRecipe, ModAssets.GetSpawnTemperature(def, selectedElements), def.BuildingComplete);

			if (finishedBuilding == null)
			{
				SgtLogger.warning("failed to place finished building " + def.PrefabID);
				return null;
			}
			ApplyBuildingData(finishedBuilding);

			def.MarkArea(cellParam, RotatedOrientation, def.ObjectLayer, finishedBuilding);
			if (def.IsTilePiece && def.BlockTileAtlas != null)
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
			Vector3 positionCbc = Grid.CellToPosCBC(cellParam, _def.SceneLayer);
			var def = _def;

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
			var def = _def;
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
			var def = _def;
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
			var def = _def;
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
			var visType = ModAssets.GetVisualizerType(_def);
			var prefab = visType switch
			{
				VisualizerType.TILE => Assets.GetPrefab(ReplacementVisualizerMultiEntityConfig.TILE_ID),
				VisualizerType.UTILITY => Assets.GetPrefab(ReplacementVisualizerMultiEntityConfig.UTILITY_ID),
				_ => Assets.GetPrefab(ReplacementVisualizerMultiEntityConfig.BUILDING_ID),
			};
			var def = _def;
			var orientation = RotatedOrientation;
			Vector3 positionCbc = Grid.CellToPosCBC(cellParam, def.SceneLayer);
			var overrider = Util.KInstantiate(prefab, positionCbc);
			var vis = overrider.GetComponent<ReplacementVis>();

			int flags = -1;
			if (_buildingConfig.GetConduitFlags(out var conduitFlags))
				flags = GetRotatedUtilityConnectionFlags(conduitFlags);

			vis.Configure(cellParam, _buildingConfig, RotatedOrientation, this.GetConstructionElements(), flags, _playerId);
			vis.gameObject.SetActive(true);
			return true;
		}

		public virtual bool TryReconstructExistingBuilding(int cellParam)
		{
			if (CanRebuildWithMaterial(cellParam, out var reconstructable))
			{
				reconstructable.RequestReconstruct(_buildingConfig.SelectedElements[0]);
				ApplyBuildingData(reconstructable.gameObject, false);
				return true;
			}
			else if (reconstructable != null && reconstructable.gameObject != null)
			{
				ApplyBuildingData(reconstructable.gameObject, false);
			}
			return false;
		}

		public virtual bool SameBuildingAlreadyFinishedInPlace(int cellParam, out Building building, bool excludeConduits, bool includePlanned)
		{
			building = null;
			var def = _def;
			var existingBuilding = Grid.Objects[cellParam, (int)def.ObjectLayer];
			if (existingBuilding == null)
				return false;

			if (existingBuilding.TryGetComponent<Building>(out building))
			{
				if (building is not BuildingComplete && !includePlanned)
					return false;

				//is same def AND the building cell is aligned with the visualizer cell (aka the building is in the exact same spot as the vis.)
				if (building.Def == def && Grid.PosToCell(existingBuilding) == cellParam)
				{
					bool isDrywall = def.ObjectLayer == ObjectLayer.Backwall && def.WidthInCells == 1 && def.HeightInCells == 1;
					bool hasSameRotation = (!existingBuilding.TryGetComponent<Rotatable>(out var rota) || rota.Orientation == RotatedOrientation);

					//take rotation in consideration, unless its drywall
					if (!isDrywall && !hasSameRotation)
						return false;

					if (excludeConduits)
						return !building.TryGetComponent<IHaveUtilityNetworkMgr>(out _);

					return true;
				}
			}
			return false;
		}

		public virtual bool CanApplyConduitSettings(int cellParam)
		{
			if (!SameBuildingAlreadyFinishedInPlace(cellParam, out var otherConduit, false, true))
				return false;
			if (otherConduit.TryGetComponent<IHaveUtilityNetworkMgr>(out var mng) && _buildingConfig.GetConduitFlags(out var ownFlags))
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

			if (SameBuildingAlreadyFinishedInPlace(cellParam, out var bc, false, true))
			{
				if (bc.TryGetComponent<PrimaryElement>(out var e) && e.Element.tag == GetConstructionElements()[0])
					return false;
			}
			return allowed;
		}

		public virtual bool CanRebuildWithMaterial(int cellParam, out Reconstructable reconstructable)
		{
			reconstructable = null;
			var def = _def;
			if (SameBuildingAlreadyFinishedInPlace(cellParam, out var bc, false, false))
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
				_def.RunOnArea(cell, RotatedOrientation, offset_cell =>
				{
					if (Grid.IsSolidCell(offset_cell) && !Grid.Foundation[offset_cell])
						SimMessages.Dig(offset_cell, skipEvent: true, backwall: false);
				});

				if (_def.ObjectLayer == ObjectLayer.Building)
					_def.RunOnArea(cell, RotatedOrientation, offset_cell =>
					{
						if (!Uprootable.CanUproot(Grid.Objects[offset_cell, (int)this._def.ObjectLayer], out Uprootable uprootable))
							return;
						uprootable.CompleteWork((WorkerBase)null);
					});
				else if (_def.ObjectLayer == ObjectLayer.Backwall)
					_def.RunOnArea(cell, RotatedOrientation, offset_cell =>
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
			else if (CurrentStateInfo(_playerId).ApplySettingsToExistingBuildings && (SameBuildingAlreadyFinishedInPlace(cellParam, out var bc, true, true) || CanApplyConduitSettings(cellParam))) //apply building settings to existing, does not apply to conduits
			{
				ApplyBuildingData(bc.gameObject, false);
				if (_buildingConfig.HasAnyBuildingData)
				{
					PopFXManager.Instance.SpawnFX(ModAssets.BLUEPRINTS_APPLY_SETTINGS_SPRITE, STRINGS.UI.TOOLS.USE_TOOL.SETTINGS_APPLIED, null, offset: Grid.CellToPos(cellParam), Config.Instance.FXTime);
				}
				return true;
			}

			return false;
		}
		//public virtual void ClearTilePreview(int cell)
		//{
		//	var def =_buildingDef;

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
		//	var def =_buildingDef;
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
		//	var def =_buildingDef;
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
			return API_Methods.IsBuildable(_def);
		}

		public virtual bool HasTech()
		{
			return BlueprintState.InstantBuild || !Config.Instance.RequireConstructable_Tech || Db.Get().TechItems.IsTechItemComplete(_def.PrefabID);
		}

		static HashSet<string> _ignorableFailReasons = null;
		bool failReasonInit = false;
		public virtual bool IgnorableFailReason(int cellParam, string failReason)
		{
			if (!failReasonInit)
			{
				_ignorableFailReasons = [
					global::STRINGS.UI.TOOLTIPS.HELP_BUILDLOCATION_WALL,
					global::STRINGS.UI.TOOLTIPS.HELP_BUILDLOCATION_CORNER,
					global::STRINGS.UI.TOOLTIPS.HELP_BUILDLOCATION_CORNER_FLOOR
					];
				failReasonInit = true;
			}
			if (_ignorableFailReasons.Contains(failReason))
				return true;

			if (failReason == global::STRINGS.UI.TOOLTIPS.HELP_BUILDLOCATION_BACK_WALL_REQUIRED)
			{
				bool ignorethisFail = true;
				_def.RunOnArea(cellParam, RotatedOrientation, (occupies =>
				{
					if (!BlueprintState.LayerOccupiedAt(this, ObjectLayer.Backwall, occupies)
						|| BlueprintState.LayerOccupiedAt(this, _def.ObjectLayer, occupies))
						ignorethisFail = false;

				}));
				return ignorethisFail;
			}
			return false;
		}

		public virtual bool ValidCell(int cellParam, out bool replacement)
		{
			replacement = false;
			var pos = Grid.CellToPos(cellParam);
			if (Grid.IsValidCellInWorld(cellParam, ClusterManager.Instance.activeWorldId)
				&& Grid.IsVisible(cellParam))
			{
				bool IsValidPlaceLocation = _def.IsValidPlaceLocation(Visualizer, cellParam, RotatedOrientation, out string failReason);
				//SgtLogger.l("Fail reason of " +_buildingDef.name + ": " + faiReason);

				bool validCell = (IsValidPlaceLocation || IgnorableFailReason(cellParam, failReason));


				//replacement =_buildingDef.IsValidReplaceLocation(pos, RotatedOrientation,_buildingDef.ReplacementLayer,_buildingDef.ObjectLayer);
				//if (replacement)
				//	replacement =_buildingDef.GetReplacementCandidate(cellParam) != null;


				return (validCell || replacement);
			}
			return false;
		}

		public virtual void UpdateRequirementsState()
		{
			API_Methods.BuildableStateValid(_def, out var state);
			RequirementsState = state;
		}
		public virtual void ApplyColorIfChanged(int cellParam)
		{
			if (hasKbac)
			{
				kbac.TintColour = GetVisualizerColor(cellParam);
			}
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
			else if (SameBuildingAlreadyFinishedInPlace(cellParam, out _, false, true))
			{
				if ((_buildingConfig.HasAnyBuildingData || CanApplyConduitSettings(cellParam)) && stateInfo.ApplySettingsToExistingBuildings)
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
			var def = _def;
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

			var def = _def;
			Orientation targetRotation = _buildingConfig.Orientation;
			if (Visualizer.TryGetComponent<Rotatable>(out var rotatable))
			{
				if (allowedRotations == PermittedRotations.FlipV)
				{
					targetRotation = (targetRotation == Orientation.FlipV ^ flippedY) ? Orientation.FlipV : Orientation.Neutral;
					//ApplyEvenDimensionOffset(flippedX, flippedY, false,_buildingDef.HeightInCells % 2 == 0);
				}
				else if (allowedRotations == PermittedRotations.FlipH)
				{
					targetRotation = (targetRotation == Orientation.FlipH ^ flippedX) ? Orientation.FlipH : Orientation.Neutral;
					//ApplyEvenDimensionOffset(flippedX, flippedY,_buildingDef.WidthInCells % 2 == 0, false);
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

				if (_def.PermittedRotations == PermittedRotations.R90)
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
			//int height =_buildingDef.HeightInCells;
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
			//SgtLogger.l(_buildingDef.Tag + $": flippedX: {flippedX} flippedY: {flippedY}, offsets: ({xOffset},{yOffset})");
			Offset = new(Offset.X + xOffset, Offset.Y + yOffset);
		}


		public virtual PermittedRotations GetAnimRotations()
		{
			var allowedRotations = _def.PermittedRotations;
			if (_def.isKAnimTile)
				return PermittedRotations.R360;

			bool higherThan1 = _def.HeightInCells > 1,
				  widerThan1 = _def.WidthInCells > 1;

			if (higherThan1 && !widerThan1 && allowedRotations == PermittedRotations.Unrotatable)
				return PermittedRotations.FlipH;


			return allowedRotations;
		}

		public virtual void DestroyVisualizer()
		{
			if (Visualizer.TryGetComponent<LogicPorts>(out var ports))
			{
				ports.DestroyVisualizers();
			}
			UnityEngine.Object.Destroy(Visualizer);
		}
		public void SpawnDestroyedByForceTransformFx()
		{
			PopFXManager.Instance.SpawnFX(Assets.GetSprite("icon_action_cancel"), string.Format(FORCETRANSFORMATIONTOGGLE.FX_TEXT, _def.Name), null, offset: Grid.CellToPos(cell), Config.Instance.FXTime);
		}
	}
}
