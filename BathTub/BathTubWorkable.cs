using Klei;
using Klei.AI;
using UnityEngine;

namespace BathTub
{
	internal class BathTubWorkable : Workable, IWorkerPrioritizable
	{
		public SimHashes outputTargetElement;
		public float fractionalDiseaseRemoval;
		public int absoluteDiseaseRemoval;
		private SimUtil.DiseaseInfo accumulatedDisease;

		public BathTub bathTub;
		private bool faceLeft;

		private BathTubWorkable()
		{
			this.SetReportType(ReportManager.ReportType.PersonalTime);
		}

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			this.synchronizeAnims = false;
			this.showProgressBar = true;
			this.resetProgressOnStop = true;
			this.faceTargetWhenWorking = true;
			this.SetWorkTime(35f);
		}
		public override bool OnWorkTick(WorkerBase worker, float dt)
		{
			PrimaryElement component = worker.GetComponent<PrimaryElement>();
			if (component.DiseaseCount > 0)
			{
				SimUtil.DiseaseInfo b = new SimUtil.DiseaseInfo()
				{
					idx = component.DiseaseIdx,
					count = Mathf.CeilToInt((float)component.DiseaseCount * (1f - Mathf.Pow(this.fractionalDiseaseRemoval, dt)) - (float)this.absoluteDiseaseRemoval)
				};
				component.ModifyDiseaseCount(-b.count, "Shower.RemoveDisease");
				this.accumulatedDisease = SimUtil.CalculateFinalDiseaseInfo(this.accumulatedDisease, b);
				PrimaryElement primaryElement = bathTub.waterStorage.FindPrimaryElement(this.outputTargetElement);
				if ((UnityEngine.Object)primaryElement != (UnityEngine.Object)null)
				{
					primaryElement.GetComponent<PrimaryElement>().AddDisease(this.accumulatedDisease.idx, this.accumulatedDisease.count, "Shower.RemoveDisease");
					this.accumulatedDisease = SimUtil.DiseaseInfo.Invalid;
				}
			}
			return false;
		}
		public override Workable.AnimInfo GetAnim(WorkerBase worker) => base.GetAnim(worker) with
		{
			smi = (StateMachine.Instance)new HotTubWorkerStateMachine.StatesInstance(worker)
		};

		public override void OnStartWork(WorkerBase worker)
		{
			this.faceLeft = UnityEngine.Random.value > 0.5f;
			worker.GetComponent<Effects>().Add("HotTubRelaxing", false);
			this.WorkTimeRemaining = this.workTime * worker.GetSMI<HygieneMonitor.Instance>().GetDirtiness();
			if (ModAssets.RoomsExpandedActive)
			{
				if (ModIntegration.Rooms_Expanded.BathroomBonusActive(this))
				{
					this.WorkTimeRemaining = (1f - ModIntegration.Rooms_Expanded.BathRoomBonus) * WorkTimeRemaining;
				}
			}
			this.accumulatedDisease = SimUtil.DiseaseInfo.Invalid;
		}

		public override void OnStopWork(WorkerBase worker) => worker.GetComponent<Effects>().Remove("HotTubRelaxing");

		public override Vector3 GetFacingTarget() => this.transform.GetPosition() + (this.faceLeft ? Vector3.left : Vector3.right);

		public override void OnCompleteWork(WorkerBase worker)
		{
			Effects component = worker.GetComponent<Effects>();
			for (int index = 0; index < Shower.EffectsRemoved.Length; ++index)
			{
				string effect_id = Shower.EffectsRemoved[index];
				component.Remove(effect_id);
			}
			if (!worker.HasTag(GameTags.HasSuitTank))
				worker.GetSMI<GasLiquidExposureMonitor.Instance>()?.ResetExposure();
			component.Add(Shower.SHOWER_EFFECT, true);
			worker.GetSMI<HygieneMonitor.Instance>()?.SetDirtiness(0.0f);
		}

		public bool GetWorkerPriority(WorkerBase worker, out int priority)
		{
			priority = this.bathTub.basePriority;
			Effects component = worker.GetComponent<Effects>();
			if (!string.IsNullOrEmpty(Shower.SHOWER_EFFECT) && component.HasEffect(Shower.SHOWER_EFFECT))
			{
				priority = 0;
				return false;
			}
			return true;
		}
	}

}
