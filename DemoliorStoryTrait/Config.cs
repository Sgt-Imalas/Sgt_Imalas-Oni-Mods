using DemoliorStoryTrait.Patches;
using Newtonsoft.Json;
using PeterHan.PLib.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoliorStoryTrait
{
	[Serializable]
	[RestartRequired]
	[ConfigFile(SharedConfigLocation: true)]
	public class Config : SingletonOptions<Config>
	{

		[Option(Localization_Patches.SettingNameKey)]
		[JsonProperty]
		public bool PipReplaceDemoliorSprite { get; set; } = true;
	}
}
