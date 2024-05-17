using Newtonsoft.Json;
using PeterHan.PLib.Options;

namespace Blueprints {
    public enum DefaultSelections {
        All, None
    }

    [JsonObject]
    public class BlueprintsOptions {
        [Option("Default Menu Selections", "The default selections made when an advanced filter menu is opened.")]
        public DefaultSelections DefaultMenuSelections { get; set; } = DefaultSelections.All;

        [Option("Require Constructable", "Whether buildings must be constructable by the player to be used in blueprints.")]
        public bool RequireConstructable { get; set; } = true;

        //[Option("Legacy Format", "use the old .blueprint format instead of json")]
        //public bool CompressBlueprints { get; set; } = true;

        [Option("FX Time", "How long FX created by Blueprints remain on the screen. Measured in seconds.")]
        public float FXTime { get; set; } = 4;

        [Option("Blueprint Tool Overlay Sync", "Whether the Blueprint Tool syncs with the current overlay. (configurable in game too)")]
        public bool CreateBlueprintToolSync { get; set; } = true;

        [Option("Snapshot Tool Overlay Sync", "Whether the Snapshot Tool syncs with the current overlay. (configurable in game too)")]
        public bool SnapshotToolSync { get; set; } = true;
    }
}
