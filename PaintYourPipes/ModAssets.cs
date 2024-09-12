using PeterHan.PLib.Actions;

namespace PaintYourPipes
{
	internal class ModAssets
	{
		public class HotKeys
		{
			public static PAction ToggleOverlayColors { get; private set; }

			public const string TRIGGER_FAKE_TACORAIN_IDENTIFIER = "ICT_TRIGGERFAKETACORAIN";
			public const string TRIGGER_UNLOCKTACORECIPE = "ICT_UNLOCKTACORECIPE";
			public const string TRIGGER_RAINBOWLIQUIDTOGGLE = "ICT_RAINBOWLIQUIDTOGGLE";

			public static void Register()
			{
				ToggleOverlayColors = new PActionManager().CreateAction(
					TRIGGER_FAKE_TACORAIN_IDENTIFIER,
					STRINGS.HOTKEYACTIONS.TOGGLE_OVERLAY_COLOR,
					new PKeyBinding(KKeyCode.C, Modifier.Shift));

			}
		}
	}
}
