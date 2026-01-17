using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;
using static STRINGS.UI.SPACEDESTINATIONS;

namespace RotatableRadboltStorage
{
	class LogicHEPDirectionController : KMonoBehaviour
	{
		[MyCmpReq]
		private LogicPorts logicPorts;
		private IHighEnergyParticleDirection directionController;
		public override void OnSpawn()
		{
			directionController = this.GetComponent<IHighEnergyParticleDirection>();
			Subscribe((int)GameHashes.LogicEvent, OnLogicValueChanged);
			base.OnSpawn();
		}
		public override void OnCleanUp()
		{
			Unsubscribe((int)GameHashes.LogicEvent, OnLogicValueChanged);
			base.OnCleanUp();
		}
		void OnLogicValueChanged(object _)
		{
			if (logicPorts.inputPortInfo == null || !logicPorts.inputPortInfo.Any())
				return;

			var firstInputPort = logicPorts.inputPortInfo.First();
			HashedString portId = firstInputPort.id;

			if (!logicPorts.IsPortConnected(portId) || logicPorts.GetConnectedWireBitDepth(portId) != LogicWire.BitDepth.FourBit)
				return;

			int portValue = logicPorts.GetInputValue(firstInputPort.id);
			int stateValue = 0;
			if (LogicCircuitNetwork.IsBitActive(1, portValue))
				stateValue += 1;
			if (LogicCircuitNetwork.IsBitActive(2, portValue))
				stateValue += 2;
			if (LogicCircuitNetwork.IsBitActive(3, portValue))
				stateValue += 4;

			EightDirection targetDirection = (EightDirection)stateValue;
			if (directionController.Direction != targetDirection)
				directionController.Direction = targetDirection;

		}
	}
}
