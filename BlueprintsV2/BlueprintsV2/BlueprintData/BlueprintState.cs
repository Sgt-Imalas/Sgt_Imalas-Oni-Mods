using BlueprintsV2.BlueprintsV2.BlueprintData.LiquidInfo;
using BlueprintsV2.BlueprintsV2.BlueprintData.PlannedElements;
using BlueprintsV2.BlueprintsV2.Visualizers;
using BlueprintsV2.ModAPI;
using BlueprintsV2.Tools;
using BlueprintsV2.Visualizers;
using Epic.OnlineServices.Sessions;
using STRINGS;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using UnityEngine;
using UtilLibs;
using static STRINGS.BUILDING.STATUSITEMS;
using static STRINGS.DUPLICANTS.CHORES;

namespace BlueprintsV2.BlueprintData
{
	public struct CellColorPayload
	{
		public Color Color { get; private set; }
		public ObjectLayer TileLayer { get; private set; }
		public ObjectLayer ReplacementLayer { get; private set; }

		public CellColorPayload(Color color, ObjectLayer tileLayer, ObjectLayer replacementLayer)
		{
			Color = color;
			TileLayer = tileLayer;
			ReplacementLayer = replacementLayer;
		}
	}

	public static class BlueprintState
	{
		public static void ToggleHotkeyTooltips() => ExtendedCardTooltips = !ExtendedCardTooltips;
		public static bool ExtendedCardTooltips { get; private set; } = true;

		public static string SelectedBlueprintFolder = string.Empty;

		public static bool InstantBuild => DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive && SandboxToolParameterMenu.instance.settings.InstantBuild;

		public static bool AdvancedMaterialReplacement = false;
		public static bool ForceMaterialChange = false;
		public static bool MaterialReplacementInSnapshots = false;

		public static bool ApplyBlueprintSettings = true;
		public static bool IsPlacingSnapshot { get; set; }

		private static readonly List<IVisual> FoundationVisuals = new();
		private static readonly List<IVisual> DependentVisuals = new();
		private static readonly List<ICleanableVisual> CleanableVisuals = new();

		public static readonly Dictionary<int, CellColorPayload> ColoredCells = new();

		#region UseCreate
		public static Blueprint CreateBlueprint(Vector2I topLeft, Vector2I bottomRight, MultiToolParameterMenu filter = null, bool createsSnapshot = false)
		{
			Blueprint blueprint = new Blueprint("unnamed", "");

			int blueprintHeight = (topLeft.y - bottomRight.y);
			bool collectingGasTiles = filter != null && filter.AllowedToFilter(SolidTileFiltering.StoreNonSolidsOptionID);
			bool collectLiquidNotes = filter != null && filter.AllowedToFilter(SolidTileFiltering.StoreLiquidNotesOptionID);
			bool collectSolidNotes = filter != null && filter.AllowedToFilter(SolidTileFiltering.StoreLiquidNotesOptionID);

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
									if (building.Def.BuildingComplete.TryGetComponent<SimCellOccupier>(out var sco) && sco.doReplaceElement)
										solidTileDefInCell = true;


									if (deconstructable != null)
									{
										buildingConfig.SelectedElements.AddRange(deconstructable.constructionElements);
									}
									else
									{
										buildingConfig.SelectedElements.AddRange(constructable.selectedElementsTags);
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

						var cellLocationInBlueprint = new Vector2I(x - topLeft.x, blueprintHeight - (topLeft.y - y));
						if ((emptyCell && collectingGasTiles && !Grid.IsSolidCell(cell)) || (filter.AllowedLayer(ObjectLayer.DigPlacer) && Grid.Objects[cell, 7] != null && Grid.Objects[cell, 7].name == "DigPlacer"))
						{

							if (!blueprint.DigLocations.Contains(cellLocationInBlueprint))
							{
								blueprint.DigLocations.Add(cellLocationInBlueprint);
							}
						}

						var PotentialElementIndicator = Grid.Objects[cell, (int)ModAssets.PlannedElementLayer];
						if (!solidTileDefInCell)
						{
							if ((collectLiquidNotes && Grid.IsLiquid(cell)) || (collectSolidNotes && Grid.IsSolidCell(cell)))
								blueprint.PlannedNaturalElementInfos[cellLocationInBlueprint] = new Tuple<SimHashes, float, float>(Grid.Element[cell].id, Grid.Mass[cell], Grid.Temperature[cell]);
						}
						else if (PotentialElementIndicator != null && PotentialElementIndicator.TryGetComponent<ElementPlanInfo>(out var info))
						{
							if ((info.IsSolid && collectLiquidNotes) || (info.IsLiquid && collectLiquidNotes))
								blueprint.PlannedNaturalElementInfos[cellLocationInBlueprint] = new Tuple<SimHashes, float, float>(info.ElementId, info.ElementAmount, info.ElementTemperature);
						}
					}
				}
			}
			//empty blueprint that caught some gas/liquid pockets, clear to not spam quasi empty blueprints
			if (blueprint.BuildingConfigurations.Count == 0 && (blueprint.DigLocations.Any() || blueprint.PlannedNaturalElementInfos.Count > 0 || blueprint.PlanningToolMod_PlanDataValues.Count > 0) && !createsSnapshot)
			{
				blueprint.DigLocations.Clear();
				blueprint.PlannedNaturalElementInfos.Clear();
				blueprint.PlanningToolMod_PlanDataValues.Clear();
			}

