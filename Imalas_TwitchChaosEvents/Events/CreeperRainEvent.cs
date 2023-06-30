using Imalas_TwitchChaosEvents.Elements;
using ONITwitchLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Util_TwitchIntegrationLib;
using Util_TwitchIntegrationLib.Scripts;

namespace Imalas_TwitchChaosEvents.Events
{
    internal class CreeperRainEvent : ITwitchEventBase
    {
        public string ID => "ChaosTwitch_CreeperRain";

        public string EventGroupID => null;

        public string EventName => STRINGS.CHAOSEVENTS.CREEPERRAIN.NAME;

        public string EventDescription => STRINGS.CHAOSEVENTS.CREEPERRAIN.TOASTTEXT;

        public EventWeight EventWeight => EventWeight.WEIGHT_RARE;

        public Action<object> EventAction => (object data) =>
        {
            var go = new GameObject("creeperSpawner");

            var rain = go.AddComponent<LiquidRainSpawner>();

            rain.totalAmountRangeKg = (10, 40);
            rain.durationInSeconds = 40;
            rain.dropletMassKg = 0.05f;
            rain.elementId = ModElements.Creeper;
            rain.spawnRadius = 15;

            go.SetActive(true);

            GameScheduler.Instance.Schedule("creeper rain", 5f, _ =>
            {
                rain.StartRaining();
                //AudioUtil.PlaySound(ModAssets.Sounds.SPLAT, ModAssets.GetSFXVolume() * 0.15f); // its loud
            });

            ToastManager.InstantiateToast(
                STRINGS.CHAOSEVENTS.CREEPERRAIN.TOAST,
                string.Format(STRINGS.CHAOSEVENTS.CREEPERRAIN.TOASTTEXT, ClusterManager.Instance.activeWorld.GetProperName()));
        };

        public Func<object, bool> Condition =>
            (data) =>
            {
                return GameClock.Instance.GetCycle() > 33;
            };

        public Danger EventDanger => Danger.Extreme;

    }
}
