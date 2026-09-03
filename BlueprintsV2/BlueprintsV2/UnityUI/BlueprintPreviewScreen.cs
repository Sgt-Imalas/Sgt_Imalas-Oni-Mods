using BlueprintsV2.BlueprintData;
using BlueprintsV2.BlueprintsV2.BlueprintData.NoteToolPlacedEntities;
using BlueprintsV2.BlueprintsV2.BlueprintData.PlannedElements;
using BlueprintsV2.BlueprintsV2.BlueprintData.PlanningToolMod_Integration;
using BlueprintsV2.BlueprintsV2.BlueprintData.PlanningToolMod_Integration.EnumMirrors;
using BlueprintsV2.BlueprintsV2.UnityUI.Components;
using BlueprintsV2.BlueprintsV2.UnityUI.Components.PreviewVisualizers;
using BlueprintsV2.Tools;
using NodeEditorFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UtilLibs;
using UtilLibs.UIcmp;
using static BlueprintsV2.BlueprintsV2.UnityUI.Components.BuildingFilterDropdown;
using static BlueprintsV2.STRINGS.UI.BLUEPRINTSELECTOR.BLUEPRINTINFO.STATS;
using static BlueprintsV2.STRINGS.UI.BLUEPRINTSELECTOR.PREVIEW;
using static Database.MonumentPartResource;
using static STRINGS.LORE.BUILDINGS;
using static STRINGS.MISC.STATUSITEMS;
using static STRINGS.UI.CLUSTERMAP.ASTEROIDS;
using static UtilLibs.UIcmp.FMultiSelectDropdown;

namespace BlueprintsV2.BlueprintsV2.UnityUI
{
	internal class BlueprintPreviewScreen : FScreen
	{
		[MyCmpGet] new RectTransform _rectTransform;
		bool _init = false;
		GameObject BuildingEntry;
		List<GameObject> BPVisualizers = new List<GameObject>();

		Dictionary<string, List<Vis_BuildingPreview>> FilterLayerKbacs = [];
		Dictionary<string, List<Vis_SpritePreview>> FilterLayerImages = [];

		float lowerZoomBound = 2.5f, upperZoomBound = 0.1f;
		float currentZoomStep = 3;
		float zoomStepMin = -2, zoomStepMax = 15;
		float m_targetZoomScale = 0.25f, m_currentZoomScale = 0.25f;
		BuildingFilterDropdown FilterDropDown;

		GameObject BuildingCountWarning;
		FButton ConfirmShowOverride;
		LocText WarningText;
		Blueprint ScheduledToShow = null;

		//take priority consuming the scroll
		public override float GetSortKey()
		{
			return base.GetSortKey() + 10;
		}
		readonly List<string> filterKeys = new List<string>();
		readonly Dictionary<string, bool> PreviewFilters = [];
		string _hoveredFilter = null;

		void ResetPreviewFilters()
		{
			foreach (var key in filterKeys)
				PreviewFilters[key] = true;


			BlueprintState.CurrentStateInfo().BlockedPlacementFilterLayers.Clear();
			FilterDropDown.ResetAllToggles(true);
			RefreshVisualizerVisibility();
		}
		void OnPreviewFilterChanged(string id, bool enabled)
		{
			var currentFilters = BlueprintState.CurrentStateInfo().BlockedPlacementFilterLayers;
			if (!enabled)
				currentFilters.Add(id);
			else
				currentFilters.Remove(id);

			PreviewFilters[id] = enabled;
			RefreshVisualizerVisibility();
		}

