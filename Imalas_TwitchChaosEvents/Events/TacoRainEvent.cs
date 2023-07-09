using ONITwitchLib.Logger;
using ONITwitchLib.Utils;
using ONITwitchLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util_TwitchIntegrationLib;
using static STRINGS.BUILDINGS.PREFABS.EXTERIORWALL.FACADES;
using UtilLibs;

namespace Imalas_TwitchChaosEvents.Events
{
    internal class TacoRainEvent : ITwitchEventBase
    {
        public string ID => "ChaosTwitch_TacoRain";

        public string EventGroupID => null;

        public string EventName => STRINGS.CHAOSEVENTS.TACORAIN.NAME;

        public string EventDescription => STRINGS.CHAOSEVENTS.TACORAIN.TOASTTEXT;

        public EventWeight EventWeight => EventWeight.WEIGHT_COMMON;

        public Action<object> EventAction => (object data) =>
        {
            GameScheduler.Instance.Schedule("taco rain", 0.5f, _ =>
            {
                //rain.StartRaining();
                if(ClusterManager.Instance.activeWorld.IsModuleInterior)
                {
                    return;
                }

                GameplayEventInstance eventInstance = GameplayEventManager.Instance.StartNewEvent(MeteorPatches.ITC_TacoMeteors, ClusterManager.Instance.activeWorldId);
                // ClusterManager.Instance.activeWorld.GetSMI<GameplaySeasonManager.Instance>().Start(Db.Get().GameplaySeasons.TemporalTearMeteorShowers);
                SoundUtils.PlaySound(ModAssets.SOUNDS.TACORAIN, SoundUtils.GetSFXVolume() * 0.3f,true);
            });
            string body = STRINGS.CHAOSEVENTS.TACORAIN.TOASTTEXT;

            if(ChaosTwitch_SaveGameStorage.Instance.hasUnlockedTacoRecipe == false)
            {
                body += STRINGS.CHAOSEVENTS.TACORAIN.NEWRECIPE;
                ChaosTwitch_SaveGameStorage.Instance.hasUnlockedTacoRecipe = true;
            }


            ToastManager.InstantiateToast(
                STRINGS.CHAOSEVENTS.TACORAIN.TOAST,
                 body
            );
        };

        public Func<object, bool> Condition =>
            (data) =>
            {

                return true;
            };

        public Danger EventDanger => Danger.Medium;

        public void SpawnBuzzSaw()
        {
            //ToastManager.InstantiateToastWithPosTarget(EventName, EventDescription, targetCoords);
        }
    }
}
