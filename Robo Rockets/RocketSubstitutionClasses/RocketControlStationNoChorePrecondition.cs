using KSerialization;
using UnityEngine;

namespace KnastoronOniMods
{
	public class RocketControlStationNoChorePrecondition : RocketControlStation
	{
		[Serialize]

		private SchedulerHandle newSweepyHandle;
		private GameObject brainController;
		public void MakeNewPilotBot(bool isDebug = false)
		{
			if ((this.newSweepyHandle.IsValid || brainController != null) && !isDebug)
				return;

			if (brainController != null)
			{
				KillRobo();
			}
			this.newSweepyHandle = GameScheduler.Instance.Schedule("Make brain", 1f, (System.Action<object>)(obj =>
			{
				GameObject go = GameUtil.KInstantiate(Assets.GetPrefab((Tag)"AiBrain"), Grid.CellToPos(Grid.CellRight(Grid.PosToCell(this.gameObject))), Grid.SceneLayer.Creatures);
				go.SetActive(true);
				brainController = go;


				this.newSweepyHandle.ClearScheduler();
			}), null, (SchedulerGroup)null);

		}

		public override void OnSpawn()
		{
			base.OnSpawn();
			MakeNewPilotBot(); //smi.enableConsoleLogging = true;
		}
		public override void OnCleanUp()
		{
			base.OnCleanUp();
			KillRobo();
		}
		protected void KillRobo()
		{
			if (!brainController.IsNullOrDestroyed())
				brainController.GetComponent<SelfDestructInWrongEnvironmentComponent>().SelfDestruct();
			brainController = null;
		}
	}
}
