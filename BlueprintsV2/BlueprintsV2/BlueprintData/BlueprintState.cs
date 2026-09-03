using BlueprintsV2.BlueprintsV2.BlueprintData;
using BlueprintsV2.BlueprintsV2.BlueprintData.NoteToolPlacedEntities;
using BlueprintsV2.BlueprintsV2.BlueprintData.OniTogether_Integration;
using BlueprintsV2.BlueprintsV2.BlueprintData.OniTogether_Integration.Packets;
using BlueprintsV2.BlueprintsV2.BlueprintData.PlannedElements;
using BlueprintsV2.BlueprintsV2.BlueprintData.PlanningToolMod_Integration;
using BlueprintsV2.BlueprintsV2.BlueprintData.PlanningToolMod_Integration.EnumMirrors;
using BlueprintsV2.BlueprintsV2.Visualizers;
using BlueprintsV2.BlueprintsV2.Visualizers.CustomTileRenderer;
using BlueprintsV2.ModAPI;
using BlueprintsV2.Tools;
using BlueprintsV2.Visualizers;
using Epic.OnlineServices.Sessions;
using ONI_Together_API;
using ONI_Together_API.Networking;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UtilLibs;
using static STRINGS.BUILDING.STATUSITEMS;
using static STRINGS.DUPLICANTS.CHORES;
using static UnityEngine.UI.Image;

namespace BlueprintsV2.BlueprintData
{

	public static class BlueprintState
	{
		static BlueprintState()
		{
			OccupiedCells[PlayerId_DefaultTilePreviews] = new();
			foreach (ObjectLayer objectLayer in Enum.GetValues(typeof(ObjectLayer)))
				OccupiedCells[PlayerId_DefaultTilePreviews][objectLayer] = new();

			FoundationVisuals[PlayerId_DefaultTilePreviews] = new();
			DependentVisuals[PlayerId_DefaultTilePreviews] = new();
			CleanableVisuals[PlayerId_DefaultTilePreviews] = new();
			ColoredCells[PlayerId_DefaultTilePreviews] = new();
			NormalPlayer = PlayerBlueprintStateInfos[PlayerId_DefaultTilePreviews] = new BlueprintTransformationInfo();
		}


		public static void ToggleHotkeyTooltips() => ExtendedCardTooltips = !ExtendedCardTooltips;
		public static bool ExtendedCardTooltips { get; private set; } = true;

		public static string SelectedBlueprintFolder = string.Empty;

		public static bool InstantBuild => DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive && SandboxToolParameterMenu.instance.settings.InstantBuild;

		private static readonly Dictionary<ulong, Blueprint> CurrentVisualizers = new();
		private static readonly Dictionary<ulong, List<IVisual>> FoundationVisuals = new();
		private static readonly Dictionary<ulong, List<IVisual>> DependentVisuals = new();
		private static readonly Dictionary<ulong, List<ICleanableVisual>> CleanableVisuals = new();

		public static readonly Dictionary<ulong, Dictionary<ObjectLayer, Dictionary<int, BuildingVisual>>> OccupiedCells = new();

		public static readonly Dictionary<ulong, Dictionary<int, Color>> ColoredCells = new();

		private static BlueprintTransformationInfo NormalPlayer = new BlueprintTransformationInfo();
		static readonly Dictionary<ulong, Color> PlayerColorsCached = [];

		#region MP_Integration

		public const ulong PlayerId_DefaultTilePreviews = ulong.MinValue;
		public const ulong PlayerId_ReplacementTiles = ulong.MaxValue;

		//static readonly Dictionary<Blueprint, ulong> BlueprintPlayerIdMap = [];
		static readonly Dictionary<ulong, BlueprintTransformationInfo> PlayerBlueprintStateInfos = [];

		public static void AddCachesForPlayer(ulong id)
		{
			SgtLogger.l("Adding Blueprint Caches for " + id);
			if (OccupiedCells.ContainsKey(id))
				SgtLogger.warning(id + " was already cached");

			OccupiedCells[id] = [];
			foreach (ObjectLayer objectLayer in Enum.GetValues(typeof(ObjectLayer)))
				OccupiedCells[id][objectLayer] = new();
			FoundationVisuals[id] = new();
			DependentVisuals[id] = new();
			CleanableVisuals[id] = new();
			ColoredCells[id] = new();
			var info = new BlueprintTransformationInfo(id);
			PlayerBlueprintStateInfos[id] = info;

		}
		public static void CachePlayerColor(ulong id)
		{
			SgtLogger.l("Caching Blueprint Player color for " + id);
			if (SessionInfoAPI.TryGetPlayerColor(id, out var playerColor))
				PlayerColorsCached[id] = playerColor;
			else
			{
				SgtLogger.warning("Could not cache player color for " + id);
				PlayerColorsCached[id] = UIUtils.GetRandomRainbowColor(true);
			}
			SgtLogger.l("Color cached: " + PlayerColorsCached[id]);
		}
		public static void RemoveCachesForPlayer(ulong id)
		{
			SgtLogger.l("Removing Blueprint Caches for " + id);
			ClearVisuals(id);
			OccupiedCells.Remove(id);
			FoundationVisuals.Remove(id);
			DependentVisuals.Remove(id);
			CleanableVisuals.Remove(id);
			ColoredCells.Remove(id);
			PlayerColorsCached.Remove(id);
		}
		public static BlueprintTransformationInfo CurrentStateInfo(ulong id = PlayerId_DefaultTilePreviews)
		{
			if (!PlayerBlueprintStateInfos.TryGetValue(id, out var info))
				return NormalPlayer;
			return info;
		}

