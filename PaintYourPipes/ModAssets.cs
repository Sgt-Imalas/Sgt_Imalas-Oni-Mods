using PeterHan.PLib.Actions;

namespace PaintYourPipes
{
	internal class ModAssets
	{
		public class HotKeys
		{
			public static PAction ToggleOverlayColors { get; private set; }

			public const string TOGGLE_OVERLAY = "PYP_TOGGLE_COLORING_OVERLAY";

			public static void Register()
			{
				ToggleOverlayColors = new PActionManager().CreateAction(
					TOGGLE_OVERLAY,
					STRINGS.HOTKEYACTIONS.TOGGLE_OVERLAY_COLOR,
					new PKeyBinding());

			}
		}
	}
}
