using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static CellChangeMonitor.CellChangedEntry;
using static ProcessCondition;

namespace Rockets_TinyYetBig.Content.Scripts.Buildings.RocketModules
{
	internal class RocketLogicLaunchCondition : KMonoBehaviour, FewOptionSideScreen.IFewOptionSideScreen
	{
		[MyCmpReq] public UserNameable nameable;
		[MyCmpReq] public LogicPorts logicPorts;
		[MyCmpGet] KBatchedAnimController kbac;

		public RocketModule worldModule;

		[Serialize] ProcessConditionType conditionType = ProcessConditionType.RocketBoard;

		private ConditionLogicInputActive lastConditionApplied = null;

		public override void OnSpawn()
		{
			base.OnSpawn();

			kbac.SetSymbolTint("icon_checkmark_yes", AccessibilityUtils.LogicGood);
			kbac.SetSymbolTint("icon_checkmark_no", AccessibilityUtils.LogicBad);

			worldModule = this.GetMyWorld()?.GetComponent<Clustercraft>()?.ModuleInterface.GetPassengerModule()?.GetComponent<RocketModuleCluster>() ?? null;
			if (worldModule == null)
			{
				SgtLogger.error("no world module found for logic launch condition");
				return;
			}
			Subscribe((int)GameHashes.NameChanged, OnNameChanged);
			Subscribe((int)GameHashes.LogicEvent, OnLogicValueChanged);
			AddCurrentCondition();
			RefreshAnimation();
		}


		public override void OnCleanUp()
		{
			RemoveCurrentCondition();
			base.OnCleanUp();
		}

		void OnNameChanged(object data) => RefreshCondition();
		void OnLogicValueChanged(object data) => RefreshAnimation();

		void RefreshAnimation()
		{
			var anim = logicPorts.GetInputValue(LogicOperationalController.PORT_ID) == 1 ? "on" : "off";
			kbac.Play(anim);
		}

		void RefreshCondition()
		{
			RemoveCurrentCondition();
			AddCurrentCondition();
		}

		void AddCurrentCondition()
		{
			if (worldModule == null)
			{
				SgtLogger.warning("no world module found for logic launch condition");
				return;
			}
			if (lastConditionApplied != null)
				return;
			var condition = new ConditionLogicInputActive(this);
			worldModule.AddModuleCondition(conditionType, condition);
			lastConditionApplied = condition;
		}

		void RemoveCurrentCondition()
		{
			if (worldModule == null)
			{
				SgtLogger.warning("no world module found for logic launch condition");
				return;
			}
			if (lastConditionApplied == null) 
				return;

			if(worldModule.moduleConditions.TryGetValue(conditionType, out var conditions))
			{
				if(conditions.Contains(lastConditionApplied))
				{
					conditions.Remove(lastConditionApplied);
				}
			}
			lastConditionApplied = null;
		}

		public FewOptionSideScreen.IFewOptionSideScreen.Option[] GetOptions() => [
			new(nameof(ProcessConditionType.RocketFlight),global::STRINGS.UI.DETAILTABS.PROCESS_CONDITIONS.ROCKETFLIGHT,new(Assets.GetSprite("action_navigable_regions"),Color.black), global::STRINGS.UI.DETAILTABS.PROCESS_CONDITIONS.ROCKETFLIGHT_TOOLTIP),
			new(nameof(ProcessConditionType.RocketPrep),global::STRINGS.UI.DETAILTABS.PROCESS_CONDITIONS.ROCKETPREP,new(Assets.GetSprite("icon_archetype_build"),Color.black), global::STRINGS.UI.DETAILTABS.PROCESS_CONDITIONS.ROCKETPREP_TOOLTIP),
			new(nameof(ProcessConditionType.RocketStorage),global::STRINGS.UI.DETAILTABS.PROCESS_CONDITIONS.ROCKETSTORAGE,new(Assets.GetSprite("status_item_no_filter_set"),Color.black), global::STRINGS.UI.DETAILTABS.PROCESS_CONDITIONS.ROCKETFLIGHT_TOOLTIP),
			new(nameof(ProcessConditionType.RocketBoard),global::STRINGS.UI.DETAILTABS.PROCESS_CONDITIONS.ROCKETBOARD,new(Assets.GetSprite("action_clearance"),Color.black), global::STRINGS.UI.DETAILTABS.PROCESS_CONDITIONS.ROCKETBOARD_TOOLTIP),
		];

		public Tag GetSelectedOption() => conditionType.ToString();

		public void OnOptionSelected(FewOptionSideScreen.IFewOptionSideScreen.Option option)
		{
			var oldCOndition = conditionType;

			switch (option.tag.ToString())
			{
				case nameof(ProcessConditionType.RocketFlight):
					conditionType = ProcessConditionType.RocketFlight;
					break;
				case nameof(ProcessConditionType.RocketPrep):
					conditionType = ProcessConditionType.RocketPrep;
					break;
				case nameof(ProcessConditionType.RocketStorage):
					conditionType = ProcessConditionType.RocketStorage;
					break;
				case nameof(ProcessConditionType.RocketBoard):
					conditionType = ProcessConditionType.RocketBoard;
					break;
			}

		}
	}
}