		static bool ShouldFetchPlayerCursorPos(ulong playerId, out Vector3 otherPlayerCursorPos)
		{
			otherPlayerCursorPos = default;

			if (LocalPlayerId(playerId))
				return false;

			if (!MP_Helpers.MPInstalledAndActive())
				return false;

			return SessionInfoAPI.TryGetPlayerCursorPos(playerId, out otherPlayerCursorPos);

		}

		public static bool LocalPlayerId(ulong playerId)
		{
			return (playerId == BlueprintState.PlayerId_DefaultTilePreviews || playerId == BlueprintState.PlayerId_ReplacementTiles || playerId == SessionInfoAPI.LocalUserID);
		}
		public static bool IsMultiplayerVisualizer(ulong _playerId, ref Color playerColor)
		{
			if (LocalPlayerId(_playerId))
				return false;
			if (PlayerColorsCached.TryGetValue(_playerId, out playerColor))
				return true;
			return false;
		}

		//static bool BlueprintToPlayerMap(Blueprint bp, out ulong playerId)
		//{
		//	playerId = default;
		//	if (!MP_Helpers.MPInstalledAndActive())
		//		return false;

		//	return (BlueprintPlayerIdMap.TryGetValue(bp, out playerId) && playerId != SessionInfoAPI.LocalUserID);
		//}

		#region packetReceiver



		/// <summary>
		/// only called by packets
		/// </summary>
		/// <param name="playerId"></param>
		public static void SetDisplayBlueprintForPlayer(Blueprint bp, ulong playerId)
		{
			SgtLogger.l("SetDisplayBlueprintForPlayer recevied for " + playerId);
			if (LocalPlayerId(playerId))
			{
				SgtLogger.l("PlayerId was local, skipping");
				return;
			}
			if (!MP_Helpers.MPInstalledAndActive())
			{
				SgtLogger.l("MP mod not enabled");
				return;
			}
			if (!SessionInfoAPI.TryGetPlayerCursorPos(playerId, out var playerCursorPos))
			{
				SgtLogger.l("Cannot fetch cursor position for player");
				return;
			}

			//BlueprintPlayerIdMap[bp] = playerId;
			ClearVisuals(playerId);
			bp.CacheCost();
			var pos = new Vector2I((int)playerCursorPos.x, (int)playerCursorPos.y);
			CurrentVisualizers[playerId] = bp;
			VisualizeBlueprint(playerId, pos, bp);
		}

		public static void PlaceBlueprintForPlayer(Blueprint bp, ulong playerId, int x, int y)
		{
			SgtLogger.l("PlaceBlueprintForPlayer recevied for " + playerId);
			if (LocalPlayerId(playerId))
			{
				return;
			}
			if (!MP_Helpers.MPInstalledAndActive())
			{
				return;
			}
			ClearVisuals(playerId);
			CurrentVisualizers[playerId] = bp;
			bp.CacheCost();
			var pos = new Vector2I(x, y);
			VisualizeBlueprint(playerId, pos, bp);
			UpdateVisual(playerId, pos, true, bp);
			UseBlueprint(playerId, pos, bp);
		}

		/// <summary>CurrentStateInfo(playerId).ResetRotations()
		/// only called by packets
		/// </summary>
		/// <param name="playerId"></param>
		public static void ClearDisplayBlueprintForPlayer(ulong playerId)
		{
			SgtLogger.l("ClearDisplayBlueprintForPlayer recevied for " + playerId);
			if (LocalPlayerId(playerId))
				return;

			if (!MP_Helpers.MPInstalledAndActive())
				return;

			CurrentVisualizers.Remove(playerId);
			ClearVisuals(playerId);
			CurrentStateInfo(playerId).ResetRotations();
		}
		/// <summary>
		/// only called by packets
		/// </summary>
		/// <param name="playerId"></param>
		public static void MoveDisplayBlueprintForPlayer(ulong playerId, int x, int y)
		{
			SgtLogger.l("MoveDisplayBlueprintForPlayer recevied for " + playerId);
			if (LocalPlayerId(playerId))
				return;

			if (!MP_Helpers.MPInstalledAndActive() || !CurrentVisualizers.TryGetValue(playerId, out var playerBP))
				return;

			UpdateVisual(playerId, new Vector2I(x, y), false, playerBP);
		}

		public static void UpdateRemoteState(ulong playerId, ModeChangePacket stateInfo)
		{
			SgtLogger.l("Updating remote state for " + playerId);
			if (LocalPlayerId(playerId) || !MP_Helpers.MPInstalledAndActive() || !CurrentVisualizers.TryGetValue(playerId, out var playerBP))
				return;
			CurrentStateInfo(playerId).ConsumePacket(stateInfo);
			UpdateVisual(playerId, CurrentStateInfo(playerId).lastBlueprintPos, true, playerBP);
		}

