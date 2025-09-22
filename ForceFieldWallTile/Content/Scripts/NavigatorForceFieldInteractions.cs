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
		[MyCmpGet] MinionIdentity identity;
		float shieldDamage;

		[Serialize] bool wasStuck = false;

		bool SlowDupe = true;
		public override void OnSpawn()
		{
			shieldDamage = hp == null ? 0.1f : hp.maxHitPoints / 500f; // 100hp dupes -> 20% shield damage per second (a shield regens 10% per second)

			SlowDupe = Config.Instance.SlowEffect;

			base.OnSpawn();
		}

		public void Sim200ms(float dt)
		{
			CheckForBarriers(dt);
		}

		void CheckForBarriers(float dt)
		{
			if (!gameObject.activeSelf)
				return;
			if (transform == null)
				return;

			var cell = Grid.PosToCell(this);
			if (!Grid.IsValidCell(cell))
				return;

			bool stuckInBarrier = false;

			//var cellOffsets = occupyArea.OccupiedCellsOffsets;
			//if (cellOffsets != null)
			//{
			//	foreach (CellOffset cellOffset in occupyArea.OccupiedCellsOffsets)
			//	{
			//		var bodyCell = Grid.OffsetCell(cell, cellOffset);
			//		if (!Grid.IsValidCell(bodyCell))
			//			continue;
			if (ForceFieldTile.ForceFieldAt(cell, out var projector))
			{
				projector.RecievePercentageDamage(shieldDamage * dt);
				stuckInBarrier = true;
			}
			//	}
			//}
			if (!SlowDupe)
				return;

			if (stuckInBarrier != wasStuck)
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
