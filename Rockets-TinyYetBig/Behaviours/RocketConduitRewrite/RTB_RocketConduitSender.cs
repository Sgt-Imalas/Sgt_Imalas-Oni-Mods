using KSerialization;

namespace Rockets_TinyYetBig.Behaviours.RocketConduitRewrite
{

	public class RTB_RocketConduitSender : StateMachineComponent<RTB_RocketConduitSender.StatesInstance>, ISecondaryInput
	{

		[Serialize]
		public ConduitPortInfo conduitPortInfo;
		bool ISecondaryInput.HasSecondaryConduitType(ConduitType type) => type == this.conduitPortInfo.conduitType;
		CellOffset ISecondaryInput.GetSecondaryConduitOffset(ConduitType type) => type == this.conduitPortInfo.conduitType ? this.conduitPortInfo.offset : CellOffset.none;


		public override void OnSpawn()
		{
			smi.StartSM();
		}

		public class StatesInstance : GameStateMachine<States, StatesInstance, RTB_RocketConduitSender, object>.GameInstance
		{
			public StatesInstance(RTB_RocketConduitSender master) : base(master)
			{
			}
		}

		public class States : GameStateMachine<States, StatesInstance, RTB_RocketConduitSender>
		{
			public override void InitializeStates(out BaseState default_state)
			{
				default_state = root;
			}
		}
	}
}
