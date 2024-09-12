using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cryopod.Buildings
{

	class CryopodLiquidPortAddon : KMonoBehaviour, ISaveLoadable, IBridgedNetworkItem
	{
		[Serialize]
		public Storage storage;
		[MyCmpGet]
		Building building;
		[MyCmpGet] private PrimaryElement panel_mat;

		private int inputCell;
		private int outputCell;
		private HandleVector<int>.Handle accumulator = HandleVector<int>.InvalidHandle;
		public MeterController meter_liquid { get; private set; }

		public ConduitType type = ConduitType.Liquid;



		public override void OnSpawn()
		{
			base.OnSpawn();
			this.meter_liquid = new MeterController(this.GetComponent<KBatchedAnimController>(), "liquid_meter_target", nameof(meter_liquid), Meter.Offset.Infront, Grid.SceneLayer.NoLayer, Array.Empty<string>());
			this.meter_liquid.SetPositionPercent(0f);

			inputCell = building.GetUtilityInputCell();
			outputCell = building.GetUtilityOutputCell();

			Conduit.GetFlowManager(type).AddConduitUpdater(ConduitUpdate);
		}
		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			accumulator = Game.Instance.accumulators.Add("Flow", this);
		}
		public override void OnCleanUp()
		{
			Conduit.GetFlowManager(type).RemoveConduitUpdater(ConduitUpdate);
			Game.Instance.accumulators.Remove(accumulator);
			base.OnCleanUp();
		}

		public void ConduitUpdate(float dt)
		{
			var flowManager = Conduit.GetFlowManager(type);
			if (!flowManager.HasConduit(inputCell) || !flowManager.HasConduit(outputCell)) return;
			if (!flowManager.GetContents(outputCell).Equals(ConduitFlow.ConduitContents.Empty))
			{
				return;
			}

			var contents = flowManager.GetContents(inputCell);
			if (contents.mass <= 0f) return;
			float panel_mat_temperature = panel_mat.Temperature;
			float content_mat_temperature = contents.temperature;

			if (panel_mat_temperature <= 0f) return;
			var element = ElementLoader.FindElementByHash(contents.element);

			var maxWattsTransferred =
				MaxWattsTransferedPerSecond(element,
				contents.temperature,
				panel_mat.Element,
				panel_mat_temperature);

			//SgtLogger.l("potential heat transfer: " + maxWattsTransferred);

			var panel_heat_capacity = building.Def.MassForTemperatureModification * panel_mat.Element.specificHeatCapacity;
			var liquid_heat_capacity = contents.mass * element.specificHeatCapacity;

			float actualKJtransferred = ActualWattsTransferred(dt, maxWattsTransferred, panel_mat_temperature, content_mat_temperature, panel_heat_capacity, liquid_heat_capacity);
			//SgtLogger.l("actual heat transfer: " + actualKJtransferred);

			float newLiquidTemperature = ContactConductivePipeBridge.GetFinalContentTemperature(actualKJtransferred, panel_mat_temperature, panel_heat_capacity, content_mat_temperature, liquid_heat_capacity);
			float newPanelTemperature = ContactConductivePipeBridge.GetFinalBuildingTemperature(content_mat_temperature, newLiquidTemperature, liquid_heat_capacity, panel_mat_temperature, panel_heat_capacity);

			//SgtLogger.l(content_mat_temperature+ "<- old liquid temp, new liquid temp->" + newLiquidTemperature);
			//SgtLogger.l(panel_mat_temperature+"<- old panel temp, new panel temp->" + newPanelTemperature);

			if (newPanelTemperature <= 0f || newLiquidTemperature <= 0f || newPanelTemperature >= 10000f || newLiquidTemperature >= 10000f)
			{
				var delta = flowManager.AddElement(outputCell, contents.element, contents.mass, contents.temperature,
					contents.diseaseIdx, contents.diseaseCount);

				if (delta <= 0f) return;
				flowManager.RemoveElement(inputCell, delta);

				Game.Instance.accumulators.Accumulate(accumulator, contents.mass);
			}
			else
			{
				var delta = flowManager.AddElement(outputCell, contents.element, contents.mass, newLiquidTemperature,
					contents.diseaseIdx, contents.diseaseCount);

				panel_mat.Temperature = newPanelTemperature;

				if (delta <= 0f) return;
				flowManager.RemoveElement(inputCell, delta);

				Game.Instance.accumulators.Accumulate(accumulator, contents.mass);
			}
		}
		public static float ActualWattsTransferred(float dt, float maxWattsTransferred, float panel_mat_temperature, float content_mat_temperature, float panel_heat_capacity, float liquid_heat_capacity)
		{
			float wattsTransferredDt = maxWattsTransferred * dt;

			float lowTemp = Mathf.Min(panel_mat_temperature, content_mat_temperature);
			float highTemp = Mathf.Max(panel_mat_temperature, content_mat_temperature);

			var delta_temp_panel = (wattsTransferredDt / panel_heat_capacity);
			var delta_temp_liquid = (-wattsTransferredDt / liquid_heat_capacity);

			float clampedNewLiquidTemp = Mathf.Clamp((content_mat_temperature + delta_temp_liquid), lowTemp, highTemp);
			float clampedNewPanelTemperature = Mathf.Clamp((panel_mat_temperature + delta_temp_panel), lowTemp, highTemp);

			float clampDiffLiquid = Mathf.Abs(clampedNewLiquidTemp - content_mat_temperature);
			float clampDiffPanel = Mathf.Abs(clampedNewPanelTemperature - panel_mat_temperature);

			return Mathf.Min(clampDiffLiquid * liquid_heat_capacity, clampDiffPanel * panel_heat_capacity) * Mathf.Sign(maxWattsTransferred);
		}
		public static float MaxWattsTransferedPerSecond(Element from, float from_temp, Element panel_material, float panel_temp)
		{
			//var conductivity = Math.Min(from.thermalConductivity, panel_material.thermalConductivity);
			var conductivity = (from.thermalConductivity + panel_material.thermalConductivity) / 2f;
			return conductivity * (from_temp - panel_temp);
		}

		public void AddNetworks(ICollection<UtilityNetwork> networks)
		{
			var networkManager = Conduit.GetNetworkManager(type);
			var networkForCell1 = networkManager.GetNetworkForCell(inputCell);
			if (networkForCell1 != null)
				networks.Add(networkForCell1);
			var networkForCell2 = networkManager.GetNetworkForCell(outputCell);
			if (networkForCell2 == null)
				return;
			networks.Add(networkForCell2);
		}

		public bool IsConnectedToNetworks(ICollection<UtilityNetwork> networks)
		{
			var flag = false;
			var networkManager = Conduit.GetNetworkManager(type);
			return flag || networks.Contains(networkManager.GetNetworkForCell(inputCell)) ||
				   networks.Contains(networkManager.GetNetworkForCell(outputCell));
		}

		public int GetNetworkCell()
		{
			return inputCell;
		}
	}
}
