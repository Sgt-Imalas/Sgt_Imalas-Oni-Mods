using HarmonyLib;
using Imalas_TwitchChaosEvents.Elements;
using PeterHan.PLib.Actions;
using ProcGen;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UtilLibs;

namespace Imalas_TwitchChaosEvents
{
    public class ModAssets
    {
        public class WaterCoolerDrinks
        {
            public static Dictionary<Tag, string> Beverages = new Dictionary<Tag, string>()
            {
                { ModElements.InverseWater.Tag, Chaos_Effects.FLIPPEDWATERDRINK }
            };

            [HarmonyPatch(typeof(Db))]
            [HarmonyPatch(nameof(Db.Initialize))]
            public class Db_init
            {
                public static void Postfix(Db __instance)
                {
                    var beverages = new List<Tuple<Tag, string>>();

                    foreach (var beverage in Beverages)
                        beverages.Add(new Tuple<Tag, string>(beverage.Key, beverage.Value));

                    WaterCoolerConfig.BEVERAGE_CHOICE_OPTIONS = WaterCoolerConfig.BEVERAGE_CHOICE_OPTIONS
                        .AddRangeToArray(beverages.ToArray())
                        .ToArray();
                }
            }
        }


        public static class Chaos_Effects
        {
            [HarmonyPatch(typeof(ModifierSet), "Initialize")]
            public class ModifierSet_Initialize_Patch
            {
                public static void Postfix(ModifierSet __instance)
                {
                    RegisterEffects(__instance);
                }
            }

            public const string
            FLIPPEDWATERDRINK = "ITCE_INVERSE_WATER_DRINK"
           ;


            public static void RegisterEffects(ModifierSet set)
            {
                var db = Db.Get();
                var athlethics = db.Attributes.Athletics.Id;

                new EffectBuilder(FLIPPEDWATERDRINK, 300f, false)
                    .Modifier(db.Amounts.Stress.deltaAttribute.Id, 1f/600f)
                    .Modifier(db.Attributes.Strength.Id, 5)
                    .Modifier(athlethics, -1)
                    .Add(set);
            }
        }


        public static GameObject CurrentFogGO = null;
        public static class SOUNDS
        {
            public const string
                TACORAIN = "ICT_TACORAIN",
                THUNDERSTRIKE = "ICT_ThunderStrike",
                EVILSOUND = "ICT_EVILARRIVING",
                NUKE_DETONATION = "ICT_NUKEDETONATION";
        }
        public static bool RainbowLiquids = false;

        public static void LoadAll()
        {
            SoundUtils.LoadSound(SOUNDS.TACORAIN, "ICT_TACORAIN.wav");
            SoundUtils.LoadSound(SOUNDS.EVILSOUND, "ICT_EVIL.mp3");
            SoundUtils.LoadSound(SOUNDS.THUNDERSTRIKE, "ICT_ThunderStrike.mp3");

            LoadAssets();
        }

        public static GameObject FogSpawner;
        public static GameObject FireSpawner;
        public static GameObject CursorHP;

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


            CursorHP = bundle.LoadAsset<GameObject>("Assets/CursorHP.prefab");


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
