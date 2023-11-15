using Klei.AI;
using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static AnimEventHandler;
using static ModInfo;

namespace SubPlanetaryTransport
{

    public class SubPlanetaryItemRailgun : StateMachineComponent<SubPlanetaryItemRailgun.StatesInstance>, ISim200ms, 
        //ISecondaryInput, 
        ISidescreenButtonControl, ISim33ms
    {
        [MyCmpReq]
        private Building building;
        [MyCmpReq]
        private PrimaryElement element;
        [MyCmpReq]
        private Rotatable rotatable;
        [MyCmpReq]
        private KSelectable selectable;
        [MyCmpReq]
        private EnergyConsumerSelfSustaining energyConsumer;
        [MyCmpReq]
        private Operational operational;
        [MyCmpReq]
        private Overheatable overheatable;
        [MyCmpGet]
        KBatchedAnimController animController;
        [MyCmpGet]
        private BuildingEnabledButton button;
        [MyCmpGet] LogicPorts logicPorts;

        [Serialize]
        private float availableJoules;
        [Serialize]
        private int targetCell = -1;
        [Serialize]
        private float firingCooldown = 0;

        [Serialize]
        private float TargetMeterAngleValue = 0;
        [Serialize]
        private float OldMeterAngleValue = 0;
        [Serialize]
        private float currentLerpValue = 0;
        [Serialize]
        private float rotationTimeforCurrentRotation = 0;
        [Serialize]
        private float activeFiringCooldown = 0;

        //public ConduitPortInfo portInfo;
        //private int secondaryInputCell = -1;
        //private FlowUtilityNetwork.NetworkItem flowNetworkItem;
        //private ConduitConsumer CoolantConsumer;

        public Storage CargoStorage;
        //public Storage CoolantStorage;
        public Storage WorkingStorage;
        //public Storage CoolantOutputStorage;

        //public float CoolantPerShot = 300f;
        public float ItemMassPerShot = 500f;

        public float jouleCapacity = 1f;
        public float joulesPerLaunch = 1f;
        public float kDTUPerShot = 20000f;
        public float TimeForFullRotationSecs = 15f;
        public float MinimumRotationTimeSecs = 5f;
        public float TimeBetweenShotsSecs = 8f;
        const float WindupTime = 3.9f;

        private MeterController Rotation;
        private MeterController StorageMeter;
        private MeterController CoolantStorageMeter;
        private MeterController ChargeMeter;
        private MeterController Firing;
        private Guid connectedStatus;

        private static readonly EventSystem.IntraObjectHandler<TravelTubeEntrance> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<TravelTubeEntrance>((System.Action<TravelTubeEntrance, object>)((component, data) => component.OnRefreshUserMenu(data)));
        private static readonly EventSystem.IntraObjectHandler<TravelTubeEntrance> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<TravelTubeEntrance>((System.Action<TravelTubeEntrance, object>)((component, data) => component.OnOperationalChanged(data)));


        public CellOffset StartPosition = new CellOffset(2, 2);
        public bool pointingLeft => rotatable.Orientation == Orientation.Neutral;

        int ProjectileSourceCell = -1;


        public void RotateGun(float dt)
        {
            currentLerpValue += dt / rotationTimeforCurrentRotation; //(float)easeInOutQuint(currentLerpValue);
            Rotation.SetPositionPercent(Mathf.Lerp(OldMeterAngleValue, TargetMeterAngleValue, (float)easeInOutSine(currentLerpValue)));
        }
        public static double easeInOutSine(float x)
        {
            return -(Math.Cos(Math.PI * x) - 1) / 2;
        }
        public static double easeInOutQuint(float x)
        {
            return x < 0.5 ? 16 * x * x * x * x * x : 1 - Math.Pow(-2 * x + 2, 5) / 2;
        }


        public int TargetCell
        {
            get
            {
                return targetCell;
            }
            set
            {
                targetCell = value;
                RecalculateAimingArc();
            }
        }
        public bool IsCurrentlyTurning => currentLerpValue < 1;

        bool ShouldShoot => logicPorts.GetInputValue(SubPlanetaryItemRailgunConfig.FireInputPort) == 1;
        bool CanShoot
        {
            get
            {
                return !IsCurrentlyTurning
                    && activeFiringCooldown <= 0
                    && availableJoules >= joulesPerLaunch
                    //&& HasEnoughCoolant
                    //&& IsCoolEnough
                    && HasEnoughShootableMaterial;
            }
        }
        //public bool HasEnoughCoolant => CoolantStorage.MassStored() >= CoolantPerShot && CoolantOutputStorage.RemainingCapacity() >= CoolantPerShot;
        public bool HasEnoughShootableMaterial => CargoStorage.MassStored() >= ItemMassPerShot;

        public void RecalculateAimingArc()
        {
            float adjacentLeg = Math.Abs(Grid.CellColumn(TargetCell) - Grid.CellColumn(ProjectileSourceCell));
            float hypotenuse = CellDistance(ProjectileSourceCell, TargetCell);


            OldMeterAngleValue = TargetMeterAngleValue;
            TargetMeterAngleValue = ((Mathf.Acos(adjacentLeg / hypotenuse) * 180) / Mathf.PI) / 90f;

            currentLerpValue = 0;
            var difference = Mathf.Abs(OldMeterAngleValue - TargetMeterAngleValue);
            rotationTimeforCurrentRotation = Mathf.Clamp((difference * TimeForFullRotationSecs + TimeForFullRotationSecs) / 2f, MinimumRotationTimeSecs, TimeForFullRotationSecs);

        }

        public static float CellDistance(int sourceCell, int TargetCell)
        {
            Vector2I SourceXY = Grid.CellToXY(sourceCell), TargetXY = Grid.CellToXY(TargetCell);
            SgtLogger.l("Source for distance: " + SourceXY.ToString() + "  Target for distance: " + TargetXY.ToString());
            return Mathf.Sqrt((Mathf.Pow(SourceXY.X - TargetXY.X, 2) + Mathf.Pow(SourceXY.Y - TargetXY.Y, 2)));
        }



        public bool CanAimThere(int cell)
        {
            var pos = Grid.CellToXY(cell);

            if (!Grid.IsValidCell(cell))
            {
                return false;
            }

            var sourceXY = Grid.CellToXY(ProjectileSourceCell);

            if (pos.Y < sourceXY.Y || (!pointingLeft ? sourceXY.X > pos.X : sourceXY.X < pos.X))
                return false;

            if (CellDistance(ProjectileSourceCell, cell) < 10)
                return false;

            return true;

        }

        public override void OnSpawn()
        {

            ProjectileSourceCell = Grid.OffsetCell(Grid.PosToCell(this), rotatable.GetRotatedCellOffset(StartPosition));

            this.structureTemperature = GameComps.StructureTemperatures.GetHandle(this.gameObject);
            smi.StartSM();

            //this.CoolantConsumer = this.gameObject.AddComponent<ConduitConsumer>();
            //CoolantConsumer.conduitType = ConduitType.Liquid;
            //this.CoolantConsumer.storage = CoolantStorage;
            //this.CoolantConsumer.capacityKG = CoolantStorage.capacityKg;
            //this.CoolantConsumer.useSecondaryInput = true;
            //this.CoolantConsumer.consumptionRate = 10f;
            //CoolantConsumer.forceAlwaysSatisfied = true;
            //RequireInputs requireInputs = this.gameObject.AddComponent<RequireInputs>();
            //requireInputs.conduitConsumer = this.CoolantConsumer;
            //requireInputs.SetRequirements(false, true);

            //this.secondaryInputCell = Grid.OffsetCell(Grid.PosToCell(this.transform.GetPosition()), this.building.GetRotatedOffset(this.portInfo.offset));
            //IUtilityNetworkMgr networkManager = Conduit.GetNetworkManager(this.portInfo.conduitType);
            //this.flowNetworkItem = new FlowUtilityNetwork.NetworkItem(this.portInfo.conduitType, Endpoint.Sink, this.secondaryInputCell, this.gameObject);
            //int secondaryInputCell = this.secondaryInputCell;
            //FlowUtilityNetwork.NetworkItem flowNetworkItem = this.flowNetworkItem;
            //networkManager.AddToNetworks(secondaryInputCell, (object)flowNetworkItem, true);

            Rotation = new MeterController(animController, "gun_barrel_target", "barrelrotation", Meter.Offset.Behind, Grid.SceneLayer.BuildingBack, Array.Empty<string>());
            StorageMeter = new MeterController(animController, "storage_meter_target", "storage_meter", Meter.Offset.Infront, Grid.SceneLayer.TransferArm, Array.Empty<string>());
            CoolantStorageMeter = new MeterController(animController, "coolant_meter_target", "coolant_meter", Meter.Offset.Infront, Grid.SceneLayer.TransferArm, Array.Empty<string>());
            ChargeMeter = new MeterController(Rotation.meterController, "meter_target_charge", "battery_meter", Meter.Offset.Behind, Grid.SceneLayer.TransferArm, Array.Empty<string>());
            Firing = new MeterController(Rotation.meterController, "ShootChargeMeter_target", "rail_shooting_meter", Meter.Offset.Infront, Grid.SceneLayer.BuildingFront, Array.Empty<string>());

            this.Subscribe(-592767678, OnOperationalChangedDelegate);
            this.Subscribe(-1697596308, new System.Action<object>(this.OnStorageChanged));
            this.UpdateCharge();

            currentLerpValue = 0;
        }
        public override void OnCleanUp()
        {
            //Conduit.GetNetworkManager(this.portInfo.conduitType).RemoveFromNetworks(this.secondaryInputCell, (object)this.flowNetworkItem, true);
            base.OnCleanUp();
        }

        private void OnStorageChanged(object obj)
        {
            StorageMeter.SetPositionPercent(Mathf.Clamp01(CargoStorage.MassStored() / CargoStorage.capacityKg));
        }
        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            this.energyConsumer.OnConnectionChanged += new System.Action(this.OnConnectionChanged);
        }
        private void OnConnectionChanged()
        {
            this.UpdateActive();
            this.UpdateConnectionStatus();
        }
        public void UpdateConnectionStatus()
        {
            bool num = button != null && !button.IsEnabled;
            bool isConnected = energyConsumer.IsConnected;
            bool hasLaunchPower = HasLaunchPower;
            if (num || !isConnected || hasLaunchPower)
            {
                connectedStatus = selectable.RemoveStatusItem(connectedStatus);
            }
            else if (connectedStatus == Guid.Empty)
            {
                connectedStatus = selectable.AddStatusItem(Db.Get().BuildingStatusItems.NotEnoughPower);
            }
        }
        public float AvailableJoules => this.availableJoules;
        public float TotalCapacity => this.jouleCapacity;
        public float UsageJoules => this.joulesPerLaunch;
        public bool HasLaunchPower => (double)this.availableJoules > (double)this.joulesPerLaunch;

