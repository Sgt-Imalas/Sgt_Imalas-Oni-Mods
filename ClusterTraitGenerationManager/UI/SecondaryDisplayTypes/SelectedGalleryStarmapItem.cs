using ClusterTraitGenerationManager.ClusterData;

namespace ClusterTraitGenerationManager.UI.SecondaryDisplayTypes
{
	public class SelectedGalleryStarmapItem : ISecondaryDisplayData
	{
		public SelectedGalleryStarmapItem(StarmapItem item)
		{
			StarmapItem = item;
			ID = item.id;
		}
		public string ID { get; set; }
		public StarmapItem StarmapItem { get; set; }
		public string LocationDescription()
		{
			return (ModAssets.Strings.ApplyCategoryTypeToString(string.Format(STRINGS.UI.CGM_MAINSCREENEXPORT.DETAILS.HEADER.LABEL, StarmapItem.DisplayName), StarmapItem.category));
		}
	}
}
