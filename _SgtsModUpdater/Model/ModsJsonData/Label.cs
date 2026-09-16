namespace _SgtsModUpdater.Model.ModsJsonData
{
	internal class Label
	{
		public DistributionPlatform distribution_platform;
		public string id;
		public string title;
		public long version;
	}

	public enum DistributionPlatform
	{
		Local,
		Steam,
		Epic,
		Rail,
		Dev,
	}
}
