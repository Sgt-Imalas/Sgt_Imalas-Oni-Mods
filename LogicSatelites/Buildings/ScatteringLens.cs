namespace LogicSatellites.Buildings
{
	internal class ScatteringLens : KMonoBehaviour, ISim200ms
	{
		public Light2D lightSource;
		[MyCmpGet]
		SolarReciever _solarReciever;
		public void Sim200ms(float dt)
		{
			int cellAbove = Grid.GetCellInDirection(Grid.PosToCell(this.gameObject), Direction.Up);
			int cellBelow = Grid.GetCellInDirection(Grid.PosToCell(this.gameObject), Direction.Down);
			//int cell = Grid.PosToCell(this.gameObject);
			//Debug.Log(Grid.LightIntensity[cell]+" Source; "+lightSource.Lux);
			int lux = Grid.LightIntensity[cellAbove];
			lux += _solarReciever.SimulatedLuxFromConnectedSatellites();

			lux = 80000;

			lightSource.Lux = lux / 3; ///or /2, dependant on balance decision.
			lightSource.Range = ((float)lux) / 2000f > 20 ? 20 : ((float)lux) / 2000f;
			lightSource.FullRefresh();
		}
	}
}