		#endregion
		#region packetSender
		public static void OnStateChanged(ulong playerId = BlueprintState.PlayerId_DefaultTilePreviews)
		{
			if (!LocalPlayerId(playerId) || !MP_Helpers.MPInstalledAndActive())
				return;
			PacketSenderAPI.SendToAllOtherPeers(CurrentStateInfo().GetFilledModePacket());
		}
		public static void OnBlueprintUsed(ulong playerId, Blueprint bp, Vector2I pos)
		{
			if (!LocalPlayerId(playerId) || bp == null || !MP_Helpers.MPInstalledAndActive())
				return;
			PacketSenderAPI.SendToAllOtherPeers(new PlaceBlueprintPacket(bp, pos));

		}

		public static void OnBlueprintMoved(ulong playerId, Vector2I pos)
		{
			if (!LocalPlayerId(playerId) || !MP_Helpers.MPInstalledAndActive())
				return;
			PacketSenderAPI.SendToAllOtherPeers(new UpdateBlueprintVisualizationPacket(pos));
		}
		public static void OnBlueprintVisualized(ulong playerId, Blueprint bp, Vector2I pos)
		{
			if (!LocalPlayerId(playerId) || !MP_Helpers.MPInstalledAndActive())
				return;
			PacketSenderAPI.SendToAllOtherPeers(new StartBlueprintVisualizationPacket(bp, pos));
		}
		public static void OnBlueprintCleared(ulong playerId)
		{
			if (!LocalPlayerId(playerId) || !MP_Helpers.MPInstalledAndActive())
				return;
			PacketSenderAPI.SendToAllOtherPeers(new StopBlueprintVisualizationPacket());
		}

		#endregion

		#endregion

