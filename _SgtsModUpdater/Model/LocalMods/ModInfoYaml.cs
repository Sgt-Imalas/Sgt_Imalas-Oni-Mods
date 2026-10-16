using YamlDotNet.Serialization;

namespace _SgtsModUpdater.Model.LocalMods
{
    public class ModInfoYaml
	{
		[YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitNull | DefaultValuesHandling.OmitDefaults)]
		public string supportedContent = null;

		[YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitNull | DefaultValuesHandling.OmitDefaults | DefaultValuesHandling.OmitEmptyCollections)]
		public string[] requiredDlcIds = null;

		[YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitNull | DefaultValuesHandling.OmitDefaults | DefaultValuesHandling.OmitEmptyCollections)]
		public string[] forbiddenDlcIds = null;

		[YamlMember]
		public int minimumSupportedBuild;

		[YamlMember]
		public string version;
	}
}
