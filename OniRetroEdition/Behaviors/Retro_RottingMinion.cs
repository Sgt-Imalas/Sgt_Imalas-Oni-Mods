using Klei.AI;
using KSerialization;
using OniRetroEdition.Foods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static STRINGS.DUPLICANTS.STATUSITEMS;
using static STRINGS.ELEMENTS;

namespace OniRetroEdition.Behaviors
{

    public class Retro_RottingMinion : StateMachineComponent<Retro_RottingMinion.StatesInstance>
    {
        [MyCmpGet]
        DecorProvider DecorProvider;


        public float DecompositionPerSecond  = 0.5f/600f;


        public override void OnSpawn()
        {
            smi.StartSM();
        }

        private void DropBones()
        {
            var go = Util.KInstantiate(Assets.GetPrefab(BonesConfig.ID));

            go.transform.SetPosition(Grid.CellToPosCCC(Grid.PosToCell(this), Grid.SceneLayer.Ore));
            go.SetActive(true);
            UnityEngine.Object.Destroy(this.gameObject);
        }


        public class StatesInstance : GameStateMachine<States, StatesInstance, Retro_RottingMinion, object>.GameInstance
        {
            public AttributeModifier satisfiedDecorModifier = new AttributeModifier(Db.Get().BuildingAttributes.Decor.Id, -65f, (string)global::STRINGS.DUPLICANTS.MODIFIERS.DEAD.NAME);
            public AttributeModifier satisfiedDecorRadiusModifier = new AttributeModifier(Db.Get().BuildingAttributes.DecorRadius.Id, 4f, (string)global::STRINGS.DUPLICANTS.MODIFIERS.DEAD.NAME);
            public AttributeModifier rottenDecorModifier = new AttributeModifier(Db.Get().BuildingAttributes.Decor.Id, -100f, (string)global::STRINGS.DUPLICANTS.MODIFIERS.ROTTING.NAME);
            public AttributeModifier rottenDecorRadiusModifier = new AttributeModifier(Db.Get().BuildingAttributes.DecorRadius.Id, 4f, (string)global::STRINGS.DUPLICANTS.MODIFIERS.ROTTING.NAME);
            public StatesInstance(Retro_RottingMinion master) : base(master)
            {

            }
            public bool IsSubmerged() => PathFinder.IsSubmerged(Grid.PosToCell(this.master.transform.GetPosition()));
        }

        public class States : GameStateMachine<States, StatesInstance, Retro_RottingMinion>
        {
            [SerializeField]
            public FloatParameter decomposition;

            public State alive;
            public DeadStates dead;
            public class DeadStates : State
            {
                public State decomposing;
                public RottenStates rotten;
                public State fullyRotten;
            }
            public class RottenStates : State
            {
                public State inGas;
                public State fartingMorbs;
                public State inLiquid;
            }

            public override void InitializeStates(out BaseState default_state)
            {
                default_state = alive;
                this.alive
                    .TagTransition(GameTags.Dead, this.dead);
                this.dead
                    .defaultState= dead.decomposing
                    .Update((smi, dt) =>
                    {
                        decomposition.Delta(smi.master.DecompositionPerSecond * dt,smi);
                    })
                    ;
                this.dead.decomposing
                    .ToggleAttributeModifier("Dead", smi => smi.satisfiedDecorModifier)
                    .ToggleAttributeModifier("Dead", smi => smi.satisfiedDecorRadiusModifier)
                    .ParamTransition(decomposition, this.dead.rotten,(smi,dt) => decomposition.Get(smi) >= 1f);

                this.dead.rotten
                    .defaultState = dead.rotten.inGas
                    .ToggleStatusItem(Db.Get().DuplicantStatusItems.Rotten)
                    .ToggleAttributeModifier("Rotten", smi => smi.rottenDecorModifier)
                    .ToggleAttributeModifier("Rotten", smi => smi.rottenDecorRadiusModifier)
                    .ParamTransition(decomposition, this.dead.fullyRotten, (smi, dt) => !Config.Instance.endlessRotting && decomposition.Get(smi) >= 3f);
                this.dead.rotten.inGas
                    .Transition(dead.rotten.inLiquid, smi => smi.IsSubmerged())
                    .ToggleFX(smi => this.CreateFX(smi))
                    .Enter(smi => smi.ScheduleGoTo(UnityEngine.Random.Range(150f, 300f), dead.rotten.fartingMorbs));
                this.dead.rotten.fartingMorbs.Enter(smi =>
                {
                    GameUtil.KInstantiate(Assets.GetPrefab(new Tag("Glom")), smi.transform.GetPosition(), Grid.SceneLayer.Creatures).SetActive(true);
                    GameUtil.KInstantiate(Assets.GetPrefab(new Tag("Glom")), smi.transform.GetPosition(), Grid.SceneLayer.Creatures).SetActive(true);
                    GameUtil.KInstantiate(Assets.GetPrefab(new Tag("Glom")), smi.transform.GetPosition(), Grid.SceneLayer.Creatures).SetActive(true);
                    smi.GoTo(dead.rotten.inGas);
                });
                this.dead.rotten.inLiquid
                    .Transition(dead.rotten.inGas, smi=>!smi.IsSubmerged())
                    .Update((smi,dt)=>
                    {
                        int maxCellRange = 3;
                        int cell = Grid.PosToCell(smi.master.transform.GetPosition());
                        if (Grid.Element[cell].id == SimHashes.Water)
                        {
                            SimMessages.ReplaceElement(cell, SimHashes.DirtyWater, CellEventLogger.Instance.DecompositionDirtyWater, Grid.Mass[cell], Grid.Temperature[cell], Grid.DiseaseIdx[cell], Grid.DiseaseCount[cell]);
                        }
                        else
                        {
                            int[] list = new int[4];
                            for (int x = 0; x < maxCellRange; ++x)
                            {
                                for (int y = 0; y < maxCellRange; ++y)
                                {
                                    list[0] = Grid.OffsetCell(cell, new CellOffset(-x, y));
                                    list[1] = Grid.OffsetCell(cell, new CellOffset(x, y));
                                    list[2] = Grid.OffsetCell(cell, new CellOffset(-x, -y));
                                    list[3] = Grid.OffsetCell(cell, new CellOffset(x, -y));
                                    list.Shuffle<int>();
                                    foreach (int index in list)
                                    {
                                        if (Grid.GetCellDistance(cell, index) < maxCellRange - 1 && Grid.IsValidCell(index) && Grid.Element[index].id == SimHashes.Water)
                                        {
                                            SimMessages.ReplaceElement(index, SimHashes.DirtyWater, CellEventLogger.Instance.DecompositionDirtyWater, Grid.Mass[index], Grid.Temperature[index], Grid.DiseaseIdx[index], Grid.DiseaseCount[index]);
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                    })
                    ;

                this.dead.fullyRotten
                    .Enter(smi => smi.master.DropBones());

            }
            private FliesFX.Instance CreateFX(StatesInstance smi) => !smi.isMasterNull ? new FliesFX.Instance(smi.master, new Vector3(0.0f, 0.0f, -0.1f)) : null;
        }
    }
}
