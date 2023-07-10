using PeterHan.PLib.Actions;
using UtilLibs;

namespace Imalas_TwitchChaosEvents
{
    internal class ModAssets
    {
        public static class SOUNDS
        {
            public const string
                TACORAIN = "ICT_TACORAIN",
                NUKE_DETONATION = "ICT_NUKEDETONATION";
        }
        public static bool RainbowLiquids = false;

        public static void LoadAll()
        {
            SoundUtils.LoadSound(SOUNDS.TACORAIN, "ICT_TACORAIN.wav");
        }
        public class HotKeys
        {
            public static PAction TriggerTacoRain { get; private set; }
            public static PAction UnlockTacoRecipe { get; private set; }
            public static PAction ToggleRainbowLiquid { get; private set; }

            public const string TRIGGER_FAKE_TACORAIN_IDENTIFIER = "ICT_TRIGGERFAKETACORAIN";
            public const string TRIGGER_UNLOCKTACORECIPE = "ICT_UNLOCKTACORECIPE";
            public const string TRIGGER_RAINBOWLIQUIDTOGGLE = "ICT_RAINBOWLIQUIDTOGGLE";

            public static void Register()
            {
                TriggerTacoRain = new PActionManager().CreateAction(
                    TRIGGER_FAKE_TACORAIN_IDENTIFIER,
                    STRINGS.HOTKEYACTIONS.TRIGGER_FAKE_TACORAIN_NAME,
                    new PKeyBinding(KKeyCode.F8, Modifier.Shift));

                UnlockTacoRecipe = new PActionManager().CreateAction(
                    TRIGGER_UNLOCKTACORECIPE,
                    STRINGS.HOTKEYACTIONS.UNLOCK_TACO_RECIPE,
                    new PKeyBinding(KKeyCode.F9, Modifier.Shift));

                ToggleRainbowLiquid = new PActionManager().CreateAction(
                    TRIGGER_RAINBOWLIQUIDTOGGLE,
                    "DEBUG: Toggle Rainbow",
                    new PKeyBinding(KKeyCode.R, Modifier.Ctrl));
            }
        }
    }
}
