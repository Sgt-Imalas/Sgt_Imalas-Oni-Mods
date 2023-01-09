using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShockWormMob
{
    // GroneHogMound
    using KSerialization;
    using System.Collections.Generic;
    using UnityEngine;

    [SerializationConfig(MemberSerialization.OptIn)]
    public class GroneHogMound : StateMachineComponent<GroneHogMound.StatesInstance>, ISaveLoadable
    {
        public class StatesInstance : GameStateMachine<States, StatesInstance, GroneHogMound>.GameInstance
        {
            public StatesInstance(GroneHogMound smi)
                : base(smi)
            {
            }
        }

        public class States : GameStateMachine<States, StatesInstance, GroneHogMound>
        {
            public class HarvestableStates : State
            {
                public State idle;

                public State enter;

                public State idle_alt;

                public State respawn;
            }

            public HarvestableStates harvestableStates;

            public State collapse;

            public override void InitializeStates(out BaseState default_state)
            {
                default_state = harvestableStates.idle;
                harvestableStates.Update((smi,dt)=>
                {
                    if (!Grid.Solid[Grid.CellBelow(Grid.PosToCell(smi.transform.position))])
                    {
                        smi.GoTo(collapse);
                    }
                })
                    .EventTransition(GameHashes.Harvest, collapse)
                    ;
                harvestableStates.idle.Enter(delegate (StatesInstance smi)
                {
                    smi.Play("idle");
                    smi.ScheduleGoTo(smi.master.RespawnTime, harvestableStates.respawn);
                })
                    .EventTransition(GameHashes.Died, harvestableStates.idle_alt)
                    ;
                harvestableStates.enter.PlayAnim("enter").ScheduleGoTo(1f, harvestableStates.idle);
                harvestableStates.respawn.Enter((smi)=>
                {
                    if (smi.master.activeHogs() < smi.master.maxHogs && !CreatureHelpers.CrewNearby(smi.transform, 7))
                    {
                        smi.Play("exit");
                        smi.master.SpawnHog();
                        smi.ScheduleGoTo(3f, harvestableStates.idle);
                    }
                    else
                    {
                        smi.GoTo(harvestableStates.idle);
                    }
                });
                collapse.Enter(delegate (StatesInstance smi)
                {
                    if (SelectTool.Instance.selected != null && SelectTool.Instance.selected.gameObject == smi.gameObject)
                    {
                        SelectTool.Instance.Select(null);
                    }
                    smi.master.collapsing = true;
                    smi.Play("collapse");
                    smi.master.ReleaseAllHogs();
                    smi.Schedule(3f, delegate
                    {
                        smi.master.Cleanup();
                    });
                });
            }
        }

        private bool collapsing;

        public GameObject mHog;

        public string hogPrefabID = "GroneHog";

        private float RespawnTime = 5f;

        private int maxHogs = 3;

        private List<GameObject> hogs;

        protected override void OnPrefabInit()
        {
            RestoreHogsList();
            base.OnPrefabInit();
        }

        private void RestoreHogsList()
        {
            if (hogs == null)
            {
                hogs = new List<GameObject>();
            }
        }

        public void SpawnHog()
        {
            if (hogPrefabID == null)
            {
                return;
            }
            while (hogs.Count < maxHogs)
            {
                hogs.Add(null);
            }
            for (int i = 0; i < maxHogs; i++)
            {
                if (hogs[i] == null)
                {
                    mHog = GameUtil.KInstantiate(Assets.GetPrefab(GroneHogConfig.ID), this.transform.GetPosition(), Grid.SceneLayer.Creatures);
                    Debug.Log("AAAAAAAAAA)");
                    Debug.Log("SSSAAAAAAAAAAAW)");
                    mHog.GetComponent<GroneHog>().SetMound(this);
                    Debug.Log("ASSDSADASDASDSA)");
                    hogs[i] = mHog;
                    mHog.SetActive(true);
                    return;
                }
            }
            for (int j = 0; j < maxHogs; j++)
            {
                if (hogs[j] != null && !hogs[j].activeSelf)
                {
                    Debug.Log("VVVVVVV)");
                    EnableHog(hogs[j]);
                }
            }
        }

        private void ReleaseAllHogs()
        {
            foreach (GameObject hog in hogs)
            {
                if (hog != null && !hog.activeSelf)
                {
                    EnableHog(hog);
                }
            }
        }

        public void RestoreHog(GameObject restoredHog)
        {
            RestoreHogsList();
            for (int i = 0; i < hogs.Count; i++)
            {
                if (hogs[i] == null)
                {
                    hogs[i] = restoredHog;
                    return;
                }
            }
            hogs.Add(restoredHog);
        }

        private int activeHogs()
        {
            int num = 0;
            foreach (GameObject hog in hogs)
            {
                if (hog != null && hog.activeSelf)
                {
                    num++;
                }
            }
            return num;
        }

        private void Cleanup()
        {
            Util.KDestroyGameObject(base.gameObject);
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();
            base.smi.StartSM();
            Grid.Objects[Grid.PosToCell(base.gameObject), 3] = base.gameObject;
        }

        protected void EnableHog(GameObject hog)
        {
            hog.SetActive(true);
        }

        public bool HogEnter()
        {
            if (!collapsing)
            {
                base.smi.GoTo(base.smi.sm.harvestableStates.enter);
                return true;
            }
            return false;
        }
    }
}
