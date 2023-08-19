using ONITwitchLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UtilLibs;

namespace Imalas_TwitchChaosEvents.Fire
{
    public class ActiveFireUpdater : SlicedUpdaterSim1000ms<ActiveFire>
    {
        public ActiveFireUpdater() : base()
        {
            OnPrefabInit();
        }
    }
    public class ActiveFire : KMonoBehaviour, ISlicedSim1000ms
    {
        [MyCmpGet]
        ParticleSystem particleSystem;

        public float energyPerSecondKJStart = 600f;
        public float energyPerSecondKJPutOut = 200f;

        public float burningtime = 30f;
        public float ignitionEnergyPerSecond = 20f;

        public List<Tuple<CellOffset, float>> HeatZones = new List<Tuple<CellOffset, float>>()
        {
            new Tuple<CellOffset, float>( new CellOffset(0,0),1f ),
            new Tuple<CellOffset, float>( new CellOffset(-1,0),0.66f ),
            new Tuple<CellOffset, float>( new CellOffset(1,0),0.66f ),
            new Tuple<CellOffset, float>( new CellOffset(0,1),0.66f ),
            new Tuple<CellOffset, float>( new CellOffset(0,-1),0.66f ),
            new Tuple<CellOffset, float>( new CellOffset(-1,-1),0.33f ),
            new Tuple<CellOffset, float>( new CellOffset(1,-1),0.33f ),
            new Tuple<CellOffset, float>( new CellOffset(1,1),0.33f ),
            new Tuple<CellOffset, float>( new CellOffset(-1,1),0.33f )

        };
        public void SlicedSim1000ms(float dt)
        {
            Burn(dt);
        }

        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            if (SlicedUpdaterSim1000ms<ActiveFire>.instance == null)
            {
                Game.Instance.gameObject.AddOrGet<ActiveFireUpdater>();
            }
        }

        public override void OnSpawn()
        {
            base.OnSpawn();


            SlicedUpdaterSim1000ms<ActiveFire>.instance.RegisterUpdate1000ms(this);
            activeTime = 0;
        }

        private float activeTime;
        public float particlesStartOverTime = 100;
        public float particlesEndOverTime = 20;

        private bool scheduledForDestruction = false;

        private static readonly CellElementEvent SpawnEvent = new(
    "TwitchSpawnedElement",
    "Spawned by Twitch",
    true
    );

        public void Burn(float dt)
        {
            if (scheduledForDestruction) return;



            float TimeLerp = Mathf.InverseLerp(0, burningtime, activeTime);

            float energy = Mathf.Lerp(energyPerSecondKJStart,energyPerSecondKJPutOut, TimeLerp);

            int particles = Mathf.RoundToInt(Mathf.Lerp(particlesStartOverTime, particlesEndOverTime,  TimeLerp));

            var emission = particleSystem.emission;
            emission.rateOverTime = particles;


            int originCell = Grid.PosToCell(this);

            float multiplier = FireManager.Instance.GetElementMultiplier(originCell);
            float activeMP = multiplier < 0 ? dt * -multiplier : 1f / multiplier;

            activeTime += dt * activeMP;

            foreach (var HeatLocation in HeatZones)
            {
                int cell = Grid.OffsetCell(originCell, HeatLocation.first);
                SimMessages.ModifyEnergy(originCell, HeatLocation.second * energy * dt, UtilMethods.GetKelvinFromC(1200), SimMessages.EnergySourceID.Burner);
                if (Grid.IsValidCellInWorld(cell, ClusterManager.Instance.activeWorldId))
                {
                    float MaxHeat = (HeatLocation.second * ignitionEnergyPerSecond * dt);
                    //SgtLogger.l("" + Mathf.Lerp(MaxHeat, -MaxHeat * 0.1f, Mathf.Clamp01(TimeLerp * 5f)), "heat");
                    FireManager.Instance.ApplyIgnitionHeatToCell(cell,  Mathf.Lerp(MaxHeat, - MaxHeat*0.1f, Mathf.Clamp01(TimeLerp*5f)));


                    SimMessages.ConsumeMass(cell, Grid.Element[cell].id, 1f * dt, 1, -1);
                    if (!Grid.IsSolidCell(Grid.CellAbove(cell)))
                    {
                        SimMessages.ReplaceAndDisplaceElement(Grid.CellAbove(cell), SimHashes.CarbonDioxide, SpawnEvent, 0.1f * dt, UtilMethods.GetKelvinFromC(600));
                    }
                }
            }


            if (activeTime > burningtime)
            {
                emission.rateOverTime = 0;
                scheduledForDestruction = true;

                FireManager.Instance.RemoveFire(Grid.PosToCell(this));

                GameScheduler.Instance.Schedule("DestroyBurningCmp ", 5, (s) =>
                {
                    SlicedUpdaterSim1000ms<ActiveFire>.instance.UnregisterUpdate1000ms(this);
                    Destroy(gameObject);
                }
                );
            }
        }

    }
}
