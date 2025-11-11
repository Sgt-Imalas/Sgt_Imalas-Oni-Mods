using KSerialization;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static STRINGS.UI.NEWBUILDCATEGORIES;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts.CustomComplexFabricators
{
	internal class ContinuousLiquidCooledFabricatorAddon : KMonoBehaviour, IAdditionalRecipeDescriptorProvider, IOnWorkTickActionProvider, IOnRecipeCompeteActionProvider, ISingleSliderControl
	{
		public static readonly string PORT_ID = "ContinuousLiquidCooledFabricatorAddon_LOGIC_PORT";
		[MyCmpReq] ComplexFabricator fabricator;
		[MyCmpReq] Building building;
		[MyCmpReq] Operational operational;
		[MyCmpReq] PrimaryElement primaryElement;
		[MyCmpReq] KSelectable selectable;
		[MyCmpReq] LogicPorts ports;
		[MyCmpAdd] CopyBuildingSettings copyBuildingSettings;
		Extents extents;

		[SerializeField]
		public float MaxTemperature = 2000;
		[SerializeField]
		public float thermalFudge = 0.9f;
		[SerializeField]
		///true = heat is produced while the recipe is in use
		///false = heat is produced in a batch at the end of the recipe
		public bool ContinuousHeatProduction = false;
		private HandleVector<int>.Handle structureTemperature;

		[SerializeField]
		public ConduitType type = ConduitType.Liquid;

		//[SerializeField]
		//public float selfHeatPercentage = 0;

		[SerializeField]
		public float HeatCapacitorMaxCapKJ = 200000; //200 MJ of capacitor
		[SerializeField]
		public bool CreateMeter = true;

		[Serialize]
		public float accumulatedHeatExhaust = 0f;
		[Serialize]
		public bool Overheated = false;
		[Serialize]
		public bool PreventCoolantBoiling = true;
		[Serialize]
		public float LogicHeatThreshold = 75f;



		MeterController HeatLevelMeter;
		int inputCell = -1;
		int outputCell = -1;
		ComplexRecipe lastRecipeChecked = null;
		public float cachedRecipeExhaust = -1f;
		float lastOverheatDamageTime = 0;
		StatusItem NoInput, NoOutput;
		private Guid hasPipeOutputGuid;
		Tuple<ConduitType, Tag> StatusItemData;
		public static readonly Operational.Flag overheatedFlag = new Operational.Flag("aio_capacitorOverheated", Operational.Flag.Type.Requirement);
		private static readonly EventSystem.IntraObjectHandler<ContinuousLiquidCooledFabricatorAddon> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<ContinuousLiquidCooledFabricatorAddon>(((component, data) => component.OnCopySettings(data)));
		int copySettingsHandle;

		public string SliderTitleKey => "STRINGS.UI.LOGIC_PORTS.COOLANT_BATTERY_THRESHOLD.LOGIC_PORT";

		public string SliderUnits => global::STRINGS.UI.UNITSUFFIXES.PERCENT;

		void OnCopySettings(object data)
		{
			if(data is GameObject go && go.TryGetComponent<ContinuousLiquidCooledFabricatorAddon>(out var sauce))
			{
				LogicHeatThreshold = sauce.LogicHeatThreshold;
			}
		}

		public override void OnSpawn()
		{
			base.OnSpawn();
			StatusItemData = new Tuple<ConduitType, Tag>(type, GameTags.Any);
			NoInput = type switch
			{
				ConduitType.Gas => Db.Get().BuildingStatusItems.NeedGasIn,
				ConduitType.Liquid => Db.Get().BuildingStatusItems.NeedLiquidIn,
				ConduitType.Solid => Db.Get().BuildingStatusItems.NeedSolidIn,
				_ => throw new NotImplementedException(),
			};
			NoOutput = type switch
			{
				ConduitType.Gas => Db.Get().BuildingStatusItems.NeedGasOut,
				ConduitType.Liquid => Db.Get().BuildingStatusItems.NeedLiquidOut,
				ConduitType.Solid => Db.Get().BuildingStatusItems.NeedSolidOut,
				_ => throw new NotImplementedException(),
			};

			extents = building.GetExtents();
			this.inputCell = this.building.GetUtilityInputCell();
			this.outputCell = this.building.GetUtilityOutputCell();
			this.structureTemperature = GameComps.StructureTemperatures.GetHandle(this.gameObject);
			Conduit.GetFlowManager(this.type).AddConduitUpdater(Flow, ConduitFlowPriority.Default);

			if (CreateMeter)
				this.HeatLevelMeter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, null);

			selectable.AddStatusItem(StatusItemsDatabase.ThermalBattery_StorageLevel, this);
			//Subscribe((int)GameHashes.DeconstructComplete, DumpRemainingHeatIntoBuilding);
			UpdatePort();
			copySettingsHandle = Subscribe((int)GameHashes.CopySettings, OnCopySettingsDelegate);
		}
		public void DumpRemainingHeatIntoBuilding(object _)
		{
			if (accumulatedHeatExhaust <= 0) return;

			float currentTemp = primaryElement.Temperature;
			float capacityPerDegree = primaryElement.Mass * primaryElement.Element.specificHeatCapacity;
			float heatInjection = accumulatedHeatExhaust / capacityPerDegree;
			float newTemp = Mathf.Clamp(currentTemp + heatInjection, 0, 10000);
			SgtLogger.l("injecting " + accumulatedHeatExhaust + " into machine, final temperature: " + UtilMethods.GetCFromKelvin(newTemp) + ", previous temperature: " + UtilMethods.GetCFromKelvin(currentTemp));
			primaryElement.SetTemperature(newTemp);
		}

		public float GetThermalBatteryHeatIncrease()
		{
			if (accumulatedHeatExhaust <= 0) return 0;
			float currentTemp = primaryElement.Temperature;
			float capacityPerDegree = primaryElement.Mass * primaryElement.Element.specificHeatCapacity;
			float heatInjection = accumulatedHeatExhaust / capacityPerDegree;
			return heatInjection;
		}
		public override void OnCleanUp()
		{
			base.OnCleanUp();
			Unsubscribe(copySettingsHandle);
			Conduit.GetFlowManager(this.type).RemoveConduitUpdater(Flow);
		}

		void UpdateHeatState()
		{
			float heatCapacitorPercentage = HeatCapacitorPercentage();
			if (Overheated)
			{
				if (heatCapacitorPercentage < 0.05f)
					Overheated = false;

				///overheating damage at/above 100%
				if (heatCapacitorPercentage >= 1f && Time.time - lastOverheatDamageTime >= 4)
				{
					lastOverheatDamageTime += 4;
					BoxingTrigger((int)GameHashes.DoBuildingDamage,
						new BuildingHP.DamageSourceInfo()
						{
							damage = 1,
							source = global::STRINGS.BUILDINGS.DAMAGESOURCES.BUILDING_OVERHEATED,
							popString = global::STRINGS.UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.OVERHEAT,
							fullDamageEffectName = "smoke_damage_kanim"
						});
				}
			}
			else
			{
				if (heatCapacitorPercentage >= 1f)
					Overheated = true;
			}
			operational.SetFlag(overheatedFlag, !Overheated);
			HeatLevelMeter?.SetPositionPercent(heatCapacitorPercentage);
			selectable.ToggleStatusItem(StatusItemsDatabase.ThermalBattery_Overheated, Overheated);
			UpdatePort();
		}
		void UpdatePort()
		{
			ports.SendSignal(PORT_ID, HeatCapacitorPercentage() * 100f >= LogicHeatThreshold ? 1 : 0);
		}

		void UpdatePipeConnectedState()
		{
			ConduitFlow flowManager = Conduit.GetFlowManager(this.type);
			bool inputConnected = flowManager.HasConduit(this.inputCell);
			bool outputConnected = flowManager.HasConduit(this.outputCell);

			operational.SetFlag(RequireInputs.inputConnectedFlag, inputConnected);
			operational.SetFlag(RequireOutputs.outputConnectedFlag, inputConnected);

			selectable.ToggleStatusItem(NoInput, !inputConnected, StatusItemData);
			hasPipeOutputGuid = selectable.ToggleStatusItem(NoOutput, hasPipeOutputGuid, !outputConnected, this);
		}
		public static float GetAmountAllowedForMerging(float maxMass , ConduitFlow.ConduitContents from, ConduitFlow.ConduitContents to, float massDesiredtoBeMoved)
		{
			return Mathf.Min(massDesiredtoBeMoved, maxMass - to.mass);
		}
		public static bool CanMergeContents(float maxMass, ConduitFlow.ConduitContents from, ConduitFlow.ConduitContents to, float massToMove)
		{
			if (from.element != to.element && to.element != SimHashes.Vacuum && massToMove > 0f)
			{
				return false;
			}

			float amountAllowedForMerging = GetAmountAllowedForMerging(maxMass, from, to, massToMove);
			if (amountAllowedForMerging <= 0f)
			{
				return false;
			}

			return true;
		}

		///effectively isim200
		private void Flow(float dt)
		{
			ConduitFlow flowManager = Conduit.GetFlowManager(this.type);
			bool inputConnected = flowManager.HasConduit(this.inputCell);
			bool outputConnected = flowManager.HasConduit(this.outputCell);

			UpdatePipeConnectedState();
			UpdateHeatState();

			if (!inputConnected || !outputConnected)
			{
				return;
			}
			float maxCap = HighPressureConduitRegistration.GetMaxConduitCapacityAt(outputCell, type);

			ConduitFlow.ConduitContents inputContent = flowManager.GetContents(this.inputCell);
			ConduitFlow.ConduitContents outputContent = flowManager.GetContents(this.outputCell);
			float maxOutput = Mathf.Min(inputContent.mass, maxCap * dt);

			//bool pipeNotEmpty = maxOutput > 0f;
			bool contentsCanFlow = CanMergeContents(maxCap,inputContent, outputContent, maxOutput);

			//this.operational.SetFlag(RequireInputs.pipesHaveMass, pipeNotEmpty);
			//this.operational.SetFlag(RequireOutputs.pipesHaveRoomFlag, outputNotBlocked);
			if (!contentsCanFlow)
				return;

			///fallback check, runs alread in CanMergeContents
			float allowedForMerging = GetAmountAllowedForMerging(maxCap, inputContent, outputContent, maxOutput);
			if (allowedForMerging <= 0.0)
				return;
			float outputTemperature = InjectAccumulatedHeatEnergy(inputContent, allowedForMerging, dt);
			float delta = (type == ConduitType.Liquid ? Game.Instance.liquidConduitFlow : Game.Instance.gasConduitFlow)
				.AddElement(this.outputCell, inputContent.element, allowedForMerging, outputTemperature, inputContent.diseaseIdx, inputContent.diseaseCount);
			if (!Mathf.Approximately(allowedForMerging, delta))
				Debug.Log(("Mass Differs By: " + (allowedForMerging - delta).ToString()));
			flowManager.RemoveElement(this.inputCell, delta);
		}

		float InjectAccumulatedHeatEnergy(
			ConduitFlow.ConduitContents content,
			float mass,
			float dt)
		{
			if (mass <= 0)
				return 0;

			Element elementByHash = ElementLoader.FindElementByHash(content.element);
			float content_heat_capacity = mass * elementByHash.specificHeatCapacity;
			float elementTemperature = content.temperature;

			float heatInjected = accumulatedHeatExhaust;
			float tempIncrease = heatInjected / content_heat_capacity;
			float finalTemp = elementTemperature + tempIncrease;
			if (PreventCoolantBoiling)
			{
				float upperTemp = elementByHash.highTemp - 1;
				float degreesAboveThreshold = finalTemp - upperTemp;
				if (degreesAboveThreshold > 0)
				{
					//SgtLogger.l("Upper Temp: " + UtilMethods.GetCFromKelvin(upperTemp) + ", temp not injected: " + degreesAboveThreshold + ", final temp otherwise: " + UtilMethods.GetCFromKelvin(finalTemp));
					float kjKept = degreesAboveThreshold * content_heat_capacity;
					heatInjected -= kjKept;
					tempIncrease = heatInjected / content_heat_capacity;
					finalTemp = elementTemperature + tempIncrease;
				}
			}

			//SgtLogger.l("Injecting " + heatInjected + " into " + mass + "kg of " + elementByHash.name+ " with a shc of "+elementByHash.specificHeatCapacity+" from temperature " + UtilMethods.GetCFromKelvin(elementTemperature) +" to temperature "+ UtilMethods.GetCFromKelvin(finalTemp));

			accumulatedHeatExhaust -= heatInjected;
			return finalTemp;
		}

		void AddExhaustHeat(float kw, float dt)
		{
			StructureTemperatureComponents.ExhaustHeat(extents, kw, MaxTemperature, dt);
			var payload = GameComps.StructureTemperatures.GetPayload(structureTemperature);
			payload.energySourcesKW = GameComps.StructureTemperatures.AccumulateProducedEnergyKW(payload.energySourcesKW, kw, STRINGS.BUILDINGS.PREFABS.CHEMICAL_SELECTIVEARCFURNACE.REFINEMENT_HEAT_EXHAUST);
		}
		void AddSelfHeat(float kw, float dt)
		{
			GameComps.StructureTemperatures.ProduceEnergy(structureTemperature, kw * dt, (string)STRINGS.BUILDINGS.PREFABS.CHEMICAL_SELECTIVEARCFURNACE.REFINEMENT_SELF_HEAT, dt);
		}
		void ProduceContinuousExhaustHeat(float dt)
		{
			var recipe = fabricator.CurrentWorkingOrder;
			if (recipe == null)
				return;

			float totalKJ = GetRecipeKJ(recipe);

			float recipeTime = recipe.time;
			float recipeTimePercentage = dt / recipeTime;

			float kjPerSecond = totalKJ / recipeTime;
			//SgtLogger.l("Kj per sec injected: " + kjPerSecond + ", dt: " + dt + ", amount injected per timeframe: " + kjPerSecond * dt);

			accumulatedHeatExhaust += kjPerSecond * dt;

			//float selfHeat = kjPerSecond * selfHeatPercentage;
			//float exhaustHeat = kjPerSecond * (1f - selfHeatPercentage);



			//SgtLogger.l("Total recipe time: " + recipeTime + " dt: " + dt + ", percentage: " + recipeTimePercentage + ", total kj: " + totalKJ + ", Producing per sec: " + kjPerSecond);
			//AddSelfHeat(selfHeat, dt);
			//AddExhaustHeat(exhaustHeat, dt);
		}
		void ProduceRecipeHeat(ComplexRecipe recipe)
		{
			if (recipe == null) return;
			float totalKJ = GetRecipeKJ(recipe);
			accumulatedHeatExhaust += totalKJ;
		}

		float GetRecipeKJ(ComplexRecipe recipe)
		{
			if (recipe == null)
				return 0;

			if (recipe == lastRecipeChecked)
				return cachedRecipeExhaust;

			PrimaryElement firstProduct = Assets.GetPrefab(recipe.results[0].material).GetComponent<PrimaryElement>();
			float kiloJoules = -GameUtil.CalculateEnergyDeltaForElementChange(recipe.results[0].amount, firstProduct.Element.specificHeatCapacity, firstProduct.Element.highTemp, fabricator.heatedTemperature);
			cachedRecipeExhaust = kiloJoules * thermalFudge;
			return cachedRecipeExhaust;
		}

		public List<Descriptor> GetAdditionalRecipeEffects(ComplexRecipe recipe)
		{
			float fudgedJoules = GetRecipeKJ(recipe) * 1000f; //game is off by a factor of 1000 here in vanilla
			float joulesPerSecond = fudgedJoules / recipe.time;
			if (ContinuousHeatProduction)
				return [
					new Descriptor(
					string.Format(global::STRINGS.UI.BUILDINGEFFECTS.REFINEMENT_ENERGY, GameUtil.GetFormattedJoules(fudgedJoules)),
					string.Format(STRINGS.BUILDINGS.PREFABS.CHEMICAL_SELECTIVEARCFURNACE.REFINEMENT_HEAT_TOOLTIP_CONTINUOUS,GameUtil.GetFormattedJoules(fudgedJoules), GameUtil.GetFormattedJoules(joulesPerSecond))
					)];
			else
				return [
					new Descriptor(
					string.Format(global::STRINGS.UI.BUILDINGEFFECTS.REFINEMENT_ENERGY, GameUtil.GetFormattedJoules(fudgedJoules)),
						string.Format(STRINGS.BUILDINGS.PREFABS.CHEMICAL_SELECTIVEARCFURNACE.REFINEMENT_HEAT_TOOLTIP, GameUtil.GetFormattedJoules(fudgedJoules)), Descriptor.DescriptorType.Requirement)
				];
		}

		public void OnWorkTick(WorkerBase worker, float dt)
		{
			if (!ContinuousHeatProduction || !operational.IsActive)
				return;
			ProduceContinuousExhaustHeat(dt);
		}

		float HeatCapacitorPercentage() => (accumulatedHeatExhaust / HeatCapacitorMaxCapKJ);

		internal string GetHeatPercentage()
		{
			return HeatCapacitorPercentage().ToString("P0");
		}

		public void OnRecipeCompletedAction(ComplexRecipe recipe)
		{
			if (ContinuousHeatProduction)
				return;
			ProduceRecipeHeat(recipe);
		}

		public int SliderDecimalPlaces(int index) => 0;

		public float GetSliderMin(int index) => 0;

		public float GetSliderMax(int index) => 100;

		public float GetSliderValue(int index) => LogicHeatThreshold;

		public void SetSliderValue(float percent, int index)
		{
			LogicHeatThreshold = percent;
			UpdatePort();
		}

		public string GetSliderTooltipKey(int index) => "";

		public string GetSliderTooltip(int index) => STRINGS.UI.LOGIC_PORTS.COOLANT_BATTERY_THRESHOLD.LOGIC_PORT_ACTIVE;
	}
}
