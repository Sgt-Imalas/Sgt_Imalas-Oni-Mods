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

                GameplayEventInstance eventInstance = GameplayEventManager.Instance.StartNewEvent(TacoMeteorPatches.ITC_TacoMeteors, activeWorld);
                // ClusterManager.Instance.activeWorld.GetSMI<GameplaySeasonManager.Instance>().Start(Db.Get().GameplaySeasons.TemporalTearMeteorShowers);
                if(Config.Instance.TacoEventMusic)
                    SoundUtils.PlaySound(ModAssets.SOUNDS.TACORAIN, SoundUtils.GetSFXVolume() * 0.3f,true);

                var world = ClusterManager.Instance.GetWorld(activeWorld);
                //var pos = world.LookAtSurface();


                ToastManager.InstantiateToastWithPosTarget(
                STRINGS.CHAOSEVENTS.TACORAIN.TOAST,
                 body, GetSurfacePos(world)

            );

            });
        };

        Vector3 GetSurfacePos(WorldContainer world)
        {
            int num = (int)world.maximumBounds.y;
            for (int i = 0; i < world.worldSize.X; i++)
            {
                for (int num2 = world.worldSize.y - 1; num2 >= 0; num2--)
                {
                    int num3 = num2 + world.worldOffset.y;
                    int num4 = Grid.XYToCell(i + world.worldOffset.x, num3);
                    if (Grid.IsValidCell(num4) && (Grid.Solid[num4] || Grid.IsLiquid(num4)))
                    {
                        num = Math.Min(num, num3);
                        break;
                    }
                }
            }

            int num5 = (num + world.worldOffset.y + world.worldSize.y) / 2;
            Vector3 vector = new Vector3(world.WorldOffset.x + world.Width / 2, num5, 0f);
            return vector;
        }


        public Func<object, bool> Condition =>
            (data) =>
            {
                return 
                (GameClock.Instance.GetCycle() > 50 && !ChaosTwitch_SaveGameStorage.Instance.hasUnlockedTacoRecipe)
                || (ChaosTwitch_SaveGameStorage.Instance.lastTacoRain + 75f > GameClock.Instance.GetTimeInCycles()) 
                ;
            };

        public Danger EventDanger => Danger.None;
    }
}
