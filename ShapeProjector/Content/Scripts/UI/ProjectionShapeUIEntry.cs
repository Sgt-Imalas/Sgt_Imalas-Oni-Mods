using rail;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using UtilLibs.UI.FUI;
using UtilLibs.UIcmp;

namespace ShapeProjector.Content.Scripts.UI
{
	internal class ProjectionShapeUIEntry : KMonoBehaviour
	{
		private FItemPickerArray ShapeSelector;
		private FItemPickerArray OriginSelector;
		private FInputField2 widthInput, heightInput;
		private FButton DeleteEntryButton;
		private FToggle ProjectionActiveToggle, ThiccWallsToggle;
		private FToggleButton LinkToggle;
		private FKnobControl RotationKnob;

		ShapeProjectorLogic _currentProjector;
		ShapeProjectorLogic.ShapeData _currentData;
		GameObject _linkBroken, _linkFixed;

		bool _init = false;
		internal void Init()
		{
			if (_init)
				return;
			_init = true;

			ShapeSelector = transform.Find("SymbolPicker").gameObject.AddOrGet<FItemPickerArray>();
			ShapeSelector.Init(ModAssets.GetShapeIdWithSprite(), null);
			ShapeSelector.OnSelectionChanged += OnShapeSelected;

			OriginSelector = transform.Find("OriginPicker").gameObject.AddOrGet<FItemPickerArray>();
			OriginSelector.Init(ModAssets.GetOriginIdWithSprite(), ModAssets.OriginIconSort);
			OriginSelector.OnSelectionChanged += OnOriginSelected;

			DeleteEntryButton = transform.Find("DeleteButton").gameObject.AddOrGet<FButton>();
			DeleteEntryButton.OnClick += OnDeleteButtonClicked;

			RotationKnob = transform.Find("Dimensions/Knob").gameObject.AddOrGet<FKnobControl>();
			RotationKnob.AutoPickChildKnob();
			RotationKnob.MinStep = 90;
			RotationKnob.OnRotationChanged += OnRotationChanged;

			widthInput = transform.Find("Dimensions/WidthInput").gameObject.AddOrGet<FInputField2>();
			widthInput.AddEndListener(WidthInputChanged);
			widthInput.SetTextFromData("0");
			widthInput.ClearPlace();

			heightInput = transform.Find("Dimensions/HeightInput").gameObject.AddOrGet<FInputField2>();
			heightInput.AddEndListener(HeightInputChanged);
			heightInput.SetTextFromData("0");
			heightInput.ClearPlace();

			LinkToggle = transform.Find("Dimensions/LinkToggleButton").gameObject.AddOrGet<FToggleButton>();
			LinkToggle.OnSelectionChange += OnLinkToggleChanged;
			_linkBroken = LinkToggle.transform.Find("Broken").gameObject;
			_linkFixed = LinkToggle.transform.Find("Linked").gameObject;

			ProjectionActiveToggle = transform.Find("ProjectionActive").gameObject.AddOrGet<FToggle>();
			ProjectionActiveToggle.SetCheckmark("Checkbox/Checkmark");
			ProjectionActiveToggle.SetOnFromCode(true);
			ProjectionActiveToggle.OnChange += OnProjectionActiveToggleChanged;

			ThiccWallsToggle = transform.Find("ThiccWalls").gameObject.AddOrGet<FToggle>();
			ThiccWallsToggle.SetCheckmark("Checkbox/Checkmark");
			ThiccWallsToggle.SetOnFromCode(false);
			ThiccWallsToggle.OnChange += OnThickWallsToggleChanged;

			//UIUtils.AddSimpleTooltipToObject(transform.Find("MaterialSwitch/PerBuildingOverrides/Label").gameObject, PERBUILDINGOVERRIDES.TOOLTIP);
		}
		#region handlers
		void OnRotationChanged(float percentage)
		{
			SgtLogger.l("Updating rotation state of " + _currentData.shapeId + " to  " + percentage + " -> " + percentage * 4);
			_currentData.rotationState = Mathf.RoundToInt(percentage * 4f);
			UpdateLinkedHeight();
			_currentProjector?.RefreshVisibility(_currentData);
		}
		void OnThickWallsToggleChanged(bool isOn)
		{
			if (_currentData == null)
				return;
			_currentData.isThicc = isOn;
			_currentProjector?.RefreshVisibility(_currentData);
		}
		void OnProjectionActiveToggleChanged(bool isOn)
		{
			if (_currentData == null)
				return;
			_currentData.isActive = isOn;
			_currentProjector?.RefreshVisibility(_currentData);
		}

