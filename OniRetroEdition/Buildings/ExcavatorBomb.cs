//using ClipperLib;
//using Klei;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using static STRINGS.BUILDINGS.PREFABS;
//using UnityEngine;

//namespace OniRetroEdition.Buildings
//{
//    public class ExcavatorBomb : StateMachineComponent<ExcavatorBomb.StatesInstance>
//    {
//        [MyCmpReq]
//        private Building building;
//        [MyCmpReq]
//        private PrimaryElement primaryElement;
//        private static StatusItem statusItemUnarmed;
//        private static StatusItem statusItemArmed;
//        private static StatusItem statusItemCountdown;
//        private static StatusItem statusItemDupeDanger;
//        private static StatusItem statusItemExpoding;
//        public float CountdownTime = 10f;
//        private float currentCountdown;
//        public float minSafeDistance = 5f;
//        public float maxRadius = 5f;
//        public float currentRadius;
//        public float nextExplosion = 0.01f;
//        public bool detectMinion = true;
//        public ExcavatorBomb.ExplosionType type;
//        public Dictionary<int, ExcavatorBomb.visitedPoint> visitedPoints;
//        private Dictionary<int, float> shockwavePoints;
//        private Vector3 startLocation;
//        private int step;
//        private float maxEnergy = float.MinValue;
//        private float totalEnergy = 1f;

//        private void CreateStatusItems()
//        {
//            if (ExcavatorBomb.statusItemUnarmed != null)
//                return;
//            ExcavatorBomb.statusItemUnarmed = new StatusItem("Unarmed", (string)BUILDING.STATUSITEMS.EXCAVATOR_BOMB.UNARMED.NAME, (string)BUILDING.STATUSITEMS.EXCAVATOR_BOMB.UNARMED.TOOLTIP, string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
//            ExcavatorBomb.statusItemArmed = new StatusItem("Armed", (string)BUILDING.STATUSITEMS.EXCAVATOR_BOMB.ARMED.NAME, (string)BUILDING.STATUSITEMS.EXCAVATOR_BOMB.ARMED.TOOLTIP, string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
//            ExcavatorBomb.statusItemCountdown = new StatusItem("Countdown", (string)BUILDING.STATUSITEMS.EXCAVATOR_BOMB.COUNTDOWN.NAME, (string)BUILDING.STATUSITEMS.EXCAVATOR_BOMB.COUNTDOWN.TOOLTIP, string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
//            ExcavatorBomb.statusItemCountdown.resolveStringCallback = (Func<string, object, string>)((str, data) =>
//            {
//                ExcavatorBomb.StatesInstance statesInstance = (ExcavatorBomb.StatesInstance)data;
//                return string.Format(str, GameUtil.GetFormattedTime(statesInstance.master.CountdownRemaining));
//            });
//            ExcavatorBomb.statusItemDupeDanger = new StatusItem("DupeDanger", (string)BUILDING.STATUSITEMS.EXCAVATOR_BOMB.DUPE_DANGER.NAME, (string)BUILDING.STATUSITEMS.EXCAVATOR_BOMB.DUPE_DANGER.TOOLTIP, string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
//            ExcavatorBomb.statusItemExpoding = new StatusItem("Exploding", (string)BUILDING.STATUSITEMS.EXCAVATOR_BOMB.EXPLODING.NAME, (string)BUILDING.STATUSITEMS.EXCAVATOR_BOMB.EXPLODING.TOOLTIP, string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
//        }

//        public float CountdownRemaining => this.currentCountdown;

//        public BuildingDef Def => this.building.Def;

//        protected override void OnPrefabInit() => this.CreateStatusItems();

//        protected override void OnSpawn()
//        {
//            base.OnSpawn();
//            this.smi.StartSM();
//        }

//        private void PlayExplosion(Vector3 pos)
//        {
//            GameUtil.KInstantiate(EffectPrefabs.Instance.Explosion, pos, Grid.SceneLayer.Front, Folder.FX);
//        }

