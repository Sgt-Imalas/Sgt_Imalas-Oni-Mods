using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ProcGen.ClusterLayout;

namespace ClusterTraitGenerationManager.ClusterData
{
    /// <summary>
    /// custom data class because using it directly causes a CTD
    /// </summary>
    public class CustomClusterAudio
	{
		public string musicWelcome { get; set; }

		public string musicFirst { get; set; }

		public string stingerDay { get; set; }

		public string stingerNight { get; set; }

		public CustomClusterAudio()
		{
			musicWelcome = "Music_WattsonMessage";
			musicFirst = null;
			stingerDay = "Stinger_Day";
			stingerNight = "Stinger_Loop_Night";
		}
		public CustomClusterAudio(string musicWelcome, string musicFirst, string stingerDay, string stingerNight)
		{
			this.musicWelcome = musicWelcome;
			this.musicFirst = musicFirst;
			this.stingerDay = stingerDay;
			this.stingerNight = stingerNight;
		}
		public ClusterAudioSettings ToAudioSetting()
		{
			return new()
			{
				musicWelcome = this.musicWelcome,
				musicFirst = this.musicFirst,
				stingerDay = this.stingerDay,
				stingerNight = this.stingerNight
			};

		}
	}
}
