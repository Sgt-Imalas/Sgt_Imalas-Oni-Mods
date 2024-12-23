using Database;
using FMOD;
using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;
using UtilLibs.YeetUtils;

namespace AkisSnowThings.Content.Scripts.Entities
{
	internal class OpenFestivePresentWorkable : Workable, IWorkerPrioritizable
	{
		[MyCmpReq]
		Ownable ownable;
		[MyCmpReq]
		Prioritizable prioritizable;

		[Serialize]
		public bool hasBeenOpened = false;
		[MyCmpReq]
		KBatchedAnimController kbac;


		private WorkChore<OpenFestivePresentWorkable> OpenChore;

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			this.workerStatusItem = Db.Get().DuplicantStatusItems.Emptying;
			this.synchronizeAnims = false;
			Prioritizable.AddRef(this.gameObject);
			SetOffsetTable(OffsetGroups.InvertedStandardTable);
		}

		public override void OnSpawn()
		{
			base.OnSpawn();
			faceTargetWhenWorking = true;
			lightEfficiencyBonus = false;
			RefreshAnim();
			CreateWorkChore();
		}
		public void RefreshAnim()
		{
			kbac.Play(hasBeenOpened ? "open" : "closed");
		}

		public void CreateWorkChore()
		{
			if (hasBeenOpened)
				return;

			OpenChore = new WorkChore<OpenFestivePresentWorkable>(Db.Get().ChoreTypes.Fetch, this, only_when_operational:false);
			OpenChore.AddPrecondition(ChorePreconditions.instance.IsNotRedAlert, null);
			OpenChore.AddPrecondition(ChorePreconditions.instance.CanDoWorkerPrioritizable, this);
			OpenChore.AddPrecondition(ChorePreconditions.instance.IsAssignedtoMe, this.ownable);
		}

		public void SpawnPresentContents()
		{
			SgtLogger.l("SpawningPresentContents");
			CarePackageInfo randomCarePackage = Immigration.Instance.RandomCarePackage();
			while(!randomCarePackage.facadeID.IsNullOrWhiteSpace())
			{
				randomCarePackage = Immigration.Instance.RandomCarePackage();
			}
			var prefab = Assets.GetPrefab(randomCarePackage.id);
			if(prefab == null)
			{
				SgtLogger.warning("Could not find prefab for care package: " + randomCarePackage.id);
				return;
			}
			var item = Util.KInstantiate(prefab, Grid.CellToPosCBC(Grid.PosToCell(transform.position), Grid.SceneLayer.Ore));
			item.SetActive(true);
			YeetHelper.YeetRandomly(item, false, 2, 4, true);

			this.SpawnExplosionFX("missile_explosion_kanim", this.gameObject.transform.position with
			{
				z = Grid.GetLayerZ(Grid.SceneLayer.FXFront2)
			});
		}
		private void SpawnExplosionFX(string anim, Vector3 pos)
		{
			KBatchedAnimController effect = FXHelpers.CreateEffect(anim, pos, this.gameObject.transform, layer: Grid.SceneLayer.FXFront2);
			//effect.Offset = offset;
			effect.Play((HashedString)"idle");
			effect.onAnimComplete += (KAnimControllerBase.KAnimEvent)(obj => Util.KDestroyGameObject(this.gameObject));
		}

		public override void OnCompleteWork(WorkerBase worker)
		{
			base.OnCompleteWork(worker);
			hasBeenOpened = true;
			OpenChore.Succeed("present opened");
			OpenChore = null;
			RefreshAnim();
			SpawnPresentContents();
			worker.GetSMI<JoyBehaviourMonitor.Instance>().GoToOverjoyed();
		}

		public bool GetWorkerPriority(WorkerBase worker, out int priority)
		{
			priority = RELAXATION.PRIORITY.TIER3;
			return true;
		}
	}
}