		void Init()
		{
			if (_init)
				return;
			_init = true;
			BuildingEntry = transform.Find("BuildingPrefab").gameObject;
			BuildingEntry.SetActive(false);

			BuildingCountWarning = transform.parent.parent.Find("AmountWarning").gameObject;
			WarningText = BuildingCountWarning.transform.Find("Label").gameObject.GetComponent<LocText>();
			ConfirmShowOverride = BuildingCountWarning.transform.Find("Override").gameObject.AddOrGet<FButton>();
			ConfirmShowOverride.OnClick += ForceShowScheduledBp;

			FilterDropDown = transform.parent.parent.Find("FilterButton").FindOrAddComponent<BuildingFilterDropdown>();

			List<FDropDownEntry> entries = [];
			HashSet<string> ignoreFilters = [
				ToolParameterMenu.FILTERLAYERS.DIGPLACER,
				BlueprintCreationFilterKeys.NonSolidDigCommandssOptionID,
				BlueprintCreationFilterKeys.Collect_Natural_Elements_ID
				];

			foreach (var data in SnapshotTool.Instance.DefaultParameters)
			{
				string filterId = data.Key;

				if (ignoreFilters.Contains(filterId))
					continue;

				filterKeys.Add(filterId);
				FilterLayerKbacs[filterId] = new();
				FilterLayerImages[filterId] = new();

				entries.Add(new FHoverableDropDownEntry(
					Strings.Get("STRINGS.UI.TOOLS.FILTERLAYERS." + filterId + ".NAME"),
					(on) => OnPreviewFilterChanged(filterId, on),
					true,
					Strings.Get("STRINGS.UI.TOOLS.FILTERLAYERS." + filterId + ".TOOLTIP"),
				() => OnCategoryHovered(filterId),
				() => OnCategoryUnhovered(filterId)
				));
			}
			entries.Add(new FDropDownButtonEntry(FILTERBUTTON.RESETALL, (_) => ResetPreviewFilters()));

			FilterDropDown.DropDownEntries = entries;
			FilterDropDown.InitializeDropDown();
		}

		internal void RefreshVisualizerVisibility()
		{
			bool lowOpacity = _hoveredFilter != null;

			foreach (var layer in filterKeys)
			{
				bool highlighted = layer == _hoveredFilter;
				bool useLowOpacity = lowOpacity && layer != _hoveredFilter;
				bool layerActive = PreviewFilters[layer];

				foreach (var sprite in FilterLayerImages[layer])
				{
					sprite.RefreshOpacity(layerActive, useLowOpacity, highlighted);
				}
				foreach (var anim in FilterLayerKbacs[layer])
				{
					anim.RefreshOpacity(layerActive, useLowOpacity, highlighted);
				}
			}
		}

		void OnCategoryHovered(string category)
		{
			_hoveredFilter = category;
			RefreshVisualizerVisibility();
		}
		void OnCategoryUnhovered(string category)
		{
			if (_hoveredFilter == category)
				_hoveredFilter = null;
			RefreshVisualizerVisibility();
		}

		public override void OnSpawn()
		{
			base.OnSpawn();
		}

