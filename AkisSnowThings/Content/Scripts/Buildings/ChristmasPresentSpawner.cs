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
		public static int GlobalCooldown = -1;
		public static readonly int CooldownTimeCycles = 364; //1 cycle leeway

		[SerializeField]
		[Serialize]
		private int _globalCooldownLoc = -1;

		[MyCmpReq]
		KSelectable selectable;


		[OnDeserializing()]
		internal void OnDeserializingMethod(StreamingContext context)
		{
			if (_globalCooldownLoc > 0 || GlobalCooldown < _globalCooldownLoc)
			{
				GlobalCooldown = _globalCooldownLoc;
			}
			else
			{
				_globalCooldownLoc = GlobalCooldown;
			}
		}

		[OnSerialized()]
		internal void OnSerialized(StreamingContext context)
		{
			if(_globalCooldownLoc < GlobalCooldown)
			{
				_globalCooldownLoc = GlobalCooldown;
			}
		}

		public void CreatePresents()
		{
			GlobalCooldown += CooldownTimeCycles;
			SgtLogger.l("Spawning Christmas Presents");
			StartCoroutine(SpawnPresentsDelayed());
		}
		public IEnumerator SpawnPresentsDelayed()
		{
			SgtLogger.l("number of minions: " + Components.LiveMinionIdentities.Count);

			foreach (MinionIdentity duplicant in Components.LiveMinionIdentities)
			{
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


		public string SidescreenButtonText => SidescreenButtonInteractable() ? Strings.Get("STRINGS.UI.GIVEDUPEPRESENTS.LABEL"): string.Format(Strings.Get("STRINGS.UI.GIVEDUPEPRESENTS.LABEL_COOLDOWN"), GlobalCooldown - GameClock.Instance.GetCycle());

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
