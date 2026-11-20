using Rockets_TinyYetBig.Buildings.Utility;
using Rockets_TinyYetBig.Content.ModDb;
using Rockets_TinyYetBig.Content.Scripts.Buildings.RocketModules.ModuleConditions;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UtilLibs;
using static ElementConverter;
using static Operational;

namespace Rockets_TinyYetBig.Content.Scripts.Buildings.RocketModules
{

	public class LifeSupportRocketModule : StateMachineComponent<LifeSupportRocketModule.StatesInstance>
	{
		[MyCmpReq] SymbolOverrideController soc;
		[MyCmpReq] KBatchedAnimController kbac;
		[MyCmpReq] ConduitConsumer waterInput;
		[MyCmpReq] RocketModuleCluster rocketModule;
		[MyCmpReq] Operational operational;
		[MyCmpReq] Building building;
		[MyCmpReq] KSelectable selectable;

		[SerializeField] public Storage AlgaeStorage;
		[SerializeField] public Storage OperationalStorage;
		[SerializeField] public float Co2Target = 5;
		[SerializeField] public float OxygenTarget = 10;
		[SerializeField] public float LightThreshold = 1000;
		[MyCmpReq] ElementConverter OxygenGenerator;

		KAnim.Build.Symbol _dirtSymbol;
		Tag _fertilizerTag = SimHashes.Fertilizer.CreateTag();
		Tag _waterTag = SimHashes.Water.CreateTag();
		Tag _oxygenTag = SimHashes.Oxygen.CreateTag();
		MeterController waterMeter;
		private float _waterMaxCapacity = 0;
		private Guid _waterMissingGuid = Guid.Empty;
		private Guid _algaeMissingGuid = Guid.Empty;
		private Guid _fertilizerMissingGuid = Guid.Empty;
		private string waterName, fertilizerName, algaeName;
		private bool algaeFulfilled = false;


		public override void OnSpawn()
		{
			waterName = ElementLoader.FindElementByHash(SimHashes.Water).name;
			fertilizerName = ElementLoader.FindElementByHash(SimHashes.Fertilizer).name;
			algaeName = ElementLoader.FindElementByHash(SimHashes.Algae).name;
			rocketModule.AddModuleCondition(ProcessCondition.ProcessConditionType.RocketStorage, new CustomConditionHasResource(this.AlgaeStorage, SimHashes.Algae, AlgaeStorage.capacityKg, ProcessCondition.Status.Warning, false));
			rocketModule.AddModuleCondition(ProcessCondition.ProcessConditionType.RocketPrep, new ConditionHasLifeSupportInteriorBuilding(rocketModule));

			_dirtSymbol = UIUtils.GetSymbolFromMultiObjectAnim(kbac.animFiles[0], "dirt");
			waterMeter = new MeterController(kbac, "meter_target", "meter", Meter.Offset.Behind, Grid.SceneLayer.NoLayer);
			if (waterMeter.gameObject.TryGetComponent<KBatchedAnimTracker>(out var tracker))
				tracker.matchParentOffset = true;
			_waterMaxCapacity = waterInput.capacityKG;

			RefreshMeter();
			RefreshAlgaeSymbols(true);
			OperationalRequirementsMet();

			smi.StartSM();
			this.Sub(GameHashes.OnStorageChange, OnStorageChanged);
		}
		void OnStorageChanged(object _)
		{
			RefreshMeter();
			RefreshAlgaeSymbols();
		}
		void RefreshMeter()
		{
			float waterAmount = OperationalStorage.GetAmountAvailable(_waterTag) / _waterMaxCapacity;
			waterMeter.SetPositionPercent(Mathf.Clamp01(waterAmount));
		}

		public override void OnCleanUp()
		{
			this.UnsubAll();
			base.OnCleanUp();
		}

		void RefreshAlgaeSymbols(bool init = false)
		{
			bool algaeFull = AlgaeStorage.IsFull();
			if (algaeFull != algaeFulfilled || init)
			{
				algaeFulfilled = algaeFull;
				if (algaeFull)
				{
					soc.RemoveSymbolOverride("moss");
				}
				else
				{
					soc.AddSymbolOverride("moss", _dirtSymbol);
				}
			}
		}

		bool IsOxygenFull() => OperationalStorage.GetAmountAvailable(_oxygenTag) >= OxygenTarget;