        public string SidescreenButtonText => "Set Railgun Target";

        public string SidescreenButtonTooltip => "Target location";


        private bool CanAcceptMorePower() => this.operational.IsOperational && (this.button == null || this.button.IsEnabled) && this.energyConsumer.IsExternallyPowered && (double)this.availableJoules < (double)this.jouleCapacity;
        private void UpdateActive() => this.operational.SetActive(this.CanAcceptMorePower());
        public void Sim200ms(float dt)
        {
            
            // SgtLogger.l($"{this.operational.IsOperational} && {(this.button == null || this.button.IsEnabled)} && {this.energyConsumer.IsExternallyPowered} && {(double)this.availableJoules} < {(double)this.jouleCapacity}");

            if (this.CanAcceptMorePower())
            {
                this.availableJoules = Mathf.Min(this.jouleCapacity, this.availableJoules + this.energyConsumer.WattsUsed * dt);
                this.UpdateCharge();
                //SgtLogger.l(availableJoules.ToString(), "Juuls");
            }
            this.energyConsumer.SetSustained(this.HasLaunchPower);
            this.UpdateActive();
            this.UpdateConnectionStatus();


            //Rotation.SetPositionPercent(firingPercentage);
            //Firing.SetPositionPercent(firingPercentage);
            //Charge.SetPositionPercent(firingPercentage);
            // Storage.SetPositionPercent(firingPercentage);
            //CoolantStorage.SetPositionPercent(firingPercentage);

            //firingPercentage += dt*0.1f;
            //firingPercentage %= 1f;

        }
        float firingPercentage = 0f;

