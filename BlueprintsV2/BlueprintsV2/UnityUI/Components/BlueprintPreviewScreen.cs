using BlueprintsV2.BlueprintData;
using BlueprintsV2.BlueprintsV2.BlueprintData.NoteToolPlacedEntities;
using BlueprintsV2.BlueprintsV2.UnityUI.Components.PreviewVisualizers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UtilLibs;
using UtilLibs.UIcmp;

namespace BlueprintsV2.BlueprintsV2.UnityUI.Components
{
	internal class BlueprintPreviewScreen : FScreen
	{
		[MyCmpGet] new RectTransform _rectTransform;
		bool _init = false;
		GameObject BuildingEntry;
		List<GameObject> BPVisualizers = new List<GameObject>();
		float lowerZoomBound = 2.5f, upperZoomBound = 0.2f;
		float currentZoomStep = 1f;
		float zoomStepMin = -2, zoomStepMax = 15;
		float m_targetZoomScale = 0.25f, m_currentZoomScale = 0.25f;
		bool _cursorInside = false;

		void Init()
		{
			if (_init)
				return;
			_init = true;
			BuildingEntry = transform.Find("BuildingPrefab").gameObject;
			BuildingEntry.SetActive(false);
		}
		public override void OnSpawn()
		{
			base.OnSpawn();
		}
		public void LoadBlueprintPreview(Blueprint blueprint)
		{
			Init();
			foreach (var entry in BPVisualizers)
				Destroy(entry);


			BPVisualizers.Clear();
			Vector2I dimensions = blueprint.VisibleDimensions;
			SgtLogger.l("Visualizing " + blueprint.FriendlyName + " with dimensions: " + dimensions);
			//dimensions = new(dimensions.X+ 4, dimensions.Y+ 4);
			Vis_TilePreview.ClearTileArray(dimensions);
			float xOffset = (dimensions.X / 2f);
			float yOffset = (dimensions.Y / 2f);
			dimensions = new(dimensions.X + 2, dimensions.Y + 2);
			_rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, dimensions.X * 100f);
			_rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, dimensions.Y * 100f);

			Vector3 centerOffset = new(xOffset * 100, yOffset * 100);

			var buildings = blueprint.BuildingConfigurations.OrderBy(b => b.BuildingDef?.SceneLayer ?? 0);
			foreach (var building in buildings)
			{
				if (building.BuildingDef == null)
					continue;
				var visType = ModAssets.GetVisualizerType(building.BuildingDef);
				var entry = GameObject.Instantiate(BuildingEntry, transform);
				entry.transform.localPosition = GetCellCenterPos(building.Offset, building.BuildingDef.SceneLayer); //new(building.Offset.X * 100f - xOffset, building.Offset.Y * 100f - yOffset);
				entry.transform.localPosition -= centerOffset;
				switch (visType)
				{
					case VisualizerType.TILE:
						entry.AddOrGet<Vis_TilePreview>().Init(building); break;
					case VisualizerType.UTILITY:
						entry.AddOrGet<Vis_ConduitPreview>().Init(building); break;
					default:
						entry.AddOrGet<Vis_BuildingPreview>().Init(building); break;
				}
				entry.SetActive(true);
				BPVisualizers.Add(entry);
			}
			Vis_TilePreview.ConnectAll();
			foreach (var note in blueprint.WorldNotes)
			{
				var entry = GameObject.Instantiate(BuildingEntry, transform);
				entry.transform.localPosition = GetCellCenterPos(note.Key, Grid.SceneLayer.FXFront); //new(building.Offset.X * 100f - xOffset, building.Offset.Y * 100f - yOffset);
				entry.transform.localPosition -= centerOffset;

				entry.SetActive(true);
				var image = entry.transform.Find("TileMask/TileVis").gameObject.GetComponent<Image>();
				image.gameObject.SetActive(true);
				image.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 100);
				image.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 100);
				BlueprintNoteData noteData = note.Value;
				switch (noteData.Type)
				{
					case BlueprintNoteData.NoteType.Text:
						image.color = noteData.SymbolTint;
						image.sprite = ModAssets.Note_Placer_Sprite;
						UIUtils.AddSimpleTooltipToObject(image.gameObject, noteData.Title + "\n" + noteData.Text);
						break;
					case BlueprintNoteData.NoteType.Element:
						if (ElementLoader.FindElementByHash(noteData.ElementId) == null)
						{
							Destroy(entry);
							continue;
						}
						else
						{
							SetElementInfo(image, noteData);
						}
						break;
					default:
						Destroy(entry);
						continue;

				}

				BPVisualizers.Add(entry);
			}

		}
		void SetElementInfo(Image image, BlueprintNoteData noteData)
		{
			var element = ElementLoader.FindElementByHash(noteData.ElementId);

			bool vaccuum = false;
			switch (element.state & Element.State.Solid)
			{
				case Element.State.Gas:
					image.sprite = ModAssets.Gas_Placer_Sprite;
					break;
				case Element.State.Liquid:
					image.sprite = ModAssets.Liquid_Placer_Sprite;
					break;
				case Element.State.Solid:
					image.sprite = ModAssets.Solid_Placer_Sprite;
					break;
				default:
				case Element.State.Vacuum:
					vaccuum = true;
					image.sprite = ModAssets.Special_Placer_Sprite;
					break;
			}
			if (!vaccuum)
				image.color = element.substance.colour;
			string mass = GameUtil.GetFormattedMass(noteData.ElementMass);
			if(vaccuum)
				UIUtils.AddSimpleTooltipToObject(image.gameObject, element.name);
			else
				UIUtils.AddSimpleTooltipToObject(image.gameObject, element.name + ": " + mass);
		}

		Vector3 GetCellCenterPos(Vector2 offset, Grid.SceneLayer layer)
		{
			//return new Vector3(offset.x * 100, offset.y * 100f, Grid.GetLayerZ(layer));
			return new Vector3(offset.x * 100f + 50, offset.y * 100f + .001f, Grid.GetLayerZ(layer));
		}

		public override void ScreenUpdate(bool topLevel)
		{
			this.m_currentZoomScale = Mathf.Lerp(this.m_currentZoomScale, this.m_targetZoomScale, Mathf.Min(4f * Time.unscaledDeltaTime, 1f));
			Vector2 mousePos = (Vector2)KInputManager.GetMousePos();
			Vector3 vector3_1 = _rectTransform.InverseTransformPoint((Vector3)mousePos);
			_rectTransform.localScale = new Vector3(this.m_currentZoomScale, this.m_currentZoomScale, 1f);
			Vector3 vector3_2 = _rectTransform.InverseTransformPoint((Vector3)mousePos);
			RectTransform content = _rectTransform;
			content.localPosition = content.localPosition + (vector3_2 - vector3_1) * this.m_currentZoomScale;
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
				this.m_targetZoomScale = A * Mathf.Exp(B * currentZoomStep);

				//this.rectTransform().localScale = new Vector3(this.m_targetZoomScale, this.m_targetZoomScale, 1f);

				e.TryConsume(Action.ZoomIn);
				if (!e.Consumed)
					e.TryConsume(Action.ZoomOut);

			}
		}
	}
}
