using Rockets_TinyYetBig.Content.ModDb.RocketBlueprintData;
using Rockets_TinyYetBig.Content.Scripts.UI.UIComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using UtilLibs;
using UtilLibs.UIcmp;
using static Rockets_TinyYetBig.STRINGS.UI.ROCKETBLUEPRINTS_SECONDARYSIDESCREEN;

namespace Rockets_TinyYetBig.Content.Scripts.UI.Sidescreens
{
	public class RocketBlueprintsSecondarySidescreen : KScreen
	{
		private bool _init;
		private RocketPreviewVis _visPrefab;

		private GameObject _visContainer;
		private List<GameObject> _visualizers = [];


		private RocketBlueprintEntry _entryPrefab;

		private GameObject _entryContainer;
		private Dictionary<RocketBlueprint, RocketBlueprintEntry> _entries = [];

		private FButton _loadOrGenerateButton;
		private Image _buttonImage;
		private LocText _buttonText;
		private FInputField2 _textInputField;

		private RocketBlueprint _temporary = null, _selected = null;

		private Sprite _plus, _build;


		private LaunchPad _targetPad;
		private void Init()
		{
			if (_init)
				return;
			_init = true;
			_plus = Assets.GetSprite("icon_positive");
			_build = Assets.GetSprite("icon_archetype_build");

			//"Preview" - header
			UIUtils.TryChangeText(transform, "Preview/Title/TitleText", global::STRINGS.UI.OUTFIT_BROWSER_SCREEN.COLUMN_HEADERS.DETAILS_HEADER);
			_visContainer = transform.Find("Preview/Body").gameObject;
			_visPrefab = transform.Find("Preview/Body/Prefab").gameObject.AddOrGet<RocketPreviewVis>();
			_visPrefab.gameObject.SetActive(false);


			_entryContainer = transform.Find("Body/ScrollRectContainer").gameObject;
			_entryPrefab = transform.Find("Body/ScrollRectContainer/BlueprintEntryPrefab").gameObject.AddOrGet<RocketBlueprintEntry>();
			_entryPrefab.gameObject.SetActive(false);

			_loadOrGenerateButton = transform.Find("UseButton").gameObject.AddOrGet<FButton>();
			_loadOrGenerateButton.OnClick += OnButtonClicked;
			_buttonImage = transform.Find("UseButton/ButtonIcon").gameObject.GetComponent<Image>();
			_buttonText = transform.Find("UseButton/Text").gameObject.GetComponent<LocText>();

			_textInputField = transform.Find("BlueprintID").gameObject.AddOrGet<FInputField2>();
			_textInputField.OnValueChanged.AddListener(TextInputChanged);
			//_textInputField.OnSelect.AddListener(OnStartedTyping);

			foreach (var bp in RocketBlueprintsDb.GetBlueprints())
				AddOrGetBlueprintEntry(bp);
		}
		public override void OnSpawn()
		{
			Init();
			base.OnSpawn();
		}
		public override void OnShow(bool show)
		{
			base.OnShow(show);
			if (show) { }
			Init();
		}
		internal void OpenedFrom(LaunchPadSideScreen instance)
		{
			Init();
			_textInputField.Text = string.Empty;
			ClearExistingVisualizers();
			_temporary = null;
			_targetPad = null;
			_selected = null;

			if (instance == null || instance.selectedPad == null)
				return;
			_targetPad = instance.selectedPad;

			bool hasRocket = instance.selectedPad.HasRocket();

			SetButtonInfo(hasRocket);

			if (!hasRocket || instance.selectedPad.LandedRocket.CraftInterface == null)
				return;

			SetVisualizersFromRocket(instance.selectedPad.LandedRocket.CraftInterface);
		}


		void OnButtonClicked()
		{
			_textInputField.Text = string.Empty;
			if (_targetPad == null)
				return;

			_loadOrGenerateButton.SetInteractable(false);

			bool hasRocket = _targetPad.HasRocket();
			if (_selected != null && !hasRocket)
			{
				ConstructRocketOnPad(_selected);
			}
			else if (_temporary != null && hasRocket)
			{
				SaveTemporaryBlueprint();
				DetailsScreen.Instance.ClearSecondarySideScreen();
			}
		}
		private void SaveTemporaryBlueprint()
		{
			string name = _textInputField.Text.Trim();

			_temporary.FriendlyName = name;
			RocketBlueprintsDb.AddNew(_temporary, true);
			AddOrGetBlueprintEntry(_temporary);
		}
		private void ConstructRocketOnPad(RocketBlueprint selected)
		{
			if (_targetPad == null || selected == null || !selected.RocketModules.Any())
				return;

			void ApplyModuleSkin(GameObject go, RocketBlueprintModule module)
			{
				if (!module.Facade.IsNullOrWhiteSpace() && go.TryGetComponent<BuildingFacade>(out var skinHandler))
				{
					var skin = Db.GetBuildingFacades().Get(module.Facade);
					if (skin != null)
						skinHandler.ApplyBuildingFacade(skin);
				}
			}

			var firstModule = selected.RocketModules.First();

			GameObject mostBottomRocketModule = _targetPad.AddBaseModule(firstModule.def, [.. firstModule.SelectedElements.Select(e => e.ToTag())]);
			ApplyModuleSkin(mostBottomRocketModule, firstModule);
			ReorderableBuilding moduleToAttach = mostBottomRocketModule.GetComponent<ReorderableBuilding>();

			for (int i = 1; i < selected.RocketModules.Count; i++)
			{
				var nextModule = selected.RocketModules[i];
				if (nextModule.Valid == false)
					continue;
				var nextModuleGO = moduleToAttach.AddModule(nextModule.def, [.. nextModule.SelectedElements.Select(e => e.ToTag())]);
				moduleToAttach = nextModuleGO.GetComponent<ReorderableBuilding>();

				ApplyModuleSkin(nextModuleGO, nextModule);
			}
		}

