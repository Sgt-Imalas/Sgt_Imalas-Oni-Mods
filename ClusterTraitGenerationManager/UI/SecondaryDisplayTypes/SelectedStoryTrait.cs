using static ClusterTraitGenerationManager.ClusterData.CGSMClusterManager;

namespace ClusterTraitGenerationManager.UI.SecondaryDisplayTypes
{
	public class SelectedStoryTrait : ISecondaryDisplayData
	{
		public SelectedStoryTrait(string _id, string traitName)
		{
			ID = _id;
			TraitName = traitName;
		}
		public string TraitName;
		public string ID { get; set; }
		public string LocationDescription()
		{
			return ModAssets.Strings.ApplyCategoryTypeToString(string.Format(STRINGS.UI.CGM_MAINSCREENEXPORT.DETAILS.HEADER.LABEL, TraitName), StarmapItemCategory.StoryTraits);
		}
	}
}
