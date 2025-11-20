using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	class HEPStorageThreshold : KMonoBehaviour, IActivationRangeTarget, ISim200ms
	{
		public static readonly HashedString PORT_ID = (HashedString)"HighEnergyParticleThresholdLogicPort";
		[MyCmpGet]
		protected Operational operational;
		[Serialize]
		private int activateValue = 5;
		[Serialize]
		private int deactivateValue = 95;
		[Serialize]
		private bool activated;
		[MyCmpReq]
		private LogicPorts logicPorts;
		[MyCmpReq]
		private HighEnergyParticleStorage targetHEPStorage;
		public float PercentFull => targetHEPStorage.Particles / targetHEPStorage.Capacity();

		public override void OnSpawn()
		{
			//Subscribe((int)GameHashes.OnParticleStorageChanged, UpdateLogicCircuit);
			base.OnSpawn();
			UpdateLogicCircuit(null);
		}
		public override void OnCleanUp()
		{
			base.OnCleanUp();
		}

		public void Sim200ms(float dt)
		{
			UpdateLogicCircuit(null);
		}
		private void UpdateLogicCircuit(object data)
		{
			float currentParticlePercentage = Mathf.RoundToInt(PercentFull * 100f);
			if (activated)
			{
				if (currentParticlePercentage >= deactivateValue)
					this.activated = false;
			}
			else if (currentParticlePercentage <= activateValue)
				this.activated = true;
			bool logicportactive = this.activated & operational.IsOperational;
			logicPorts.SendSignal(PORT_ID, logicportactive ? 1 : 0);
		}

		public float ActivateValue
		{
			get => (float)this.deactivateValue;
			set
			{
				this.deactivateValue = (int)value;
				this.UpdateLogicCircuit(null);
			}
		}

		public float DeactivateValue
		{
			get => (float)this.activateValue;
			set
			{
				this.activateValue = (int)value;
				this.UpdateLogicCircuit(null);
			}
		}

		public float MinValue => 0.0f;

		public float MaxValue => 100f;

		public bool UseWholeNumbers => true;

		public string ActivateTooltip => global::STRINGS.BUILDINGS.PREFABS.BATTERYSMART.DEACTIVATE_TOOLTIP;

		public string DeactivateTooltip => global::STRINGS.BUILDINGS.PREFABS.BATTERYSMART.ACTIVATE_TOOLTIP;

		public string ActivationRangeTitleText => global::STRINGS.BUILDINGS.PREFABS.BATTERYSMART.SIDESCREEN_TITLE;

		public string ActivateSliderLabelText => global::STRINGS.BUILDINGS.PREFABS.BATTERYSMART.SIDESCREEN_DEACTIVATE;

		public string DeactivateSliderLabelText => global::STRINGS.BUILDINGS.PREFABS.BATTERYSMART.SIDESCREEN_ACTIVATE;
	}
}
