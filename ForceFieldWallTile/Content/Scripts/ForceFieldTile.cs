using ForceFieldWallTile.Content.Scripts.MeshGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForceFieldWallTile.Content.Scripts
{
	internal class ForceFieldTile : KMonoBehaviour, ISim4000ms
	{
		Node GridNode;
		int cell;
		public override void OnSpawn()
		{
			cell = Grid.PosToCell(this);
			GridNode = new Node(cell);
			base.OnSpawn();
			ShieldGrid.AddNode(cell, GridNode);

			Sim.Cell.Properties simCellProperties = this.GetSimCellProperties();
			SimMessages.SetCellProperties(cell, (byte)simCellProperties);
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

		public void Sim4000ms(float dt)
		{
			GridNode.Cycle();
		}
	}
}
