using ClusterTraitGenerationManager.ClusterData;
using ProcGen;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using UtilLibs.UIcmp;
using static ClusterTraitGenerationManager.ClusterData.CGSMClusterManager;
using static ClusterTraitGenerationManager.STRINGS.UI.CGMEXPORT_SIDEMENUS;
using static ClusterTraitGenerationManager.STRINGS.UI.CGMEXPORT_SIDEMENUS.TRAITPOPUP.SCROLLAREA.CONTENT.LISTVIEWENTRYPREFAB;

namespace ClusterTraitGenerationManager.UI.Screens
{
	internal class TraitSelectorScreen : FScreen
	{
		public class BlacklistTrait : MonoBehaviour
		{
			public LocText buttonDescription;
			public Image backgroundImage;
			public Color originalColor;
			public FButton ToggleBlacklistTrait;
			public string referencedTraitId;

			public void Init(string traitID)
			{

				gameObject.transform.Find("AddThisTraitButton").gameObject.SetActive(true);
				buttonDescription = gameObject.transform.Find("AddThisTraitButton/Text").GetComponent<LocText>();
				ToggleBlacklistTrait = gameObject.transform
					.Find("AddThisTraitButton")
					.FindOrAddComponent<FButton>();
				backgroundImage = gameObject.transform.Find("Background").GetComponent<Image>();
				originalColor = backgroundImage.color;
				referencedTraitId = traitID;
				ToggleBlacklistTrait.OnClick +=
					() =>
					{
						bool isBlacklisted = CGSMClusterManager.ToggleRandomTraitBlacklist(referencedTraitId);
						UpdateState(isBlacklisted);
					};

				RefreshState();
			}

			public void RefreshState()
			{
				UpdateState(CGSMClusterManager.RandomTraitInBlacklist(referencedTraitId));
			}
			public void UpdateState(bool isInBlacklist)
			{
				Color logicColour = isInBlacklist ? GlobalAssets.Instance.colorSet.logicOff : GlobalAssets.Instance.colorSet.logicOn;
				logicColour.a = 1f;
				backgroundImage.color = Color.Lerp(logicColour, originalColor, 0.8f);
				buttonDescription.text = isInBlacklist ? TOGGLETRAITBUTTON.REMOVEFROMBLACKLIST : TOGGLETRAITBUTTON.ADDTOBLACKLIST;
				UIUtils.AddSimpleTooltipToObject(ToggleBlacklistTrait.transform, isInBlacklist ? TOGGLETRAITBUTTON.REMOVEFROMBLACKLISTTOOLTIP : TOGGLETRAITBUTTON.ADDTOBLACKLISTTOOLTIP);
			}
		}


		Dictionary<string, BlacklistTrait> BlacklistedRandomTraits = new Dictionary<string, BlacklistTrait>();
		public static TraitSelectorScreen Instance { get; private set; }

		public FToggle ToggleTraitRule;


		Dictionary<string, GameObject> Traits = new Dictionary<string, GameObject>();
		public StarmapItem SelectedPlanet;
		public static System.Action OnCloseAction;

		public bool IsCurrentlyActive = false;
		bool isEditingRandomBlacklist = false;
		bool ignoreWorldTraitRules = false;

		public static void InitializeView(StarmapItem _planet, System.Action onclose, bool editingRandomBlacklist = false)
		{
			if (Instance == null)
			{
				var screen = Util.KInstantiateUI(ModAssets.TraitPopup, FrontEndManager.Instance.gameObject, true);
				Instance = screen.AddOrGet<TraitSelectorScreen>();
				Instance.Init();
			}
			OnCloseAction = onclose;

			Instance.Show(true);
			Instance.SelectedPlanet = _planet;
			Instance.ConsumeMouseScroll = true;
			Instance.transform.SetAsLastSibling();
			Instance.isEditingRandomBlacklist = editingRandomBlacklist;
			Instance.SetUIState();
		}

		void SetUIState()
		{
			if (isEditingRandomBlacklist)
			{
				foreach (var traitContainer in Instance.Traits.Values)
				{
					traitContainer.SetActive(false);
				}
				foreach (var traitContainer in Instance.BlacklistedRandomTraits.Values)
				{
					traitContainer.gameObject.SetActive(true);
					traitContainer.RefreshState();
				}
				return;
			}
			else if (SelectedPlanet != null && CustomCluster.HasStarmapItem(SelectedPlanet.id, out var item))
			{
				foreach (var traitContainer in Instance.BlacklistedRandomTraits.Values)
				{
					traitContainer.gameObject.SetActive(false);
				}
				foreach (var traitContainer in Instance.Traits.Values)
				{
					traitContainer.SetActive(false);
				}
				foreach (var activeTrait in item.AllowedPlanetTraits(ignoreWorldTraitRules))
				{
					Instance.Traits[activeTrait.filePath].SetActive(true);
				}
			}
		}