		#region UseCreate
		public static Blueprint CreateBlueprint(Vector2I topLeft, Vector2I bottomRight, MultiToolParameterMenu filter = null, bool createsSnapshot = false)
		{
			Blueprint blueprint = new Blueprint("unnamed", "");

			int blueprintHeight = (topLeft.y - bottomRight.y);
			bool storeDigCommandForNonSolidCells = filter != null && filter.AllowedToFilter(BlueprintCreationFilterKeys.NonSolidDigCommandssOptionID);
			bool collectNotes = filter != null && filter.AllowedToFilter(BlueprintCreationFilterKeys.Collect_Notes_ID);
			bool collectPlanShapes = filter != null && filter.AllowedToFilter(BlueprintCreationFilterKeys.PlanningToolMod_ShapesID);
			if (createsSnapshot)
				SgtLogger.l("Capturing Snapshot with settings: " + $"storeDigCommandForNonSolidCells: {storeDigCommandForNonSolidCells}, collectNotes: {collectNotes}, collectPlanShapes: {collectPlanShapes}");
			else
				SgtLogger.l("Capturing Blueprint with settings: " + $"storeDigCommandForNonSolidCells: {storeDigCommandForNonSolidCells}, collectNotes: {collectNotes}, collectPlanShapes: {collectPlanShapes}");

			for (int x = topLeft.x; x <= bottomRight.x; ++x)
			{
				for (int y = bottomRight.y; y <= topLeft.y; ++y)
				{
					int cell = Grid.XYToCell(x, y);

					if (Grid.IsVisible(cell))
					{
						bool emptyCell = true;
						bool solidTileDefInCell = false;

						for (int layer = 0; layer < Grid.ObjectLayers.Length; ++layer)
						{
							if (layer == (int)ObjectLayer.DigPlacer)
							{
								continue;
							}

							GameObject gameObject = Grid.Objects[cell, layer];
							if (gameObject == null)
								continue;

							bool hasConstructable = gameObject.TryGetComponent<Constructable>(out var constructable);
							bool hasDeconstructable = gameObject.TryGetComponent<Deconstructable>(out var deconstructable);

							var haulingPoint = gameObject.GetComponent("DeconstructableHaulingPoint");
							if (!hasDeconstructable && haulingPoint != null)
							{
								hasDeconstructable = true;
							}

							if (hasConstructable || hasDeconstructable)
							{
								Building building = null;

								if (gameObject.TryGetComponent<BuildingComplete>(out var complete))
								{
									building = complete;
								}
								else if (building == null && gameObject.TryGetComponent<BuildingUnderConstruction>(out var underConstruction))
								{
									building = underConstruction;
								}
								else if (building == null)
								{
									gameObject.TryGetComponent(out building);
								}
								//SgtLogger.l($"{gameObject != null} && {building != null} && {API_Methods.IsBuildable(building.Def)} && {(filter == null || filter.BuildingDefAllowedWithCurrentFilters(building.Def))}");
								if (building != null && API_Methods.AllowedByRules(building.Def) && (filter == null || filter.BuildingDefAllowedWithCurrentFilters(building.Def)))
								{
									Vector2I centre = Grid.CellToXY(GameUtil.NaturalBuildingCell(building));

									BuildingConfig buildingConfig = new()
									{
										Offset = new(centre.x - topLeft.x, blueprintHeight - (topLeft.y - centre.y)),
										BuildingDef = building.Def,
										Orientation = building.Orientation
									};
									buildingConfig.BuildingDefId = building.Def.PrefabID;

									if (building.Def.BuildingComplete.TryGetComponent<SimCellOccupier>(out var sco) && sco.doReplaceElement)
										solidTileDefInCell = true;

									if (deconstructable != null)
									{
										buildingConfig.SelectedElements.AddRange(deconstructable.constructionElements);
									}
									else if (constructable != null)
									{
										buildingConfig.SelectedElements.AddRange(constructable.selectedElementsTags);
									}
									else
									{
										SgtLogger.warning("building " + building.Def.Name + " at cell " + cell + " had neither constructable nor deconstructable component");
										foreach (var tagCombine in building.Def.MaterialCategory)
										{
											var available = MaterialSelectionPanel.Filter(tagCombine);
											buildingConfig.SelectedElements.Add(available.element);
										}
									}

									IHaveUtilityNetworkMgr networkMngCmp = building.Def.BuildingComplete.GetComponent<IHaveUtilityNetworkMgr>();
									if (networkMngCmp != null)
									{
										buildingConfig.SetConduitFlags((int)networkMngCmp.GetNetworkManager()?.GetConnections(cell, false));
									}
									API_Methods.StoreAdditionalBuildingData(gameObject, buildingConfig);

									if (!blueprint.BuildingConfigurations.Contains(buildingConfig))
									{
										blueprint.BuildingConfigurations.Add(buildingConfig);
									}

									emptyCell = false;
								}
							}
						}

						var cellOffsetInBlueprint = new Vector2I(x - topLeft.x, blueprintHeight - (topLeft.y - y));
						if ((emptyCell && storeDigCommandForNonSolidCells && !Grid.IsSolidCell(cell)) || (filter.AllowedLayer(ObjectLayer.DigPlacer) && Grid.Objects[cell, 7] != null && Grid.Objects[cell, 7].name == "DigPlacer"))
						{
							if (!blueprint.DigLocations.Contains(cellOffsetInBlueprint))
							{
								blueprint.DigLocations.Add(cellOffsetInBlueprint);
							}
						}
						if (collectPlanShapes && PlanningTool_Integration.HasPlan(cell, out var shape, out var color))
						{
							blueprint.PlanningToolMod_PlanDataValues[cellOffsetInBlueprint] = new Tuple<PlanShape, PlanColor>(shape, color);
						}
						var existingBpNote = Grid.Objects[cell, (int)ModAssets.BlueprintNotesLayer];
						if (collectNotes && existingBpNote != null && existingBpNote.TryGetComponent<BlueprintNote>(out var note))
						{
							SgtLogger.l("found note at cell " + cell + " with title: " + note.name);
							var data = note.GetNoteData(cellOffsetInBlueprint);
							if (data.IsValid())
							{
								SgtLogger.l("Writing note to blueprint at: " + cellOffsetInBlueprint);
								blueprint.WorldNotes[cellOffsetInBlueprint] = (data);
							}
							else
								SgtLogger.l("data was invalid for note at cell " + cell + " with title: " + note.name);

						}
						else if (!solidTileDefInCell && filter.AllowedElementState(Grid.Element[cell].state))
						{
							var data = BlueprintNoteData.CreateElementNote(cellOffsetInBlueprint, Grid.Element[cell].id, Grid.Mass[cell], Grid.Temperature[cell]);
							if (data.IsValid())
							{
								blueprint.WorldNotes[cellOffsetInBlueprint] = data;
							}
						}
					}
				}
			}
			//empty blueprint that caught some gas/liquid pockets, clear to not spam quasi empty blueprints
			if (!createsSnapshot && blueprint.BuildingConfigurations.Count == 0 &&
				blueprint.WorldNotes.Count == 0 && blueprint.PlanningToolMod_PlanDataValues.Count == 0 && blueprint.DigLocations.Any())
			{
				blueprint.DigLocations.Clear();
			}

			blueprint.CacheCost();
			return blueprint;
		}
		public static void UseBlueprint(ulong playerId, Vector2I origin, Blueprint snapshotBp = null)
		{
			var transformData = CurrentStateInfo(playerId);

			//CleanDirtyVisuals(playerId);
			transformData.StoreDimensions(snapshotBp);
			FoundationVisuals[playerId].ForEach(foundationVisual =>
			{
				foundationVisual.TryUse(transformData.GetRotatedCell(origin, foundationVisual));
			});
			DependentVisuals[playerId].ForEach(dependentVisual =>
			{
				dependentVisual.TryUse(transformData.GetRotatedCell(origin, dependentVisual));
			});

			if (snapshotBp == null && !LocalPlayerId(playerId) && CurrentVisualizers.TryGetValue(playerId, out var current))
				snapshotBp = current;
			OnBlueprintUsed(playerId, snapshotBp == null ? ModAssets.SelectedBlueprint : snapshotBp, origin);
		}
		#endregion
		#region Visualizers

