using Cryopod.Buildings;

namespace Cryopod
{
	class OpenCryopodWorkable : Workable
	{
		private Chore openChore;

		public bool ChoreExisting()
		{
			return openChore != null;
		}
		public override void OnPrefabInit()
		{

			base.OnPrefabInit();
			this.synchronizeAnims = false;
			this.overrideAnims = new KAnimFile[1]
			{
			Assets.GetAnim((HashedString) "anim_interacts_warp_portal_sender_kanim")
			};
			this.SetWorkTime(3f);
			this.showProgressBar = false;
		}

		public void CancelOpenChore(object param = null)
		{
			if (this.openChore == null)
				return;
			this.openChore.Cancel("User cancelled");
			this.openChore = (Chore)null;
		}
		private void CompleteOpenChore()
		{
			this.GetComponent<CryopodReusable>().OpenChoreDone();
			this.openChore = (Chore)null;
			Game.Instance.userMenu.Refresh(this.gameObject);
		}
		public Chore CreateOpenChore()
		{
			openChore = (Chore)new WorkChore<OpenCryopodWorkable>(Db.Get().ChoreTypes.EmptyStorage, (IStateMachineTarget)this, null, true, null, null, null, false, null, true, false, override_anims: Assets.GetAnim((HashedString)"anim_interacts_cryo_activation_kanim"), false, true, true, PriorityScreen.PriorityClass.high, 5, true, false);

			this.requireMinionToWork = true;
			return openChore;
		}
		public override void OnStartWork(WorkerBase worker) => base.OnStartWork(worker);

		public override bool OnWorkTick(WorkerBase worker, float dt)
		{
			base.OnWorkTick(worker, dt);
			return false;
		}
		public override void OnStopWork(WorkerBase worker) => base.OnStopWork(worker);

		public override void OnCompleteWork(WorkerBase worker)
		{

			CompleteOpenChore();
		}
	}
}
