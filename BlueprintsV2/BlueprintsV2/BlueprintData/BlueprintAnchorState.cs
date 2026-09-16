using PeterHan.PLib.Options;

namespace BlueprintsV2.BlueprintsV2.BlueprintData
{
	public enum BlueprintAnchorState
	{
		[Option("STRINGS.BLUEPRINT_ANCHORSTATE.BC")]
		BottomCenter = 0,
		[Option("STRINGS.BLUEPRINT_ANCHORSTATE.C")]
		Center = 1,
		[Option("STRINGS.BLUEPRINT_ANCHORSTATE.BL")]
		BottomLeft = 2,
		[Option("STRINGS.BLUEPRINT_ANCHORSTATE.BR")]
		BottomRight = 3,
		[Option("STRINGS.BLUEPRINT_ANCHORSTATE.TL")]
		TopLeft = 4,
		[Option("STRINGS.BLUEPRINT_ANCHORSTATE.TR")]
		TopRight = 5,
	}
}
