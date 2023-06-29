using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateClasses;
using UnityEngine;
using UtilLibs;

namespace Radiator_Mod
{
    public class RadiatorBase : StateMachineComponent<RadiatorBase.SMInstance>, ISaveLoadable, IBridgedNetworkItem
    //, IGameObjectEffectDescriptor
    {
        [MyCmpReq] protected Operational operational;
        [MyCmpGet] private KSelectable selectable;
        [MyCmpGet] private Building building;
        [MyCmpGet] private Rotatable rotatable; 
        [MyCmpGet] private PrimaryElement panel_mat;
        

        [Serialize]
        bool RocketInteriorModule = false;

        public static string Category = "BUILDING", InSpaceRadiating = "RadiatorInSpaceRadiating", NotInSpace = "RadiatorNotInSpace", BunkerDown = "RadiatorBunkeredDown";

        public StatusItem _radiating_status;
        public StatusItem _no_space_status;
        public StatusItem _protected_from_impacts_status;

        private int inputCell;
        private int outputCell;
        public float CurrentCoolingRadiation { get; private set; }
        private static readonly double stefanBoltzmanConstant = 5.67e-8;
        public float emissivity = .9f;
        public List<CellOffset> RadiatorArea;
        private HandleVector<int>.Handle accumulator = HandleVector<int>.InvalidHandle;
        private HandleVector<int>.Handle structureTemperature;
        public float buildingDefSHC_Modifier = 1f;


        #region NetworkStuff

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

        internal void SetRocketInternal()
        {
            RocketInteriorModule = true;
        }

        #endregion

        /// <summary>
        /// Method that runs while radiating, deletes the heat from the building "into space"
        /// </summary>
        public void RadiateIntoSpace()
        {
            var temperature =  gameObject.GetComponent<PrimaryElement>().Temperature;
            if (temperature < 5f)
                return;

            var cooling = heatRadiationAmount(temperature);
            if (cooling > 1f)
            {
                CurrentCoolingRadiation = (float)cooling;
                GameComps.StructureTemperatures.ProduceEnergy(structureTemperature, (float)-cooling / 1000,
                    BUILDING.STATUSITEMS.OPERATINGENERGY.PIPECONTENTS_TRANSFER, (float)-cooling / 1000);
                UpdateRadiation();
            }
        }

        /// <summary>
        /// Sets the Bunker state to make the building immune to meteors, also updates the status message to show up in that state
        /// </summary>
        /// <param name="on"></param>
        public void SetBunkerState(bool on)
        {

            if (on)
            {
                if (GetComponent<KPrefabID>() != null)
                    GetComponent<KPrefabID>().AddTag(GameTags.Bunker);
            }
            else
            {
                if (GetComponent<KPrefabID>() != null)
                    GetComponent<KPrefabID>().RemoveTag(GameTags.Bunker);
            }

            if (selectable != null)
                selectable.ToggleStatusItem(_protected_from_impacts_status, on);

        }

        /// <summary>
        /// Shows/hides the dtu/sec status msg
        /// </summary>
        /// <param name="isOn"></param>
        public void UpdateRadiation(bool isOn = true)
        {
            if (selectable != null)
                selectable.ToggleStatusItem(_radiating_status, isOn, this);
        }

        /// <summary>
        /// formatting for dtu/s status msg
        /// </summary>
        /// <param name="formatstr"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private static string _FormatStatusCallback(string formatstr, object data)
        {
            var radiator = (RadiatorBase)data;
            var radiation_rate = GameUtil.GetFormattedHeatEnergyRate(radiator.CurrentCoolingRadiation * 5);
            return string.Format(formatstr, radiation_rate);
        }

        #region Spawn&Cleanup
        public override void OnSpawn()
        {
            base.OnSpawn();
            inputCell = building.GetUtilityInputCell();
            outputCell = building.GetUtilityOutputCell();
            RocketInteriorModule = this.GetMyWorld().IsModuleInterior;
            SetRadiatorArea();

            Conduit.GetFlowManager(type).AddConduitUpdater(ConduitUpdate);
            structureTemperature = GameComps.StructureTemperatures.GetHandle(gameObject);

            _radiating_status = new StatusItem(InSpaceRadiating, Category, string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.HeatFlow.ID);
            _radiating_status.resolveTooltipCallback = _FormatStatusCallback;
            _radiating_status.resolveStringCallback = _FormatStatusCallback;
            _no_space_status = new StatusItem(NotInSpace, Category, string.Empty, StatusItem.IconType.Exclamation, NotificationType.Neutral, false, OverlayModes.TileMode.ID);
            _protected_from_impacts_status = new StatusItem(BunkerDown, Category, string.Empty, StatusItem.IconType.Info, NotificationType.Good, false, OverlayModes.TileMode.ID);

            AmIInSpace();
            smi.StartSM();
        }

        public void SetRadiatorArea()
        {

            RadiatorArea = new List<CellOffset>();

            if (!RocketInteriorModule)
            {
                //SgtLogger.l("Not a Rocket interior");
                for (int i = 1; i < 6; i++)
                {
                    for (int j = 0; j <= 1; j++)
                    {
                        //Debug.Log("x: " + i + ", y: " + j);
                        RadiatorArea.Add(new CellOffset(j, i));
                    }
                }

            }
            else
            {
                //SgtLogger.l("RocketInterior");
                for (int i = -2; i > -7; i--)
                {
                    for (int j = 0; j <= 1; j++)
                    {
                        //Debug.Log("x: " + i + ", y: " + j);
                        RadiatorArea.Add(new CellOffset(i, j));
                    }
                }
            }

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

        #endregion

        /// <summary>
        /// Updates for Liquids inside the radiator + connected liquid networks
        /// </summary>
        /// <param name="dt"></param>
        public void ConduitUpdate(float dt)
        {
            var flowManager = Conduit.GetFlowManager(type);
            if (!flowManager.HasConduit(inputCell)) return;
            if (!flowManager.GetContents(outputCell).Equals(ConduitFlow.ConduitContents.Empty))
            {
                return;
            }

            var contents = flowManager.GetContents(inputCell);
            if (contents.mass <= 0f) return;
            var panel_mat_temperature = panel_mat.Temperature;

            var element = ElementLoader.FindElementByHash(contents.element);

            var potentialHeatTransfer =
                conductive_heat(element,
                contents.temperature,
                panel_mat.Element,
                panel_mat_temperature);

            potentialHeatTransfer *= dt;

           // SgtLogger.l(potentialHeatTransfer + "", "pot heat transfer");

            var panel_heat_capacity = building.Def.MassForTemperatureModification;// (RadiatorBaseConfig.matCosts[0] * panel_mat.Element.specificHeatCapacity);
            var liquid_heat_capacity = contents.mass * element.specificHeatCapacity;

            //SgtLogger.l(panel_heat_capacity + "", "panel_heat_capacity");
            //SgtLogger.l(liquid_heat_capacity + "", "liquid_heat_capacity");



            float minimumTemperature = Mathf.Min(panel_mat_temperature, contents.temperature);
            float maximumTemperature = Mathf.Max(panel_mat_temperature, contents.temperature);

            //SgtLogger.l(minimumTemperature + "", "minimumTemperature");
            //SgtLogger.l(maximumTemperature + "", "maximumTemperature");

            var delta_temp_panel = (potentialHeatTransfer / panel_heat_capacity) * dt;
            var delta_temp_liquid = (-potentialHeatTransfer / liquid_heat_capacity) * dt;

            //SgtLogger.l(delta_temp_panel + "", "delta_temp_panel");
            //SgtLogger.l(delta_temp_liquid + "", "delta_temp_liquid");

            float newLiquidTemperature = (contents.temperature + delta_temp_liquid);
            float newPanelTemperature = panel_mat_temperature + delta_temp_panel;
            //SgtLogger.l(UtilMethods.GetCFromKelvin (newLiquidTemperature) + "", "newLiquidTemperature pre");
            //SgtLogger.l(UtilMethods.GetCFromKelvin (newPanelTemperature) + "", "newPanelTemperature pre");

            if (newLiquidTemperature < minimumTemperature)
            {
                delta_temp_panel *= -((contents.temperature - minimumTemperature) / (delta_temp_liquid));
                //SgtLogger.l(delta_temp_panel + "", "delta_temp_panel post");
                newLiquidTemperature = minimumTemperature;
                newPanelTemperature = panel_mat_temperature + delta_temp_panel;
            }
            else if(newLiquidTemperature > maximumTemperature)
            {
                delta_temp_panel *= -((contents.temperature - maximumTemperature) / (delta_temp_liquid));
                //SgtLogger.l(delta_temp_panel + "", "delta_temp_panel post");
                newLiquidTemperature = maximumTemperature;
                newPanelTemperature = panel_mat_temperature + delta_temp_panel;
            }

            else if (newPanelTemperature < minimumTemperature)
            {
                delta_temp_liquid *= -((panel_mat_temperature - minimumTemperature) / (delta_temp_panel));
                //SgtLogger.l(delta_temp_liquid + "", "delta_temp_liquid post");
                newPanelTemperature = minimumTemperature;
                newLiquidTemperature = (contents.temperature + delta_temp_liquid);
            }
            else if (newPanelTemperature > maximumTemperature)
            {
                delta_temp_liquid *= -((panel_mat_temperature - maximumTemperature) / (delta_temp_panel));
                //SgtLogger.l(delta_temp_liquid + "", "delta_temp_liquid post");
                newPanelTemperature = maximumTemperature;
                newLiquidTemperature = (contents.temperature + delta_temp_liquid);
            }



            //SgtLogger.l(UtilMethods.GetCFromKelvin(newLiquidTemperature) + "", "newLiquidTemperature post");
            //SgtLogger.l(UtilMethods.GetCFromKelvin(newPanelTemperature) + "", "newPanelTemperature post");


            var delta = flowManager.AddElement(outputCell, contents.element, contents.mass, newLiquidTemperature,
                contents.diseaseIdx, contents.diseaseCount);
            
            panel_mat.Temperature = newPanelTemperature;

            if (delta <= 0f) return;
            flowManager.RemoveElement(inputCell, delta);
            Game.Instance.accumulators.Accumulate(accumulator, contents.mass);
        }
        private static float conductive_heat(Element from, float from_temp, Element panel_material, float panel_temp)
        {
            //var conductivity = Math.Min(from.thermalConductivity, panel_material.thermalConductivity);
            var conductivity = (from.thermalConductivity + panel_material.thermalConductivity)/2f;
            return conductivity * (from_temp - panel_temp);
        }

        /// <summary>
        /// Calculates Thermal radiation based on temperature & some fancy formula thats based on the Stefan-Boltzman-Constant
        /// </summary>
        /// <param name="temp"></param>
        /// <returns></returns>
        private double heatRadiationAmount(float temp)
        {
            return Math.Pow(temp, 4) * stefanBoltzmanConstant * emissivity * RadiatorArea.Count * 0.2f;
        }

        /// <summary>
        /// Checks if all panel tiles are space exposed & in vacuum, updates the status msg and the state bool of the state machine
        /// </summary>
        /// <returns></returns>
        public bool AmIInSpace()
        {
            bool currentlyInSpace = true;
            var root_cell = Grid.PosToCell(this);
            foreach (var _cell in RadiatorArea)
            {
                var _cellRotated = Rotatable.GetRotatedCellOffset(_cell, rotatable.Orientation);
                if (!UtilMethods.IsCellInSpaceAndVacuum(Grid.OffsetCell(root_cell, _cellRotated), root_cell))
                {
                    //SgtLogger.l("notInSpace");
                    currentlyInSpace = false;
                    break;
                }
            }
            selectable.ToggleStatusItem(_no_space_status, !currentlyInSpace);
            smi.sm.IsInTrueSpace.Set(currentlyInSpace, smi);
            return currentlyInSpace;
        }

        #region StateMachine
        public class SMInstance : GameStateMachine<States, SMInstance, RadiatorBase, object>.GameInstance
        {
            private readonly Operational _operational;
            public readonly KSelectable _selectable;

            public SMInstance(RadiatorBase master) : base(master)
            {
                _operational = master.GetComponent<Operational>();
                _selectable = master.GetComponent<KSelectable>();

            }
            public bool IsOperational => _operational.IsFunctional && _operational.IsOperational;

        }

        public class States : GameStateMachine<States, SMInstance, RadiatorBase>
        {
            public BoolParameter IsInTrueSpace;
            public State Radiating;
            public State Retracting;
            public State Extending;
            public State Protecting;
            public State NotRadiating;
            public override void InitializeStates(out BaseState defaultState)
            {

                defaultState = NotRadiating;

                NotRadiating
                    .QueueAnim("on")
                    .Update((smi, dt) => smi.master.AmIInSpace())
                    .EventTransition(GameHashes.OperationalChanged, Retracting, smi => !smi.IsOperational)
                    .ParamTransition(this.IsInTrueSpace, Radiating, IsTrue);


                Radiating
                    .Update("Radiating", (smi, dt) =>
                {
                    smi.master.RadiateIntoSpace();
                    smi.master.AmIInSpace();

                }, UpdateRate.SIM_200ms)
                    .QueueAnim("on_rad", true)
                    .Exit(smi => smi.master.UpdateRadiation(false))
                    .EventTransition(GameHashes.OperationalChanged, Retracting, smi => !smi.IsOperational)
                    .ParamTransition(this.IsInTrueSpace, NotRadiating, IsFalse);

                Retracting
                    .PlayAnim("on_pst")
                    .OnAnimQueueComplete(Protecting);

                Protecting
                    .Enter(smi => smi.master.SetBunkerState(true))
                    .Exit(smi => smi.master.SetBunkerState(false))
                    .QueueAnim("off", true)
                    .EventTransition(GameHashes.OperationalChanged, Extending, smi => smi.IsOperational);

                Extending
                    .PlayAnim("on_pre")
                    .OnAnimQueueComplete(NotRadiating);
            }
        }
        #endregion
    }
}
