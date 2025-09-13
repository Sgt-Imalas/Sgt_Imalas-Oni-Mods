using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	internal class ComplexFabricatorActiveLogicOutput : KMonoBehaviour
	{
		public static readonly string PORT_ID = "ComplexFabricatorActiveLogicOutput_LOGIC_PORT";
		[MyCmpReq] Operational operational;
		[MyCmpReq] LogicPorts ports;

		public override void OnSpawn()
		{
			base.OnSpawn();
			Subscribe((int) GameHashes.ActiveChanged, OnActiveChanged);
			OnActiveChanged(null);
		}
		public override void OnCleanUp()
		{
			Unsubscribe((int)GameHashes.ActiveChanged, OnActiveChanged);
			base.OnCleanUp();
		}
		void OnActiveChanged(object _)
		{
			ports.SendSignal(PORT_ID, operational.IsActive ? 1 : 0);
		}

		public static List<LogicPorts.Port> CreateSingleOutputPortList(CellOffset offset)
		{
			return new List<LogicPorts.Port> { LogicPorts.Port.OutputPort(PORT_ID, offset, 
				STRINGS.UI.LOGIC_PORTS.FABRICATOR_ACTIVE.LOGIC_PORT,
				STRINGS.UI.LOGIC_PORTS.FABRICATOR_ACTIVE.LOGIC_PORT_ACTIVE,
				STRINGS.UI.LOGIC_PORTS.FABRICATOR_ACTIVE.LOGIC_PORT_INACTIVE) };
		}
	}
}
