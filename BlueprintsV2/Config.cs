using PeterHan.PLib.Options;
using System;

namespace BlueprintsV2
{
	public enum DefaultSelections
	{
		All, None
	}

	[Serializable]
	[RestartRequired]
	[ConfigFile(SharedConfigLocation: true)]
	public class Config : SingletonOptions<Config>
	{
		[Option("Default Menu Selections", "The default selections made when an advanced filter menu is opened.")]
		public DefaultSelections DefaultMenuSelections { get; set; } = DefaultSelections.All;

		[Option("Require Constructable", "Whether buildings must be constructable by the player to be used in blueprints.")]
		public bool RequireConstructable { get; set; } = true;

		[Option("FX Time", "How long FX created by Blueprints remain on the screen. Measured in seconds.")]
		public float FXTime { get; set; } = 4;

		[Option("Blueprint Tool Overlay Sync", "Whether the Blueprint Tool syncs with the current overlay. (configurable in game too)")]
		public bool CreateBlueprintToolSync { get; set; } = true;

		[Option("Snapshot Tool Overlay Sync", "Whether the Snapshot Tool syncs with the current overlay. (configurable in game too)")]
		public bool SnapshotToolSync { get; set; } = true;
		//[Option("Legacy Blueprint Navigation", "Navigate blueprints with the old system (arrow keys) instead of the new UI")]
		//public bool LegacyNavigation { get; set; } = false;
	}
}
