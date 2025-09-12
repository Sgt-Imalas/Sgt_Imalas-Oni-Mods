using ForceFieldWallTile.Content.ModDb;
using Klei.AI;
using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace ForceFieldWallTile.Content.Scripts
{
	internal class NavigatorForceFieldInteractions : KMonoBehaviour, ISim200ms
	{
		[MyCmpGet] Health hp;
		[MyCmpReq] Effects effects;
		[MyCmpGet] OccupyArea occupyArea;
		float shieldDamage;

		[Serialize] bool wasStuck = false;

		public override void OnSpawn()
		{
			shieldDamage = hp == null ? 0.1f : hp.maxHitPoints / 500f; // 100hp dupes -> 20% shield damage per second (a shield regens 10% per second)


			base.OnSpawn();
		}

		public void Sim200ms(float dt)
		{
			CheckForBarriers(dt);
		}

		void CheckForBarriers(float dt)
		{
			var cell = Grid.PosToCell(this);
			bool stuckInBarrier = false;

			foreach(CellOffset cellOffset in occupyArea.OccupiedCellsOffsets)
			{
				var bodyCell = Grid.OffsetCell(cell,cellOffset);
				if (ForceFieldTile.ForceFieldAt(bodyCell, out var projector))
				{
					projector.RecievePercentageDamage(shieldDamage * dt);				
					stuckInBarrier = true;
				}
			}
			if(stuckInBarrier != wasStuck)
			{
				wasStuck = stuckInBarrier;
				if (wasStuck)
					effects.Add(ModEffects.FFT_StuckInBarrier_ID, true);
				else
					effects.Remove(ModEffects.FFT_StuckInBarrier_ID);
			}
		}
	}
}
