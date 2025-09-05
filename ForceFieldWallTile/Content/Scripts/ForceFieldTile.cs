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
		[MyCmpReq] KBatchedAnimController kbac;

		Node GridNode;
		int cell;


		public override void OnSpawn()
		{
			cell = Grid.PosToCell(this);

			var pos = Grid.CellToXY(cell);
			bool variant = pos.x % 2 == 0 ^ pos.y % 2 == 0;

			if(variant)
				kbac.flipX = true;


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
			//public string Anim(string animName) => animName + master.AnimSuffix;
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

				off.PlayAnim("off");

				on.PlayAnim(("on"));

				working_pre.PlayAnim(("working_pre"))
					.OnAnimQueueComplete(working_loop);

				working_loop.PlayAnim(("working_loop"), KAnim.PlayMode.Loop);

				working_pst.PlayAnim(("working_pst"))
					.OnAnimQueueComplete(on);
			}
		}
	}

}