//        private List<Vector2> DoCircularExplosion(float x, float y, float radius)
//        {
//            List<Vector2> circle = Generated.Util.GetCircle(new Vector2(x, y), (int)Mathf.Floor(radius));
//            for (int index = 0; index < circle.Count; ++index)
//            {
//                Vector3 pos = new Vector3(circle[index].x, circle[index].y, (float)(-(double)Grid.CellSizeInMeters * 0.5));
//                int cell = Grid.PosToCell(pos);
//                if (Grid.IsValidCell(cell) && Grid.Element[cell].id != SimHashes.Unobtanium)
//                {
//                    this.PlayExplosion(pos);
//                    int callbackIdx = -1;
//                    if (Grid.Solid[cell])
//                    {
//                        Element elem = Grid.Element[cell];
//                        float mass = Grid.Mass[cell] * 0.25f;
//                        callbackIdx = Game.Instance.callbackManager.Add(() =>
//                        {
//                            if (!elem.IsSolid)
//                                return;
//                            elem.substance.SpawnResource(Grid.CellToPosCCC(cell, Grid.SceneLayer.Ore), mass, 300f,255,0);
//                        }, "ExcavatorBombCircleExplosion").index;
//                    }
//                    SimMessages.ReplaceElement(cell, SimHashes.CarbonDioxide, CellEventLogger.Instance.Excavator, 8f, 1000f, 255,0, callbackIdx);
//                }
//            }
//            return circle;
//        }

//        private List<int> GetNeighbors(int cell)
//        {
//            List<int> neighbors = new List<int>();
//            for (int index1 = -1; index1 < 2; ++index1)
//            {
//                for (int index2 = -1; index2 < 2; ++index2)
//                {
//                    if ((double)Vector3.Distance(Grid.CellToPos(cell, (float)index1, (float)index2, 0.0f), this.startLocation) < (double)this.maxRadius * (double)this.maxRadius)
//                    {
//                        CellOffset offset = new CellOffset(index1, index2);
//                        int index3 = Grid.OffsetCell(cell, offset);
//                        if (!this.visitedPoints.ContainsKey(index3) && Grid.IsValidCell(index3) && Grid.Element[index3].id != SimHashes.Unobtanium)
//                            neighbors.Add(index3);
//                    }
//                }
//            }
//            return neighbors;
//        }

//        private void DoShockwaveExplosion(float x, float y)
//        {
//            if (this.shockwavePoints == null)
//            {
//                this.startLocation = (Vector3)new Vector2(x, y);
//                List<Vector2> vector2List = this.DoCircularExplosion(x, y, 1f);
//                this.visitedPoints = new Dictionary<int, ExcavatorBomb.visitedPoint>();
//                this.shockwavePoints = new Dictionary<int, float>();
//                for (int index = 0; index < vector2List.Count; ++index)
//                {
//                    int cell = Grid.PosToCell(vector2List[index]);
//                    if (Grid.Element[cell].id != SimHashes.Unobtanium && Grid.Element[cell].id != SimHashes.Vacuum)
//                        this.shockwavePoints[cell] = this.totalEnergy;
//                }
//                this.maxEnergy = this.totalEnergy;
//            }
//            else
//            {
//                Dictionary<int, float> dictionary = new Dictionary<int, float>((IDictionary<int, float>)this.shockwavePoints);
//                foreach (int key in this.shockwavePoints.Keys)
//                    this.visitedPoints[key] = new ExcavatorBomb.visitedPoint();
//                this.shockwavePoints.Clear();
//                this.maxEnergy = float.MinValue;
//                foreach (int key in dictionary.Keys)
//                {
//                    if (Grid.Element[key].id != SimHashes.Unobtanium && Grid.Element[key].id != SimHashes.Vacuum)
//                    {
//                        float mass = Grid.Mass[key];
//                        float num1 = this.totalEnergy / 1f;
//                        Element element = Grid.Element[key];
//                        float p = 0.370370358f;
//                        float num2 = 0.7f;
//                        float num3 = 1f;
//                        float in_a1 = 2f;
//                        float in_b1 = 6f;
//                        float num4 = 0.5f;
//                        float num5 = 1f;
//                        float in_a2 = 1f;
//                        float in_b2 = 300f;
//                        float num6 = 0.1f;
//                        float num7 = 0.05f;
//                        float num8 = 0.2f;
//                        float num9 = dictionary[key];
//                        float hm = 0.0f;
//                        float mm;
//                        float energySpent;
//                        if (element.IsSolid)
//                        {
//                            float val = Mathf.Pow((float)element.hardness, p);
//                            hm = MathUtil.Clamp(num2, num3, MathUtil.ReRange(val, in_a1, in_b1, num2, num3));
//                            mm = MathUtil.Clamp(num4, num5, MathUtil.ReRange(mass, in_a2, in_b2, num4, num5));
//                            energySpent = (float)(1.0 - (double)mm * (double)hm) + num6;
//                        }
//                        else if (element.IsLiquid)
//                        {
//                            mm = MathUtil.Clamp(num4, num5, MathUtil.ReRange(mass, in_a2, in_b2, num4, num5));
//                            energySpent = 1f - mm + num7;
//                        }
//                        else
//                        {
//                            mm = MathUtil.Clamp(num4, num5, MathUtil.ReRange(mass, in_a2 / 1000f, in_b2 / 1000f, num4, num5));
//                            energySpent = 1f - mm + num8;
//                        }
//                        float num10 = num9 - energySpent;
//                        this.visitedPoints[key] = new ExcavatorBomb.visitedPoint(dictionary[key], num10, energySpent, this.step, !element.IsSolid ? (!element.IsLiquid ? "gas" : "liquid") : "solid", element.name, hm, mm);
//                        this.maxEnergy = Mathf.Max(num10, this.maxEnergy);
//                        if ((double)num10 > 0.0)
//                        {
//                            KBatchedAnimController effect = FXHelpers.CreateEffect("snore_fx", Grid.CellToPosCCC(key, Grid.SceneLayer.FXFront),null ,false, Grid.SceneLayer.FXFront);
//                            effect.destroyOnAnimComplete = true;
//                            effect.Play("snore");
//                            if (element.IsSolid)
//                            {
//                                float amount = num10 * 2f / this.totalEnergy;
//                                //Output.Log("step", this.step, "\tprevEnergy", dictionary[key], "\tenergy", num10, "\tDamageb", amount);
//                                float kilojoules = 10000f * dictionary[key] / this.totalEnergy;
//                                float temperature = Grid.Temperature[key];
//                                Element elem = Grid.Element[key];
//                                int local_cell = key;
//                                //HandleVector<System.Action>.Handle handle = Game.Instance.callbackManager.Add((System.Action)(() =>
//                                //{
//                                //    if (!elem.IsSolid)
//                                //        return;
//                                //    float temperature1 = temperature + SimUtil.EnergyFlowToTemperatureDelta(kilojoules, elem.specificHeatCapacity, mass);
//                                //    elem.substance.SpawnResource(Grid.CellToPos(local_cell, CellAlignment.RandomInternal, Grid.SceneLayer.Ore), mass * 0.25f, temperature1);
//                                //}), "ExcavatorBombShockwave");
//                                if (WorldDamage.Instance.ApplyDamage(key, amount, -1,WorldDamage.DamageType.Absolute)>0)
//                                    SimMessages.ModifyEnergy(key, kilojoules,4000, SimMessages.EnergySourceID.Excavator);
//                            }
//                            foreach (int neighbor in this.GetNeighbors(key))
//                                this.shockwavePoints[neighbor] = num10;
//                        }
//                    }
//                }
//                ++this.step;
//            }
//        }

