using BionicBoostersPlus.Content.ModDb;
using Database;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UtilLibs;
using static StateMachine;
using static STRINGS.INPUT_BINDINGS;

namespace BionicBoostersPlus.Content.Scripts
{
	internal class BionicUpgrade_Medikit : BionicUpgrade_SM<BionicUpgrade_Medikit, BionicUpgrade_Medikit.Instance>
	{
		public static void OnBoosterAdded(Instance smi)
		{
			smi.OnAdded();
		}
		public static void OnBoosterRemoved(Instance smi)
		{
			smi.OnRemoved();
		}

		public new class Def : BionicUpgrade_SM<BionicUpgrade_Medikit, Instance>.Def
		{
			public AttributeModifier[] modifiers;
			public Def(string upgradeID, AttributeModifier[] modifiers = null) : base(upgradeID)
			{
				this.modifiers = modifiers;
			}
			public override string GetDescription()
			{
				string description = "";
				if (this.modifiers.Length != 0)
				{
					description += global::STRINGS.UI.UISIDESCREENS.BIONIC_SIDE_SCREEN.BOOSTER_ASSIGNMENT.HEADER_ATTRIBUTES;
					for (int index = 0; index < this.modifiers.Length; ++index)
						description = $"{description + "\n"}{this.modifiers[index].GetName()}: {this.modifiers[index].GetFormattedString()}";
				}
				return description;
			}
		}

		public new class Instance : BaseInstance
		{
			[MyCmpGet]
			public MinionResume resume;

			public readonly AttributeModifier radiationRemoval;
			public readonly AttributeModifier healthRegen;

			private readonly AmountInstance _health, _rads;

			public bool activelyRepairing = false;

			public Instance(IStateMachineTarget master, Def def) : base(master, def)
			{
				var db = Db.Get();
				if (DlcManager.IsExpansion1Active())
				{
					this.radiationRemoval = new AttributeModifier(db.Attributes.RadiationRecovery.Id, BB_TUNING.Medkit_RadsRemovedPerSecond, STRINGS.ITEMS.BIONIC_BOOSTERS.BB_BOOSTER_MEDIKIT.NAME);
					this._rads = Db.Get().Amounts.RadiationBalance.Lookup(this.gameObject);
				}
				this.healthRegen = new AttributeModifier(db.Amounts.HitPoints.deltaAttribute.Id, BB_TUNING.Medkit_HealthRegeneratedPerSecond, STRINGS.ITEMS.BIONIC_BOOSTERS.BB_BOOSTER_MEDIKIT.NAME);
				this._health = Db.Get().Amounts.HitPoints.Lookup(this.gameObject);
			}

			public override float GetCurrentWattageCost() => IsRepairingDamage() ? Data.WattageCost : 0;

			public override string GetCurrentWattageCostName()
			{
				float currentWattageCost = GetCurrentWattageCost();
				if (IsRepairingDamage())
				{
					string text = "<b>" + ((currentWattageCost >= 0f) ? "+" : "-") + "</b>";
					return string.Format(global::STRINGS.DUPLICANTS.MODIFIERS.BIONIC_WATTS.TOOLTIP.STANDARD_ACTIVE_TEMPLATE, upgradeComponent.GetProperName(), text + GameUtil.GetFormattedWattage(currentWattageCost));
				}
				return string.Format(global::STRINGS.DUPLICANTS.MODIFIERS.BIONIC_WATTS.TOOLTIP.STANDARD_INACTIVE_TEMPLATE, upgradeComponent.GetProperName(), GameUtil.GetFormattedWattage(upgradeComponent.PotentialWattage));
			}

			public bool CanStartReparing() => CanStartRepairing_Health() || CanStartRepairing_Rads();
			private bool CanStartRepairing_Health()
			{
				SgtLogger.l($"CanStartReparing? {_health.value} < {_health.GetMax()}");
				float current = _health.value;
				return current < _health.GetMax();
			}
			private bool CanStartRepairing_Rads()
			{
				if(!DlcManager.IsExpansion1Active())
					return false;

				float current = _rads.value;
				return current > BB_TUNING.Medkit_RadsThreshold_Upper;
			}

			public bool CanStopRepairing() => CanStopRepairing_Health() && CanStopRepairing_Rads();
			private bool CanStopRepairing_Health()
			{
				SgtLogger.l($"CanStopRepariing? {_health.value} >= {_health.GetMax()}");
				return _health.value >= _health.GetMax();
			}
			private bool CanStopRepairing_Rads()
			{
				if (!DlcManager.IsExpansion1Active())
					return false;
				return _rads.value <= BB_TUNING.Medkit_RadsThreshold_Lower;
			}


			public void OnAdded()
			{
				ToggleAttributeModifiers(true);
			}
			public void OnRemoved()
			{
				ToggleAttributeModifiers(false);
				ToggleRepairAttributeModifiers(false);
			}
			public bool IsRepairingDamage() => IsInsideState(sm.repairing);

			private void ToggleAttributeModifiers(bool on)
			{
				Klei.AI.Attributes attributes = this.resume.GetIdentity.GetAttributes();

				foreach (AttributeModifier modifier in ((BionicUpgrade_Medikit.Def)this.smi.def).modifiers)
				{
					if (on)
						attributes.Add(modifier);
					else
						attributes.Remove(modifier);
				}
				//trigger BionicBatteryMonitor.OnSkillsChanged to drop excess powerbanks on removal
				Trigger((int)GameHashes.AssignedRoleChanged);
			}
			public void ToggleRepairAttributeModifiers(bool on)
			{
				bool SO = DlcManager.IsExpansion1Active();
				Klei.AI.Attributes attributes = resume.GetIdentity.GetAttributes();
				if (on)
				{
					attributes.Add(healthRegen);
					if (SO)
						attributes.Add(radiationRemoval);
				}
				else
				{
					attributes.Remove(healthRegen);
					if (SO)
						attributes.Remove(radiationRemoval);
				}
			}
		}

		public State repairing;
		public State idle;

		public override void InitializeStates(out BaseState default_state)
		{
			base.serializable = SerializeType.ParamsOnly;
			default_state = idle;

			root.Enter(OnBoosterAdded)
				.Exit(OnBoosterRemoved);

			idle
				.TriggerOnEnter(GameHashes.BionicUpgradeWattageChanged)
				.UpdateTransition(repairing, (smi,dt)=>smi.CanStartReparing(),UpdateRate.SIM_1000ms);

			repairing
				.TriggerOnEnter(GameHashes.BionicUpgradeWattageChanged)
				.UpdateTransition(idle, (smi, dt) => smi.CanStopRepairing(), UpdateRate.SIM_1000ms)
				.Enter(smi => smi.ToggleRepairAttributeModifiers(true))
				.Exit(smi => smi.ToggleRepairAttributeModifiers(false))
				.ToggleStatusItem(BB_StatusItems.MedBooster_Repairing);
				;
		}
	}
}