		private GameObject TraitPrefab;
		private GameObject PossibleTraitsContainer;
		private bool init = false;


		private void Init()
		{
			if (init) return;
			init = true;
			TraitPrefab = transform.Find("ScrollArea/Content/ListViewEntryPrefab").gameObject;
			TraitPrefab.SetActive(false);
			PossibleTraitsContainer = transform.Find("ScrollArea/Content").gameObject;
			ToggleTraitRule = transform.Find("Toggle/Background")?.gameObject?.AddOrGet<FToggle>();
			ToggleTraitRule.SetCheckmark("Checkmark");
			ToggleTraitRule.SetOnFromCode(false);
			ToggleTraitRule.OnClick += ToggleTraitRuleOverride;
			UIUtils.AddSimpleTooltipToObject(transform.Find("Toggle").gameObject, TRAITPOPUP.TOGGLE.TOOLTIP);

			var closeButton = transform.Find("CancelButton").FindOrAddComponent<FButton>();
			closeButton.OnClick += () =>
			{
				OnCloseAction.Invoke();
				Show(false);
			};


			InitializeTraitContainer();
		}
		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			ConsumeMouseScroll = true;

			Init();
		}
		void ToggleTraitRuleOverride(bool overrideEnabled)
		{
			ignoreWorldTraitRules = overrideEnabled;
			SetUIState();
		}
		void InitializeTraitContainer()
		{
			foreach (var kvp in ModAssets.AllTraitsWithRandom)
			{
				//SgtLogger.l(kvp.Key, "INIT");

				var TraitHolder = Util.KInstantiateUI(TraitPrefab, PossibleTraitsContainer, true);
				//UIUtils.ListAllChildrenWithComponents(TraitHolder.transform);
				var AddTraitButton = TraitHolder
					//.transform.Find("AddThisTraitButton").gameObject
					.FindOrAddComponent<FButton>();
				Strings.TryGet(kvp.Value.name, out var name);
				Strings.TryGet(kvp.Value.description, out var description);
				var combined = "<color=#" + kvp.Value.colorHex + ">" + name.ToString() + "</color>";

				var icon = TraitHolder.transform.Find("Label/TraitImage").GetComponent<Image>();
				icon.sprite = ModAssets.GetTraitSprite(kvp.Value);
				icon.color = Util.ColorFromHex(kvp.Value.colorHex);

				UIUtils.TryChangeText(TraitHolder.transform, "Label", combined);
				UIUtils.AddSimpleTooltipToObject(TraitHolder.transform, description);


				AddTraitButton.OnClick += () =>
				{
					if (CustomCluster.HasStarmapItem(SelectedPlanet.id, out var item))
					{
						item.AddWorldTrait(kvp.Value);
					}
					CloseThis();
				};
				Traits[kvp.Value.filePath] = TraitHolder;
			}


			var worldTraits = SettingsCache.worldTraits.OrderBy(kvp => Strings.Get(kvp.Value.name).ToString());

			foreach (var kvp in worldTraits)
			{
				var TraitHolder = Util.KInstantiateUI(TraitPrefab, PossibleTraitsContainer, true);
				var blacklistContainer = TraitHolder.AddOrGet<BlacklistTrait>();
				blacklistContainer.Init(kvp.Value.filePath);

				Strings.TryGet(kvp.Value.name, out var name);
				Strings.TryGet(kvp.Value.description, out var description);
				var combined = "<color=#" + kvp.Value.colorHex + ">" + name.ToString() + "</color>";

				var icon = TraitHolder.transform.Find("Label/TraitImage").GetComponent<Image>();
				icon.sprite = ModAssets.GetTraitSprite(kvp.Value);
				icon.color = Util.ColorFromHex(kvp.Value.colorHex);

				UIUtils.TryChangeText(TraitHolder.transform, "Label", combined);
				UIUtils.AddSimpleTooltipToObject(TraitHolder.transform.Find("Label"), description);

				BlacklistedRandomTraits[kvp.Value.filePath] = blacklistContainer;
			}
		}


		public override void Show(bool show = true)
		{
			base.Show(show);
			IsCurrentlyActive = show;
		}
		void CloseThis()
		{
			OnCloseAction.Invoke();
			Show(false);
		}

		public override void OnKeyDown(KButtonEvent e)
		{
			if (e.TryConsume(Action.Escape) || e.TryConsume(Action.MouseRight))
			{
				CloseThis();
			}

			base.OnKeyDown(e);
		}
	}
}
