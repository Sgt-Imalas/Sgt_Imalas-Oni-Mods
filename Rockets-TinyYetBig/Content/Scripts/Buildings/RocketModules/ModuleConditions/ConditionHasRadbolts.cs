using STRINGS;

namespace Rockets_TinyYetBig.Behaviours
{
	class ConditionHasRadbolts : ProcessCondition
	{
		private HighEnergyParticleStorage hepStorage;
		private float thresholdRadbolts;
		public ConditionHasRadbolts(HighEnergyParticleStorage storage, float radboltThreshold)
		{
			this.hepStorage = storage;
			this.thresholdRadbolts = radboltThreshold;
		}
		public override ProcessCondition.Status EvaluateCondition() => (double)this.hepStorage.Particles < (double)this.thresholdRadbolts ? ProcessCondition.Status.Warning : ProcessCondition.Status.Ready;

		public override string GetStatusMessage(ProcessCondition.Status status)
		{
			string statusMessage;
			switch (status)
			{
				case ProcessCondition.Status.Failure:
					statusMessage = string.Format((string)UI.STARMAP.LAUNCHCHECKLIST.HAS_RESOURCE.STATUS.FAILURE, this.hepStorage.GetProperName(), UI.UNITSUFFIXES.HIGHENERGYPARTICLES.PARTRICLES.ToString().Trim());
					break;
				case ProcessCondition.Status.Ready:
					statusMessage = string.Format((string)UI.STARMAP.LAUNCHCHECKLIST.HAS_RESOURCE.STATUS.READY, this.hepStorage.GetProperName(), UI.UNITSUFFIXES.HIGHENERGYPARTICLES.PARTRICLES.ToString().Trim());
					break;
				default:
					statusMessage = string.Format((string)UI.STARMAP.LAUNCHCHECKLIST.HAS_RESOURCE.STATUS.WARNING, this.hepStorage.GetProperName(), UI.UNITSUFFIXES.HIGHENERGYPARTICLES.PARTRICLES.ToString().Trim());
					break;
			}
			return statusMessage;
		}

		public override string GetStatusTooltip(ProcessCondition.Status status)
		{
			string statusTooltip;
			switch (status)
			{
				case ProcessCondition.Status.Failure:
					statusTooltip = string.Format((string)UI.STARMAP.LAUNCHCHECKLIST.HAS_RESOURCE.TOOLTIP.FAILURE, this.hepStorage.GetProperName(), GameUtil.GetFormattedMass(this.thresholdRadbolts), UI.UNITSUFFIXES.HIGHENERGYPARTICLES.PARTRICLES);
					break;
				case ProcessCondition.Status.Ready:
					statusTooltip = string.Format((string)UI.STARMAP.LAUNCHCHECKLIST.HAS_RESOURCE.TOOLTIP.READY, this.hepStorage.GetProperName(), UI.UNITSUFFIXES.HIGHENERGYPARTICLES.PARTRICLES);
					break;
				default:
					statusTooltip = string.Format((string)UI.STARMAP.LAUNCHCHECKLIST.HAS_RESOURCE.TOOLTIP.WARNING, this.hepStorage.GetProperName(), GameUtil.GetFormattedMass(this.thresholdRadbolts), UI.UNITSUFFIXES.HIGHENERGYPARTICLES.PARTRICLES);
					break;
			}
			return statusTooltip;
		}

		public override bool ShowInUI() => true;
	}
}
