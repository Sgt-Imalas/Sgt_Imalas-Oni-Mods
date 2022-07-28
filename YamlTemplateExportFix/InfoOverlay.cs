namespace YamlTemplateExportFix
{
	public class InfoOverlay : OverlayModes.Mode
	{
		public static readonly HashedString ID = nameof(InfoOverlay);

		public override HashedString ViewMode()
		{
			return ID;
		}

		public override string GetSoundName()
		{
			return "Lights";
		}
	}
}
