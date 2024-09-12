using Rockets_TinyYetBig.Docking;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using UtilLibs.UIcmp;

namespace Rockets_TinyYetBig.UI_Unity
{
	internal class CrewAssignmentSidescreen : KScreen
	{
		GameObject OwnDupeContainer;

		GameObject TargetDupeContainer;

		GameObject OwnDupePreset;
		GameObject TargetDupePreset;

		Dictionary<MinionIdentity, GameObject> OwnDupePresets = new Dictionary<MinionIdentity, GameObject>();
		Dictionary<MinionIdentity, GameObject> TargetDupePresets = new Dictionary<MinionIdentity, GameObject>();


		LocText OwnHeader, TargetHeader;
		int firstWorldId = -1, secondWorldId = -1;

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			Init();
		}

		void Init()
		{
			//UIUtils.ListAllChildrenPath(this.transform);
			OwnDupeContainer = transform.Find("OwnDupesContainer/ScrollRectContainer").gameObject;
			TargetDupeContainer = transform.Find("TargetDupesContainer/ScrollRectContainer").gameObject;

			OwnDupePreset = transform.Find("OwnDupesContainer/ScrollRectContainer/ItemPrefab").gameObject;
			TargetDupePreset = transform.Find("TargetDupesContainer/ScrollRectContainer/ItemPrefab").gameObject;
			OwnHeader = transform.Find("ContentHeaderOwn/TitleText").GetComponent<LocText>();
			TargetHeader = transform.Find("ContentHeaderDocked/TitleText").GetComponent<LocText>();


			OwnDupePreset.SetActive(false);
			TargetDupePreset.SetActive(false);

			foreach (MinionIdentity dupe in Components.LiveMinionIdentities)
			{
				AddDupeEntry(dupe, true);
				AddDupeEntry(dupe, false);
			}

			Components.LiveMinionIdentities.OnAdd += (minion) =>
			{
				AddDupeEntry(minion, true);
				AddDupeEntry(minion, false);
			};
		}

		public override void OnShow(bool show)
		{
			base.OnShow(show);
			if (show)
			{
				foreach (var dupe in OwnDupePresets)
				{
					UpdateName(dupe.Key, dupe.Value);
				}
				foreach (var dupe in TargetDupePresets)
				{
					UpdateName(dupe.Key, dupe.Value);
				}
			}
		}

		public void UpdateForConnection(int _firstWorld, int _secondWorld)
		{
			SgtLogger.l("updating for connected: " + _firstWorld + ", " + _secondWorld);
			firstWorldId = _firstWorld;
			secondWorldId = _secondWorld;

			var own = ClusterManager.Instance.GetWorld(firstWorldId);
			OwnHeader.SetText(string.Format(STRINGS.UI.DOCKINGTRANSFERSCREEN.DUPESASSIGNEDTO, own.GetProperName()));
			var target = ClusterManager.Instance.GetWorld(secondWorldId);
			TargetHeader.SetText(string.Format(STRINGS.UI.DOCKINGTRANSFERSCREEN.DUPESASSIGNEDTO, target.GetProperName()));



			foreach (var kvp in OwnDupePresets)
			{
				kvp.Value.SetActive(false);
			}
			foreach (var kvp in TargetDupePresets)
			{
				kvp.Value.SetActive(false);
			}

			foreach (var Duplicant in Components.LiveMinionIdentities.Items)
			{
				if (!OwnDupePresets.ContainsKey(Duplicant))
				{
					AddDupeEntry(Duplicant, true);
					AddDupeEntry(Duplicant, false);
				}
				if (DockingManagerSingleton.Instance.TryGetMinionAssignment(Duplicant, out int worldId))
				{
					SgtLogger.l(Duplicant.GetProperName() + " currently assigned to " + worldId, "Current Docking dupe");
					SgtLogger.l(ClusterManager.Instance.GetWorld(firstWorldId).GetProperName(), "WorldFirst - " + firstWorldId);
					SgtLogger.l(ClusterManager.Instance.GetWorld(secondWorldId).GetProperName(), "WorldSecond - " + secondWorldId);

					if (worldId == firstWorldId)
					{
						OwnDupePresets[Duplicant].SetActive(true);
						TargetDupePresets[Duplicant].SetActive(false);
					}
					else if (worldId == secondWorldId)
					{
						OwnDupePresets[Duplicant].SetActive(false);
						TargetDupePresets[Duplicant].SetActive(true);
					}

				}
				else
					SgtLogger.l(Duplicant.GetProperName() + " is not currently assigned to anything rn");

			}



		}


		void UpdateName(MinionIdentity identity, GameObject target)
		{
			UIUtils.TryChangeText(target.transform, ("TitleText"), identity.name);
		}



		void AddDupeEntry(MinionIdentity minionIdentity, bool ownTrueTargetFalse)
		{
			var DoopEntry = Util.KInstantiateUI(
				ownTrueTargetFalse ? OwnDupePreset : TargetDupePreset,
				ownTrueTargetFalse ? OwnDupeContainer : TargetDupeContainer, true);
			DoopEntry.transform.Find("DupeHeadContainer/Image").GetComponent<Image>().sprite = Db.Get().Personalities.Get(minionIdentity.personalityResourceId).GetMiniIcon();
			UpdateName(minionIdentity, DoopEntry);
			UIUtils.TryChangeText(DoopEntry.transform, "AssignButtonContainer/AssignButton/Label", STRINGS.UI.DOCKINGTRANSFERSCREEN.ASSIGNTOOTHERBUTTONTEXT);
			var btn = DoopEntry.transform.Find("AssignButtonContainer/AssignButton").gameObject.AddOrGet<FButton>();

			if (ownTrueTargetFalse)
			{
				btn.OnClick += () =>
				{
					DockingManagerSingleton.Instance.SetMinionAssignment(minionIdentity.assignableProxy, secondWorldId);
					TargetDupePresets[minionIdentity].SetActive(true);
					OwnDupePresets[minionIdentity].SetActive(false);
				};
				OwnDupePresets.Add(minionIdentity, DoopEntry);
			}
			else
			{
				btn.OnClick += () =>
				{
					DockingManagerSingleton.Instance.SetMinionAssignment(minionIdentity.assignableProxy, firstWorldId);
					TargetDupePresets[minionIdentity].SetActive(false);
					OwnDupePresets[minionIdentity].SetActive(true);

				};
				TargetDupePresets.Add(minionIdentity, DoopEntry);
			}

		}
	}
}
