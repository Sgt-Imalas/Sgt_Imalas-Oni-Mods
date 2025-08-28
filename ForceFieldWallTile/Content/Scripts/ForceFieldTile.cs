using ForceFieldWallTile.Content.Scripts.MeshGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForceFieldWallTile.Content.Scripts
{
	public class ForceFieldTile : StateMachineComponent<ForceFieldTile.StatesInstance>
	{
		Node GridNode;
		int cell;

		public string AnimSuffix { get; private set; }

		public override void OnSpawn()
		{
			cell = Grid.PosToCell(this);

			var pos = Grid.CellToXY(cell);
			bool variant = pos.x % 2 == 0 ^ pos.y % 2 == 0;

			AnimSuffix = variant ? "_A" : "_B";

			GridNode = new Node(cell);
			base.OnSpawn();
			ShieldGrid.AddNode(cell, GridNode);

			Sim.Cell.Properties simCellProperties = this.GetSimCellProperties();
			SimMessages.SetCellProperties(cell, (byte)simCellProperties);
			smi.StartSM();
		}
		public override void OnCleanUp()
		{
			base.OnCleanUp();
			ShieldGrid.RemoveNode(cell, GridNode);
			SimMessages.ClearCellProperties(cell, (byte)GetSimCellProperties());
		}

		private Sim.Cell.Properties GetSimCellProperties()
		{
			Sim.Cell.Properties simCellProperties = Sim.Cell.Properties.Transparent;

			simCellProperties |= Sim.Cell.Properties.GasImpermeable;
			simCellProperties |= Sim.Cell.Properties.LiquidImpermeable;

			return simCellProperties;
		}

		public class StatesInstance : GameStateMachine<States, StatesInstance, ForceFieldTile, object>.GameInstance
		{
			public StatesInstance(ForceFieldTile master) : base(master)
			{
			}
			public string Anim(string animName) => animName + master.AnimSuffix;
		}

		public class States : GameStateMachine<States, StatesInstance, ForceFieldTile>
		{
			public State off;
			public State on;
			public State working_pre;
			public State working_loop;
			public State working_pst;

			public override void InitializeStates(out BaseState default_state)
			{
				default_state = off;

				off.PlayAnim((smi) => smi.Anim("off"));

				on.PlayAnim((smi) => smi.Anim("on"));

				working_pre.PlayAnim((smi) => smi.Anim("working_pre"))
					.OnAnimQueueComplete(working_loop);

				working_loop.PlayAnim((smi) => smi.Anim("working_loop"), KAnim.PlayMode.Loop);

				working_pst.PlayAnim((smi) => smi.Anim("working_pst"))
					.OnAnimQueueComplete(on);
			}
		}
	}

}