//        private bool Explode(float dt)
//        {
//            this.nextExplosion -= dt;
//            if ((double)this.nextExplosion < 0.0)
//            {
//                Collider componentInChildren = this.GetComponentInChildren<Collider>();
//                Bounds bounds = !((UnityEngine.Object)componentInChildren != (UnityEngine.Object)null) ? this.GetComponentInChildren<Collider2D>().bounds : componentInChildren.bounds;
//                float x = bounds.center.x;
//                float y = bounds.center.y;
//                switch (this.type)
//                {
//                    case ExcavatorBomb.ExplosionType.CircularFilled:
//                        ++this.currentRadius;
//                        this.DoCircularExplosion(x, y, this.currentRadius);
//                        break;
//                    case ExcavatorBomb.ExplosionType.Shockwave:
//                        this.DoShockwaveExplosion(x, y);
//                        break;
//                }
//                this.nextExplosion = 1f / 1000f;
//            }
//            if ((double)this.currentRadius <= (double)this.maxRadius && (double)this.maxEnergy > 0.0)
//                return false;
//            int cell1 = Grid.PosToCell((KMonoBehaviour)this);
//            foreach (CellOffset placementOffset in this.Def.PlacementOffsets)
//            {
//                int cell2 = Grid.OffsetCell(cell1, placementOffset);
//                if (Grid.Element[cell2].id != SimHashes.Unobtanium)
//                    this.PlayExplosion(Grid.CellToPosCCC(cell2, Grid.SceneLayer.Building) with
//                    {
//                        z = (float)(-(double)Grid.CellSizeInMeters * 0.5)
//                    });
//            }
//            KFMOD.BeginOneShot(Sounds.Instance.BlowUp_GenericMigrated, transform.GetPosition());
//            GameUtil.CreateExplosion(Grid.CellToPosCCC(cell1, Grid.SceneLayer.Move));
//            UnityEngine.Object.Destroy((UnityEngine.Object)this.gameObject);
//            return true;
//        }

