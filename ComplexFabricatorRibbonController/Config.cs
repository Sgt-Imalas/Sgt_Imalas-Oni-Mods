using Newtonsoft.Json;
using PeterHan.PLib.Options;
using System;

namespace ComplexFabricatorRibbonController
{
	[Serializable]
	[RestartRequired]
	[ConfigFile(SharedConfigLocation: true)]
	public class Config : SingletonOptions<Config>
	{

		[Option("STRINGS.UI.RFRC_CONFIG.MICROCHIP_REQUIRED.TEXT","STRINGS.UI.RFRC_CONFIG.MICROCHIP_REQUIRED.TOOLTIP")]
		[JsonProperty]
		public bool MicrochipBuildingCost { get; set; } = true;
	}
}
