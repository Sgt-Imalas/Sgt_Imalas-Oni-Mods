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
		private FInputField2 widthInput, heightInput;
		private FButton DeleteEntryButton;
		private FToggle ProjectionActiveToggle, ThiccWallsToggle;
		private FToggleButton LinkToggle;

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

			DeleteEntryButton = transform.Find("DeleteButton").gameObject.AddOrGet<FButton>();
			DeleteEntryButton.OnClick += OnDeleteButtonClicked;

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
			_linkBroken.SetActive(!isOn);
			_linkFixed.SetActive(isOn);
			SetHeightInputInteractable();
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
				_currentData.height = ModAssets.GetLinkedHeight(_currentData.shapeId, _currentData.width);
				heightInput.SetTextFromData(_currentData.height.ToString());
			}
		}

		internal void UpdateDisplayData(ShapeProjectorLogic target, ShapeProjectorLogic.ShapeData shape)
		{
			_currentProjector = target;
			_currentData = shape;
			ProjectionActiveToggle.SetOnFromCode(shape.isActive);
			ThiccWallsToggle.SetOnFromCode(shape.isThicc);
			ShapeSelector.SetSelected(shape.shapeId);
			RefreshDimensionInputs();
		}
	}
}
