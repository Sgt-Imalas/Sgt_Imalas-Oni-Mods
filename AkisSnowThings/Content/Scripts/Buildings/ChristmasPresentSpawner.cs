using AkisSnowThings.Content.Defs.Entities;
using AkisSnowThings.Content.Scripts.Entities;
using KSerialization;
using NodeEditorFramework.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using UtilLibs.YeetUtils;

namespace AkisSnowThings.Content.Scripts.Buildings
{
	internal class ChristmasPresentSpawner : KMonoBehaviour, ISidescreenButtonControl
	{
		public static readonly int CooldownTimeCycles = 364; //1 cycle leeway

		[Serialize]
		private int GlobalCooldownLoc = 0;

		[MyCmpReq]
		KSelectable selectable;

		public static HashSet<ChristmasPresentSpawner> GlobalSpawners = [];

		public override void OnSpawn()
		{
			base.OnSpawn();
			GlobalSpawners.Add(this);
		}
		public override void OnCleanUp()
		{
			GlobalSpawners.Remove(this);
			base.OnCleanUp();
		}

		internal void OnPresentsCreated()
		{
			GlobalCooldownLoc += CooldownTimeCycles;
		}

		public void CreatePresents()
		{
			OnPresentsCreated();
			var myWorld = this.GetMyParentWorldId();
			foreach (var entry in GlobalSpawners)
			{
				if (entry.IsNullOrDestroyed())
					continue;

				if (entry.GetMyParentWorldId() != myWorld)
					continue;
				entry.OnPresentsCreated();
			}
			SgtLogger.l("Spawning Christmas Presents");
			StartCoroutine(SpawnPresentsDelayed());
		}
		public IEnumerator SpawnPresentsDelayed()
		{
			SgtLogger.l("number of minions: " + Components.LiveMinionIdentities.Count);
			var myWorld = this.GetMyParentWorldId();

			foreach (MinionIdentity duplicant in Components.LiveMinionIdentities)
			{
				if (duplicant.GetMyParentWorldId() != myWorld)
					continue;

				SpawnPresent(duplicant);
				yield return new WaitForSeconds(0.5f);
			}
		}

		public void SpawnPresent(MinionIdentity dupe)
		{
			var present = Util.KInstantiate(Assets.GetPrefab(FestivePresentConfig.ID), Grid.CellToPosCBC(Grid.CellAbove(Grid.PosToCell(transform.position)), Grid.SceneLayer.Ore));
			present.GetComponent<Ownable>().Assign(dupe.assignableProxy.Get());
			present.SetActive(true);

			YeetHelper.YeetRandomly(present, false, 3, 5, true);
		}

		#region isidescreenbuttoncontrol


		public string SidescreenButtonText => SidescreenButtonInteractable() ? Strings.Get("STRINGS.UI.GIVEDUPEPRESENTS.LABEL") : string.Format(Strings.Get("STRINGS.UI.GIVEDUPEPRESENTS.LABEL_COOLDOWN"), GlobalCooldown - GameClock.Instance.GetCycle());

		public string SidescreenButtonTooltip => Strings.Get("STRINGS.UI.GIVEDUPEPRESENTS.TOOLTIP");

		public int ButtonSideScreenSortOrder() => 200;

		public int HorizontalGroupID() => -1;

		public void OnSidescreenButtonPressed()
		{
			CreatePresents();
			//Ui Refresh
			SelectTool.Instance.Select(selectable);
		}
		public void SetButtonTextOverride(ButtonMenuTextOverride textOverride)
		{
		}

		public bool SidescreenButtonInteractable() => GlobalCooldown <= GameClock.Instance.GetCycle();

		public bool SidescreenEnabled() => true;
		#endregion
	}
}
