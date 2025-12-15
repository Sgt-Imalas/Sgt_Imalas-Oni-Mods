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
		public static Dictionary<int, int> CachedGlobalTimers = [];

		[Serialize]
		int GlobalCooldownLoc = 0;

		[MyCmpReq]
		KSelectable selectable;

		public static HashSet<ChristmasPresentSpawner> GlobalSpawners = [];

		public override void OnSpawn()
		{
			var myWorld = this.GetMyParentWorldId();
			base.OnSpawn();
			if (!CachedGlobalTimers.TryGetValue(myWorld, out int cachedCycle) || cachedCycle < GlobalCooldownLoc)
				UpdateOtherTreesOnWorld(myWorld);
			else
				GlobalCooldownLoc = CachedGlobalTimers[myWorld];

			GlobalSpawners.Add(this);
		}

		private void UpdateOtherTreesOnWorld(int myWorld)
		{
			CachedGlobalTimers[myWorld] = this.GlobalCooldownLoc;
			foreach(var item in GlobalSpawners)
			{
				if (item.IsNullOrDestroyed())continue;
				item.SetCycleIfSameWorld(this.GlobalCooldownLoc, myWorld);
			}
		}

		public override void OnCleanUp()
		{
			GlobalSpawners.Remove(this);
			base.OnCleanUp();
		}

		public void SetCycleIfSameWorld(int targetCycle, int otherSpawnerWorld)
		{
			var myWorldId = this.GetMyWorldId();
			if (myWorldId != otherSpawnerWorld)
				return;
			SetNextCycle(targetCycle);
		}

		internal void SetNextCycle(int target)
		{
			GlobalCooldownLoc = target;
		}

		public void CreatePresents()
		{
			int nextTargetCycle = GlobalCooldownLoc + CooldownTimeCycles;
			SetNextCycle(nextTargetCycle);
			var myWorld = this.GetMyParentWorldId();
			UpdateOtherTreesOnWorld(myWorld);
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


		public string SidescreenButtonText => SidescreenButtonInteractable()
			? Strings.Get("STRINGS.UI.GIVEDUPEPRESENTS.LABEL")
			: string.Format(Strings.Get("STRINGS.UI.GIVEDUPEPRESENTS.LABEL_COOLDOWN"), GlobalCooldownLoc - GameClock.Instance.GetCycle());

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

		public bool SidescreenButtonInteractable() => GlobalCooldownLoc <= GameClock.Instance.GetCycle();

		public bool SidescreenEnabled() => true;
		#endregion
	}
}