		public static void RefreshBlueprintVisualizers(ulong playerId = PlayerId_DefaultTilePreviews, Blueprint snapshot = null)
		{

			BlueprintState.UpdateVisual(playerId, CurrentStateInfo(playerId).lastBlueprintPos, true, snapshot);
		}
		public static void VisualizeBlueprint(Vector2I topLeft, Blueprint blueprint) => VisualizeBlueprint(PlayerId_DefaultTilePreviews, topLeft, blueprint);
		public static void VisualizeBlueprint(ulong playerId, Vector2I topLeft, Blueprint blueprint)
		{
			if (blueprint == null)
			{
				return;
			}
			int errors = 0;
			ClearVisuals(playerId);
			var transformData = CurrentStateInfo(playerId);
			transformData.lastBlueprintPos = topLeft;

			bool notSnapshot = !transformData.IsPlacingSnapshot;
			var blockedLayers = transformData.BlockedPlacementFilterLayers;

			foreach (BuildingConfig buildingConfig in blueprint.BuildingConfigurations)
			{
				if (buildingConfig.BuildingDef == null || buildingConfig.SelectedElements.Count == 0)
				{
					++errors;
					continue;
				}
				if (notSnapshot && ModAssets.TryGetFilterLayerId(buildingConfig.BuildingDef.ObjectLayer, out var filterLayerId) && blockedLayers.Contains(filterLayerId))
					continue;
				if (buildingConfig.SkipPlacement)
					continue;

				if (buildingConfig.BuildingDef.BuildingPreview != null)
				{
					int cell = Grid.XYToCell(topLeft.x + buildingConfig.Offset.x, topLeft.y + buildingConfig.Offset.y);

					switch (ModAssets.GetVisualizerType(buildingConfig.BuildingDef))
					{
						case VisualizerType.TILE:
							AddVisual(new TileVisual(buildingConfig, cell, playerId), buildingConfig.BuildingDef);
							break;
						case VisualizerType.UTILITY:
							AddVisual(new UtilityVisual(buildingConfig, cell, playerId), buildingConfig.BuildingDef);
							break;
						case VisualizerType.BUILDING:
						default:
							AddVisual(new BuildingVisual(buildingConfig, cell, playerId), buildingConfig.BuildingDef);
							break;
					}
				}
			}

			foreach (var digLocation in blueprint.DigLocations)
			{
				FoundationVisuals[playerId].Add(new DigVisual(playerId, Grid.XYToCell(topLeft.x + digLocation.x, topLeft.y + digLocation.y), digLocation));
			}

			foreach (var elementIndicator in blueprint.WorldNotes)
			{
				var liquidLocation = elementIndicator.Key;
				var note = elementIndicator.Value;
				if (note.IsValid())
				{
					switch (note.Type)
					{
						case BlueprintNoteData.NoteType.Text:
							FoundationVisuals[playerId].Add(new TextNoteVisual(playerId, Grid.XYToCell(topLeft.x + liquidLocation.x, topLeft.y + liquidLocation.y), liquidLocation, note.Title, note.Text, note.Symbol, note.SymbolTint));
							break;
						case BlueprintNoteData.NoteType.Element:
							FoundationVisuals[playerId].Add(new ElementNoteVisual(playerId, Grid.XYToCell(topLeft.x + liquidLocation.x, topLeft.y + liquidLocation.y), liquidLocation, note.ElementId, note.ElementMass, note.ElementTemperature));
							break;
					}
				}
			}
			foreach (var shapePreview in blueprint.PlanningToolMod_PlanDataValues)
			{
				var shapeLocation = shapePreview.Key;
				FoundationVisuals[playerId].Add(new PlanningToolMod_ShapeVisual(playerId, Grid.XYToCell(topLeft.x + shapeLocation.x, topLeft.y + shapeLocation.y), shapeLocation, shapePreview.Value.first, shapePreview.Value.second));
			}

			if (UseBlueprintTool.Instance.HoverCard != null)
			{
				UseBlueprintTool.Instance.HoverCard.prefabErrorCount = errors;
			}

			transformData.CheckPermittedRotations();

			OnBlueprintVisualized(playerId, blueprint, topLeft);
			UpdateVisual(playerId, topLeft, true, blueprint);
		}

		private static void AddVisual(IVisual visual, BuildingDef buildingDef)
		{
			//SgtLogger.l(buildingDef.PrefabID + " -> adding visual of type: " + visual.GetType());
			ulong owner = visual.GetPlayerId();
			if (buildingDef.IsFoundation)
			{
				FoundationVisuals[owner].Add(visual);
			}
			else
			{
				DependentVisuals[owner].Add(visual);
			}

			if (visual is ICleanableVisual)
			{
				CleanableVisuals[owner].Add((ICleanableVisual)visual);
			}
		}


		//static Dictionary<int, Dictionary<int, GameObject>> VisualizerTargets = [];

