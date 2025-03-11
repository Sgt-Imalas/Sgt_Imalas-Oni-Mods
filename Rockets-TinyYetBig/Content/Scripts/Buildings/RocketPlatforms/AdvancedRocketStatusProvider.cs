using KSerialization;
using System.Collections;
using UnityEngine;
using UtilLibs;
using static ProcessCondition;

namespace Rockets_TinyYetBig.NonRocketBuildings
{
	public class AdvancedRocketStatusProvider : KMonoBehaviour, ISim1000ms
	{
		[MyCmpGet] LaunchPad launchPad;
		[MyCmpGet] LogicPorts logicPorts;
		[MyCmpGet] KBatchedAnimController kbac;
		[MyCmpGet] Operational operational;
		public HashedString rocketStateOutput;
		public HashedString ignoreWarningInput;
		const int RocketPath = 1, RocketConstruction = 2, RocketStorage = 4, RocketCrew = 8;

		[Serialize]
		int LaunchSignalValue = 0;
		[Serialize]
		bool[] LaunchSignalBits = null;


		public override void OnSpawn()
		{
			base.OnSpawn();
			var launchpadMaterialDistributor = gameObject.GetSMI<LaunchPadMaterialDistributor.Instance>();

			OnOperationalChanged(null);
			this.UpdateVisuals();
			this.Subscribe((int)GameHashes.LogicEvent, OnLogicValueChanged);
			this.Subscribe((int)GameHashes.OperationalChanged, OnOperationalChanged);
		}

		public override void OnCleanUp()
		{
			base.OnCleanUp();
			this.Unsubscribe((int)GameHashes.LogicEvent, OnLogicValueChanged);
			this.Unsubscribe((int)GameHashes.OperationalChanged, OnOperationalChanged);

		}
		void OnOperationalChanged(object data)
		{
			if (operational.IsOperational)
				kbac.Play("idle", KAnim.PlayMode.Loop);
			else
				kbac.Play("off");
			this.UpdateVisuals();
		}
		private void OnLogicValueChanged(object data)
		{
			LogicValueChanged logicValueChanged = (LogicValueChanged)data;
			this.UpdateVisuals();
			if (logicValueChanged.portID != ignoreWarningInput)
				return;
			this.UpdateSignalBitmap();
		}