		bool OperationalRequirementsMet()
		{
			bool enoughSun = EnoughSunlight();
			bool hasConverterIngredients = true;
			bool anyFert = false, anyWater = false;


			var consumedElements = OxygenGenerator.consumedElements;
			var items = OperationalStorage.items;

			//mirroring ElementConverter.CanConvertAtAll with additional check for algae+water to avoid extra items iterating
			for (int i = 0; i < consumedElements.Length; i++)
			{
				ConsumedElement consumedElement = consumedElements[i];
				if (!consumedElement.IsActive)
				{
					continue;
				}
				var tag = consumedElement.Tag;

				bool anyItemWithTag = false;
				for (int j = 0; j < items.Count; j++)
				{
					GameObject item = items[j];
					if (item != null && item.HasTag(tag) && item.GetComponent<PrimaryElement>().Mass > 0f)
					{
						if (tag == _waterTag)
							anyWater = true;
						else if (tag == _fertilizerTag)
							anyFert = true;

						anyItemWithTag = true;
						break;
					}
				}

				if (!anyItemWithTag)
				{
					hasConverterIngredients = false;
				}
			}

			selectable.ToggleStatusItem(StatusItemDatabase.RTB_NoSunLight, !enoughSun);

			if (anyWater)
				_waterMissingGuid = selectable.RemoveStatusItem(_waterMissingGuid);
			else if (_waterMissingGuid == Guid.Empty)
				_waterMissingGuid = selectable.AddStatusItem(StatusItemDatabase.RTB_MissingElement, waterName);

			if (anyFert)
				_fertilizerMissingGuid = selectable.RemoveStatusItem(_fertilizerMissingGuid);
			else if (_fertilizerMissingGuid == Guid.Empty)
				_fertilizerMissingGuid = selectable.AddStatusItem(StatusItemDatabase.RTB_MissingElement, fertilizerName);

			if (algaeFulfilled)
				_algaeMissingGuid = selectable.RemoveStatusItem(_algaeMissingGuid);
			else if (_algaeMissingGuid == Guid.Empty)
				_algaeMissingGuid = selectable.AddStatusItem(StatusItemDatabase.RTB_MissingElement, algaeName);



			return enoughSun && hasConverterIngredients && algaeFulfilled;
		}

		bool EnoughSunlight()
		{
			if (rocketModule.CraftInterface.m_clustercraft.Status != Clustercraft.CraftStatus.Grounded)
				return true;

			int middle = building.NaturalBuildingCell(),
				left = Grid.CellLeft(middle),
				right = Grid.CellRight(middle);

			if (!Grid.IsValidCell(left) || Grid.ExposedToSunlight[left] < 1)
				return false;

			if (!Grid.IsValidCell(middle) || Grid.ExposedToSunlight[middle] < 1)
				return false;

			if (!Grid.IsValidCell(right) || Grid.ExposedToSunlight[right] < 1)
				return false;

			return true;
		}

		public class StatesInstance : GameStateMachine<States, StatesInstance, LifeSupportRocketModule, object>.GameInstance
		{
			public StatesInstance(LifeSupportRocketModule master) : base(master)
			{
			}
		}

		public class States : GameStateMachine<States, StatesInstance, LifeSupportRocketModule>
		{
			class OperationalStates : State
			{
				public State OxygenFull, Producing;
			}

			OperationalStates OperationalState;
			State RequirementsMissing;

			public override void InitializeStates(out BaseState default_state)
			{
				default_state = RequirementsMissing;

				OperationalState
					.UpdateTransition(RequirementsMissing, (smi, dt) => !smi.master.OperationalRequirementsMet(), UpdateRate.SIM_1000ms)
					.DefaultState(OperationalState.OxygenFull);

				OperationalState.Producing
					.Enter(smi => smi.master.operational.SetActive(true))
					.Exit(smi => smi.master.operational.SetActive(false))
					.EventTransition(GameHashes.OnStorageChange, OperationalState.OxygenFull, smi => smi.master.IsOxygenFull());

				OperationalState.OxygenFull
					.EnterTransition(OperationalState.Producing, smi => !smi.master.IsOxygenFull())
					.EventTransition(GameHashes.OnStorageChange, OperationalState.Producing, smi => !smi.master.IsOxygenFull());

				RequirementsMissing
					.UpdateTransition(OperationalState, (smi, dt) => smi.master.OperationalRequirementsMet(), UpdateRate.SIM_1000ms);
			}
		}
	}

}
