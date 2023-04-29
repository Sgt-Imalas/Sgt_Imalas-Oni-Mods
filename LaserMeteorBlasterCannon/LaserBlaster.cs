using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace LaserMeteorBlasterCannon
{
    internal class LaserBlaster :
  GameStateMachine<LaserBlaster, LaserBlaster.Instance, IStateMachineTarget, LaserBlaster.Def>
    {
        private static StatusItem NoSurfaceSight = new StatusItem("LaserBlaster_NoSurfaceSight", "BUILDING", "status_item_no_sky", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
        private static StatusItem PartiallyBlockedStatus = new StatusItem("LaserBlaster_PartiallyBlocked", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, false);
        public float shutdownDuration = 50f;
        public float shootDelayDuration = 0.12f;
        public BoolParameter rotationComplete;
        public ObjectParameter<GameObject> meteorTarget = new ObjectParameter<GameObject>();
        public TargetParameter cannonTarget;
        public BoolParameter fullyBlocked;
        public int RemainingShots = 0;
        float internalShotCooldown = 0; 

        public State Off;
        public OnState On;
        public LaunchState Launch;
        public CooldownState Cooldown;
        public State Nosurfacesight;
        public State NoAmmo;

        public override void InitializeStates(out BaseState default_state)
        {
            default_state = Off;
            this.root.Update((smi, dt) => smi.HasLineOfSight());
            this.Off
                .PlayAnim("on")
                .EventTransition(GameHashes.OperationalChanged, On, smi => smi.Operational.IsOperational)
                .Enter(smi => smi.Operational.SetActive(false));

            this.On
                .DefaultState(this.On.searching)
                .EventTransition(GameHashes.OperationalChanged, this.Off, smi => !smi.Operational.IsOperational)
                .ParamTransition(fullyBlocked, this.Nosurfacesight, IsTrue)
                .ScheduleGoTo(this.shutdownDuration, On.idle)
                .Enter(smi => smi.Operational
                .SetActive(smi.Operational.IsOperational));

            this.On
                .searching
                .PlayAnim("on", KAnim.PlayMode.Loop)
                .Enter(smi =>
                {
                    smi.sm.rotationComplete.Set(false, smi);
                    smi.sm.meteorTarget.Set(null, smi);
                    smi.cannonRotation = smi.def.scanningAngle;
                })
                .Update("FindMeteor", 
                (smi, dt) => smi.Searching(dt), UpdateRate.SIM_EVERY_TICK)
                //.EventTransition(GameHashes.OnStorageChange, this.NoAmmo, smi => smi.MissileStorage.Count <= 0)
                .ParamTransition(meteorTarget, this.Launch.targeting, (smi, meteor) => meteor != null)
                .Exit(smi => smi.sm.rotationComplete.Set(false, smi));

            this.On
                .idle
                .Target(this.masterTarget)
                .PlayAnim("on", KAnim.PlayMode.Loop)
                .UpdateTransition(On, (smi, dt) => smi.Operational.IsOperational && smi.MeteorDetected())
                .Target(this.cannonTarget).PlayAnim("Cannon_working_pst");
            this.Launch
                .PlayAnim("on", KAnim.PlayMode.Loop)
                .Update("Rotate", (smi, dt) => smi.RotateToMeteor(dt), UpdateRate.SIM_EVERY_TICK);
            this.Launch
                .targeting
                .Update("Targeting", 
                (smi, dt) =>
                {
                    if (smi.sm.meteorTarget.Get(smi).IsNullOrDestroyed())
                    {
                        smi.GoTo(On.searching);
                    }
                    else
                    {
                        //if ((double)smi.cannonAnimController.Rotation >= smi.def.maxAngle * -1.0 && (double)smi.cannonAnimController.Rotation <= smi.def.maxAngle)
                        //return;
                        //smi.sm.meteorTarget.Get(smi).GetComponent<Comet>().Targeted = false;
                        //smi.sm.meteorTarget.Set(null, smi);
                        //smi.GoTo(On.searching);
                    }
                }, UpdateRate.SIM_EVERY_TICK)
                .ParamTransition(rotationComplete, this.Launch.InitSalvo, IsTrue);
            this.Launch
                .InitSalvo
                .Enter( (smi)=>
                {
                    RemainingShots = smi.def.SalvoShots;
                })
                .GoTo(Launch.shootSalvo);
            this.Launch.shootSalvo
                .Update((smi, dt) =>
                {
                    if(RemainingShots == 0)
                    {
                        smi.GoTo(Launch.pst);
                    }
                    if(internalShotCooldown <= 0)
                    {
                        internalShotCooldown += smi.def.ShotCooldown;
                        smi.FireProjectile(RemainingShots);
                        RemainingShots--;
                    }
                    else
                    {
                        internalShotCooldown -= dt;
                    }
                }
                );

            this.Launch
                .pst
                .GoTo(Cooldown);
            this.Cooldown
                .Update("Rotate", (smi, dt) => smi.RotateToMeteor(dt), UpdateRate.SIM_EVERY_TICK, false)
                .Enter(smi =>
            {
                //KAnimControllerBase component = smi.GetComponent<KAnimControllerBase>();
                //if (smi.GetComponent<Storage>().Count <= 0)
                //    component.Play((HashedString)"base_shooting_pst_last");
                //else
                //    component.Play((HashedString)"base_shooting_pst");
            }).GoTo(this.On.searching);
            this.Nosurfacesight
                .Target(this.masterTarget)
                .PlayAnim("on")
                .QueueAnim("on")
                .ParamTransition(fullyBlocked, On, IsFalse)
                //.Target(this.cannonTarget).PlayAnim("Cannon_working_pst")
                .Enter(smi => smi.Operational.SetActive(false));
            this.NoAmmo.PlayAnim("on")
                //.EventTransition(GameHashes.OnStorageChange, On, smi => smi.MissileStorage.Count > 0)
                .Enter(smi => smi.Operational
                .SetActive(false)).Exit(smi => smi.GetComponent<KAnimControllerBase>()
                .Play((HashedString)"on"));
                //.Target(this.cannonTarget).PlayAnim("Cannon_working_pst");
        }

        public class Def : BaseDef
        {
            public static readonly CellOffset LaunchOffset = new CellOffset(0, 1);
            public float projectileSpeed = 70f;
            public float rotationSpeed = 150f;
            public static readonly Vector2I launchRange = new Vector2I(32, 60);
            public float scanningAngle = 85f;
            public float maxAngle = 85f;
            public Vector2[] BarrelOffsets = new Vector2[] { new Vector2(0f, 0f )};
            public int SalvoShots = 4;
            public float ShotCooldown = 0.25f;
            public float SalvoCooldown = 3f;
        }

        public new class Instance :
          GameInstance
        {
            [MyCmpReq]
            public Operational Operational;
            //[MyCmpReq]
            //public HighEnergyParticleStorage ProjectileStorage;
            [MyCmpReq]
            public KSelectable Selectable;
            private Vector3 launchPosition;
            private Vector2I launchXY;

            public float cannonRotation;
            public float simpleAngle;
            private Tag missileElement;
            private MeterController meter;
            private WorldContainer worldContainer;
            private MeterController barrelRotationController;

            public WorldContainer myWorld
            {
                get
                {
                    if (worldContainer == null)
                        this.worldContainer = this.GetMyWorld();
                    return this.worldContainer;
                }
            }

            public Instance(IStateMachineTarget master, Def def)
              : base(master, def)
            {
                KBatchedAnimController component = this.GetComponent<KBatchedAnimController>();
                string name = component.name + ".cannon";
                
                this.meter = new MeterController(this.GetComponent<KBatchedAnimController>(), "meter_target", nameof(meter), Meter.Offset.Infront, Grid.SceneLayer.NoLayer, Array.Empty<string>());
                launchPosition = transform.position;
                launchPosition.y++;

                Grid.PosToXY(this.launchPosition, out this.launchXY);
                barrelRotationController = new MeterController(GetComponent<KBatchedAnimController>(), "ball_target", "ball_rotation", Meter.Offset.Behind, Grid.SceneLayer.BuildingBack, Array.Empty<string>());
                barrelRotationController.gameObject.GetComponent<KBatchedAnimTracker>().matchParentOffset = true;
            }

            public override void StartSM()
            {
                base.StartSM();
            }

            public override void OnCleanUp()
            {
                base.OnCleanUp();
            }

            public void Searching(float dt)
            {
                this.FindMeteor();
                this.RotateCannon(dt, this.def.rotationSpeed / 2f);
                if (!this.smi.sm.rotationComplete.Get(this.smi))
                    return;
                this.cannonRotation *= -1f;
                this.smi.sm.rotationComplete.Set(false, this.smi);
            }

            public void FindMeteor()
            {
                GameObject gameObject = this.ChooseClosestInterceptionPoint(this.myWorld.id);
                if (!(gameObject != null))
                    return;
                this.smi.sm.meteorTarget.Set(gameObject, this.smi);
                gameObject.GetComponent<Comet>().Targeted = true;
                this.smi.cannonRotation = this.CalculateLaunchAngle(gameObject.transform.position);
            }

            private float CalculateLaunchAngle(Vector3 targetPosition) => MathUtil.AngleSigned(Vector3.up, Vector3.Normalize(targetPosition - this.launchPosition), Vector3.forward);

            public void FireProjectile(int barrel)
            {
                GameObject gameObject = this.smi.sm.meteorTarget.Get(this.smi);
                if (gameObject.IsNullOrDestroyed())
                    return;
                else 
                { 
                    var Location = (Vector2)this.launchPosition + def.BarrelOffsets[barrel%def.BarrelOffsets.Length];

                    GameObject Projectile = Util.KInstantiate(Assets.GetPrefab((Tag)NuclearWasteCometConfig.ID), Location);
                    Projectile.SetActive(true);
                    Comet comet = Projectile.GetComponent<Comet>();
                    comet.ignoreObstacleForDamage.Set(smi.master.gameObject.GetComponent<KPrefabID>());
                    comet.addTiles = 0;
                    float f = (float)((double)cannonRotation * 3.14159274101257 / 180.0);
                    comet.Velocity = new Vector2((float)(-(double)Mathf.Cos(f) * def.projectileSpeed), Mathf.Sin(f) * def.projectileSpeed);
                    comet.GetComponent<KBatchedAnimController>().Rotation = (float)-cannonRotation - 90f;
                }
            }

            public GameObject ChooseClosestInterceptionPoint(int world_id)
            {
                GameObject gameObject = null;
                List<Comet> items = Components.Meteors.GetItems(world_id);
                float num1 = Def.launchRange.y;
                foreach (Comet comet in items)
                {
                    if (!comet.IsNullOrDestroyed() && !comet.Targeted)
                    {
                        Vector3 targetPosition = comet.TargetPosition;
                        float timeToCollision;
                        Vector3 collisionPoint = this.CalculateCollisionPoint(targetPosition, (Vector3)comet.Velocity, out timeToCollision);
                        int cell = Grid.PosToCell(collisionPoint);
                        float num2 = Vector3.Distance(collisionPoint, this.launchPosition);

                       // SgtLogger.l("bools meteor: " + Grid.IsValidCell(cell) + ", "+ !Grid.IsSolidCell(cell) + ", "+ ((double)num2 < (double)num1) + ", "+ this.IsMeteorInRange(collisionPoint) + ", " + this.IsPathClear(this.launchPosition, targetPosition));
                        if (Grid.IsValidCell(cell) && !Grid.IsSolidCell(cell) && (double)num2 < (double)num1 && this.IsMeteorInRange(collisionPoint) && this.IsPathClear(this.launchPosition, targetPosition))
                        {
                            gameObject = comet.gameObject;
                            num1 = num2;
                        }
                    }
                }
                //SgtLogger.l("finding meteor: "+gameObject);
                return gameObject;
            }

            private bool IsMeteorInRange(Vector3 interception_point)
            {
                Vector2I xy;
                Grid.PosToXY(interception_point, out xy);
                return Math.Abs(xy.X - this.launchXY.X) <= Def.launchRange.X && xy.Y - this.launchXY.Y > 0 && xy.Y - this.launchXY.Y <= Def.launchRange.Y;
            }

            public bool IsPathClear(Vector3 startPoint, Vector3 endPoint)
            {
                Vector2I xy1 = Grid.PosToXY(startPoint);
                Vector2I xy2 = Grid.PosToXY(endPoint);
                return Grid.TestLineOfSight(xy1.x, xy1.y, xy2.x, xy2.y, new Func<int, bool>(Grid.IsSolidCell));
            }

            public Vector3 CalculateCollisionPoint(
              Vector3 targetPosition,
              Vector3 targetVelocity,
              out float timeToCollision)
            {
                Vector3 vector3 = targetVelocity - this.smi.def.projectileSpeed * (targetPosition - this.launchPosition).normalized;
                timeToCollision = (targetPosition - this.launchPosition).magnitude / vector3.magnitude;
                return targetPosition + targetVelocity * timeToCollision;
            }

            public void HasLineOfSight()
            {
                bool flag = false;
                bool on = true;
                Extents extents = this.GetComponent<Building>().GetExtents();
                int val2_1 = this.launchXY.x - Def.launchRange.X;
                int val2_2 = this.launchXY.x + Def.launchRange.X;
                int y = extents.y + extents.height;
                int cell1 = Grid.XYToCell(Math.Max((int)this.myWorld.minimumBounds.x, val2_1), y);
                int cell2 = Grid.XYToCell(Math.Min((int)this.myWorld.maximumBounds.x, val2_2), y);
                for (int i = cell1; i <= cell2; ++i)
                {
                    flag = flag || Grid.ExposedToSunlight[i] <= 0;
                    on = on && Grid.ExposedToSunlight[i] <= 0;
                }
                this.Selectable.ToggleStatusItem(PartiallyBlockedStatus, flag && !on);
                this.Selectable.ToggleStatusItem(NoSurfaceSight, on);
                this.smi.sm.fullyBlocked.Set(on, this.smi);
            }

            public bool MeteorDetected() => Components.Meteors.GetItems(this.myWorld.id).Count > 0;


            public void RotateCannon(float dt, float rotation_speed)
            {

                float num1 = this.cannonRotation - this.simpleAngle;
                if ((double)num1 > 180.0)
                    num1 -= 360f;
                else if ((double)num1 < -180.0)
                    num1 += 360f;
                float num2 = rotation_speed * dt;
                if ((double)num1 > 0.0 && (double)num2 < (double)num1)
                {
                    this.simpleAngle += num2;
                    barrelRotationController.SetPositionPercent((simpleAngle +def.maxAngle) / (def.maxAngle*2));
                }
                else if ((double)num1 < 0.0 && -(double)num2 > (double)num1)
                {
                    this.simpleAngle -= num2;
                    barrelRotationController.SetPositionPercent((simpleAngle +def.maxAngle) / (def.maxAngle*2));
                }
                else
                {
                    this.simpleAngle = this.cannonRotation;

                    
                    barrelRotationController.SetPositionPercent((simpleAngle +def.maxAngle) / (def.maxAngle*2));
                    this.smi.sm.rotationComplete.Set(true, this.smi);
                }
                //SgtLogger.l(simpleAngle.ToString());
            }

            public void RotateToMeteor(float dt)
            {
                GameObject gameObject = this.sm.meteorTarget.Get(this);
                if (gameObject.IsNullOrDestroyed())
                    return;
                float num1 = this.CalculateLaunchAngle(gameObject.transform.position) - this.simpleAngle;
                if ((double)num1 > 180.0)
                    num1 -= 360f;
                else if ((double)num1 < -180.0)
                    num1 += 360f;
                float num2 = this.def.rotationSpeed * dt;
                if ((double)num1 > 0.0 && (double)num2 < (double)num1)
                {
                    this.simpleAngle += num2;
                }
                else if ((double)num1 < 0.0 && -(double)num2 > (double)num1)
                {
                    this.simpleAngle -= num2;
                }
                else
                {
                    this.smi.sm.rotationComplete.Set(true, this.smi);
                }
                barrelRotationController.SetPositionPercent((simpleAngle + def.maxAngle) / (def.maxAngle * 2));
            }
        }

        public class OnState :
          State
        {
            public State searching;
            public State idle;
        }

        public class LaunchState :
          State
        {
            public State targeting;
            public State InitSalvo;
            public State shootSalvo;
            public State pst;
        }

        public class CooldownState :
          State
        {
            public State cooling;
            public State exit;
            public State exitNoAmmo;
        }
    }

}
