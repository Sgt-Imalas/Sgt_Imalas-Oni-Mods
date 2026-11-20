using KSerialization;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UtilLibs.BuildingPortUtils;
using static OniRetroEdition.STRINGS.UI;

namespace OniRetroEdition.Behaviors
{
	internal class ConduitSplitter : KMonoBehaviour, ISingleSliderControl, IBridgedNetworkItem, ISecondaryOutput
	{
		[MyCmpReq] private Building building;

		[SerializeField]public ConduitType ConduitType;
		[SerializeField]public ConduitPortInfo SecondaryPort;
		[SerializeField]public bool Flipped;

		private int inputCell;
		//left output cell
		private int leftOutputCell;
		//right output cell
		private int rightOutputCell;
		private HandleVector<int>.Handle accumulator = HandleVector<int>.InvalidHandle;
		private FlowUtilityNetwork.NetworkItem secondaryOutput;
		private IUtilityNetworkMgr networkManager;
		private ConduitFlow conduitFlowManager;

		[Serialize] private float splitPercentage = 50;

		#region slider
		public string SliderTitleKey => "STRINGS.UI.RETRO_SPLITTERS.TITLE";

		public string SliderUnits => global::STRINGS.UI.UNITSUFFIXES.PERCENT;


		public float GetSliderMax(int index) => 100;

		public float GetSliderMin(int index) => 0;

		public string GetSliderTooltip(int index) => RETRO_SPLITTERS.TOOLTIP;

		public string GetSliderTooltipKey(int index) => string.Empty;

		public float GetSliderValue(int index) => splitPercentage;


		public void SetSliderValue(float percent, int index) => splitPercentage = percent;

		public int SliderDecimalPlaces(int index) => 1;
		#endregion
		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			this.accumulator = Game.Instance.accumulators.Add("Flow", this);
		}
		public override void OnSpawn()
		{
			base.OnSpawn();
			this.inputCell = building.GetUtilityInputCell();
			this.leftOutputCell = building.GetUtilityOutputCell();
			rightOutputCell = Grid.OffsetCell(Grid.PosToCell(transform.GetPosition()), building.GetRotatedOffset(SecondaryPort.offset));
			secondaryOutput = new FlowUtilityNetwork.NetworkItem(SecondaryPort.conduitType, Endpoint.Source, rightOutputCell,this.gameObject);

			networkManager = Conduit.GetNetworkManager(ConduitType);
			networkManager.AddToNetworks(rightOutputCell, secondaryOutput, true);
			conduitFlowManager = Conduit.GetFlowManager(ConduitType);

			Conduit.GetFlowManager(ConduitType).AddConduitUpdater(new Action<float>(this.ConduitUpdate), ConduitFlowPriority.Default);
		}
		public override void OnCleanUp()
		{
			networkManager?.RemoveFromNetworks(rightOutputCell, secondaryOutput, true);
			conduitFlowManager?.RemoveConduitUpdater(new Action<float>(this.ConduitUpdate));
			Game.Instance.accumulators.Remove(this.accumulator);
			base.OnCleanUp();
		}
		private void ConduitUpdate(float dt)
		{
			if (!conduitFlowManager.HasConduit(inputCell))
				return;

			bool connectedLeft = conduitFlowManager.HasConduit(leftOutputCell);
			bool connectedRight = conduitFlowManager.HasConduit(rightOutputCell);
			if(!connectedLeft && !connectedRight) 
				return;


			var contents = conduitFlowManager.GetContents(inputCell);
			if (contents.mass <= 0 || contents.element == SimHashes.Vacuum)
				return;
			var inputElement = contents.element;

			//only right side connected
			if (!connectedLeft)
			{
				float delta = conduitFlowManager.AddElement(rightOutputCell, contents.element, contents.mass, contents.temperature, contents.diseaseIdx, contents.diseaseCount);
				conduitFlowManager.RemoveElement(inputCell, delta);
			}
			//only left side connected
			else if (!connectedRight)
			{
				float delta = conduitFlowManager.AddElement(leftOutputCell, contents.element, contents.mass, contents.temperature, contents.diseaseIdx, contents.diseaseCount);
				conduitFlowManager.RemoveElement(inputCell, delta);
			}
			//both are connected
			else
			{
				float desiredLeft  = splitPercentage * 0.01f * contents.mass;
				float desiredRight = contents.mass - desiredLeft;
				if (Flipped)
					(desiredLeft, desiredRight) = (desiredRight, desiredLeft);


				float spaceLeft = SpaceInPipe(leftOutputCell, inputElement);
				float spaceRight = SpaceInPipe(rightOutputCell, inputElement);

				float toLeft = Mathf.Min(desiredLeft, spaceLeft);
				float toRight = Mathf.Min(desiredRight, spaceRight);

				//redistribute overflow into whichever side still has room
				float remaining = contents.mass - toLeft - toRight;
				if (remaining > 0f)
				{
					float extraLeft = Mathf.Min(remaining, spaceLeft - toLeft);
					toLeft += extraLeft;
					remaining -= extraLeft;

					float extraRight = Mathf.Min(remaining, spaceRight - toRight);
					toRight += extraRight;
				}

				float moved = 0f;
				moved += PutIntoPipe(leftOutputCell, contents, toLeft);
				moved += PutIntoPipe(rightOutputCell, contents, toRight);

				if (moved > 0f)
					conduitFlowManager.RemoveElement(inputCell, moved);
			}
			Game.Instance.accumulators.Accumulate(this.accumulator, contents.mass);
		}
		private float SpaceInPipe(int cell, SimHashes element)
		{
			var cellContents = conduitFlowManager.GetContents(cell);
			if (cellContents.mass <= 0f || cellContents.element == SimHashes.Vacuum)
				return conduitFlowManager.MaxMass;
			if (cellContents.element == element)
				return Mathf.Max(0f, conduitFlowManager.MaxMass - cellContents.mass);
			return 0f;
		}

		private float PutIntoPipe(int cell, ConduitFlow.ConduitContents contents, float amount)
		{
			if (amount <= 0f)
				return 0f;

			//scale disease proportionally to the share of mass being moved
			int disease = Mathf.RoundToInt(contents.diseaseCount * (amount / contents.mass));
			return conduitFlowManager.AddElement(cell, contents.element, amount, contents.temperature, contents.diseaseIdx, disease);
		}
		#region networksStuff
		public void AddNetworks(ICollection<UtilityNetwork> networks)
		{
			var outputCellNetwork = networkManager.GetNetworkForCell(leftOutputCell);
			if (outputCellNetwork != null) 
				networks.Add(outputCellNetwork);

			var secondaryOutputCellNetwork = networkManager.GetNetworkForCell(rightOutputCell);
			if (secondaryOutputCellNetwork != null)
				networks.Add(secondaryOutputCellNetwork);

			var inputCellNetwork = networkManager.GetNetworkForCell(inputCell);
			if (inputCellNetwork != null)
				networks.Add(inputCellNetwork);
		}

		public int GetNetworkCell() => inputCell;

		public bool IsConnectedToNetworks(ICollection<UtilityNetwork> networks) => networks.Contains(networkManager.GetNetworkForCell(inputCell)) || networks.Contains(networkManager.GetNetworkForCell(leftOutputCell)) || networks.Contains(networkManager.GetNetworkForCell(rightOutputCell));

		public bool HasSecondaryConduitType(ConduitType type) => type == ConduitType;

		public CellOffset GetSecondaryConduitOffset(ConduitType type) => SecondaryPort.offset;
		#endregion
	}
}