		void ClearExisting()
		{
			_rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 100f);
			_rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 100f);
			foreach (var entry in BPVisualizers)
				Destroy(entry);
			BPVisualizers.Clear();
			ResetPreviewFilters();
			foreach (var key in filterKeys)
			{
				FilterLayerImages[key].Clear();
				FilterLayerKbacs[key].Clear();
			}
		}
		void ForceShowScheduledBp()
		{

			BuildingCountWarning.SetActive(false);
			if (ScheduledToShow == null)
				return;
			GeneratePreview(ScheduledToShow);
			ScheduledToShow = null;
		}

		void ShowWarningAndCache(Blueprint bp, int count)
		{
			ScheduledToShow = bp;
			WarningText.SetText(string.Format(STRINGS.UI.BLUEPRINTSELECTOR.PREVIEW.AMOUNTWARNING.LABEL, count));
			BuildingCountWarning.SetActive(true);
		}

		void GeneratePreview(Blueprint blueprint)
		{
			Vector2I dimensions = blueprint.VisibleDimensions;
			SgtLogger.l("Visualizing " + blueprint.FriendlyName + " with dimensions: " + dimensions);
			//dimensions = new(dimensions.X+ 4, dimensions.Y+ 4);
			Vis_TilePreview.ClearTileArray(dimensions);
			float xOffset = dimensions.X / 2f;
			float yOffset = dimensions.Y / 2f;
			dimensions = new(dimensions.X + 2, dimensions.Y + 2);
			_rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, dimensions.X * 100f);
			_rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, dimensions.Y * 100f);

			Vector3 centerOffset = new(xOffset * 100, yOffset * 100);
			GeneratePreview_Buildings(blueprint, centerOffset);
			GeneratePreview_Notes(blueprint, centerOffset);
			GeneratePreview_Shapes(blueprint, centerOffset);

			RefreshVisualizerVisibility();

		}
		void GeneratePreview_Buildings(Blueprint blueprint, Vector3 centerOffset)
		{
			var buildings = blueprint.BuildingConfigurations.OrderBy(b => b.BuildingDef?.SceneLayer ?? 0);
			foreach (var building in buildings)
			{
				if (building.BuildingDef == null)
					continue;
				var visType = ModAssets.GetVisualizerType(building.BuildingDef);
				var entry = Instantiate(BuildingEntry, transform);
				entry.transform.localPosition = GetCellCenterPos(building.Offset, building.BuildingDef.SceneLayer); //new(building.Offset.X * 100f - xOffset, building.Offset.Y * 100f - yOffset);
				entry.transform.localPosition -= centerOffset;
				switch (visType)
				{
					case VisualizerType.TILE:
						RegisterImageToLayer(building, entry.AddOrGet<Vis_TilePreview>().Init(building));
						break;
					case VisualizerType.UTILITY:
						RegisterBuildingToLayer(building, entry.AddOrGet<Vis_ConduitPreview>().Init(building));
						break;
					default:
						RegisterBuildingToLayer(building, entry.AddOrGet<Vis_BuildingPreview>().Init(building));
						break;
				}
				entry.SetActive(true);
				BPVisualizers.Add(entry);
			}
			Vis_TilePreview.ConnectAll();
		}

		void RegisterBuildingToLayer(BuildingConfig building, Vis_BuildingPreview buildVis) => RegisterBuildingToLayer(building.BuildingDef.ObjectLayer, buildVis);
		void RegisterBuildingToLayer(ObjectLayer layer, Vis_BuildingPreview buildVis)
		{
			if (ModAssets.TryGetFilterLayerId(layer, out var layerId))
			{
				FilterLayerKbacs[layerId].Add(buildVis);
			}
			else
			{
				SgtLogger.warning("Could not find valid filter layer for building: " + layer);
				FilterLayerKbacs[ToolParameterMenu.FILTERLAYERS.BUILDINGS].Add(buildVis);
			}
		}
		void RegisterImageToLayer(BuildingConfig building, Vis_SpritePreview spriteVis) => RegisterImageToLayer(building.BuildingDef.ObjectLayer, spriteVis);
		void RegisterImageToLayer(ObjectLayer layer, Vis_SpritePreview spriteVis)
		{
			//if (spriteVis is Vis_TilePreview tilePreview)
			//	SgtLogger.l("registering tile preview to " + layer);
			if (ModAssets.TryGetFilterLayerId(layer, out var layerId))
			{
				FilterLayerImages[layerId].Add(spriteVis);
			}
			else
			{
				SgtLogger.warning("Could not find valid filter layer for building: " + layer);
				FilterLayerImages[ToolParameterMenu.FILTERLAYERS.BUILDINGS].Add(spriteVis);
			}
		}
		void RegisterImageToLayer(string filterLayer, Vis_SpritePreview spriteVis)
		{
			if (FilterLayerImages.TryGetValue(filterLayer, out var layerItems))
			{
				layerItems.Add(spriteVis);
			}
			else
			{
				SgtLogger.warning("Could not find valid filterLayer: " + filterLayer);
			}
		}

		void GeneratePreview_Notes(Blueprint blueprint, Vector3 centerOffset)
		{
			foreach (var note in blueprint.WorldNotes)
			{
				var entry = Instantiate(BuildingEntry, transform);
				entry.transform.localPosition = GetCellCenterPos(note.Key, Grid.SceneLayer.FXFront); //new(building.Offset.X * 100f - xOffset, building.Offset.Y * 100f - yOffset);
				entry.transform.localPosition -= centerOffset;
				var preview = entry.AddOrGet<Vis_SpritePreview>().Init();
				entry.SetActive(true);
				BlueprintNoteData noteData = note.Value;
				switch (noteData.Type)
				{
					case BlueprintNoteData.NoteType.Text:
						preview.SetDisplayed(new(noteData.GetNoteSprite(), noteData.SymbolTint));
						entry.AddOrGet<Vis_Tooltip>().SetText(noteData.Title, noteData.Text);
						break;
					case BlueprintNoteData.NoteType.Element:
						if (ElementLoader.FindElementByHash(noteData.ElementId) == null)
						{
							Destroy(entry);
							continue;
						}
						else
						{
							preview.SetDisplayed(GetElementInfoDisplay(noteData));
							entry.AddOrGet<Vis_Tooltip>().SetText(ElementLoader.FindElementByHash(noteData.ElementId).name, GetElementInfoString(noteData));
						}
						break;
					default:
						Destroy(entry);
						continue;

				}
				RegisterImageToLayer(BlueprintCreationFilterKeys.Collect_Notes_ID, preview);
				BPVisualizers.Add(entry);
			}
		}

		string GetElementInfoString(BlueprintNoteData noteData)
		{
			var element = ElementLoader.FindElementByHash(noteData.ElementId);
			if ((element.state & Element.State.Solid) == Element.State.Vacuum)
			{
				return string.Empty;
			}
			else
			{
				string mass = GameUtil.GetFormattedMass(noteData.ElementMass);
				string temperature = GameUtil.GetFormattedTemperature(noteData.ElementTemperature);
				return string.Format("{0}, {1}", mass, temperature);
			}
		}

		void GeneratePreview_Shapes(Blueprint blueprint, Vector3 centerOffset)
		{
			foreach (var note in blueprint.PlanningToolMod_PlanDataValues)
			{
				var entry = Instantiate(BuildingEntry, transform);
				entry.transform.localPosition = GetCellCenterPos(note.Key, Grid.SceneLayer.FXFront); //new(building.Offset.X * 100f - xOffset, building.Offset.Y * 100f - yOffset);
				entry.transform.localPosition -= centerOffset;
				var preview = entry.AddOrGet<Vis_SpritePreview>().Init();
				entry.SetActive(true);
				var noteData = note.Value;

				Sprite sprite = noteData.first switch
				{
					PlanShape.Circle => ModAssets.PlanningToolPreview_Circle,
					PlanShape.Diamond => ModAssets.PlanningToolPreview_Diamond,
					_ => ModAssets.PlanningToolPreview_Square,
				};
				preview.SetDisplayed(new(sprite, PlanningTool_EnumMapping.AsColor(noteData.second)));

				BPVisualizers.Add(entry);
				RegisterImageToLayer(BlueprintCreationFilterKeys.PlanningToolMod_ShapesID, preview);
			}
		}
		public Vector3 DragStartPosition;

		public override void OnBeginDrag(PointerEventData eventData)
		{
			transform.parent.TryGetComponent<ScrollRect>(out var scrollRect);
			scrollRect.OnBeginDrag(eventData);
			base.OnBeginDrag(eventData);
		}
		public override void OnDrag(PointerEventData eventData)
		{
			transform.parent.TryGetComponent<ScrollRect>(out var scrollRect);
			scrollRect.OnDrag(eventData);
			base.OnDrag(eventData);
		}
		public override void OnEndDrag(PointerEventData eventData)
		{
			transform.parent.TryGetComponent<ScrollRect>(out var scrollRect);
			scrollRect.OnEndDrag(eventData);
			base.OnEndDrag(eventData);
		}

		public void LoadBlueprintPreview(Blueprint blueprint)
		{
			Init();
			BuildingCountWarning.SetActive(false);
			ClearExisting();
			int buildingCount = blueprint.BuildingConfigurations.Count;
			SgtLogger.l(blueprint.FriendlyName + " has " + buildingCount + " buildings");
			if (buildingCount > Config.Instance.AutoPreviewCuttoff)
				ShowWarningAndCache(blueprint, buildingCount);
			else
				GeneratePreview(blueprint);
		}
		Tuple<Sprite, Color> GetElementInfoDisplay(BlueprintNoteData noteData)
		{
			var element = ElementLoader.FindElementByHash(noteData.ElementId);
			var color = Color.white;
			Sprite sprite;
			bool vaccuum = false;
			switch (element.state & Element.State.Solid)
			{
				case Element.State.Gas:
					sprite = ModAssets.Gas_Placer_Sprite;
					break;
				case Element.State.Liquid:
					sprite = ModAssets.Liquid_Placer_Sprite;
					break;
				case Element.State.Solid:
					sprite = ModAssets.Solid_Placer_Sprite;
					break;
				default:
				case Element.State.Vacuum:
					vaccuum = true;
					sprite = ModAssets.Special_Placer_Sprite;
					break;
			}
			if (!vaccuum)
				color = element.substance.colour;

			return new Tuple<Sprite, Color>(sprite, color);
		}

		Vector3 GetCellCenterPos(Vector2 offset, Grid.SceneLayer layer)
		{
			//return new Vector3(offset.x * 100, offset.y * 100f, Grid.GetLayerZ(layer));
			return new Vector3(offset.x * 100f + 50, offset.y * 100f + .001f, Grid.GetLayerZ(layer));
		}

		public override void ScreenUpdate(bool topLevel)
		{
			m_currentZoomScale = Mathf.Lerp(m_currentZoomScale, m_targetZoomScale, Mathf.Min(4f * Time.unscaledDeltaTime, 1f));
			Vector2 mousePos = (Vector2)KInputManager.GetMousePos();
			Vector3 vector3_1 = _rectTransform.InverseTransformPoint((Vector3)mousePos);
			_rectTransform.localScale = new Vector3(m_currentZoomScale, m_currentZoomScale, 1f);
			Vector3 vector3_2 = _rectTransform.InverseTransformPoint((Vector3)mousePos);
			RectTransform content = _rectTransform;
			content.localPosition = content.localPosition + (vector3_2 - vector3_1) * m_currentZoomScale;
			//if (_currentlySimDragged != null) _currentlySimDragged.transform.SetPosition(mousePos);
			//if (_currentlySimDraggedToolkit != null) _currentlySimDraggedToolkit.transform.SetPosition(mousePos);
		}
		public override void OnKeyDown(KButtonEvent e)
		{
			if (!e.Consumed && mouseOver && (e.IsAction(Action.ZoomIn) || e.IsAction(Action.ZoomOut)))
			{
				if (e.IsAction(Action.ZoomIn) && currentZoomStep < zoomStepMax)
					currentZoomStep += 1;
				else if (e.IsAction(Action.ZoomOut) && currentZoomStep > zoomStepMin)
					currentZoomStep -= 1;
				else
					return;

				var B = Mathf.Log(lowerZoomBound / upperZoomBound, 2.718f) / 17f;
				var A = upperZoomBound;

				//this.m_targetZoomScale = Mathf.Clamp(this.m_targetZoomScale + (!KInputManager.currentControllerIsGamepad ? UnityEngine.Input.mouseScrollDelta.y * 0.1f : (float)((e.IsAction(Action.ZoomIn) ? -0.003 : 0.003))), 0.15f, 2f);
				m_targetZoomScale = A * Mathf.Exp(B * currentZoomStep);

				//this.rectTransform().localScale = new Vector3(this.m_targetZoomScale, this.m_targetZoomScale, 1f);

				e.TryConsume(Action.ZoomIn);
				if (!e.Consumed)
					e.TryConsume(Action.ZoomOut);

			}
		}
	}
}
