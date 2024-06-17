using static ClusterTraitGenerationManager.ClusterData.CGSMClusterManager;

namespace ClusterTraitGenerationManager.UI.SecondaryDisplayTypes
{
    public class SelectedSinglePOI_HexGrid : ISecondaryDisplayData
    {
        public SelectedSinglePOI_HexGrid(string id, string name, Tuple<int, int> _location)
        {
            ID = id;
            Name = name;
            Location = _location;
        }
        public string ID { get; set; }
        public string Name { get; set; }
        public Tuple<int, int> Location;
        public string LocationDescription()
        {
            return (ModAssets.Strings.ApplyCategoryTypeToString(string.Format(STRINGS.UI.CGM_MAINSCREENEXPORT.DETAILS.HEADER.LABEL_LOCATION, Name, (Location.first + "," + Location.second)), StarmapItemCategory.SpacedOutStarmap));
        }
    }
}
