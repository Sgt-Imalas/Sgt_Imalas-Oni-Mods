using static ClusterTraitGenerationManager.ClusterData.CGSMClusterManager;
using static ClusterTraitGenerationManager.STRINGS.UI.CGM_MAINSCREENEXPORT.DETAILS.CONTENT.SCROLLRECTCONTAINER;

namespace ClusterTraitGenerationManager.UI.SecondaryDisplayTypes
{

    public class SelectedSinglePOI_SO : ISecondaryDisplayData
    {
        public SelectedSinglePOI_SO(string id, string groupId)
        {
            ID = id;
            GroupID = groupId;
        }

        public string ID { get; set; }
        public string GroupID;
        public string LocationDescription()
        {
            var data = ModAssets.SO_POIs[ID];
            return
             ModAssets.Strings.ApplyCategoryTypeToString(
             string.Format(VANILLAPOI_RESOURCES.SELECTEDDISTANCE_SO,
                    data.Name,
                    GroupID.Substring(0, 8))
            , StarmapItemCategory.VanillaStarmap);
        }
    }
}
