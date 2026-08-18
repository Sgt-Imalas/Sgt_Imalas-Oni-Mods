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
	internal class BionicUpgrade_Solar : BionicUpgrade_SM<BionicUpgrade_Solar, BionicUpgrade_Solar.Instance>
	{
		public static void OnBoosterAdded(Instance smi)
		{
			smi.OnAdded();
		}
		public static void OnBoosterRemoved(Instance smi)
		{
			smi.OnRemoved();
		}
		public static void LightConversionUpdate(Instance smi, float dt)
		{
			smi.lastSolarWattage = SolarPanelConfig.WATTS_PER_LUX * smi.LightAmount();
			smi.Trigger((int)GameHashes.BionicUpgradeWattageChanged);
		}

		public new class Def : BionicUpgrade_SM<BionicUpgrade_Solar, Instance>.Def
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

			internal float lastSolarWattage = 0;

			public Instance(IStateMachineTarget master, Def def) : base(master, def)
			{

			}

			public override float GetCurrentWattageCost() => -lastSolarWattage;

			public override string GetCurrentWattageCostName()
			{
				float currentWattageCost = GetCurrentWattageCost();
				string text = "<b>" + ((currentWattageCost >= 0f) ? "+" : "-") + "</b>";
				return string.Format(global::STRINGS.DUPLICANTS.MODIFIERS.BIONIC_WATTS.TOOLTIP.STANDARD_ACTIVE_TEMPLATE, upgradeComponent.GetProperName(), text + GameUtil.GetFormattedWattage(-currentWattageCost));
			}

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
				Klei.AI.Attributes attributes = this.resume.GetIdentity.GetAttributes();

				foreach (AttributeModifier modifier in ((BionicUpgrade_Solar.Def)this.smi.def).modifiers)
				{
					if (on)
						attributes.Add(modifier);
					else
						attributes.Remove(modifier);
				}
			}
			public bool LightAboveThreshold() => LightAmount() >= BB_TUNING.SolarBooster_LuxThreshold;
			public int LightAmount()
			{
				var headCell = Grid.CellAbove(Grid.PosToCell(this.master.gameObject));
				if (!Grid.IsValidCell(headCell))
					return 0;

				return Grid.LightIntensity[headCell];
			}

		}

		public State eatingLight;
		public State idle;

		public override void InitializeStates(out BaseState default_state)
		{
			base.serializable = SerializeType.ParamsOnly;
			default_state = idle;

			root.Enter(OnBoosterAdded)
				.Exit(OnBoosterRemoved);

			idle
				.TriggerOnEnter(GameHashes.BionicUpgradeWattageChanged)
				.UpdateTransition(eatingLight, (smi,dt)=>smi.LightAboveThreshold(),UpdateRate.SIM_1000ms);

			eatingLight
				.TriggerOnEnter(GameHashes.BionicUpgradeWattageChanged)
				.UpdateTransition(idle, (smi, dt) => !smi.LightAboveThreshold(), UpdateRate.SIM_1000ms)
				.ToggleStatusItem(BB_StatusItems.SolarBooster_ConsumingSun)
				.Update(LightConversionUpdate);
		}
	}
}