		void TextInputChanged(string newText)
		{
			newText = newText.Trim();

			if (newText.IsNullOrWhiteSpace() || _targetPad == null)
				return;

			SetButtonInfo(_targetPad.HasRocket());
		}

		void SetButtonInfo(bool hasRocket)
		{
			if (hasRocket)
			{
				_buttonImage.sprite = _plus;
				_buttonText.SetText(USEBUTTON.TEXT);
				_loadOrGenerateButton.SetInteractable(_temporary != null && !_textInputField.Text.IsNullOrWhiteSpace());
			}
			else
			{
				_buttonImage.sprite = _build;
				_buttonText.SetText(USEBUTTON.TEXT_USE);
				_loadOrGenerateButton.SetInteractable(_selected != null);
			}
		}

		void ClearExistingVisualizers()
		{
			foreach (var item in _visualizers)
			{
				UnityEngine.Object.Destroy(item);
			}
			_visualizers.Clear();
		}

		void SetVisualizersFromRocket(CraftModuleInterface rocketInterface)
		{
			if (rocketInterface == null || rocketInterface.ClusterModules == null) return;

			List<Building> modules = new();

			for (int i = 0; i < rocketInterface.ClusterModules.Count; i++)
			{
				RocketModuleCluster module = rocketInterface.ClusterModules[i].Get();
				if (module == null)
				{
					SgtLogger.warning("module was null?!");
					continue;
				}
				if (!module.TryGetComponent<Building>(out var moduleBuilding))
				{
					SgtLogger.warning(module + " had no building component!");
					continue;
				}
				modules.Add(moduleBuilding);
			}
			modules.Sort((a, b) => a.NaturalBuildingCell() - b.NaturalBuildingCell());
			_temporary = RocketBlueprint.Generate(modules);
			SetVisualizers(_temporary);
		}
		void SetVisualizers(RocketBlueprint bp)
		{
			float totalRocketTileHeight = 0;
			float maxRocketWidth = 3;//all modules are at least 3 wide
			for (int i = 0; i < bp.RocketModules.Count; i++)
			{
				var moduleEntry = bp.RocketModules[i];

				totalRocketTileHeight += moduleEntry.height;
				if (maxRocketWidth < moduleEntry.width)
					maxRocketWidth = moduleEntry.width;
			}

			//dimensions - border buffer, 15px up/down, 10px left/right
			float containerHeight = 268 - 30;
			float containerwidth = 138 - 20f;

			float maxPx_X = Mathf.Clamp(containerwidth / maxRocketWidth, 6, containerwidth);
			float maxPx_Y = Mathf.Clamp(containerHeight / totalRocketTileHeight, 6, containerHeight);

			int pxPerTile = Mathf.RoundToInt(Mathf.Min(maxPx_Y, maxPx_X));

			//SgtLogger.l($"Container Dimensions: {containerwidth}x{containerHeight}, rocketDimensions:{maxRocketWidth}x{totalRocketTileHeight}, maxPx_X: {maxPx_X}, maxPx_Y: {maxPx_Y}");

			foreach (var module in bp.RocketModules)
			{
				var moduleItem = Util.KInstantiateUI<RocketPreviewVis>(_visPrefab.gameObject, _visContainer);
				if (module.Valid)
					moduleItem.Init(module.def, pxPerTile);
				else
					moduleItem.InitMissing(module, pxPerTile);
				moduleItem.transform.SetAsFirstSibling();
				moduleItem.gameObject.SetActive(true);
				_visualizers.Add(moduleItem.gameObject);
			}
		}

		RocketBlueprintEntry AddOrGetBlueprintEntry(RocketBlueprint bp)
		{
			if (_entries.TryGetValue(bp, out var entry))
				return entry;

			var moduleItem = Util.KInstantiateUI<RocketBlueprintEntry>(_entryPrefab.gameObject, _entryContainer);
			moduleItem.blueprint = bp;
			moduleItem.gameObject.SetActive(true);
			_entries.Add(bp, moduleItem);
			return moduleItem;
		}
	}
}