		public static void UpdateVisual(ulong playerId, Vector2I origin, bool forcingRedraw = false, Blueprint snapshotBp = null)
		{
			OnStateChanged(playerId);
			var transformData = CurrentStateInfo(playerId);

			if (transformData.lastBlueprintPos == origin && !forcingRedraw)
				return;

			transformData.lastBlueprintPos = origin;
			CleanDirtyVisuals(playerId);
			transformData.StoreDimensions(snapshotBp);
			//VisualizerTargets.Clear();
			ClearOccupiedCells(playerId);

			FoundationVisuals[playerId].ForEach(foundationVisual =>
			{
				transformData.ApplyRotatedCellAndMove(origin, foundationVisual, forcingRedraw);
				StoreOccupiedArea(playerId, foundationVisual);
			});
			DependentVisuals[playerId].ForEach(dependentVisual =>
			{
				transformData.ApplyRotatedCellAndMove(origin, dependentVisual, forcingRedraw);
				StoreOccupiedArea(playerId, dependentVisual);
			});
			DependentVisuals[playerId].ForEach(dependentVisual =>
			{
				dependentVisual.RefreshColor();
			});

			OnBlueprintMoved(playerId, origin);
		}


		public static void ClearVisuals(ulong playerId = BlueprintState.PlayerId_DefaultTilePreviews)
		{
			CleanDirtyVisuals(playerId);


			var foundations = FoundationVisuals[playerId];
			foundations.ForEach(foundationVis => foundationVis.DestroyVisualizer());
			foundations.Clear();

			var dependents = DependentVisuals[playerId];
			dependents.ForEach(dependantVisual => dependantVisual.DestroyVisualizer());
			dependents.Clear();
			ClearOccupiedCells(playerId);

			if (LocalPlayerId(playerId))
				CurrentStateInfo(playerId).ResetRotations();

			OnBlueprintCleared(playerId);
		}
		static void ClearOccupiedCells(ulong playerId)
		{
			foreach (var cellCollection in OccupiedCells[playerId].Values)
				cellCollection.Clear();
		}
		static void StoreOccupiedArea(ulong playerId, IVisual visual)
		{
			if (visual is not BuildingVisual buildingVisual)
				return;
			if (!OccupiedCells[playerId].TryGetValue(buildingVisual.BuildingDef.ObjectLayer, out var cells))
			{
				SgtLogger.error("Unknown object layer: " + buildingVisual.BuildingDef.ObjectLayer);
				return;
			}


			if (buildingVisual.BuildingDef.BuildingComplete.TryGetComponent<OccupyArea>(out var area))
			{
				foreach (var cellOffset in area.OccupiedCellsOffsets)
					cells[Grid.OffsetCell(buildingVisual.CurrentCell, cellOffset)] = buildingVisual;
			}
			else
			{
				cells[buildingVisual.CurrentCell] = buildingVisual;
				SgtLogger.warning("No occupy area on " + buildingVisual);
			}
		}

		public static void CleanDirtyVisuals(ulong playerId)
		{
			var coloredCells = ColoredCells[playerId];
			//foreach (int cell in coloredCells.Keys)
			//{
			//	CustomTileRenderer.RefreshCell(playerId, cell, ObjectLayer.FoundationTile);
			//}

			coloredCells.Clear();
			CleanableVisuals[playerId].ForEach(cleanableVisual => cleanableVisual.Clean());
		}
		#endregion

		/// <summary>
		/// holds individual blueprint transformation info for one blueprint placer (== player)
		/// connecting players in the multiplayer mod get their own cached so their settings can be synced up
		/// </summary>
		public class BlueprintTransformationInfo
		{
			public void ConsumePacket(ModeChangePacket data)
			{
				//SgtLogger.l("Syncing TransferState");
				this.AdvancedMaterialReplacement = data.AdvancedMaterialReplacement;
				this.ForceBuild = data.ForceBuild;
				this.MaterialReplacementInSnapshots = data.MaterialReplacementInSnapshots;
				this.IsPlacingSnapshot = data.IsPlacingSnapshot;
				this.BlueprintOrientation = data.BlueprintOrientation;
				this.FlippedX = data.FlippedX;
				this.FlippedY = data.FlippedY;
				this.Permitted = data.Permitted;
				this._state = (BlueprintAnchorState)data._state;
				this.originShiftX = data.originShiftX;
				this.originShiftY = data.originShiftY;
				this.UseToolPriority = data.UseToolPriority;
				this.BlockedPlacementFilterLayers = data.BlockedPlacementFilterLayers.ToHashSet();
			}
			public ModeChangePacket GetFilledModePacket()
			{
				var packet = new ModeChangePacket();
				packet.AdvancedMaterialReplacement = AdvancedMaterialReplacement;
				packet.ForceBuild = ForceBuild;
				packet.MaterialReplacementInSnapshots = MaterialReplacementInSnapshots;
				packet.IsPlacingSnapshot = IsPlacingSnapshot;
				packet.BlueprintOrientation = BlueprintOrientation;
				packet.FlippedX = FlippedX;
				packet.FlippedY = FlippedY;
				packet.Permitted = Permitted;
				packet._state = (int)_state;
				packet.originShiftX = originShiftX;
				packet.originShiftY = originShiftY;
				packet.UseToolPriority = UseToolPriority;
				packet.BlockedPlacementFilterLayers = BlockedPlacementFilterLayers.ToList();
				return packet;
			}

