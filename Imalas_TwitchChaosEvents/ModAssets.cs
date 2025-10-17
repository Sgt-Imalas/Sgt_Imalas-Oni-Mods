using HarmonyLib;
using Imalas_TwitchChaosEvents.Elements;
using Klei.AI;
using PeterHan.PLib.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using TUNING;
using UnityEngine;
using UtilLibs;

namespace Imalas_TwitchChaosEvents
{
	public class ModAssets
	{
		public class Techs
		{
			//public static string TacoTechID = "ITCE_TacoTech";
			//public static Tech TacoTech;
		}
		public class WaterCoolerDrinks
		{
			public static Dictionary<Tag, string> Beverages = new Dictionary<Tag, string>()
			{
				{ ModElements.InverseWater.Tag, Chaos_Effects.FLIPPEDWATERDRINK }
			};
			public static void Register(Db __instance)
			{
				var beverages = new List<Tuple<Tag, string>>();

				foreach (var beverage in Beverages)
					beverages.Add(new Tuple<Tag, string>(beverage.Key, beverage.Value));

				WaterCoolerConfig.BEVERAGE_CHOICE_OPTIONS = WaterCoolerConfig.BEVERAGE_CHOICE_OPTIONS
					.AddRangeToArray(beverages.ToArray())
					.ToArray();
			}
		}
		public class ModTraits
		{
			public static string InquisitionMember = "ITCE_InquisitionMember";
			public static void RegisterTraits()
			{
				TUNING.TRAITS.TRAIT_CREATORS.Add(TraitUtil.CreateNamedTrait(InquisitionMember, STRINGS.CHAOSEVENTS.SPANISHINQUISITION.TRAIT_NAME, STRINGS.CHAOSEVENTS.SPANISHINQUISITION.TRAIT_DESC, positiveTrait: true));
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

				new EffectBuilder(FLIPPEDWATERDRINK, 300f, false)
					.Modifier(db.Amounts.Stress.deltaAttribute.Id, 1f / 600f)
					.Modifier(db.Attributes.Learning.Id, 1)
					.Modifier(db.Attributes.Machinery.Id, 1)
					.Modifier(db.Attributes.Cooking.Id, 1)
					.Modifier(db.Attributes.Caring.Id, 1)
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
				CAVE_NOISE = "ICTE_CaveNoise.mp3",
				NUKE_DETONATION = "ICT_NUKEDETONATION",
				SPANISH_INQUISITION = "ICTE_SPANISH_INQUISITION";
		}
		public static bool RainbowLiquids = false;

		public static void LoadAll()
		{
			SoundUtils.LoadSound(SOUNDS.TACORAIN, "ICT_TACORAIN.wav");
			SoundUtils.LoadSound(SOUNDS.EVILSOUND, "ICT_EVIL.mp3");
			SoundUtils.LoadSound(SOUNDS.THUNDERSTRIKE, "ICT_ThunderStrike.mp3");
			SoundUtils.LoadSound(SOUNDS.CAVE_NOISE, "ICTE_CaveNoise.mp3");
			SoundUtils.LoadSound(SOUNDS.SPANISH_INQUISITION, "ITCE_SpanishInquisition.mp3");

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
		public class StatusItems
		{
			public static StatusItem WorkerOnStrike;
			public static StatusItem CreeperBurns;
			public static StatusItem VoidBurns;
			public static StatusItem VoidTarget;

			static List<string> StrikeDemands = [
				STRINGS.DUPLICANTS.STATUSITEMS.ITCE_WORKERSTRIKE.STRIKE_REASON_0,
				STRINGS.DUPLICANTS.STATUSITEMS.ITCE_WORKERSTRIKE.STRIKE_REASON_1,
				STRINGS.DUPLICANTS.STATUSITEMS.ITCE_WORKERSTRIKE.STRIKE_REASON_2,
				STRINGS.DUPLICANTS.STATUSITEMS.ITCE_WORKERSTRIKE.STRIKE_REASON_3,
				STRINGS.DUPLICANTS.STATUSITEMS.ITCE_WORKERSTRIKE.STRIKE_REASON_4,
				STRINGS.DUPLICANTS.STATUSITEMS.ITCE_WORKERSTRIKE.STRIKE_REASON_5,
				STRINGS.DUPLICANTS.STATUSITEMS.ITCE_WORKERSTRIKE.STRIKE_REASON_6,
				STRINGS.DUPLICANTS.STATUSITEMS.ITCE_WORKERSTRIKE.STRIKE_REASON_7,
				];

			public static void CreateStatusItems()
			{
				WorkerOnStrike = new StatusItem("ITCE_WorkerStrike", "DUPLICANTS", string.Empty, StatusItem.IconType.Exclamation, NotificationType.Bad, false, OverlayModes.None.ID, true);

				WorkerOnStrike.SetResolveStringCallback((str, obj) =>
				{
					System.Random rand = new System.Random(Hash.SDBMLower(obj.ToString()));					
					string randomReason = StrikeDemands[rand.Next(0, StrikeDemands.Count)];
					return string.Format(str, randomReason);
				});
				CreeperBurns = new StatusItem("ITCE_HurtingElement", "DUPLICANTS", string.Empty, StatusItem.IconType.Exclamation, NotificationType.DuplicantThreatening, false, OverlayModes.None.ID, true, 63486);
				CreeperBurns.AddNotification();

				VoidBurns = new StatusItem("ITCE_HurtingElement_VOID", "DUPLICANTS", string.Empty, StatusItem.IconType.Exclamation, NotificationType.DuplicantThreatening, false, OverlayModes.None.ID, true, 63486);
				VoidBurns.AddNotification();

				VoidTarget = new StatusItem("ITCE_VoidTarget", "DUPLICANTS", string.Empty, StatusItem.IconType.Exclamation, NotificationType.Bad, false, OverlayModes.None.ID, true);

			}
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
					new PKeyBinding(KKeyCode.F10, Modifier.Ctrl));
			}
		}
	}
}
