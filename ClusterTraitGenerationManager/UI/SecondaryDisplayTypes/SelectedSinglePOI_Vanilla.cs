using static ClusterTraitGenerationManager.ClusterData.CGSMClusterManager;
using static ClusterTraitGenerationManager.STRINGS.UI.CGM_MAINSCREENEXPORT.DETAILS.CONTENT.SCROLLRECTCONTAINER;

namespace ClusterTraitGenerationManager.UI.SecondaryDisplayTypes
{
    public class SelectedSinglePOI_Vanilla : ISecondaryDisplayData
    {
        public SelectedSinglePOI_Vanilla(string id, int band)
        {
            ID = id;
            Band = band;
        }

        public string ID { get; set; }
        public int Band;

        public string LocationDescription()
        {
            return
                ModAssets.Strings.ApplyCategoryTypeToString(string.Format(VANILLAPOI_RESOURCES.SELECTEDDISTANCE,
            Db.Get().SpaceDestinationTypes.TryGet(ID).Name,
            (Band + 1) * 10000,
            global::STRINGS.UI.UNITSUFFIXES.DISTANCE.KILOMETER.Replace(" ", ""))
                , StarmapItemCategory.VanillaStarmap);
        }
    }
}
