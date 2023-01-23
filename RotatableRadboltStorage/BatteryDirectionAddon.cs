using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RotatableRadboltStorage
{
    class BatteryDirectionAddon : KMonoBehaviour,
        IHighEnergyParticleDirection,
        ISaveLoadable
    {
        [MyCmpReq]
        private KSelectable selectable;
        [Serialize]
        private EightDirection _direction;
        private MeterController directionController;
        
        public int GetCircularHEPOutputCell()
        {
            int x = 0, y = 0;
            if (Direction.ToString().Contains("Down"))
                y -= 1;
            else if (Direction.ToString().Contains("Up"))
                y += 1;
            if (Direction.ToString().Contains("Right"))
                x += 1;
            else if (Direction.ToString().Contains("Left"))
                x -= 1;
            var build = GetComponent<Building>();

            var offset = build.GetHighEnergyParticleOutputOffset();
            offset.x += x;
            offset.y += y;

            int cell = Grid.OffsetCell(GetComponent<Building>().GetCell(), offset);
            return cell;
        }

        //public void Fire()
        //{
        //    int particleOutputCell = GetCircularHEPOutputCell();
        //    GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab((Tag)"HighEnergyParticle"), Grid.CellToPosCCC(particleOutputCell, Grid.SceneLayer.FXFront2), Grid.SceneLayer.FXFront2);
        //    gameObject.SetActive(true);
        //    if (!(gameObject != null))
        //        return;
        //    HighEnergyParticle component = gameObject.GetComponent<HighEnergyParticle>();
        //    component.payload = hepStorage.ConsumeAndGet(particleThreshold);
        //    component.SetDirection(Direction);
        //}


        //public LocString CapacityUnits => UI.UNITSUFFIXES.HIGHENERGYPARTICLES.PARTRICLES;

        public EightDirection Direction
        {
            get => _direction;
            set
            {
                _direction = value;
                if (directionController == null)
                    return;
                directionController.SetPositionPercent(45f * EightDirectionUtil.GetDirectionIndex(_direction) / 360f);
            }
        }

        public override void OnSpawn()
        {
            base.OnSpawn();
            directionController = new MeterController(GetComponent<KBatchedAnimController>(), "redirector_target", "redirector", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, Array.Empty<string>());
            directionController.gameObject.GetComponent<KBatchedAnimTracker>().matchParentOffset = true;
            Direction = Direction;
        }
    }
}
