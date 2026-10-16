using YamlDotNet.Serialization;

namespace _SgtsModUpdater.Model.LocalMods
{
	public class ModYaml
	{
		[YamlMember]
		public string title;

		[YamlMember]
		public string description;

		[YamlMember]
		public string staticID;
	}
}
