using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace Imalas_TwitchChaosEvents.BeeGeyser
{
    internal class BeeGeyser : KMonoBehaviour, ISim200ms
    {
        float beeIntervalMin = 30;
        float beeIntervalMax = 2f;
        float beeIntervalChange = 1/4f;

        [Serialize] float currentInterval = 2f;

        [Serialize] float timeToNextBee = 0;

        [MyCmpGet]
        KBatchedAnimController animController;
        public override void OnSpawn()
        {
            animController.flipY = true;
            animController.offset = new UnityEngine.Vector3(0, 4);
            animController.onAnimComplete += (s) => SpawnBee(s);
            base.OnSpawn();
        }

        public override void OnCleanUp()
        {
            YeetOre(333);
            base.OnCleanUp();
        }
        void YeetOre(float amount)
        {

            GameObject go = ElementLoader.FindElementByHash(SimHashes.EnrichedUranium).substance.SpawnResource(this.transform.position, amount, UtilLibs.UtilMethods.GetKelvinFromC(33f), byte.MaxValue, 0, false);
            go.transform.SetPosition(Grid.CellToPosCCC(Grid.PosToCell(this), Grid.SceneLayer.Ore));
            go.SetActive(true);

            Vector2 initial_velocity = new Vector2(UnityEngine.Random.Range(-2f, 2f) * 1f, (float)((double)UnityEngine.Random.value * 2.5 + 3.0));
            if (GameComps.Fallers.Has((object)go))
                GameComps.Fallers.Remove(go);
            GameComps.Fallers.Add(go, initial_velocity);
        }
        public void Sim200ms(float dt)
        {
            timeToNextBee -= dt;
            if (timeToNextBee <= 0)
            {
                currentInterval += beeIntervalChange;
                currentInterval = Mathf.Min(beeIntervalMin, Mathf.Max(currentInterval, beeIntervalMax));

                timeToNextBee = currentInterval;
                //SgtLogger.l(currentInterval.ToString(), "currentInvervalBee");

                if (LiveBees.Count > 25)
                    return;

                animController.Play("erupt");
            }
        }

        private void SpawnBee(HashedString s)
        {
            SgtLogger.l(s.ToString(), new HashedString("erupt").ToString());  
            if (s.hash != new HashedString("erupt").hash)
                return;

            var bee = GameUtil.KInstantiate(Assets.GetPrefab(BeeConfig.ID), Grid.SceneLayer.Creatures);
            bee.AddComponent<GeyserBeeLiveState>().parent = this;
            bee.transform.SetPosition(this.transform.position + new Vector3(0.5f, 1));
            bee.SetActive(true);
           

        }
        [Serialize]
        List<GeyserBeeLiveState> LiveBees = new List<GeyserBeeLiveState>();
        public int BeeCount => LiveBees.Count;

        internal void AddBee(GeyserBeeLiveState geyserBeeLiveState)
        {
            if(LiveBees.Contains(geyserBeeLiveState)) return;
            LiveBees.Add(geyserBeeLiveState);
        }

        internal void RemoveBee(GeyserBeeLiveState geyserBeeLiveState)
        {
            if (!LiveBees.Contains(geyserBeeLiveState)) return;
            LiveBees.Remove(geyserBeeLiveState);
        }
    }
}
