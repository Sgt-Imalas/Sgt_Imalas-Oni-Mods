using BionicBoostersPlus.Content.ModDb;
using Database;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static StateMachine;
using static STRINGS.INPUT_BINDINGS;

namespace BionicBoostersPlus.Content.Scripts
{
	internal class BionicUpgrade_BatterySlot : BionicUpgrade_SM<BionicUpgrade_BatterySlot, BionicUpgrade_BatterySlot.Instance>
	{
		public static void OnBoosterAdded(Instance smi)
		{
			smi.OnAdded();
		}
		public static void OnBoosterRemoved(Instance smi)
		{
			smi.OnRemoved();
		}

		public new class Def : BionicUpgrade_SM<BionicUpgrade_BatterySlot, Instance>.Def
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

			public AttributeModifier radiationRemoval;
			public AttributeModifier healthRegen;

			public Instance(IStateMachineTarget master, Def def) : base(master, def)
			{
				var db = Db.Get();
				this.radiationRemoval = new AttributeModifier(db.Attributes.RadiationRecovery.Id, BB_TUNING.Medkit_RadsRemovedPerSecond, STRINGS.ITEMS.BIONIC_BOOSTERS.BB_BOOSTER_MEDIKIT.NAME);
				this.healthRegen = new AttributeModifier(db.Amounts.HitPoints.deltaAttribute.Id, BB_TUNING.Medkit_HealthRegeneratedPerSecond, STRINGS.ITEMS.BIONIC_BOOSTERS.BB_BOOSTER_MEDIKIT.NAME);
			}

			public override float GetCurrentWattageCost() => 0;

			public override string GetCurrentWattageCostName() => string.Empty;

			public void OnAdded()
			{
				ToggleAttributeModifiers(true);
			}
			public void OnRemoved()
			{
				ToggleAttributeModifiers(false);
			}
			private void ToggleAttributeModifiers(bool on)
			{
				Klei.AI.Attributes attributes = resume.GetIdentity.GetAttributes();

				foreach (AttributeModifier modifier in ((Def)smi.def).modifiers)
				{
					if (on)
						attributes.Add(modifier);
					else
						attributes.Remove(modifier);
				}
				//trigger BionicBatteryMonitor.OnSkillsChanged to drop excess powerbanks on removal
				Trigger((int)GameHashes.AssignedRoleChanged);
			}
		}

		public State idle;

		public override void InitializeStates(out BaseState default_state)
		{
			base.serializable = SerializeType.ParamsOnly;
			default_state = root;

			root.Enter(OnBoosterAdded)
				.Exit(OnBoosterRemoved);


		}
	}
}
