namespace RonivansLegacy_ChemicalProcessing.Content.Scripts.Buildings.ConfigInterfaces
{
    interface IHasConfigurableStorageCapacity
	{
		float GetStorageCapacity();
		void SetStorageCapacity(float mass);
	}
}