        private void UpdateCharge()
        {
            //this.smi.sm.hasLaunchCharges.Set(this.HasLaunchPower, this.smi);
            this.ChargeMeter.SetPositionPercent(Mathf.Clamp01(this.availableJoules / this.jouleCapacity));
            this.energyConsumer.UpdatePoweredStatus();
        }

        //bool ISecondaryInput.HasSecondaryConduitType(ConduitType type) => this.portInfo.conduitType == type;

        //public CellOffset GetSecondaryConduitOffset(ConduitType type) => this.portInfo.conduitType == type ? this.portInfo.offset : CellOffset.none;



        public void SetButtonTextOverride(ButtonMenuTextOverride textOverride)
        {
        }

        public bool SidescreenEnabled() => true;

        public bool SidescreenButtonInteractable() => !IsCurrentlyTurning;

        public void OnSidescreenButtonPressed()
        {
            TargetGunTool.Instance.Activate(this);
        }

        public int HorizontalGroupID() => -1;

        public int ButtonSideScreenSortOrder() => 20;

        public void Sim33ms(float dt)
        {
            if (currentLerpValue < 1)
                RotateGun(dt);

            if (activeFiringCooldown > 0)
                activeFiringCooldown -= dt;


            CoolantStorageMeter.SetPositionPercent(Mathf.Clamp01(1f-activeFiringCooldown/ TimeBetweenShotsSecs));

            if (ShouldShoot && CanShoot)
            {
                FireShot();
                Firing.meterController.Play(("rail_shooting_meter"), KAnim.PlayMode.Once);
            }
        }

