using Newtonsoft.Json;
using PeterHan.PLib.Options;
using System;

namespace DebugButton
{
	[Serializable]
	[RestartRequired]
	[ConfigFile(SharedConfigLocation: true)]
	public class Config : SingletonOptions<Config>
	{
	
		[Option("STRINGS.UI.TOOLS.CFG_SANDBOX_ONLY.NAME", "STRINGS.UI.TOOLS.CFG_SANDBOX_ONLY.TOOLTIP")]
		[JsonProperty]
		public bool SandboxModeOnly { get; set; } = false;
	}
}
