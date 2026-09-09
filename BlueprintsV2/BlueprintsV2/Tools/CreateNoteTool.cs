using BlueprintsV2.BlueprintData;
using BlueprintsV2.BlueprintsV2.BlueprintData.NoteToolPlacedEntities;
using BlueprintsV2.BlueprintsV2.BlueprintData.OniTogether_Integration;
using BlueprintsV2.BlueprintsV2.BlueprintData.PlannedElements;
using BlueprintsV2.BlueprintsV2.UnityUI;
using BlueprintsV2.Tools;
using FMOD.Studio;
using FMODUnity;
using HarmonyLib;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static STRINGS.ELEMENTS;
using static STRINGS.RESEARCH.TYPES;

namespace BlueprintsV2.BlueprintsV2.Tools
{
	internal class CreateNoteTool : DragTool
	{
		public static CreateNoteTool Instance;
		private EventInstance audioEvent;
		SpriteRenderer visualizerIcon;

		public static void DestroyInstance()
		{
			CreateNoteTool.Instance = null;
		}

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			Instance = this;

			base.OnPrefabInit();

			visualizer = new GameObject("CreateNoteVisualizer");
			visualizer.SetActive(false);
			var color = UIUtils.rgb(244, 171, 57);

			GameObject offsetObject = new GameObject();
			visualizerIcon = offsetObject.AddComponent<SpriteRenderer>();
			visualizerIcon.color = color;
			visualizerIcon.sprite = ModAssets.AddNoteToolIcon_Sprite;

			offsetObject.transform.SetParent(visualizer.transform);
			offsetObject.transform.localPosition = new Vector3(0, Grid.HalfCellSizeInMeters);
			offsetObject.transform.localPosition = new Vector3(-Grid.HalfCellSizeInMeters, 0);
			var sprite = visualizerIcon.sprite;
			offsetObject.transform.localScale = new Vector3(
				Grid.CellSizeInMeters / (sprite.texture.width / sprite.pixelsPerUnit),
				Grid.CellSizeInMeters / (sprite.texture.height / sprite.pixelsPerUnit)
			);

			offsetObject.SetLayerRecursively(LayerMask.NameToLayer("Overlay"));
			visualizer.transform.SetParent(transform);

			GameObject areaVisualizer = Util.KInstantiate(Traverse.Create(DeconstructTool.Instance).Field("areaVisualizer").GetValue<GameObject>());
			areaVisualizer.SetActive(false);

			areaVisualizer.name = "CreateNoteAreaVisualizer";
			areaVisualizerSpriteRenderer = areaVisualizer.GetComponent<SpriteRenderer>();

			areaVisualizer.GetComponent<SpriteRenderer>().color = color;
			areaVisualizer.GetComponent<SpriteRenderer>().material.color = color;
			areaVisualizer.transform.SetParent(transform);
			this.areaVisualizer = areaVisualizer;
			this.areaVisualizerTextPrefab = DigTool.Instance.areaVisualizerTextPrefab;

		}

		public override string GetDragSound() => "";

		public void Activate() => PlayerController.Instance.ActivateTool(this);
		public override void OnActivateTool()
		{
			base.OnActivateTool();
			NoteToolScreen.ShowScreen(true);
			OnElementChanged(null);
		}
		public static void SetElementSelectorVisibility(bool visible)
		{
			if (visible)
			{
				SandboxToolParameterMenu.instance.gameObject.SetActive(true);
				SandboxToolParameterMenu.instance.DisableParameters();
				SandboxToolParameterMenu.instance.brushRadiusSlider.row.SetActive(false);
				SandboxToolParameterMenu.instance.massSlider.row.SetActive(true);
				SandboxToolParameterMenu.instance.temperatureSlider.row.SetActive(true);
				SandboxToolParameterMenu.instance.elementSelector.row.SetActive(true);
				SandboxToolParameterMenu.instance.diseaseSelector.row.SetActive(false);
				SandboxToolParameterMenu.instance.diseaseCountSlider.row.SetActive(false);
				SandboxToolParameterMenu.instance.elementSelector.onValueChanged += Instance.OnElementChanged;
			}
			else
			{
				SandboxToolParameterMenu.instance.elementSelector.onValueChanged -= Instance.OnElementChanged;
				SandboxToolParameterMenu.instance.gameObject.SetActive(false);
			}

		}
		private void OnElementChanged(object _)
		{
			return;
			Element element = ElementLoader.elements[SandboxToolParameterMenu.instance.settings.GetIntSetting("SandboxTools.SelectedElement")];
			switch (element.state & Element.State.Solid)
			{
				case Element.State.Gas:
					visualizerIcon?.sprite = ModAssets.Gas_Placer_Sprite;
					break;
				case Element.State.Liquid:
					visualizerIcon?.sprite = ModAssets.Liquid_Placer_Sprite;
					break;
				case Element.State.Solid:
					visualizerIcon?.sprite = ModAssets.Solid_Placer_Sprite;
					break;
				default:
				case Element.State.Vacuum:
					visualizerIcon?.sprite = ModAssets.Special_Placer_Sprite;
					break;
			}

		}
		void CreateElementNote(int cell)
		{
			if (!Grid.IsValidCell(cell))
				return;
			var settings = SandboxToolParameterMenu.instance.settings;
			Element element = ElementLoader.elements[settings.GetIntSetting("SandboxTools.SelectedElement")];
			var ElementId = element.id;
			var Amount = settings.GetFloatSetting("SandboxTools.Mass");
			var Temperature = settings.GetFloatSetting("SandbosTools.Temperature");

			var data = ElementNote.Create(cell, ElementId, Amount, Temperature, true);
			MP_Helpers.HandleNoteCreation(data);
		}
		void CreateTextNote(int cell)
		{
			if (!Grid.IsValidCell(cell))
				return;

			NoteToolScreen.Instance.GetTextNoteInfo(out string title, out string text, out string symbol, out Color color);

			var data = TextNote.Create(cell, title, text, symbol, color, true);
			MP_Helpers.HandleNoteCreation(data);
		}
		public override void OnDragTool(int cell, int distFromOrigin)
		{
			if (!Grid.IsValidCell(cell))
				return;

			BlueprintNote.ClearExistingNote(cell);

			PopFXManager.Instance.SpawnFX(ModAssets.NoteToolIcon_Sprite, STRINGS.UI.TOOLS.NOTE_TOOL.CREATED, null, Grid.CellToPos(cell), Config.Instance.FXTime);
			if (NoteToolScreen.Instance.IsTextMode)
				CreateTextNote(cell);
			else
				CreateElementNote(cell);
		}
		public override void OnDeactivateTool(InterfaceTool new_tool)
		{
			base.OnDeactivateTool(new_tool);
			SetElementSelectorVisibility(false);
			audioEvent.release();
			NoteToolScreen.ShowScreen(false);
		}
	}
}
