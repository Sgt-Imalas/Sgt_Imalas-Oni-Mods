namespace LocalModLoader.DataClasses
{
	internal class RemoteModInfo
	{
		public string staticID, version, minimumSupportedBuild, downloadURL;

		public string ZipName => staticID + ".zip";
	}
}
