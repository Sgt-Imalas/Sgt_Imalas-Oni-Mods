using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForceFieldWallTile.Content.Scripts
{
	internal class CometShieldImpactor : KMonoBehaviour, ISim33ms
	{
		[MyCmpReq] Comet comet;

		public void Sim33ms(float dt)
		{
			CheckForForcefield();
		}
		void CheckForForcefield()
		{
			//comet is above the map;
			if (comet.offsetPosition.y > 0)
				return;

			int cell = Grid.PosToCell(this);
			ForceFieldTile.HandleCometAt(comet, cell);
		}
	}
}
