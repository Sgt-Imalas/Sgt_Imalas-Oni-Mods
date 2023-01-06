using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LogicSatellites.Buildings
{
    internal class LaserLens : KMonoBehaviour, ISim200ms
    {
        private WorldContainer world;
        private const float KJ_PER_LUX = 0.0005f;
        protected override void OnSpawn()
        {
            base.OnSpawn();
            world = this.GetMyWorld();
            this.hitEffectPrefab = Assets.GetPrefab((Tag)"fx_powertinker_splash");
        }

        private int lastFocusedCell = Grid.InvalidCell;
        private GameObject hitEffect;
        private GameObject hitEffectPrefab;
        public void Sim200ms(float dt)
        {
            int cellAbove = Grid.GetCellInDirection(Grid.PosToCell(this.gameObject), Direction.Up);
            //int cell = Grid.PosToCell(this.gameObject);
            //Debug.Log(Grid.LightIntensity[cell]+" Source; "+lightSource.Lux);
            int lux = Grid.LightIntensity[cellAbove];

            int targetCell = RefocusLens();
            float energy = CalculateHeatEnergy(dt, lux);

            if (energy > 0)
            {
                HeatCell(targetCell, energy);
                if (targetCell != lastFocusedCell || hitEffect == null)
                {
                    lastFocusedCell = targetCell;
                    CreateHitEffect();
                }
            }
            else
                DestroyHitEffect();
        }
        public float CalculateHeatEnergy(float dt, float lux)
        {
            return lux * KJ_PER_LUX * dt;
        }

        public void HeatCell(int cell, float energy)
        {
            if (cell == Grid.InvalidCell || world == null)
                return;

            SimMessages.ModifyEnergy(cell, energy, Sim.MaxTemperature, SimMessages.EnergySourceID.HeatBulb);
        }
        private void CreateHitEffect()
        {
            if (this.hitEffectPrefab == null)
                return;
            if (this.hitEffect != null)
                this.DestroyHitEffect();
            this.hitEffect = GameUtil.KInstantiate(this.hitEffectPrefab, Grid.CellToPosCCC(lastFocusedCell, Grid.SceneLayer.FXFront2), Grid.SceneLayer.FXFront2);
            this.hitEffect.SetActive(true);

            KBatchedAnimController component = this.hitEffect.GetComponent<KBatchedAnimController>();
            component.sceneLayer = Grid.SceneLayer.FXFront2;
            component.initialMode = KAnim.PlayMode.Loop;
            component.enabled = false;
            component.enabled = true;

            LoopingSounds sound = hitEffect.GetComponent<LoopingSounds>();
            if (sound != null)
                sound.vol = 0;
        }
        public void DestroyHitEffect()
        {
            if (this.hitEffectPrefab == null || !(this.hitEffect != null))
                return;
            this.hitEffect.DeleteObject();
            this.hitEffect = null;
        }

        public int RefocusLens()
        {
            if (world == null)
                return Grid.InvalidCell;

            int testingCell = Grid.GetCellInDirection(Grid.PosToCell(this.gameObject), Direction.Down);

            while (Grid.IsValidCellInWorld(testingCell, world.id))
            {
                if (Grid.IsLiquid(testingCell) || Grid.IsSolidCell(testingCell))
                    return testingCell;
                testingCell = Grid.OffsetCell(testingCell, 0, -1);
            }

            return Grid.InvalidCell;
        }
    }
}
