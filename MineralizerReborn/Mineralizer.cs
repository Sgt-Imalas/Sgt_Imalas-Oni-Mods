using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;
using UtilLibs;

namespace MineralizerReborn
{
	internal class Mineralizer : StateMachineComponent<Mineralizer.SMInstance>, FewOptionSideScreen.IFewOptionSideScreen
	{
		[MyCmpGet]
		public ElementConverter converter;

		[MyCmpGet]
		public ManualDeliveryKG manualDelivery;

		public Storage saltStorage;
		public MeterController salt_meter { get; private set; }
		public Mineralizer() { }

		[Serialize]
		[SerializeField]
		public Tag mineral = SimHashes.SaltWater.CreateTag();

		[Serialize]
		[SerializeField]
		private SimHashes inputMaterial = SimHashes.Salt;


		public float RatePerSecond = 5f;

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
		}

		public override void OnSpawn()
		{
			base.OnSpawn();

			LoadMineralizerConfig(mineral);
			smi.StartSM();
			salt_meter = new MeterController(this.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Behind, Grid.SceneLayer.NoLayer, Array.Empty<string>());
			this.Subscribe((int)GameHashes.OnStorageChange, OnStorageChange);
			OnStorageChange(null);
		}

		private void OnStorageChange(object data)
		{
			var salt = saltStorage.GetMassAvailable(inputMaterial);
			this.salt_meter.SetPositionPercent(salt / (saltStorage.capacityKg - 20f));
		}

		private void CleaningUpOldAccumulators()
		{
			for (int i = converter.consumedElements.Length - 1; i >= 0; i--)
			{
				Game.Instance.accumulators.Remove(converter.consumedElements[i].Accumulator);
				converter.consumedElements[i].Accumulator.index = 0;
			}

			for (int i = converter.outputElements.Length - 1; i >= 0; i--)
			{
				Game.Instance.accumulators.Remove(converter.outputElements[i].accumulator);
				converter.outputElements[i].accumulator._index = 0;
			}
		}
		private void CreatingNewAccumulators()
		{

			for (int i = 0; i < converter.consumedElements.Length; i++)
			{
				converter.consumedElements[i].Accumulator = Game.Instance.accumulators.Add("ElementsConsumed", converter);
			}

			converter.totalDiseaseWeight = 0f;
			for (int j = 0; j < converter.outputElements.Length; j++)
			{
				converter.outputElements[j].accumulator = Game.Instance.accumulators.Add("OutputElements", converter);
				converter.totalDiseaseWeight += converter.outputElements[j].diseaseWeight;
			}
		}

		public class MineralizerOption
		{
			public SimHashes OutputLiquid;
			public SimHashes InputMaterial;
			public float materialPercentage;
			public string dlcRequirement = string.Empty;

			public MineralizerOption(SimHashes input, SimHashes output, float percentage, string dlc = "")
			{
				InputMaterial = input;
				OutputLiquid = output;
				materialPercentage = percentage;
				dlcRequirement = dlc;
			}
		}

		public static Dictionary<Tag, MineralizerOption> MineralizerOptions
		{
			get
			{
				if (_mineralizerOptions == null)
				{
					InitializeMineralizerOptions();
				}
				return _mineralizerOptions;
			}
		}

		public static void InitializeMineralizerOptions()
		{
			_mineralizerOptions = new();
			_mineralizerOptions.Add(SimHashes.SaltWater.CreateTag(), new(SimHashes.Salt, SimHashes.SaltWater, 0.07f));
			//_mineralizerOptions.Add(SimHashes.Brine.CreateTag(), new(SimHashes.Salt, SimHashes.Brine, 0.30f));
			_mineralizerOptions.Add(SimHashes.SugarWater.CreateTag(), new(SimHashes.Sucrose, SimHashes.SugarWater, 0.77f, DlcManager.DLC2_ID));
		}

		public static Tag WaterToSaltWater = nameof(WaterToSaltWater);

		private static Dictionary<Tag, MineralizerOption> _mineralizerOptions = null;

		public FewOptionSideScreen.IFewOptionSideScreen.Option[] GetOptions()
		{
			var list = new List<FewOptionSideScreen.IFewOptionSideScreen.Option>();

			foreach (var entry in MineralizerOptions)
			{
				var entryVal = entry.Value;
				var inputElement = ElementLoader.GetElement(entryVal.InputMaterial.CreateTag());
				var outputElement = ElementLoader.GetElement(entryVal.OutputLiquid.CreateTag());



				if (entryVal.InputMaterial == SimHashes.Salt || DiscoveredResources.Instance.IsDiscovered(entryVal.InputMaterial.CreateTag()) && (entryVal.dlcRequirement == string.Empty || SaveLoader.Instance.GameInfo.dlcIds.Contains(entryVal.dlcRequirement)))
				{
					var inputTag = inputElement.tag;
					var inputProperName = inputTag.ProperName();

					var tooltip = string.Format(global::STRINGS.CODEX.FORMAT_STRINGS.TRANSITION_LABEL_TO_ONE_ELEMENT,
					   inputProperName, outputElement.tag.ProperName());
					list.Add(new(outputElement.tag, outputElement.tag.ProperName(), Def.GetUISprite(outputElement.tag), tooltip));
				}
			}
			return list.ToArray();
		}

