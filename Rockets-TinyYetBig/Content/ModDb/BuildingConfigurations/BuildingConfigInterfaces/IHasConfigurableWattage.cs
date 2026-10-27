namespace Rockets_TinyYetBig.Content.ModDb.BuildingConfigurations.BuildingConfigInterfaces
{
    interface IHasConfigurableWattage
	{
		float GetWattage();
		void SetWattage(float wattage);
		public Tuple<int, int> GetLimitsWattage();
	}
}
