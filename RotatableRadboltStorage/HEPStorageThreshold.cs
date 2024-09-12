using KSerialization;
using UnityEngine;

namespace RotatableRadboltStorage
{
	internal class HEPStorageThreshold : KMonoBehaviour, IActivationRangeTarget, ISim200ms
	{
		[MyCmpGet]
		protected Operational operational;
		[Serialize]
		private int activateValue = 5;
		[Serialize]
		private int deactivateValue = 95;
		[Serialize]
		private bool activated;
		[MyCmpGet]
		private LogicPorts logicPorts;
		[MyCmpGet]
		private HighEnergyParticleStorage targetHEPStorage;
		public float PercentFull => targetHEPStorage.Particles / targetHEPStorage.capacity;

		void ISim200ms.Sim200ms(float dt)
		{
			this.UpdateLogicCircuit((object)null);
		}

		private void UpdateLogicCircuit(object data)
		{
			float num = (float)Mathf.RoundToInt(this.PercentFull * 100f);
			if (this.activated)
			{
				if ((double)num >= (double)this.deactivateValue)
					this.activated = false;
			}
			else if ((double)num <= (double)this.activateValue)
				this.activated = true;
			bool flag = this.activated & this.operational.IsOperational;
			logicPorts.SendSignal(targetHEPStorage.PORT_ID, flag ? 1 : 0);
		}


		public float ActivateValue
		{
			get => (float)this.deactivateValue;
			set
			{
				this.deactivateValue = (int)value;
				this.UpdateLogicCircuit((object)null);
			}
		}

		public float DeactivateValue
		{
			get => (float)this.activateValue;
			set
			{
				this.activateValue = (int)value;
				this.UpdateLogicCircuit((object)null);
			}
		}

		public float MinValue => 0.0f;

		public float MaxValue => 100f;

		public bool UseWholeNumbers => true;

		public string ActivateTooltip => STRINGS.BUILDINGS.PREFABS.BATTERYSMART.DEACTIVATE_TOOLTIP;

		public string DeactivateTooltip => STRINGS.BUILDINGS.PREFABS.BATTERYSMART.ACTIVATE_TOOLTIP;

		public string ActivationRangeTitleText => STRINGS.BUILDINGS.PREFABS.BATTERYSMART.SIDESCREEN_TITLE;

		public string ActivateSliderLabelText => STRINGS.BUILDINGS.PREFABS.BATTERYSMART.SIDESCREEN_DEACTIVATE;

		public string DeactivateSliderLabelText => STRINGS.BUILDINGS.PREFABS.BATTERYSMART.SIDESCREEN_ACTIVATE;
	}
}
