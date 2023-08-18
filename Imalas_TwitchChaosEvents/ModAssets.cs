using PeterHan.PLib.Actions;
using ProcGen;
using UnityEngine;
using UtilLibs;

namespace Imalas_TwitchChaosEvents
{
    internal class ModAssets
    {
        public static GameObject CurrentFogGO = null;
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

            LoadAssets();
        }

        public static GameObject FogSpawner;
        public static GameObject FireSpawner;

        public static void LoadAssets()
        {
            var bundle = AssetUtils.LoadAssetBundle("chaos_twitch_assets", platformSpecific: true);
            FogSpawner = bundle.LoadAsset<GameObject>("Assets/FogObject.prefab").transform.Find("Particle System").gameObject;
            SgtLogger.Assert("FogSpawner", FogSpawner);
            var renderer = FogSpawner.GetComponent<ParticleSystemRenderer>();
            renderer.material.renderQueue = RenderQueues.Liquid;

            FireSpawner = bundle.LoadAsset<GameObject>("Assets/FireParticleSystem.prefab");

            var renderer2 = FireSpawner.GetComponent<ParticleSystemRenderer>();
            renderer2.material.renderQueue = RenderQueues.Liquid;

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