		void UpdateVisuals()
		{
			bool isEnabled = operational.IsOperational;

			//colorblindness color support
			var colourOn = (Color)GlobalAssets.Instance.colorSet.logicOn;
			colourOn.a = 1; //a is 0 for these by default, but that doesnt allow tinting the symbols

			var colourOff = (Color)GlobalAssets.Instance.colorSet.logicOff;
			colourOff.a = 1;

			//no direct yellow in logic, using crop color yellow
			var colourWarning = (Color)GlobalAssets.Instance.colorSet.cropGrowing;
			colourWarning.a = 1;

			kbac.SetSymbolVisiblity("logic_O_0_0_bloom", isEnabled);
			kbac.SetSymbolVisiblity("logic_O_1_0_bloom", isEnabled);
			kbac.SetSymbolVisiblity("logic_O_2_0_bloom", isEnabled);
			kbac.SetSymbolVisiblity("logic_O_3_0_bloom", isEnabled);
			kbac.SetSymbolVisiblity("logic_O_4_0_bloom", isEnabled);
			kbac.SetSymbolVisiblity("logic_O_4_1_bloom", isEnabled);
			kbac.SetSymbolVisiblity("logic_I_0_1_bloom", isEnabled);
			kbac.SetSymbolVisiblity("logic_I_0_1_bloom", isEnabled);
			kbac.SetSymbolVisiblity("logic_I_1_1_bloom", isEnabled);
			kbac.SetSymbolVisiblity("logic_I_2_1_bloom", isEnabled);
			kbac.SetSymbolVisiblity("logic_I_3_1_bloom", isEnabled);

			kbac.SetSymbolVisiblity("lights", isEnabled);
			kbac.SetSymbolVisiblity("lights_bloom", isEnabled);
			kbac.SetSymbolVisiblity("screen_bloom", isEnabled);

			if (!isEnabled)
				return;

			//rocket presence port (top left output)
			var landedRocket = launchPad.LandedRocket;
			bool hasLandedRocket = landedRocket != null;
			kbac.SetSymbolTint("logic_O_0_0_bloom", hasLandedRocket ? colourOn : colourOff);


			//total flight status port (bottom right output)
			var craftModuleInterface = landedRocket?.CraftInterface ?? null;
			bool automatedLaunchReady = landedRocket != null ? (craftModuleInterface.CheckReadyForAutomatedLaunch() || craftModuleInterface.HasTag(GameTags.RocketNotOnGround)) : false;
			kbac.SetSymbolTint("logic_O_4_1_bloom", automatedLaunchReady ? colourOn : colourOff);

			Color GetProcessConditionColor(Status status)
			{
				switch (status)
				{
					case Status.Ready:
						return colourOn;
					case Status.Warning:
						return colourWarning;
					case Status.Failure:
					default:
						return colourOff;
				}
			}

			//individual flight status values (top right ribbon output)

			kbac.SetSymbolTint("logic_O_1_0_bloom", !hasLandedRocket ? colourOff : GetProcessConditionColor(craftModuleInterface.EvaluateConditionSet(ProcessConditionType.RocketFlight)));
			kbac.SetSymbolTint("logic_O_2_0_bloom", !hasLandedRocket ? colourOff : GetProcessConditionColor(craftModuleInterface.EvaluateConditionSet(ProcessConditionType.RocketPrep)));
			kbac.SetSymbolTint("logic_O_3_0_bloom", !hasLandedRocket ? colourOff : GetProcessConditionColor(craftModuleInterface.EvaluateConditionSet(ProcessConditionType.RocketStorage)));
			kbac.SetSymbolTint("logic_O_4_0_bloom", !hasLandedRocket ? colourOff : GetProcessConditionColor(craftModuleInterface.EvaluateConditionSet(ProcessConditionType.RocketBoard)));

			//launch & overrides (bottom left ribbon input)

			kbac.SetSymbolTint("logic_I_0_1_bloom", GetBitMaskValAtIndex(0) ? colourOn : colourOff);
			kbac.SetSymbolTint("logic_I_1_1_bloom", GetBitMaskValAtIndex(1) ? colourOn : colourOff);
			kbac.SetSymbolTint("logic_I_2_1_bloom", GetBitMaskValAtIndex(2) ? colourOn : colourOff);
			kbac.SetSymbolTint("logic_I_3_1_bloom", GetBitMaskValAtIndex(3) ? colourOn : colourOff);


			//Right Screen
			kbac.SetSymbolTint("icon_rocket", automatedLaunchReady ? colourOn : colourOff);
			kbac.SetSymbolTint("icon_platform", !hasLandedRocket ? colourWarning : automatedLaunchReady ? colourOn : colourOff);
			kbac.SetSymbolTint("icon_checkmark_yes", colourOn);
			kbac.SetSymbolTint("icon_checkmark_no", colourOff);
			kbac.SetSymbolTint("icon_warnin", colourWarning);

			kbac.SetSymbolVisiblity("icon_rocket", hasLandedRocket);
			kbac.SetSymbolVisiblity("icon_checkmark_yes", hasLandedRocket && automatedLaunchReady);
			kbac.SetSymbolVisiblity("icon_checkmark_no", hasLandedRocket && !automatedLaunchReady);
			kbac.SetSymbolVisiblity("icon_warnin", !hasLandedRocket);

			//Left Screen
			kbac.SetSymbolTint("icon_cargo", GetBitMaskValAtIndex(1) ? colourWarning : Color.gray);
			kbac.SetSymbolTint("icon_fuel", GetBitMaskValAtIndex(2) ? colourWarning : Color.gray);

		}
		public void Sim1000ms(float dt)
		{
			RocketModuleCluster landedRocket = launchPad.LandedRocket;
			if (landedRocket != null)
			{
				int status = GetRocketProcessCondition(landedRocket.CraftInterface);
				logicPorts.SendSignal(this.rocketStateOutput, status);
			}
			else
			{
				logicPorts.SendSignal(this.rocketStateOutput, 0);
			}
			this.UpdateVisuals();
		}

		public bool GetBitMaskValAtIndex(int index)
		{
			if (LaunchSignalBits == null)
				UpdateSignalBitmap();
			return LaunchSignalBits[index];
		}

		void UpdateSignalBitmap()
		{
			var inputBitsInt = logicPorts.GetInputValue(ignoreWarningInput);
			BitArray inputs = new BitArray(new int[] { inputBitsInt });
			LaunchSignalBits = new bool[inputs.Count];
			inputs.CopyTo(LaunchSignalBits, 0);
		}

		int GetRocketProcessCondition(CraftModuleInterface rocket)
		{
			int value = 0;
			if (rocket.EvaluateConditionSet(ProcessConditionType.RocketFlight) == ProcessCondition.Status.Ready)
				value += RocketPath;
			if (rocket.EvaluateConditionSet(ProcessConditionType.RocketPrep) == ProcessCondition.Status.Ready)
				value += RocketConstruction;
			if (rocket.EvaluateConditionSet(ProcessConditionType.RocketStorage) == ProcessCondition.Status.Ready)
				value += RocketStorage;
			if (rocket.EvaluateConditionSet(ProcessConditionType.RocketBoard) == ProcessCondition.Status.Ready)
				value += RocketCrew;
			return value;
		}
	}
}
