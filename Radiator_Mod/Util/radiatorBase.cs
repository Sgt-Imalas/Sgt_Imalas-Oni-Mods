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
        [MyCmpReq] private KSelectable selectable;
        [MyCmpGet] private Rotatable rotatable;

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
            SetRadiatorArea();
            var building = GetComponent<Building>();
            inputCell = building.GetUtilityInputCell();
            outputCell = building.GetUtilityOutputCell();

            smi.StartSM();
            Conduit.GetFlowManager(type).AddConduitUpdater(ConduitUpdate);
            structureTemperature = GameComps.StructureTemperatures.GetHandle(gameObject);

            _radiating_status = new StatusItem(InSpaceRadiating, Category, string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.HeatFlow.ID);
            _radiating_status.resolveTooltipCallback = _FormatStatusCallback;
            _radiating_status.resolveStringCallback = _FormatStatusCallback;
            _no_space_status = new StatusItem(NotInSpace, Category, string.Empty, StatusItem.IconType.Exclamation, NotificationType.Neutral, false, OverlayModes.TileMode.ID);
            _protected_from_impacts_status = new StatusItem(BunkerDown, Category, string.Empty, StatusItem.IconType.Info, NotificationType.Good, false, OverlayModes.TileMode.ID);

            AmIInSpace();
        }

        public void SetRadiatorArea()
        {

            RadiatorArea = new List<CellOffset>();

            if (RocketInteriorModule)
            {
                for (int i = 1; i < 6; i++)
                {
                    for (int j = 0; j <= 1; j++)
                    {
                        RadiatorArea.Add(new CellOffset(j, i));
                    }
                }

            }
            else
            {
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
            var contents = flowManager.GetContents(inputCell);
            if (contents.mass <= 0f) return;
            var panel_mat = gameObject.GetComponent<PrimaryElement>();
            var panel_mat_temperature = panel_mat.Temperature;

            var element = ElementLoader.FindElementByHash(contents.element);
            var deltaheat =
                conductive_heat(element,
                contents.temperature,
                panel_mat.Element,
                panel_mat_temperature,
                RadiatorArea.Count);

            // heat change = mass * specific heat capacity * temp change        
            var deltatemp_panel = deltaheat / RadiatorBaseConfig.matCosts[0] / panel_mat.Element.specificHeatCapacity * dt;
            var deltatemp_liquid = -deltaheat / contents.mass / element.specificHeatCapacity * dt;
            var panel_newtemp = panel_mat_temperature + deltatemp_panel;
            var liquid_newtemp = contents.temperature + deltatemp_liquid;
            // In this case, the panel can at most be cooled to the content temperature
            if (panel_mat_temperature > contents.temperature)
            {
                panel_newtemp = Math.Max(panel_newtemp, contents.temperature);
                liquid_newtemp = Math.Min(liquid_newtemp, panel_mat_temperature);
            }
            else
            {
                panel_newtemp = Math.Min(panel_newtemp, contents.temperature);
                liquid_newtemp = Math.Max(liquid_newtemp, panel_mat_temperature);
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
            bool retVal = true;
            var root_cell = Grid.PosToCell(this);
            foreach (var _cell in RadiatorArea)
            {
                var _cellRotated = Rotatable.GetRotatedCellOffset(_cell, rotatable.Orientation);
                if (!UtilMethods.IsCellInSpaceAndVacuum(Grid.OffsetCell(root_cell, _cellRotated)))
                {
                    retVal = false;
                    break;
                }
            }
            selectable.ToggleStatusItem(_no_space_status, !retVal);
            smi.sm.IsInTrueSpace.Set(retVal, smi);
            return retVal;
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
