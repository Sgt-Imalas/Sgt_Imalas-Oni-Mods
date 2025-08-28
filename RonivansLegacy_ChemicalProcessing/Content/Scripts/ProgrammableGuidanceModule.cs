using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	public class ProgrammableGuidanceModule : Workable
	{
		[MyCmpReq]
		KPrefabID prefabID;
		[MyCmpReq]
		Durability durability;

		[Serialize]
		Tag PendingReprogramming = null;


		Chore ReprogrammChore = null;

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			this.faceTargetWhenWorking = true;

			this.overrideAnims = [Assets.GetAnim((HashedString)"anim_use_remote_kanim")];
			this.synchronizeAnims = false;
			this.surpressWorkerForceSync = true;
		}
		public override void OnSpawn()
		{
			base.OnSpawn();
			if (PendingReprogramming != null)
				StartReprogramTask(PendingReprogramming);
			this.SetWorkTime(3f);
		}
		public override void OnCompleteWork(WorkerBase worker)
		{
			base.OnCompleteWork(worker);
			DoReprogram();
		}

		public void StartReprogramTask(Tag target)
		{
			if (IsThisConfiguration(target.ToString()))
				return;

			CancelReprogrammTask();
			PendingReprogramming = target;
			ReprogrammChore = new WorkChore<ProgrammableGuidanceModule>(Db.Get().ChoreTypes.Toggle, this, only_when_operational: false);
			ReprogrammChore.AddPrecondition(ChorePreconditions.instance.IsNotRedAlert, null);
			ReprogrammChore.AddPrecondition(ChorePreconditions.instance.HasSkillPerk, Db.Get().SkillPerks.ConveyorBuild);
		}
		public void CancelReprogrammTask()
		{
			if (ReprogrammChore != null)
			{
				ReprogrammChore.Cancel("Cancelled by user");
				ReprogrammChore = null;
			}
			PendingReprogramming = null;
		}
		public void SelfDestruct()
		{
			UnityEngine.Object.Destroy(this.gameObject);
		}

		void DoReprogram()
		{
			float currentDurability = durability.GetDurability();
			if (currentDurability <= 0f)
			{
				SelfDestruct();
				return;
			}

			var prefab = Assets.GetPrefab(PendingReprogramming);
			if (prefab == null)
			{
				Debug.LogError($"ProgrammableGuidanceModule: No prefab found for tag {PendingReprogramming}");
				return;
			}
			var newItem = Util.KInstantiate(prefab, transform.position);
			newItem.SetActive(true);
			newItem.GetComponent<ProgrammableGuidanceModule>().SetDurability(currentDurability);

			UnityEngine.Object.Destroy(this.gameObject);
		}
		public void SetDurability(float durabilityValue)
		{
			if (durability != null)
			{
				durability.durability = (durabilityValue);
			}
			else
			{
				Debug.LogError("ProgrammableGuidanceModule: Durability component is missing.");
			}
		}

		internal bool IsThisConfiguration(string tag)
		{
			return prefabID.PrefabID() == tag;
		}
		public bool HasPendingReprogramming()
		{
			return PendingReprogramming != null;
		}

		internal void DealWearDamage(float itemDamage, out bool destroyedAfter)
		{
			destroyedAfter = false;
			var currentHealth = durability.GetDurability();
			currentHealth -= itemDamage;
			if (currentHealth <= 0f)
			{
				destroyedAfter = true;
				SetDurability(0f);
			}
			else
			{
				SetDurability(currentHealth);
			}
		}
	}
}

