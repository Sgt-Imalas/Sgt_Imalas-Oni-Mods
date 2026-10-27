namespace Rockets_TinyYetBig.Content.ModDb.BuildingConfigurations.BuildingConfigInterfaces
{
    interface IHasConfigurableStorageCapacity
	{
		float GetStorageCapacity();
		string GetStorageLabel();
		void SetStorageCapacity(float mass);
		public Tuple<int, int> GetLimitsStorage();
		string StorageUnitLabel();
		string GetStorageTooltip();
	}
}
