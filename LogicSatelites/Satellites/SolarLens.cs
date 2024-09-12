using LogicSatellites.Behaviours;

namespace LogicSatellites.Satellites
{
	class SolarLens : KMonoBehaviour
	{
		[MyCmpGet]
		SatelliteGridEntity entity;


		int OrbitWorldId = -1;
		public override void OnSpawn()
		{
			base.OnSpawn();
			var asteroid = ClusterGrid.Instance.GetVisibleEntityOfLayerAtAdjacentCell(entity.Location, EntityLayer.Asteroid);
			WorldContainer world = asteroid.GetComponent<WorldContainer>();
			OrbitWorldId = world.id;

		}
	}
}