			blueprint.CacheCost();
			return blueprint;
		}
		public static void UseBlueprint(Vector2I origin, Blueprint snapshotBp = null)
		{
			CleanDirtyVisuals();
			StoreDimensions(snapshotBp);
			FoundationVisuals.ForEach(foundationVisual =>
			{
				foundationVisual.TryUse(GetRotatedCell(origin, foundationVisual));
			});
			DependentVisuals.ForEach(dependentVisual =>
			{
				dependentVisual.TryUse(GetRotatedCell(origin, dependentVisual));
			});
		}
		#endregion
		#region Visualizers

		public static void RefreshBlueprintVisualizers(Blueprint snapshot = null)
		{
			BlueprintState.UpdateVisual(lastBlueprintPos, true, snapshot);
		}

		public static void VisualizeBlueprint(Vector2I topLeft, Blueprint blueprint)
		{
			if (blueprint == null)
			{
				return;
			}

			int errors = 0;
			ClearVisuals();

			foreach (BuildingConfig buildingConfig in blueprint.BuildingConfigurations)
			{
				if (buildingConfig.BuildingDef == null || buildingConfig.SelectedElements.Count == 0)
				{
					++errors;
					continue;
				}

				if (buildingConfig.BuildingDef.BuildingPreview != null)
				{
					int cell = Grid.XYToCell(topLeft.x + buildingConfig.Offset.x, topLeft.y + buildingConfig.Offset.y);

					if (buildingConfig.BuildingDef.IsTilePiece
						&& !buildingConfig.BuildingDef.BuildingComplete.TryGetComponent<Door>(out _)
						&& buildingConfig.BuildingDef.TileLayer != ObjectLayer.LadderTile
						)
					{
						if (buildingConfig.BuildingDef.BuildingComplete.GetComponent<IHaveUtilityNetworkMgr>() != null)
						{
							AddVisual(new UtilityVisual(buildingConfig, cell), buildingConfig.BuildingDef);
						}
						else
						{
							AddVisual(new TileVisual(buildingConfig, cell), buildingConfig.BuildingDef);
						}
					}
					else
					{
						AddVisual(new BuildingVisual(buildingConfig, cell), buildingConfig.BuildingDef);
					}
				}
			}

			foreach (var digLocation in blueprint.DigLocations)
			{
				FoundationVisuals.Add(new DigVisual(Grid.XYToCell(topLeft.x + digLocation.x, topLeft.y + digLocation.y), digLocation));
			}

			foreach (var elementIndicator in blueprint.PlannedNaturalElementInfos)
			{
				var liquidLocation = elementIndicator.Key;
				FoundationVisuals.Add(new ElementIndicatorVisual(Grid.XYToCell(topLeft.x + liquidLocation.x, topLeft.y + liquidLocation.y), liquidLocation, elementIndicator.Value.first, elementIndicator.Value.second, elementIndicator.Value.third));
			}

			if (UseBlueprintTool.Instance.HoverCard != null)
			{
				UseBlueprintTool.Instance.HoverCard.prefabErrorCount = errors;
			}

			CheckPermittedRotations();
		}

		private static void AddVisual(IVisual visual, BuildingDef buildingDef)
		{
			SgtLogger.l(buildingDef.PrefabID + " -> adding visual of type: " + visual.GetType());
			if (buildingDef.IsFoundation)
			{
				FoundationVisuals.Add(visual);
			}
			else
			{
				DependentVisuals.Add(visual);
			}

			if (visual is ICleanableVisual)
			{
				CleanableVisuals.Add((ICleanableVisual)visual);
			}
		}
		static Vector2I lastBlueprintPos, lastBlueprintDimensions;

		static void StoreDimensions(Blueprint bp)
		{
			if (bp == null)
				bp = ModAssets.SelectedBlueprint;
			if (bp == null)
				return;
			lastBlueprintDimensions = bp.Dimensions;
		}

		public static void UpdateVisual(Vector2I origin, bool forcingRedraw = false, Blueprint snapshotBp = null)
		{
			lastBlueprintPos = origin;
			CleanDirtyVisuals();
			StoreDimensions(snapshotBp);

			FoundationVisuals.ForEach(foundationVisual =>
			{
				ApplyRotatedCell(origin, foundationVisual, forcingRedraw);
			});
			DependentVisuals.ForEach(dependentVisual =>
			{
				ApplyRotatedCell(origin, dependentVisual, forcingRedraw);
			});
			DependentVisuals.ForEach(dependentVisual =>
			{
				dependentVisual.RefreshColor();
			});
		}


		public static void ClearVisuals()
		{
			CleanDirtyVisuals();
			CleanableVisuals.Clear();

			FoundationVisuals.ForEach(foundationVisual => UnityEngine.Object.DestroyImmediate(foundationVisual.Visualizer));
			FoundationVisuals.Clear();

			DependentVisuals.ForEach(dependantVisual => UnityEngine.Object.DestroyImmediate(dependantVisual.Visualizer));
			DependentVisuals.Clear();
			ResetRotations();
		}

		public static void CleanDirtyVisuals()
		{
			foreach (int cell in ColoredCells.Keys)
			{
				CellColorPayload cellColorPayload = ColoredCells[cell];
				TileVisualizer.RefreshCell(cell, cellColorPayload.TileLayer, cellColorPayload.ReplacementLayer);
			}

			ColoredCells.Clear();
			CleanableVisuals.ForEach(cleanableVisual => cleanableVisual.Clean());
		}
		#endregion

		#region Rotation

		/// <summary>
		/// the blueprint of this building supports both rotating and flipping in any direction, e.g a tile or backwall
		/// only to be used in visualizer checks, not in actual building placement!
		/// </summary>
		public const PermittedRotations All = (PermittedRotations)411;

		static Orientation BlueprintOrientation = Orientation.Neutral;
		static bool FlippedX, FlippedY;
		static PermittedRotations Permitted = All;
		public static string TransformationBlockedByBuildingName;

		public static bool CanRotate => Permitted == All || Permitted == PermittedRotations.R360;
		public static bool CanFlipH => Permitted == All || Permitted == PermittedRotations.FlipH;
		public static bool CanFlipV => Permitted == All || Permitted == PermittedRotations.FlipV;

		public static void CheckPermittedRotations()
		{
			Permitted = All;
			TransformationBlockedByBuildingName = string.Empty;

			foreach (var vis in FoundationVisuals)
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
			foreach (var vis in DependentVisuals)
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

		static void ResetRotations()
		{
			FlippedX = false;
			FlippedY = false;
			BlueprintOrientation = Orientation.Neutral;
		}
		public static void ApplyRotatedCell(Vector2I origin, IVisual bpEntryVis, bool forcingRedraw)
		{
			bpEntryVis.ApplyRotation(BlueprintOrientation, FlippedX, FlippedY);
			bpEntryVis.MoveVisualizer(GetRotatedCell(origin, bpEntryVis), forcingRedraw);
		}

		public static int GetRotatedCell(Vector2I originI, IVisual bpEntryVis)
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

			return Grid.PosToCell(origin + visPos);
		}


		public static void FlipVertical()
		{
			if (Permitted != All && Permitted != PermittedRotations.FlipV)
				return;

			FlippedY = !FlippedY;
			SgtLogger.l("Flipped Vertically: " + FlippedY);
		}
		public static void FlipHorizontal()
		{
			if (Permitted != All && Permitted != PermittedRotations.FlipH)
				return;
			FlippedX = !FlippedX;
			SgtLogger.l("Flipped Horizontally: " + FlippedX);
		}

		public static void TryRotateBlueprint(bool inverted = false)
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
		static int _state = 0;
		static float originShiftX = 0, originShiftY = 0;
		static List<AnchorState> ShiftStates = new()
		{
			new("middle",0.5f,0.5f),
			new ("bottomLeft",0,0),
			new ("topLeft",0,1),
			new ("topRight",1,1),
			new ("bottomRight",1,0),
		};

		public class AnchorState
		{
			public string Name;
			public float diffX, diffY;
			public AnchorState(string name, float diffX, float diffY)
			{
				Name = name;
				this.diffX = diffX;
				this.diffY = diffY;
			}
		}
		public static void SetAnchorState(float newDiffX = -1, float newDiffY = -1, Blueprint snapshotBlueprint = null)
		{
			if (newDiffX != -1)
				originShiftX = newDiffX;
			if (newDiffY != -1)
				originShiftY = newDiffY;
			var mousePos = PlayerController.GetCursorPos(KInputManager.GetMousePos());
			UpdateVisual(new((int)mousePos.x, (int)mousePos.y), true, snapshotBlueprint);
		}
		public static void NextAnchorState(Blueprint snapshotBlueprint = null)
		{
			_state = (_state + 1) % ShiftStates.Count;
			originShiftX = ShiftStates[_state].diffX;
			originShiftY = ShiftStates[_state].diffY;

			var mousePos = PlayerController.GetCursorPos(KInputManager.GetMousePos());

			UpdateVisual(new((int)mousePos.x, (int)mousePos.y), true, snapshotBlueprint);
		}


		static Vector2I GetShiftedPositions(Vector2I startPos, Blueprint bp = null)
		{
			if (bp == null)
				bp = ModAssets.SelectedBlueprint;
			if (bp == null)
				return startPos;

			return startPos;

			var dimensions = bp.Dimensions;
			int shiftX = (int)(dimensions.X * originShiftX);
			int shiftY = (int)(dimensions.Y * originShiftY);


			var newPosX = startPos.X - shiftX;
			var newPosY = startPos.Y - shiftY;

			//if (FlippedX)
			//	newPosX += dimensions.X;
			//if (FlippedY)
			//	newPosY += dimensions.Y;


			switch (BlueprintOrientation)
			{
				case Orientation.Neutral:
					break;
				case Orientation.R90:
					newPosY += dimensions.X;
					break;
				case Orientation.R180:
					newPosX += dimensions.X;
					newPosY += dimensions.Y;
					break;
				case Orientation.R270:
					newPosX += dimensions.Y;
					break;
			}

			var newVector = new Vector2I(newPosX, newPosY);

			return newVector;
		}
		#endregion
	}
}