			public BlueprintTransformationInfo(ulong playerId = BlueprintState.PlayerId_DefaultTilePreviews)
			{
				this.playerId = playerId;
			}

			public bool AdvancedMaterialReplacement = false;
			public bool ForceBuild = false;
			public bool MaterialReplacementInSnapshots = false;
			public bool UseToolPriority = true;
			public bool IsPlacingSnapshot { get; set; }
			public bool ApplyBlueprintSettings = true;
			public HashSet<string> BlockedPlacementFilterLayers = [];

			public void StoreDimensions(Blueprint bp)
			{
				//SgtLogger.l(playerId + "-State refreshing BP dimensions, is Local: "+ LocalPlayerId(playerId)+", id has bp: "+CurrentVisualizers.ContainsKey(playerId));

				if (bp == null && !LocalPlayerId(playerId) && CurrentVisualizers.TryGetValue(playerId, out var current))
				{
					bp = current;
					//SgtLogger.l("Storing dimensions of remote bp: " + current);
				}
				if (bp == null)
				{
					//SgtLogger.l("Storing dimensions of selected bp");
					bp = ModAssets.SelectedBlueprint;
				}
				if (bp == null)
					return;
				lastBlueprintDimensions = bp.Dimensions;
			}
			#region Rotation
			public ulong playerId = PlayerId_DefaultTilePreviews;
			/// <summary>
			/// the blueprint of this building supports both rotating and flipping in any direction, e.g a tile or backwall
			/// only to be used in visualizer checks, not in actual building placement!
			/// </summary>
			public const PermittedRotations All = (PermittedRotations)411;

			Orientation BlueprintOrientation = Orientation.Neutral;
			bool FlippedX, FlippedY;
			PermittedRotations Permitted = All;
			public string TransformationBlockedByBuildingName;

			public bool CanRotate => Permitted == All || Permitted == PermittedRotations.R360;
			public bool CanFlipH => Permitted == All || Permitted == PermittedRotations.FlipH;
			public bool CanFlipV => Permitted == All || Permitted == PermittedRotations.FlipV;

			internal Vector2I lastBlueprintPos, lastBlueprintDimensions;