        public void FireShot()
        {

            activeFiringCooldown += TimeBetweenShotsSecs;

            GameScheduler.Instance.Schedule("FiringProjectile", WindupTime,
           (obj) =>
           {
               availableJoules -= joulesPerLaunch;
               UpdateCharge();
               HeatAndTransferCoolant();

               Vector2I target = Grid.CellToXY(GetWorldBorderTargetCell()), source = Grid.CellToXY(ProjectileSourceCell);
               var shotPos = BresenhamsDmg(source.x, source.y, target.x, target.y, 400f);
               TransferItemsToTarget(new Vector3(shotPos.Last().X, shotPos.Last().Y, (float)Grid.SceneLayer.Ore));
           });
        }

        public void TransferItemsToTarget(Vector3 Position)
        {
            SgtLogger.l(Position.ToString());

            var remainingMass = ItemMassPerShot;
            SgtLogger.l(remainingMass.ToString());
            SgtLogger.l(CargoStorage.items.Count.ToString());
            for (int i = CargoStorage.items.Count - 1; i >= 0; --i)
            {
                SgtLogger.l(i.ToString());
                GameObject gameObject = CargoStorage.items[i];
                gameObject.TryGetComponent<PrimaryElement>(out var element);
                if (element.Mass > 0.0f)
                {
                    SgtLogger.l(gameObject.ToString());
                    if (gameObject.TryGetComponent<Pickupable>(out var pickupable))
                    {
                        Pickupable amountTaken = pickupable.Take(remainingMass);
                        SgtLogger.l(amountTaken.TotalAmount.ToString());
                        remainingMass -= amountTaken.TotalAmount;
                        amountTaken.transform.SetPosition(Position);
                    }
                }
                if (remainingMass <= 0)
                    break;
            }
        }

