using Newtonsoft.Json;
using PeterHan.PLib.Options;
using System;

namespace SkillingQueue
{
	[Serializable]
	[RestartRequired]
	[ConfigFile(SharedConfigLocation: true)]
	public class Config : SingletonOptions<Config>
	{

		[Option("STRINGS.SKILLQUEUE_CONFIG.NOTIFICATION_ON_QUEUE_COMPLETE.NAME", "STRINGS.SKILLQUEUE_CONFIG.NOTIFICATION_ON_QUEUE_COMPLETE.DESCRIPTION")]
		[JsonProperty]
		public bool NotificationOnQueueComplete { get; set; }

		[Option("STRINGS.SKILLQUEUE_CONFIG.NOTIFICATION_ON_SKILL.NAME", "STRINGS.SKILLQUEUE_CONFIG.NOTIFICATION_ON_SKILL.DESCRIPTION")]
		[JsonProperty]
		public bool NotificationOnSkill { get; set; }
		[Option("STRINGS.SKILLQUEUE_CONFIG.RAINBOW_QUEUE.NAME", "STRINGS.SKILLQUEUE_CONFIG.RAINBOW_QUEUE.DESCRIPTION")]
		[JsonProperty]
		public bool RainbowIndicator { get; set; }

		public Config()
		{
			NotificationOnSkill = true;
			NotificationOnQueueComplete = true;
			RainbowIndicator = true;
		}
	}
}