		void OnLinkToggleChanged(bool isOn)
		{
			if (_currentData == null)
				return;
			_currentData.linkedDimensions = isOn;
			SetLinkToggleState(isOn);
			SetHeightInputInteractable();
			UpdateLinkedHeight();
			RefreshDimensionInputs();
			_currentProjector?.RefreshDimensions(_currentData);
		}
		void SetLinkToggleState(bool isLinked)
		{
			if (LinkToggle == null)
				return;
			LinkToggle.SetIsSelected(isLinked);
			_linkBroken.SetActive(!isLinked);
			_linkFixed.SetActive(isLinked);
		}

		void WidthInputChanged(string value)
		{
			if (_currentData == null)
				return;
			if (!int.TryParse(value, out int x))
			{
				return;
			}
			if (x <= 0)
				x = 1;
			_currentData.width = x;
			UpdateLinkedHeight();
			RefreshDimensionInputs();
			_currentProjector?.RefreshDimensions(_currentData);
		}
		void HeightInputChanged(string value)
		{
			if (_currentData == null)
				return;
			if (!int.TryParse(value, out int y))
			{
				return;
			}
			if (y <= 0)
				y = 1;
			_currentData.height = y;
			RefreshDimensionInputs();
			_currentProjector?.RefreshDimensions(_currentData);
		}
		void OnShapeSelected(string shapeId)
		{
			if (_currentData == null)
				return;
			_currentData.shapeId = shapeId;
			UpdateLinkedHeight();
			_currentProjector?.RefreshDimensions(_currentData);
		}
		void OnOriginSelected(string originId)
		{
			if (_currentData == null)
				return;
			_currentData.originId = originId;
			_currentProjector?.RefreshDimensions(_currentData);
		}
		void OnDeleteButtonClicked()
		{
			if (_currentData == null)
				return;
			_currentProjector?.RemoveShape(_currentData);
			transform.SetAsLastSibling();
			gameObject.SetActive(false);
		}
		#endregion
		void SetHeightInputInteractable()
		{
			if (_currentData == null)
				return;
			heightInput.SetInteractable(!_currentData.linkedDimensions);
		}
		void RefreshDimensionInputs()
		{
			if (_currentData == null)
				return;
			widthInput.SetTextFromData(_currentData.width.ToString());
			heightInput.SetTextFromData(_currentData.height.ToString());
			SetLinkToggleState(_currentData.linkedDimensions);
			UpdateLinkedHeight();
			SetHeightInputInteractable();
		}
		void UpdateLinkedHeight()
		{
			if (_currentData == null)
				return;
			if (_currentData.linkedDimensions)
			{
				_currentData.height = ModAssets.GetLinkedHeight(_currentData, _currentData.width);
				heightInput.SetTextFromData(_currentData.height.ToString());
			}
		}

		internal void UpdateDisplayData(ShapeProjectorLogic target, ShapeProjectorLogic.ShapeData shape)
		{
			_currentProjector = target;
			_currentData = shape;
			RotationKnob.SetAngle(-shape.rotationState / 4f * 360f);
			ProjectionActiveToggle.SetOnFromCode(shape.isActive);
			ThiccWallsToggle.SetOnFromCode(shape.isThicc);
			ShapeSelector.SetSelected(shape.shapeId);
			RefreshDimensionInputs();
		}
	}
}
