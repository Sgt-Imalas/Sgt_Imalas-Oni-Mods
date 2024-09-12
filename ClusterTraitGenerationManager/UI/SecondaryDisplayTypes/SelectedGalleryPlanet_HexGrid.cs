using ClusterTraitGenerationManager.ClusterData;

namespace ClusterTraitGenerationManager.UI.SecondaryDisplayTypes
{
	public class SelectedGalleryPlanet_HexGrid : ISecondaryDisplayData
	{
		public SelectedGalleryPlanet_HexGrid(StarmapItem item, Tuple<int, int> _location)
		{
			StarmapItem = item;
			ID = item.id;
			Location = _location;
		}
		public Tuple<int, int> Location;
		public string ID { get; set; }
		public StarmapItem StarmapItem { get; set; }
		public string LocationDescription()
		{
			return (ModAssets.Strings.ApplyCategoryTypeToString(string.Format(STRINGS.UI.CGM_MAINSCREENEXPORT.DETAILS.HEADER.LABEL_LOCATION, StarmapItem.DisplayName, (Location.first + "," + Location.second)), StarmapItem.category));
		}
	}
}
