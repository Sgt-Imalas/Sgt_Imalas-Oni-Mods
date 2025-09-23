using KSerialization;
using Newtonsoft.Json.Linq;
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


		[MyCmpAdd]
		CopyBuildingSettings settings;

		private static readonly EventSystem.IntraObjectHandler<HEPStorageThreshold> OnCopySettingsDelegate = new((component, data) => component.OnCopySettings(data));
		private static readonly EventSystem.IntraObjectHandler<HEPStorageThreshold> UpdateLogicCircuitDelegate = new((component, data) => component.UpdateLogicCircuit(data));

		public override void OnSpawn()
		{
			base.OnSpawn();
			this.Subscribe((int)GameHashes.CopySettings, OnCopySettingsDelegate);
			this.Subscribe((int)GameHashes.OperationalChanged, UpdateLogicCircuitDelegate);

		}
		public override void OnCleanUp()
		{
			this.Unsubscribe((int)GameHashes.CopySettings, OnCopySettingsDelegate);
			this.Unsubscribe((int)GameHashes.OperationalChanged, UpdateLogicCircuitDelegate);
			base.OnCleanUp();
		}

		public void Sim200ms(float dt)
		{
			this.UpdateLogicCircuit(null);
		}
		private void OnCopySettings(object data)
		{
			if (data is not GameObject go)
				return;
			if (!go.TryGetComponent<HEPStorageThreshold>(out var component))
				return;

			this.ActivateValue = component.ActivateValue;
			this.DeactivateValue = component.DeactivateValue;
		}
		private void UpdateLogicCircuit(object data)
		{
			float num = Mathf.RoundToInt(this.PercentFull * 100f);
			if (this.activated)
			{
				if (num >= deactivateValue)
					this.activated = false;
			}

			else if (num <= activateValue)
				this.activated = true;

			bool flag = this.activated & this.operational.IsOperational;

			logicPorts.SendSignal(targetHEPStorage.PORT_ID, flag ? 1 : 0);
		}

		public float ActivateValue
		{
			get => deactivateValue;
			set
			{
				this.deactivateValue = (int)value;
				this.UpdateLogicCircuit(null);
			}
		}

		public float DeactivateValue
		{
			get => activateValue;
			set
			{
				this.activateValue = (int)value;
				this.UpdateLogicCircuit(null);
			}
		}
		public static void Blueprints_SetData(GameObject source, JObject data)
		{
			if (source.TryGetComponent<HEPStorageThreshold>(out var behavior))
			{
				var t1 = data.GetValue("ActivateValue");
				var t2 = data.GetValue("DeactivateValue");
				if (t1 == null || t2 == null)
					return;
				var _ActivateValue = t1.Value<float>();
				var _DeactivateValue = t2.Value<float>();

				behavior.ActivateValue = _ActivateValue;
				behavior.DeactivateValue = _DeactivateValue;
			}
		}
		public static JObject Blueprints_GetData(GameObject source)
		{
			if (source.TryGetComponent<HEPStorageThreshold>(out var behavior))
			{
				return new JObject()
				{
					{ "ActivateValue", behavior.ActivateValue},
					{ "DeactivateValue", behavior.DeactivateValue},
				};
			}
			return null;
		}

		public float MinValue => 0.0f;

		public float MaxValue => 100f;

		public bool UseWholeNumbers => true;

		public string ActivateTooltip => STRINGS.BUILDINGS.PREFABS.SMARTRESERVOIR.DEACTIVATE_TOOLTIP;

		public string DeactivateTooltip => STRINGS.BUILDINGS.PREFABS.SMARTRESERVOIR.ACTIVATE_TOOLTIP;

		public string ActivationRangeTitleText => STRINGS.BUILDINGS.PREFABS.SMARTRESERVOIR.SIDESCREEN_TITLE;

		public string ActivateSliderLabelText => STRINGS.BUILDINGS.PREFABS.SMARTRESERVOIR.SIDESCREEN_DEACTIVATE;

		public string DeactivateSliderLabelText => STRINGS.BUILDINGS.PREFABS.SMARTRESERVOIR.SIDESCREEN_ACTIVATE;
	}
}
