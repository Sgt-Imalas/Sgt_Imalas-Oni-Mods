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
using UnityEngine;
using PeterHan.PLib.UI;

namespace Imalas_TwitchChaosEvents.Events
{
    /// <summary>
    /// Taco Meteor shower
    /// </summary>
    internal class TacoRainEvent : ITwitchEventBase
    {
        public string ID => "ChaosTwitch_TacoRain";

        public string EventGroupID => null;

        public string EventName => STRINGS.CHAOSEVENTS.TACORAIN.NAME;

        public string EventDescription => STRINGS.CHAOSEVENTS.TACORAIN.TOASTTEXT;

        public EventWeight EventWeight => EventWeight.WEIGHT_COMMON;

        public Action<object> EventAction => (object data) =>
        {
            string body = STRINGS.CHAOSEVENTS.TACORAIN.TOASTTEXT;

            if (ChaosTwitch_SaveGameStorage.Instance.hasUnlockedTacoRecipe == false)
            {
                body += STRINGS.CHAOSEVENTS.TACORAIN.NEWRECIPE;
                ChaosTwitch_SaveGameStorage.Instance.hasUnlockedTacoRecipe = true; 
            }

            ChaosTwitch_SaveGameStorage.Instance.lastTacoRain = GameClock.Instance.GetTimeInCycles();



        GameScheduler.Instance.Schedule("taco rain", 0.5f, _ =>
            {
                int activeWorld = ClusterManager.Instance.activeWorldId;
                //rain.StartRaining();
                if (ClusterManager.Instance.activeWorld.IsModuleInterior)
                {
                    activeWorld = 0;
                }

                var world = ClusterManager.Instance.GetWorld(activeWorld);


                foreach (var planet in ClusterManager.Instance.WorldContainers)
                {
                    if ((planet.IsDupeVisited || planet.IsStartWorld) && planet.IsSurfaceRevealed && !planet.IsModuleInterior)
                    {
                        world = planet;
                        break;
                    }
                }

                SpeedControlScreen.Instance.SetSpeed(0);
                GameplayEventInstance eventInstance = GameplayEventManager.Instance.StartNewEvent(TacoMeteorPatches.ITC_TacoMeteors, activeWorld);
                // ClusterManager.Instance.activeWorld.GetSMI<GameplaySeasonManager.Instance>().Start(Db.Get().GameplaySeasons.TemporalTearMeteorShowers);
                if(Config.Instance.TacoEventMusic)
                    SoundUtils.PlaySound(ModAssets.SOUNDS.TACORAIN, SoundUtils.GetSFXVolume() * 0.3f,true);

                //var pos = world.LookAtSurface();


                ToastManager.InstantiateToastWithPosTarget(
                STRINGS.CHAOSEVENTS.TACORAIN.TOAST,
                 body, GetSurfacePos(world)

            );

            });
        };

        Vector3 GetSurfacePos(WorldContainer world)
        {
            Vector3 vector = new Vector3(world.WorldOffset.x + (world.Width / 2), world.WorldOffset.y + (world.Height -15 ), 0f);
            return vector;
        }


        public Func<object, bool> Condition =>
            (data) =>
            {
                bool anyUnlockedPlanetRevealed = false;

                foreach(var planet in ClusterManager.Instance.WorldContainers)
                {
                    if((planet.IsDupeVisited||planet.IsStartWorld) && planet.IsSurfaceRevealed && !planet.IsModuleInterior)
                    {
                        anyUnlockedPlanetRevealed = true;
                        break;
                    }
                }

                if(!anyUnlockedPlanetRevealed)
                    return false;


                return 
                (GameClock.Instance.GetCycle() > 50 && !ChaosTwitch_SaveGameStorage.Instance.hasUnlockedTacoRecipe)
                || 
                (ChaosTwitch_SaveGameStorage.Instance.lastTacoRain + 75f > GameClock.Instance.GetTimeInCycles()) 
                ;
            };

        public Danger EventDanger => Danger.None;
    }
}