			public void CheckPermittedRotations()
			{
				Permitted = All;
				TransformationBlockedByBuildingName = string.Empty;

				foreach (var vis in FoundationVisuals[playerId])
				{
					var rotation = vis.GetAllowedRotations();
					switch (rotation)
					{
						case All:
							continue;
						case PermittedRotations.Unrotatable:
							Permitted = PermittedRotations.Unrotatable;
							if (vis.BuildingID != null)
								TransformationBlockedByBuildingName = Assets.GetBuildingDef(vis.BuildingID).Name;
							return;
						case PermittedRotations.FlipH:
							Permitted = PermittedRotations.FlipH;
							if (vis.BuildingID != null)
								TransformationBlockedByBuildingName = Assets.GetBuildingDef(vis.BuildingID).Name;
							break;
					}
				}
				foreach (var vis in DependentVisuals[playerId])
				{
					var rotation = vis.GetAllowedRotations();
					switch (rotation)
					{
						case All:
							continue;
						case PermittedRotations.Unrotatable:
							if (vis.BuildingID != null)
								TransformationBlockedByBuildingName = Assets.GetBuildingDef(vis.BuildingID).Name;
							Permitted = PermittedRotations.Unrotatable;
							return;
						case PermittedRotations.FlipH:
							Permitted = PermittedRotations.FlipH;
							if (vis.BuildingID != null)
								TransformationBlockedByBuildingName = Assets.GetBuildingDef(vis.BuildingID).Name;
							break;
					}
				}
			}
			public void ResetRotations()
			{
				FlippedX = false;
				FlippedY = false;
				BlueprintOrientation = Orientation.Neutral;

				RefreshAnchorState();
			}
			public void ApplyRotatedCellAndMove(Vector2I origin, IVisual bpEntryVis, bool forcingRedraw)
			{
				bpEntryVis.ApplyRotation(BlueprintOrientation, FlippedX, FlippedY);
				bpEntryVis.MoveVisualizer(GetRotatedCell(origin, bpEntryVis), forcingRedraw);
			}
			public int GetRotatedCell(Vector2I originI, IVisual bpEntryVis)
			{
				Vector2 visPos = bpEntryVis.Offset; //the original bp offset
				Vector2 origin = originI;

				///origin shift
				int shiftX = (int)(lastBlueprintDimensions.X * originShiftX);
				int shiftY = (int)(lastBlueprintDimensions.Y * originShiftY);
				visPos.x -= shiftX;
				visPos.y -= shiftY;


				///rotation
				Matrix4x4 rotationMatrix = default;
				switch (BlueprintOrientation)
				{
					case Orientation.Neutral:
						rotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 0, 0));
						break;
					case Orientation.R90:
						rotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 0, -90));
						break;
					case Orientation.R180:
						rotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 0, -180));
						break;
					case Orientation.R270:
						rotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 0, -270));
						break;
				}
				visPos = rotationMatrix.MultiplyVector(visPos);

				///flipping
				var flipMatrix = Matrix4x4.Scale(new Vector3(FlippedX ? -1 : 1, FlippedY ? -1 : 1, 1));
				visPos = flipMatrix.MultiplyVector(visPos);

				///Fixes some tiles going offset by 1 tile when rotated, ty StuffyDoll for finding this fix
				visPos.x = Mathf.Round(visPos.x);
				visPos.y = Mathf.Round(visPos.y);

				return Grid.PosToCell(origin + visPos);
			}

			public void FlipVertical()
			{
				if (Permitted != All && Permitted != PermittedRotations.FlipV)
					return;

				FlippedY = !FlippedY;
				SgtLogger.l("Flipped Vertically: " + FlippedY);
			}
			public void FlipHorizontal()
			{
				if (Permitted != All && Permitted != PermittedRotations.FlipH)
					return;
				FlippedX = !FlippedX;
				SgtLogger.l("Flipped Horizontally: " + FlippedX);
			}
			public void TryRotateBlueprint(bool inverted = false)
			{
				if (Permitted != All && Permitted != PermittedRotations.R360)
					return;

				bool flipInversion = FlippedX != FlippedY;
				inverted ^= flipInversion;

				switch (BlueprintOrientation)
				{
					case Orientation.Neutral:
						BlueprintOrientation = inverted ? Orientation.R270 : Orientation.R90;
						break;
					case Orientation.R90:
						BlueprintOrientation = inverted ? Orientation.Neutral : Orientation.R180;
						break;
					case Orientation.R180:
						BlueprintOrientation = inverted ? Orientation.R90 : Orientation.R270;
						break;
					case Orientation.R270:
						BlueprintOrientation = inverted ? Orientation.R180 : Orientation.Neutral;
						break;
				}
			}
			#endregion
			#region AnchorShift
			BlueprintAnchorState _state = 0;
			float originShiftX = 0, originShiftY = 0;
			static readonly Dictionary<BlueprintAnchorState, AnchorState> ShiftStates = new()
			{
				{ BlueprintAnchorState.BottomCenter, new(0.5f,0f) },
				{ BlueprintAnchorState.Center,new(0.5f,0.5f)},
				{ BlueprintAnchorState.BottomLeft,new(0,0)},
				{ BlueprintAnchorState.TopLeft,new(0,1)},
				{ BlueprintAnchorState.TopRight,new(1,1)},
				{ BlueprintAnchorState.BottomRight,new(1,0)},
			};
			public class AnchorState
			{
				public float diffX, diffY;
				public AnchorState(float diffX, float diffY)
				{
					this.diffX = diffX;
					this.diffY = diffY;
				}
			}
			public void ResetAnchorState()
			{
				_state = Config.Instance.DefaultAnchorState;
				originShiftX = ShiftStates[_state].diffX;
				originShiftY = ShiftStates[_state].diffY;
			}

			public void SetAnchorState(float newDiffX = -1, float newDiffY = -1, Blueprint snapshotBlueprint = null)
			{
				if (newDiffX != -1)
					originShiftX = newDiffX;
				if (newDiffY != -1)
					originShiftY = newDiffY;
				UpdateVisual(playerId, lastBlueprintPos, true, snapshotBlueprint);
			}
			public void RefreshAnchorState(Blueprint snapshotBlueprint = null)
			{
				originShiftX = ShiftStates[_state].diffX;
				originShiftY = ShiftStates[_state].diffY;

				UpdateVisual(playerId, lastBlueprintPos, true, snapshotBlueprint);
			}

			public void NextAnchorState(Blueprint snapshotBlueprint = null)
			{
				int state = (int)_state;
				state = (state + 1) % ShiftStates.Count;
				_state = (BlueprintAnchorState)state;
				RefreshAnchorState(snapshotBlueprint);

			}
			//public Vector2I GetMousePos()
			//{
			//	var mousePos = PlayerController.GetCursorPos(KInputManager.GetMousePos());

			//	if (ShouldFetchPlayerCursorPos(playerId, out var otherPlayerPos))
			//	{
			//		mousePos = otherPlayerPos;
			//	}

			//	return new((int)mousePos.x, (int)mousePos.y);
			//}
		}

		internal static bool LayerOccupiedAt(IVisual checkingEntity, ObjectLayer layer, int cellParam)
		{
			if (BackwallManager.HasBackwall(cellParam))
				return true;
			var objectAtLayer = Grid.Objects[cellParam, (int)layer];

			if (objectAtLayer != null && objectAtLayer != checkingEntity.Visualizer)
				return true;

			if (!OccupiedCells[PlayerId_DefaultTilePreviews].TryGetValue(layer, out var collection))
			{
				SgtLogger.error("Unknown object layer: " + layer);
				return false;
			}

			return collection.TryGetValue(cellParam, out var vis) && vis != checkingEntity;
		}
		#endregion
	}
}
