using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Cryopod.Buildings
{
    
    class CryopodLiquidPortAddon : KMonoBehaviour, ISim200ms, ISaveLoadable, IBridgedNetworkItem
    {
        [Serialize]
        public Storage storage;

        private int inputCell;
        private int outputCell;
        private HandleVector<int>.Handle accumulator = HandleVector<int>.InvalidHandle;
        public MeterController meter_liquid { get; private set; }


        protected override void OnSpawn()
        {
            base.OnSpawn();
            this.meter_liquid = new MeterController(this.GetComponent<KBatchedAnimController>(), "liquid_meter_target", nameof(meter_liquid), Meter.Offset.Infront, Grid.SceneLayer.NoLayer, Array.Empty<string>());
            this.meter_liquid.SetPositionPercent(0f);

            var building = GetComponent<Building>();
            inputCell = building.GetUtilityInputCell();
            outputCell = building.GetUtilityOutputCell();

            Conduit.GetFlowManager(type).AddConduitUpdater(ConduitUpdate);
        }
        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            accumulator = Game.Instance.accumulators.Add("Flow", this);
        }
        protected override void OnCleanUp()
        {
            Conduit.GetFlowManager(type).RemoveConduitUpdater(ConduitUpdate);
            Game.Instance.accumulators.Remove(accumulator);
            base.OnCleanUp();
        }

        public void ConduitUpdate(float dt)
        {
            var flowManager = Conduit.GetFlowManager(type);
            if (!flowManager.HasConduit(inputCell)) return;
            var contents = flowManager.GetContents(inputCell);
            if (contents.mass <= 0f) return;
            var panel_mat = gameObject.GetComponent<PrimaryElement>();
            var element = ElementLoader.FindElementByHash(contents.element);

            var deltaheat = conductive_heat( panel_mat.Element, panel_mat.Temperature, element, contents.temperature, 7);
            // heat change = mass * specific heat capacity * temp change        
            var deltatemp_panel = -deltaheat / BuildableCryopodLiquidConfig.MetalCost / panel_mat.Element.specificHeatCapacity * dt;
            var deltatemp_liquid = deltaheat / contents.mass / element.specificHeatCapacity * dt;

            var panel_newtemp = panel_mat.Temperature + deltatemp_panel;
            var liquid_newtemp = contents.temperature + deltatemp_liquid;

            if (panel_mat.Temperature > contents.temperature)
            {
                panel_newtemp = Math.Max(panel_newtemp, contents.temperature);
                liquid_newtemp = Math.Min(liquid_newtemp, panel_mat.Temperature);
            }
            else
            {
                panel_newtemp = Math.Min(panel_newtemp, contents.temperature);
                liquid_newtemp = Math.Max(liquid_newtemp, panel_mat.Temperature);
            }

            var delta = flowManager.AddElement(outputCell, contents.element, contents.mass, liquid_newtemp,
                contents.diseaseIdx, contents.diseaseCount);
            panel_mat.Temperature = panel_newtemp;
            if (delta <= 0f) return;
            flowManager.RemoveElement(inputCell, delta);
            Game.Instance.accumulators.Accumulate(accumulator, contents.mass);
        }
        private static float conductive_heat(Element from, float from_temp, Element panel_material, float panel_temp, float area)
        {
            var conductivity = Math.Min(from.thermalConductivity, panel_material.thermalConductivity);
            return conductivity * area * (from_temp - panel_temp) * 1f;
        }

        public void Sim200ms(float dt)
        {
            ////if (inStorage.MassStored() <= 0f)
            ////    operational.SetActive(false);
            ////else//
            //this.meter_liquid.SetPositionPercent(Mathf.Clamp01(storage.MassStored() / 50));
            //if (storage.MassStored() > 0f)
            //{
            //    var contents = storage.GetItems().First().GetComponent<PrimaryElement>();
            //    var element = ElementLoader.FindElementByHash(contents.ElementID);
            //    meter_liquid.SetSymbolTint(new KAnimHashedString("meter_fill_liquid"), element.substance.colour);
            //}
        }


        public ConduitType type = ConduitType.Liquid;
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