//        public class StatesInstance :
//          GameStateMachine<ExcavatorBomb.States, ExcavatorBomb.StatesInstance, ExcavatorBomb>.GameInstance
//        {
//            public StatesInstance(ExcavatorBomb master)
//              : base(master)
//            {
//            }

//            public bool DupeInDanger()
//            {
//                if (!this.smi.master.detectMinion)
//                    return false;
//                float a = float.MaxValue;
//                for (int idx = 0; idx < Components.LiveMinionIdentities.Count; ++idx)
//                {
//                    float b = Vector3.Distance(Components.LiveMinionIdentities[idx].gameObject.transform.position, this.transform.position);
//                    a = Mathf.Min(a, b);
//                }
//                return (double)a < (double)this.master.maxRadius + (double)this.master.minSafeDistance;
//            }
//        }

//        public class States :
//          GameStateMachine<ExcavatorBomb.States, ExcavatorBomb.StatesInstance, ExcavatorBomb>
//        {
//            public GameStateMachine<ExcavatorBomb.States, ExcavatorBomb.StatesInstance, ExcavatorBomb>.State idle;
//            public GameStateMachine<ExcavatorBomb.States, ExcavatorBomb.StatesInstance, ExcavatorBomb>.State armed;
//            public GameStateMachine<ExcavatorBomb.States, ExcavatorBomb.StatesInstance, ExcavatorBomb>.State countdown;
//            public GameStateMachine<ExcavatorBomb.States, ExcavatorBomb.StatesInstance, ExcavatorBomb>.State dupe_danger;
//            public GameStateMachine<ExcavatorBomb.States, ExcavatorBomb.StatesInstance, ExcavatorBomb>.State exploding;
//            public GameStateMachine<ExcavatorBomb.States, ExcavatorBomb.StatesInstance, ExcavatorBomb>.State defunct;

//            public override void InitializeStates(out StateMachine.BaseState default_state)
//            {
//                base.InitializeStates(out default_state);
//                default_state = (StateMachine.BaseState)this.idle;
//                this.idle.PlayAnim("off", KAnim.PlayMode.Loop)
//                    .ToggleMainStatusItem(ExcavatorBomb.statusItemUnarmed)
//                    .GoTo(this.armed);
//                this.armed
//                    .PlayAnim("on", KAnim.PlayMode.Loop)
//                    .ToggleMainStatusItem(ExcavatorBomb.statusItemArmed)
//                    .GoTo(this.dupe_danger);
//                this.dupe_danger
//                    .PlayAnim("working_post", KAnim.PlayMode.Loop)
//                    .ToggleMainStatusItem(ExcavatorBomb.statusItemDupeDanger)
//                    .Transition(this.countdown, (smi => !smi.DupeInDanger()));
//                this.countdown.PlayAnim("working", KAnim.PlayMode.Loop)
//                    .ToggleMainStatusItem(ExcavatorBomb.statusItemCountdown)
//                    .Transition(this.dupe_danger, (smi => smi.DupeInDanger()))
//                    .Enter((smi => smi.master.currentCountdown = smi.master.CountdownTime))
//                    .Update((smi, dt) =>
//                    {
//                        smi.master.currentCountdown -= dt;
//                        if (smi.master.currentCountdown > 0)
//                            return;
//                        smi.GoTo((StateMachine.BaseState)this.exploding);
//                    });
//                this.exploding
//                    .ToggleMainStatusItem(ExcavatorBomb.statusItemExpoding).Update((smi, dt) =>
//                    {
//                        if (!smi.master.Explode(dt))
//                            return;
//                        smi.GoTo(this.defunct);
//                    });
//            }
//        }

//        public enum ExplosionType
//        {
//            CircularFilled,
//            CircularCenterOnly,
//            Shockwave,
//        }

//        public struct visitedPoint
//        {
//            public float energyIn;
//            public float energyOut;
//            public float energySpent;
//            public int step;
//            public string state;
//            public string element;
//            public float hm;
//            public float mm;

//            public visitedPoint(
//              float energyIn,
//              float energyOut,
//              float energySpent,
//              int step,
//              string state,
//              string element,
//              float hm,
//              float mm)
//            {
//                this.energyIn = energyIn;
//                this.energyOut = energyOut;
//                this.energySpent = energySpent;
//                this.step = step;
//                this.state = state;
//                this.element = element;
//                this.hm = hm;
//                this.mm = mm;
//            }
//        }
//    }

//}
