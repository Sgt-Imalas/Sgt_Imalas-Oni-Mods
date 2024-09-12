using PeterHan.PLib.Options;
using System;

namespace PedestalFilter
{

	[Serializable]
	[RestartRequired]
	[ConfigFile(SharedConfigLocation: true)]
	public class Config : SingletonOptions<Config>
	{
		[Option("STRINGS.PEDESTALFILTER_CONFIG.FILTERARTIFACTS", "STRINGS.PEDESTALFILTER_CONFIG.FILTERARTIFACTS_TOOLTIP")]
		public bool DefaultToArtifactsOnly { get; set; }
		public Config()
		{
			DefaultToArtifactsOnly = false;
		}

	}
}