        private HandleVector<int>.Handle structureTemperature;
        protected void HeatAndTransferCoolant()
        {
          GameComps.StructureTemperatures.ProduceEnergy(this.structureTemperature, kDTUPerShot, "Intraplanetary Railgun shot", 1f);
            
            //float coolantTotalHeatCapacity = 0.0f;
            //float remainingMass = CoolantPerShot;
            //SgtLogger.l(remainingMass.ToString());
            //SgtLogger.l(CoolantStorage.items.Count.ToString());
            //for (int i = CoolantStorage.items.Count - 1; i >= 0; --i)
            //{
            //    SgtLogger.l(i.ToString());
            //    GameObject gameObject = CoolantStorage.items[i];
            //    gameObject.TryGetComponent<PrimaryElement>(out var element);
            //    if (element.Mass > 0.0f)
            //    {
            //        SgtLogger.l(gameObject.ToString());
            //        if (gameObject.TryGetComponent<Pickupable>(out var pickupable))
            //        {
            //            Pickupable amountTaken = pickupable.Take(remainingMass);
            //            remainingMass -= amountTaken.TotalAmount;
            //            SgtLogger.l(amountTaken.TotalAmount.ToString());
            //            WorkingStorage.Store(amountTaken.gameObject,true);
            //        }
            //    }
            //    if (remainingMass <= 0)
            //        break;
            //}
            //List<PrimaryElement> ActiveCoolant = new List<PrimaryElement>();
            //foreach (GameObject gameObject in WorkingStorage.items)
            //{

            //    gameObject.TryGetComponent<PrimaryElement>(out var element);
            //    if (element.Mass <= 0.0f)
            //        continue;
            //    coolantTotalHeatCapacity += element.Mass * element.Element.specificHeatCapacity;
            //    ActiveCoolant.Add(element);
            //}
            //foreach (var coolant in ActiveCoolant)
            //{
            //    float percentageOfTotalHeatCapacity = coolant.Mass * coolant.Element.specificHeatCapacity / coolantTotalHeatCapacity;
            //    float kilowatts = kDTUPerShot * percentageOfTotalHeatCapacity;
            //    float temperatureChange = GameUtil.CalculateTemperatureChange(coolant.Element.specificHeatCapacity, coolant.Mass, kilowatts);
            //    coolant.Temperature += temperatureChange;
            //}
            //this.WorkingStorage.Transfer(this.CoolantOutputStorage, hide_popups: true);
        }

        public static Vector3 GetPointOnUnitSphereCap(Quaternion targetDirection, float angle)
        {
            var angleInRad = UnityEngine.Random.Range(0.0f, angle) * Mathf.Deg2Rad;
            var PointOnCircle = (UnityEngine.Random.insideUnitCircle.normalized) * Mathf.Sin(angleInRad);
            var V = new Vector3(PointOnCircle.x, PointOnCircle.y, Mathf.Cos(angleInRad));
            return targetDirection * V;
        }
        public static Vector3 GetPointOnUnitSphereCap(Vector3 targetDirection, float angle)
        {
            return GetPointOnUnitSphereCap(Quaternion.LookRotation(targetDirection), angle);
        }

        public int GetWorldBorderTargetCell()
        {
            WorldContainer world = this.GetMyWorld();
            if (targetCell == -1)
                return -1;
            Vector2I originPosition = Grid.CellToXY(ProjectileSourceCell);
            Vector2I targetedCell = Grid.CellToXY(TargetCell);

            SgtLogger.l($"SourceCell: {originPosition} Targeted: {targetedCell}");

            float leftBorderX = world.minimumBounds.x + 1, rightBorderX = world.maximumBounds.x - 1, topBorderY = world.maximumBounds.y - 1, bottomBorderY = world.minimumBounds.y + 1;

            float XCoordinateCheckingPos = pointingLeft ? leftBorderX : rightBorderX;

            int YAtXBorder = (int)GetY(originPosition, targetedCell, XCoordinateCheckingPos);
            int XAtYBorder = (int)GetX(originPosition, targetedCell, topBorderY);
            SgtLogger.l($"XAtYBorder: {XAtYBorder} YatXBorder: {YAtXBorder}");


            if (originPosition.X == targetedCell.X)
                return Grid.XYToCell((int)originPosition.X, (int)topBorderY);
            else if (originPosition.Y == targetedCell.Y)
                return Grid.XYToCell((int)XCoordinateCheckingPos, (int)originPosition.Y);


            if (YAtXBorder >= topBorderY)
            {
                SgtLogger.l("projectile will fly out of top border");
                SgtLogger.l($"Targeted: X:{XAtYBorder},Y:{topBorderY}");
                return Grid.XYToCell(XAtYBorder, (int)topBorderY);
            }
            else
            {
                SgtLogger.l("projectile will fly out of side border");
                SgtLogger.l($"Targeted: X:{XCoordinateCheckingPos},Y:{YAtXBorder}");

                return Grid.XYToCell((int)XCoordinateCheckingPos, YAtXBorder);
            }

        }
        public static float GetY(Vector2I point1, Vector2I point2, float x)
        {
            float m = (float)(point2.Y - point1.Y) / (float)(point2.X - point1.X);
            float b = (float)point1.Y - (float)(m * point1.X);
            return m * x + b;
        }
        public static float GetX(Vector2I point1, Vector2I point2, float y)
        {
            float m = (float)(point2.Y - point1.Y) / (float)(point2.X - point1.X);
            float b = (float)point1.Y - (float)(m * point1.X);

            return (y - b) / m;
        }