		public void OnOptionSelected(FewOptionSideScreen.IFewOptionSideScreen.Option option)
		{
			var newMineral = option.tag;
			if (newMineral == mineral)
				return;
			LoadMineralizerConfig(option.tag);
		}

		public void LoadMineralizerConfig(Tag target)
		{
			if (!MineralizerOptions.TryGetValue(target, out MineralizerOption option))
			{
				SgtLogger.warning(target + " was an invalid tag!");
				return;
			}
			mineral = target;
			var inputTag = option.InputMaterial.CreateTag();
			this.inputMaterial = option.InputMaterial;

			if (inputTag != manualDelivery.requestedItemTag)
			{
				saltStorage.DropHasTags(new[] { manualDelivery.requestedItemTag });
			}

			manualDelivery.RequestedItemTag = inputTag;

			float matPerSecond = RatePerSecond * option.materialPercentage;
			float waterPerSecond = RatePerSecond - matPerSecond;


			CleaningUpOldAccumulators();
			converter.consumedElements = new ElementConverter.ConsumedElement[]
			{
				new ElementConverter.ConsumedElement(inputTag, matPerSecond),
				new ElementConverter.ConsumedElement(SimHashes.Water.CreateTag(), waterPerSecond)
			};
			converter.outputElements = new ElementConverter.OutputElement[]
			{
				new ElementConverter.OutputElement(RatePerSecond, option.OutputLiquid, 0.0f, false, true, 0.0f, 0.5f, 0.75f, byte.MaxValue, 0),
			};

			CreatingNewAccumulators();

		}


		public Tag GetSelectedOption() => mineral;

		public class SMInstance : GameStateMachine<States, SMInstance, Mineralizer, object>.GameInstance
		{
			public SMInstance(Mineralizer master) : base(master) { }
		}

		public class States : GameStateMachine<States, SMInstance, Mineralizer>
		{
			public State Working;
			public State StartWorking;
			public State StopWorking;
			public State Operational;
			public State NotOperational;

			public override void InitializeStates(out BaseState defaultState)
			{
				defaultState = NotOperational;

				NotOperational
					.QueueAnim("off")
					.EventTransition(GameHashes.OperationalChanged, Operational, smi => smi.GetComponent<Operational>().IsOperational);

				Operational
					.QueueAnim("on")
					.EventTransition(GameHashes.OperationalChanged, NotOperational, smi => !smi.GetComponent<Operational>().IsOperational)
					.EventTransition(GameHashes.OnStorageChange, StartWorking, smi => smi.GetComponent<ElementConverter>().HasEnoughMassToStartConverting());

				StartWorking
					.PlayAnim("working_pre")
					.Enter(smi => smi.GetComponent<LoopingSounds>().PlayEvent(new GameSoundEvents.Event("event:/Buildings/BuildCategories/05Utilities/LiquidConditioner/LiquidConditioner_start")))
					.OnAnimQueueComplete(Working);

				Working
					.Enter(smi => smi.GetComponent<Operational>().SetActive(true, false))
					.Enter(smi => smi.GetComponent<LoopingSounds>().StartSound("event:/Buildings/BuildCategories/05Utilities/LiquidConditioner/LiquidConditioner_lP"))
					.QueueAnim("working_loop", true)
					.EventTransition(GameHashes.OnStorageChange, StopWorking, smi => !smi.GetComponent<ElementConverter>().CanConvertAtAll())
					.EventTransition(GameHashes.OperationalChanged, StopWorking, smi => !smi.GetComponent<Operational>().IsOperational)
					.Exit(smi => smi.GetComponent<Operational>().SetActive(false, false))
					.Exit(smi => smi.GetComponent<LoopingSounds>().StopSound("event:/Buildings/BuildCategories/05Utilities/LiquidConditioner/LiquidConditioner_lP"));

				StopWorking
					.PlayAnim("working_pst")
					.Enter(smi => smi.GetComponent<LoopingSounds>().PlayEvent(new GameSoundEvents.Event("event:/Buildings/BuildCategories/05Utilities/LiquidConditioner/LiquidConditioner_end")))
					.OnAnimQueueComplete(Operational);
			}
		}
	}
}