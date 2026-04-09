using Newtonsoft.Json;
using PeterHan.PLib.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace MassMoveTo
{
	[Serializable]
	[RestartRequired]
	[ConfigFile(SharedConfigLocation: true)]
	public class Config : SingletonOptions<Config>
	{

		[Option("STRINGS.UI.MASSMOVETOOL_MULTITARGET.TITLE", "STRINGS.UI.MASSMOVETOOL_MULTITARGET.TOOLTIP")]
		[JsonProperty]
		public bool MultiDeliveryTargets { get; set; } = true;

		public static bool UseMultiDelivery => Config.Instance.MultiDeliveryTargets && !ModIntegration_ChainTool.ChainToolActive;
	}
}
