using RoboRockets.Buildings;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateClasses;
using UnityEngine;
using UtilLibs;

namespace RadiatorMod.Util
{
    public class RadiatorBase : StateMachineComponent<RadiatorBase.SMInstance>, ISim200ms, IBridgedNetworkItem
	//, IGameObjectEffectDescriptor
	{
		[MyCmpReq] protected Operational operational;
		[MyCmpReq] private KSelectable selectable;
        [MyCmpGet] private Rotatable rotatable;
		[MyCmpGet] protected PrimaryElement mainElement;

		//public Operational.Flag visualHeatRadiationInSpace = new Operational.Flag("VisualHeatRadiationInSpace", Operational.Flag.Type.Functional);
		public StatusItem _radiating_status;
		private Guid handle_radiating;
		
		private int inputCell;
		private int outputCell;

		private static readonly double stefanBoltzmanConstant = 5.67e-8;
		public float emissivity = .9f;
		public int RadiatorAreaCurrent = 20;
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

		#endregion

		public void RadiateIntoSpace()
        {
			if (mainElement.Temperature > 5f)
				return;
			var cooling = heatRadiationAmount(mainElement.Temperature);
			if (cooling > 1f)
			{
				GameComps.StructureTemperatures.ProduceEnergy(structureTemperature, (float)-cooling / 1000,
					BUILDING.STATUSITEMS.OPERATINGENERGY.PIPECONTENTS_TRANSFER, (float)-cooling / 1000);
			}
		}
        public void Sim200ms(float dt)
		{
			var temperature = gameObject.GetComponent<PrimaryElement>().Temperature;
			if (temperature > 5f)
			{
				var cooling = heatRadiationAmount(temperature);
				if (cooling > 1f && AmIInSpace())
				{
					GameComps.StructureTemperatures.ProduceEnergy(structureTemperature, (float)-cooling / 1000,
						BUILDING.STATUSITEMS.OPERATINGENERGY.PIPECONTENTS_TRANSFER, (float)-cooling / 1000);
				}
			}
		}
        #region Spawn&Cleanup
        protected override void OnSpawn()
		{
			base.OnSpawn();
			SetRadiatorArea();
			AmIInSpace();
			var building = GetComponent<Building>();
			inputCell = building.GetUtilityInputCell();
			outputCell = building.GetUtilityOutputCell();

			smi.StartSM(); 
			Conduit.GetFlowManager(type).AddConduitUpdater(ConduitUpdate);
			structureTemperature = GameComps.StructureTemperatures.GetHandle(gameObject);
		}

		public void SetRadiatorArea()
        {
			RadiatorArea = new List<CellOffset>();
			for(int i = 1; i<6; i++)
            {
				for (int j = 0; j <= 1; j++)
                {
					RadiatorArea.Add(new CellOffset(j, i));
                }
            }

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

		#endregion
		public void ConduitUpdate(float dt)
        {
			var flowManager = Conduit.GetFlowManager(type);
			if (!flowManager.HasConduit(inputCell)) return;
			var contents = flowManager.GetContents(inputCell);
			if (contents.mass <= 0f) return;
			var panel_mat = gameObject.GetComponent<PrimaryElement>();
			var element = ElementLoader.FindElementByHash(contents.element);
			var deltaheat = conductive_heat(element, contents.temperature, panel_mat.Element, panel_mat.Temperature, RadiatorAreaCurrent);
			// heat change = mass * specific heat capacity * temp change        
			var deltatemp_panel = deltaheat / RadiatorBaseConfig.matCosts[0] / panel_mat.Element.specificHeatCapacity * dt;
			var deltatemp_liquid = -deltaheat / contents.mass / element.specificHeatCapacity * dt;
			var panel_newtemp = panel_mat.Temperature + deltatemp_panel;
			var liquid_newtemp = contents.temperature + deltatemp_liquid;
			// In this case, the panel can at most be cooled to the content temperature
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
		private double heatRadiationAmount(float temp)
		{
			return Math.Pow(temp, 4) * stefanBoltzmanConstant * emissivity * RadiatorAreaCurrent * 0.2f;
		}
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
			smi.sm.IsInTrueSpace.Set(retVal, smi);
			return retVal;
		}

        #region StateMachine
		public class SMInstance : GameStateMachine<States, SMInstance, RadiatorBase, object>.GameInstance
		{
			private readonly Operational _operational;

			public SMInstance(RadiatorBase master) : base(master)
			{
				_operational = master.GetComponent<Operational>();
			}
			public bool IsOperational => _operational.IsOperational;

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
				defaultState = Extending;

				NotRadiating
					.QueueAnim("on")
					.EventTransition(GameHashes.OperationalChanged, Retracting, smi => !smi.IsOperational)
					.ParamTransition(this.IsInTrueSpace, Radiating, IsTrue);


				Radiating
					.QueueAnim("on_rad", true)
					.EventTransition(GameHashes.OperationalChanged, Retracting, smi => !smi.IsOperational)
					.ParamTransition(this.IsInTrueSpace, NotRadiating, IsFalse);

				Retracting
					.PlayAnim("on_pst")
					.OnAnimQueueComplete(Protecting);

				Protecting
					.Enter(smi => smi.master.GetComponent<KPrefabID>().AddTag(GameTags.Bunker))
					.Exit(smi => smi.master.GetComponent<KPrefabID>().RemoveTag(GameTags.Bunker))
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
