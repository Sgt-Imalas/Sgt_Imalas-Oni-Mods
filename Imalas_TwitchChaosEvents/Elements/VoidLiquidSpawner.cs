using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;
using static PropertyTextures;

namespace Imalas_TwitchChaosEvents.Elements
{
	internal class VoidLiquidSpawner : KMonoBehaviour, ISim1000ms
	{
		private static readonly CellElementEvent spawnEvent = new(
			"ChaosTwitch_VoidLiquidSpawner",
			"Spawned by Twitch",
			true
		);
		public int spawnRateSecs = 5;
		public int Duration = 60;
		public int timer = 0;

		public void Sim1000ms(float dt)
		{
			if (timer % spawnRateSecs == 0)
			{
				SpawnVoidBlob();
			}
			if(timer >= Duration)
			{
				Destroy(this);
			}
			else
				timer++;
		}
		void SpawnVoidBlob()
		{
			int cell = Grid.PosToCell(this);
			SimMessages.ReplaceAndDisplaceElement(
					cell,
					ModElements.VoidLiquid.SimHash,
					spawnEvent,
					100,
					ElementLoader.GetElement(ModElements.VoidLiquid.id).defaultValues.temperature
				);
			int cellBelow = Grid.CellBelow(cell);
			if (!Grid.IsSolidCell(cellBelow))
				return;

			var firstOnLayer = Grid.ObjectLayers.FirstOrDefault(layer => layer.ContainsKey(cellBelow));

			if (firstOnLayer != null && firstOnLayer.TryGetValue(cellBelow, out var toFleeFrom))
			{
				SgtLogger.l("fleeing from: " + toFleeFrom);
				this.GetSMI<ThreatMonitor.Instance>().ClearMainThreat();
				FleeChore fleeChore = new FleeChore(this.GetComponent<IStateMachineTarget>(), toFleeFrom);

			}

		}
	}
}
