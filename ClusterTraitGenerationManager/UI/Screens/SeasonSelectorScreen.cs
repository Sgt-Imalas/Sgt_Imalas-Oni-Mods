using ClusterTraitGenerationManager.ClusterData;
using Klei.AI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using UtilLibs.UIcmp;
using static ClusterTraitGenerationManager.ClusterData.CGSMClusterManager;
using static ClusterTraitGenerationManager.STRINGS.UI.CGM_MAINSCREENEXPORT.DETAILS.CONTENT.SCROLLRECTCONTAINER;

namespace ClusterTraitGenerationManager.UI.Screens
{
	internal class SeasonSelectorScreen : FScreen
	{
		public static SeasonSelectorScreen Instance { get; private set; }

		Dictionary<string, GameObject> Seasons = new Dictionary<string, GameObject>();
		public StarmapItem SelectedPlanet;
		public static System.Action OnCloseAction;

		public bool IsCurrentlyActive = false;
		public string ReplaceOldSeason = string.Empty;

		public static void InitializeView(StarmapItem _planet, System.Action onclose, string seasonToReplace = "")
		{
			if (Instance == null)
			{
				var screen = Util.KInstantiateUI(ModAssets.TraitPopup, FrontEndManager.Instance.gameObject, true);
				Instance = screen.AddOrGet<SeasonSelectorScreen>();
				Instance.Init();
			}
			OnCloseAction = onclose;
			Instance.ReplaceOldSeason = seasonToReplace;
			Instance.Show(true);
			Instance.SelectedPlanet = _planet;
			Instance.ConsumeMouseScroll = true;
			Instance.transform.SetAsLastSibling();


			if (CustomCluster.HasStarmapItem(_planet.id, out var item))
			{
				foreach (var traitContainer in Instance.Seasons.Values)
				{
					traitContainer.SetActive(true);
				}
				foreach (var activeSeason in item.CurrentMeteorSeasons)
				{
					Instance.Seasons[activeSeason.Id].SetActive(false);
				}
			}
		}

		private GameObject SeasonPrefab;
		private GameObject PossibleSeasonContainer;
		private bool init = false;
		private void Init()
		{
			if (init) return;
			init = true;
			SeasonPrefab = transform.Find("ScrollArea/Content/ListViewEntryPrefab").gameObject;
			PossibleSeasonContainer = transform.Find("ScrollArea/Content").gameObject;
			transform.Find("Toggle")?.gameObject?.SetActive(false);
			UIUtils.TryChangeText(transform, "Text", METEORSEASONCYCLE.CONTENT.TITLE);
			UIUtils.TryChangeText(PossibleSeasonContainer.transform, "NoTraitAvailable/Label", METEORSEASONCYCLE.CONTENT.NOSEASONTYPESAVAILABLE);

			var closeButton = transform.Find("CancelButton").FindOrAddComponent<FButton>();
			closeButton.OnClick += () =>
			{
				OnCloseAction.Invoke();
				Show(false);
			};


			InitializeMeteorSeasonContainer();
		}
		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			ConsumeMouseScroll = true;

			Init();
		}

		void InitializeMeteorSeasonContainer()
		{
			foreach (var gameplaySeason in Db.Get().GameplaySeasons.resources)
			{
				//if(gameplaySeason.Id == "LargeImpactor" && DlcManager.IsCorrectDlcSubscribed(gameplaySeason.GetRequiredDlcIds(), gameplaySeason.GetForbiddenDlcIds()))
				//{
				//	var Dlc4ImpactorGO = Util.KInstantiateUI(SeasonPrefab, PossibleSeasonContainer, true);

				//	UIUtils.TryChangeText(Dlc4ImpactorGO.transform, "Label", name);
				//	continue;
				//}


				if (!(gameplaySeason is MeteorShowerSeason season)
					|| gameplaySeason.Id.Contains("Fullerene")
					|| gameplaySeason.Id.Contains("TemporalTear")
					|| !DlcManager.IsCorrectDlcSubscribed(gameplaySeason.GetRequiredDlcIds(), gameplaySeason.GetForbiddenDlcIds())
					|| (gameplaySeason.GetRequiredDlcIds() != null && gameplaySeason.GetRequiredDlcIds().Contains(DlcManager.VANILLA_ID) && DlcManager.IsExpansion1Active())
					|| season.clusterTravelDuration <= 0 && DlcManager.IsExpansion1Active()
					)
					continue;
				var meteorSeason = gameplaySeason as MeteorShowerSeason;

				var seasonInstanceHolder = Util.KInstantiateUI(SeasonPrefab, PossibleSeasonContainer, true);


				string name = meteorSeason.Name.Replace("MeteorShowers", string.Empty);
				if (name == string.Empty)
					name = METEORSEASONCYCLE.VANILLASEASON;
				string description = meteorSeason.events.Count == 0 ? METEORSEASONCYCLE.CONTENT.SEASONTYPENOMETEORSTOOLTIP : METEORSEASONCYCLE.CONTENT.SEASONTYPETOOLTIP;

				foreach (var meteorShower in meteorSeason.events)
				{
					var shower = meteorShower as MeteorShowerEvent;
					description += "\n • ";
					description += shower.Id;// Assets.GetPrefab((Tag)meteor.prefab).GetProperName();
					description += ":";
					foreach (var info in shower.GetMeteorsInfo())
					{
						var meteor = Assets.GetPrefab(info.prefab);
						if (meteor == null) continue;

						description += "\n    • ";
						description += meteor.GetProperName();
					}
				}
				UIUtils.AddSimpleTooltipToObject(seasonInstanceHolder.transform, description);

				var icon = seasonInstanceHolder.transform.Find("Label/TraitImage").GetComponent<Image>();
				icon.gameObject.SetActive(false);

				UIUtils.TryChangeText(seasonInstanceHolder.transform, "Label", name);

				var AddTraitButton = seasonInstanceHolder
					//.transform.Find("AddThisTraitButton").gameObject
					.FindOrAddComponent<FButton>();
				///seasonInstanceHolder.transform.Find("AddThisTraitButton/Text").gameObject.FindOrAddComponent<LocText>().text = METEORSEASONCYCLE.SEASONSELECTOR.ADDSEASONTYPEBUTTONLABEL;

				AddTraitButton.OnClick += () =>
				{
					if (CustomCluster.HasStarmapItem(SelectedPlanet.id, out var item))
					{
						if (ReplaceOldSeason != string.Empty)
						{
							item.RemoveMeteorSeason(ReplaceOldSeason);
						}
						item.AddMeteorSeason(gameplaySeason.Id);
					}
					CloseThis();
				};

				Seasons[gameplaySeason.Id] = seasonInstanceHolder;
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
				//SgtLogger.l("CONSUMING 3");
				CloseThis();
			}

			base.OnKeyDown(e);
		}
	}
}
