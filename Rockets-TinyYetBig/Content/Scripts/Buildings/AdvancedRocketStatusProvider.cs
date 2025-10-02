using KSerialization;
using System.Collections;
using UnityEngine;
using UtilLibs;
using static ProcessCondition;

namespace Rockets_TinyYetBig.NonRocketBuildings
{
	public class AdvancedRocketStatusProvider : KMonoBehaviour, ISim1000ms
	{
		[MyCmpGet] ModularConduitPortTiler mcpt;
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

		private MeterController FuelStateMeter;
		private MeterController CargoStateMeter;


		public override void OnSpawn()
		{
			base.OnSpawn();


			this.CargoStateMeter = new MeterController(kbac,
				"meter_target_cargo",
				"meter_cargo"
				, Meter.Offset.Infront,
				Grid.SceneLayer.NoLayer, []);
			this.FuelStateMeter = new MeterController(kbac,
				"meter_target_fuel",
				"meter_fuel"
				, Meter.Offset.Infront,
				Grid.SceneLayer.NoLayer, []);

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
			var colourGood = AccessibilityUtils.LogicGood;
			var colourBad = AccessibilityUtils.LogicBad;
			var colourWarning = AccessibilityUtils.LogicWarning;

			if (!isEnabled)
			{
				return;
			}

			//rocket presence port (top left output)
			var landedRocket = launchPad.LandedRocket;
			bool hasLandedRocket = landedRocket != null;
			kbac.SetSymbolTint("logic_O_0_0_bloom", hasLandedRocket ? colourGood : colourBad);


			//total flight status port (bottom right output)
			CraftModuleInterface craftModuleInterface = landedRocket?.CraftInterface ?? null;
			bool automatedLaunchReady = landedRocket != null ? (craftModuleInterface.CheckReadyForAutomatedLaunch() || craftModuleInterface.HasTag(GameTags.RocketNotOnGround)) : false;
			kbac.SetSymbolTint("logic_O_4_1_bloom", automatedLaunchReady ? colourGood : colourBad);

			Color GetProcessConditionColor(Status status, bool includeWarning = false)
			{
				switch (status)
				{
					case Status.Ready:
						return colourGood;
					case Status.Warning:
						return includeWarning ? colourWarning : colourBad;
					case Status.Failure:
					default:
						return colourBad;
				}
			}

			//individual flight status values (top right ribbon output)

			kbac.SetSymbolTint("logic_O_1_0_bloom", !hasLandedRocket ? colourBad : GetProcessConditionColor(craftModuleInterface.EvaluateConditionSet(ProcessConditionType.RocketFlight)));
			kbac.SetSymbolTint("logic_O_2_0_bloom", !hasLandedRocket ? colourBad : GetProcessConditionColor(craftModuleInterface.EvaluateConditionSet(ProcessConditionType.RocketPrep)));
			kbac.SetSymbolTint("logic_O_3_0_bloom", !hasLandedRocket ? colourBad : GetProcessConditionColor(craftModuleInterface.EvaluateConditionSet(ProcessConditionType.RocketStorage)));
			kbac.SetSymbolTint("logic_O_4_0_bloom", !hasLandedRocket ? colourBad : GetProcessConditionColor(craftModuleInterface.EvaluateConditionSet(ProcessConditionType.RocketBoard)));

			//right cap symbols
			if (mcpt.rightCapConduit.synchronizedController is KBatchedAnimController kbac2)
			{
				kbac2.SetSymbolTint("logic_O_4_1_bloom", automatedLaunchReady ? colourGood : colourBad);
				kbac2.SetSymbolTint("logic_O_1_0_bloom", !hasLandedRocket ? colourBad : GetProcessConditionColor(craftModuleInterface.EvaluateConditionSet(ProcessConditionType.RocketFlight)));
				kbac2.SetSymbolTint("logic_O_2_0_bloom", !hasLandedRocket ? colourBad : GetProcessConditionColor(craftModuleInterface.EvaluateConditionSet(ProcessConditionType.RocketPrep)));
				kbac2.SetSymbolTint("logic_O_3_0_bloom", !hasLandedRocket ? colourBad : GetProcessConditionColor(craftModuleInterface.EvaluateConditionSet(ProcessConditionType.RocketStorage)));
				kbac2.SetSymbolTint("logic_O_4_0_bloom", !hasLandedRocket ? colourBad : GetProcessConditionColor(craftModuleInterface.EvaluateConditionSet(ProcessConditionType.RocketBoard)));
			}
			else SgtLogger.l("no kbac :(");

			//launch & overrides (bottom left ribbon input)

			kbac.SetSymbolTint("logic_I_0_1_bloom", GetBitMaskValAtIndex(0) ? colourGood : colourBad);
			kbac.SetSymbolTint("logic_I_1_1_bloom", GetBitMaskValAtIndex(1) ? colourGood : colourBad);
			kbac.SetSymbolTint("logic_I_2_1_bloom", GetBitMaskValAtIndex(2) ? colourGood : colourBad);
			kbac.SetSymbolTint("logic_I_3_1_bloom", GetBitMaskValAtIndex(3) ? colourGood : colourBad);


			//Right Screen

			kbac.SetSymbolTint("icon_checkmark_yes", colourGood);
			kbac.SetSymbolTint("icon_checkmark_no", colourBad);
			kbac.SetSymbolTint("icon_warnin", colourWarning);

			if (hasLandedRocket && craftModuleInterface != null)
			{
				Status final = Status.Ready;
				var prep = craftModuleInterface.EvaluateConditionSet(ProcessConditionType.RocketPrep);
				if (prep < final)
					final = prep;
				var storage = craftModuleInterface.EvaluateConditionSet(ProcessConditionType.RocketStorage);
				if (storage < final)
					final = storage;

				kbac.SetSymbolTint("icon_platform", GetProcessConditionColor(final, true));
				kbac.SetSymbolTint("icon_rocket", GetProcessConditionColor(final, true));

				kbac.SetSymbolVisiblity("icon_rocket", true);
				kbac.SetSymbolVisiblity("icon_checkmark_yes", final == Status.Ready);
				kbac.SetSymbolVisiblity("icon_checkmark_no", final == Status.Failure);
				kbac.SetSymbolVisiblity("icon_warnin", final == Status.Warning);
			}
			else
			{
				kbac.SetSymbolTint("icon_platform", colourGood);

				kbac.SetSymbolVisiblity("icon_rocket", false);
				kbac.SetSymbolVisiblity("icon_checkmark_yes", true);
				kbac.SetSymbolVisiblity("icon_checkmark_no", false);
				kbac.SetSymbolVisiblity("icon_warnin", false);
			}


			//Left Screen Meters

			if (hasLandedRocket)
			{
				bool engineUsesOxidizer = true, engineFound = false;
				float maxCargo = 0, maxFuel = 0, currentCargo = 0, currentEffectiveOxidizer = 0;
				//SgtLogger.l("RocketModuleCount: " + craftModuleInterface.clusterModules.Count);
				foreach (var moduleRef in craftModuleInterface.clusterModules)
				{
					var module = moduleRef.Get();
					if (module.TryGetComponent<CargoBayCluster>(out var cargoBay))
					{
						maxCargo += cargoBay.UserMaxCapacity;
						currentCargo += cargoBay.AmountStored;
						//SgtLogger.l("cargobay detected: " + cargoBay.UserMaxCapacity + " with " + cargoBay.AmountStored);
					}
					if (module.TryGetComponent<OxidizerTank>(out var oxidizerTank))
					{
						currentEffectiveOxidizer += oxidizerTank.TotalOxidizerPower;
						//SgtLogger.l("OxidizerTank detected with " + oxidizerTank.TotalOxidizerPower + " capacity");
					}
					if (module.TryGetComponent<FuelTank>(out var fuelTank))
					{
						maxFuel += fuelTank.UserMaxCapacity;
						//SgtLogger.l("FuelTank detected with " + fuelTank.UserMaxCapacity+ " capacity");
					}
					if (module.TryGetComponent<RocketEngineCluster>(out var rocketEngine))
					{
						engineFound = true;
						engineUsesOxidizer = rocketEngine.requireOxidizer;
					}
				}
				if (engineFound)
				{
					var currentFuel = craftModuleInterface.BurnableMassRemaining;
					if (maxFuel <= 0)
						FuelStateMeter.meterController.SetPositionPercent(0);
					else
						FuelStateMeter.meterController.SetPositionPercent(currentFuel / maxFuel);
					//SgtLogger.l("FuelState- Max: " + maxFuel + ", current: " + currentFuel);
				}
				else
				{
					CargoStateMeter.meterController.SetPositionPercent(0);
				}

				if (maxCargo <= 0)
					CargoStateMeter.meterController.SetPositionPercent(0);
				else
					CargoStateMeter.meterController.SetPositionPercent(currentCargo / maxCargo);
				//SgtLogger.l("CargoStateMeter - Max: " + maxCargo + ", current: " + currentCargo);
			}
			else
			{
				CargoStateMeter.meterController.SetPositionPercent(0);
				FuelStateMeter.meterController.SetPositionPercent(0);
			}
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