        public List<Vector2I> BresenhamsDmg(int x0, int y0, int x1, int y1, float remainingProjectilePierce)
        {
            float vectorLength = (float)Math.Sqrt((Math.Pow(x0 - x1, 2) + Math.Pow(y0 - y1, 2)));
            float percentile = 1f;
            float stepDecrease = 1f / vectorLength;
            var retList = new List<Vector2I>();

            int xDist = Math.Abs(x1 - x0);
            int yDist = -Math.Abs(y1 - y0);
            int xStep = (x0 < x1 ? +1 : -1);
            int yStep = (y0 < y1 ? +1 : -1);
            int error = xDist + yDist;
            while (x0 != x1 || y0 != y1)
            {
                if (2 * error - yDist > xDist - 2 * error)
                {
                    // horizontal step
                    error += yDist;
                    x0 += xStep;
                }
                else
                {
                    // vertical step
                    error += xDist;
                    y0 += yStep;
                }

                if (!CanDamageThisTile(x0, y0, ref remainingProjectilePierce) || remainingProjectilePierce<=0)
                {
                    return retList;
                }
                else
                {
                    retList.Add(new Vector2I(x0, y0));
                    percentile -= stepDecrease;
                }
            }
            return retList;
        }
        public bool CanDamageThisTile(int x, int y, ref float remainingPierce)
        {
            int cell = Grid.XYToCell(x, y);
            if (!Grid.IsValidCell(cell) || Grid.Element[cell].hardness == byte.MaxValue)
            {
                remainingPierce = 0;
                return false;
            }

            Element element = Grid.Element[cell];
            float CellHp = (element.defaultValues.mass / Grid.Mass[cell]) * Grid.Element[cell].hardness;

            if (CellHp > 0)
                remainingPierce -= CellHp;

            GameObject targetTile = Grid.Objects[cell, (int)ObjectLayer.FoundationTile];
            if (targetTile != null && targetTile.HasTag(GameTags.Bunker))
            {
                var vector = new Vector2I(x, y);
                //bunkerTilesHit.Add(new Tuple<int, float>(cell, -1f));

                remainingPierce = 0;
                return false;
            }
            else
            {
                TileDamage(cell, remainingPierce, ProjectileSourceCell);
            }

            return true;
        }

        public void TileDamage(int cell, float damage, int sourceCell)
        {
            GameObject tile_go = Grid.Objects[cell, (int)ObjectLayer.FoundationTile];

            if (damage <= 0)
                return;

            if (tile_go != null)
            {
                //SimCellOccupier component = tile_go.GetComponent<SimCellOccupier>();
                //if (component != null && !component.doReplaceElement)

                if (tile_go.TryGetComponent<BuildingHP>(out BuildingHP hp))
                {
                    tile_go.Trigger(-794517298, new BuildingHP.DamageSourceInfo()
                    {
                        damage = Mathf.RoundToInt(damage)
                    });
                    return;
                }
            }
            else
            {
                WorldDamage.Instance.ApplyDamage(cell, damage, sourceCell);
            }
        }

        public class StatesInstance : GameStateMachine<States, StatesInstance, SubPlanetaryItemRailgun, object>.GameInstance
        {
            public StatesInstance(SubPlanetaryItemRailgun master) : base(master)
            {
            }
        }

        public class States : GameStateMachine<States, StatesInstance, SubPlanetaryItemRailgun>
        {
            public State MissinPrerequisites;
            public State Ready;
            public State NoCoolant;

            public override void InitializeStates(out BaseState default_state)
            {
                default_state = root;
            }
        }
    }

}
