using Klei;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static STRINGS.MISC.STATUSITEMS;

namespace DupeStations.PajamasLocker
{
    internal class PajamasLocker : KMonoBehaviour
    {
        [MyCmpGet]
        private Building building;

        [MyCmpGet]
        private Operational operational;
        public bool IsOperational=>operational!=null&&operational.IsOperational;

        [MyCmpGet]
        private Rotatable rotatable;
        public bool IsRotated => rotatable.IsRotated;

        private PajamasLocker.PajamasLockerReactable equipReactable;
        private PajamasLocker.PajamasLockerReactable unequipReactable;
        private int cell;
        public KAnimFile interactAnim = Assets.GetAnim((HashedString)"anim_interacts_gravitas_container_kanim");
        


        public override void OnSpawn()
        {
            base.OnSpawn();
            Debug.Assert(interactAnim != null, "interactAnim is null");
            this.CreateNewEquipReactable();
            this.CreateNewUnequipReactable();
            this.cell = Grid.PosToCell(this);
        }

        private void CreateNewEquipReactable() => this.equipReactable = new PajamasLocker.EquipPajamasReactable(this);

        private void CreateNewUnequipReactable() => this.unequipReactable = new PajamasLocker.UnequipPajamasReactable(this);

        public override void OnCleanUp()
        {
            base.OnCleanUp();
            if (this.equipReactable != null)
                this.equipReactable.Cleanup();
            if (this.unequipReactable != null)
                this.unequipReactable.Cleanup();
        }

        private class EquipPajamasReactable : PajamasLocker.PajamasLockerReactable
        {
            public EquipPajamasReactable(PajamasLocker marker)
              : base((HashedString)nameof(EquipPajamasReactable), marker)
            {
            }

            public override bool InternalCanBegin(
              GameObject newReactor,
              Navigator.ActiveTransition transition)
            {

                bool SlotOccupied = newReactor.GetComponent<MinionIdentity>().GetEquipment().IsSlotOccupied(Db.Get().AssignableSlots.Outfit);
                bool baseBegin = base.InternalCanBegin(newReactor, transition);
                bool rightWay = this.MovingTheRightWay(newReactor, transition);
                SgtLogger.l($"slot free: {!SlotOccupied}, baseBegin: {baseBegin} , right way: {rightWay}","EquipReactable");

                return !SlotOccupied && baseBegin && rightWay;
            }

            public override void InternalBegin()
            {
                base.InternalBegin();
                this.PajamasLocker.CreateNewEquipReactable();
            }

            public override bool MovingTheRightWay(
              GameObject newReactor,
              Navigator.ActiveTransition transition)
            {
                bool flag = transition.navGridTransition.x < 0;
                bool rightWay= this.IsRocketDoorExitEquip(newReactor, transition) || flag == this.PajamasLocker.IsRotated;

                return rightWay;
            }

            private bool IsRocketDoorExitEquip(
              GameObject new_reactor,
              Navigator.ActiveTransition transition)
            {
                bool flag = transition.end != NavType.Teleport && transition.start != NavType.Teleport;
                return transition.navGridTransition.x == 0 && new_reactor.GetMyWorld().IsModuleInterior && !flag;
            }

            public override void Run()
            {
                var identity = this.reactor.GetComponent<MinionIdentity>();
                Equipment equipment = identity.GetEquipment();

                Vector3 targetPoint = Grid.CellToPos(PajamasLocker.cell) with
                {
                    z = Grid.GetLayerZ(Grid.SceneLayer.BuildingFront)
                };
                var pajamas = Util.KInstantiate(Assets.GetPrefab("SleepClinicPajamas"), targetPoint, Quaternion.identity);
                pajamas.SetActive(true);
                pajamas.TryGetComponent<Equippable>(out var equippable);
                pajamas.GetComponent<EquippableWorkable>().CancelChore("Manual equip");

                equippable.Assign(identity);
                equipment.refreshHandle.ClearScheduler();
                equipment.Equip(equippable);
            }
        }

        private class UnequipPajamasReactable : PajamasLocker.PajamasLockerReactable
        {
            public UnequipPajamasReactable(PajamasLocker marker)
              : base((HashedString)nameof(UnequipPajamasReactable), marker)
            {
            }

            public override bool InternalCanBegin(
              GameObject newReactor,
              Navigator.ActiveTransition transition)
            {
                var equipment = newReactor.GetComponent<MinionIdentity>().GetEquipment();
                if (equipment.IsSlotOccupied(Db.Get().AssignableSlots.Outfit))
                {
                    Assignable assignable = equipment.GetAssignable(Db.Get().AssignableSlots.Outfit);
                    bool isPajamas = assignable.PrefabID() == "SleepClinicPajamas";
                    bool rightWay = this.MovingTheRightWay(newReactor, transition);
                    bool baseCanBegin = base.InternalCanBegin(newReactor, transition);
                    SgtLogger.l($"item in slot: {assignable}, isPajamas: {isPajamas} , right way: {rightWay}, baseCanbegin {baseCanBegin}", "UnequipPajamas");

                    
                    return isPajamas && rightWay && baseCanBegin;
                }
                SgtLogger.l("no equip","UnequipPajamas");
                return false;
            }

            public override void InternalBegin()
            {
                base.InternalBegin();
                this.PajamasLocker.CreateNewUnequipReactable();
            }

            public override bool MovingTheRightWay(
              GameObject newReactor,
              Navigator.ActiveTransition transition)
            {
                bool flag = transition.navGridTransition.x < 0;
                SgtLogger.l((flag != this.PajamasLocker.IsRotated)+"","OnUnequip");
                return transition.navGridTransition.x != 0 && flag != this.PajamasLocker.IsRotated;
            }

            public override void Run()
            {
                Equipment equipment = this.reactor.GetComponent<MinionIdentity>().GetEquipment();

                Assignable assignable = equipment.GetAssignable(Db.Get().AssignableSlots.Outfit);
                if (assignable == null || !assignable.TryGetComponent<KPrefabID>(out var id) || id.PrefabTag != "SleepClinicPajamas" || !assignable.TryGetComponent<Equippable>(out var equippable))
                {
                    SgtLogger.l("canceling Unequip");
                    return;

                }
                equippable.Unassign();
                equipment.Unequip(equippable);
            }
        }

        private abstract class PajamasLockerReactable : Reactable
        {
            public PajamasLocker PajamasLocker;
            public float startTime;

            public PajamasLockerReactable(HashedString id, PajamasLocker suit_marker)
              : base(suit_marker.gameObject, id, Db.Get().ChoreTypes.SuitMarker, 1, 1)
            {
                this.PajamasLocker = suit_marker;
            }

            public override bool InternalCanBegin(
              GameObject new_reactor,
              Navigator.ActiveTransition transition)
            {
                SgtLogger.l($"reactor  null? {reactor = null}, Locker not null? {PajamasLocker != null}, operational {PajamasLocker.IsOperational}","baseReactable");
                if (reactor != null)
                    return false;
                if (PajamasLocker == null)
                {
                    this.Cleanup();
                    return false;
                }
                return this.PajamasLocker.IsOperational;
            }

            public override void InternalBegin()
            {
                this.startTime = Time.time;
                KBatchedAnimController component1 = this.reactor.GetComponent<KBatchedAnimController>();
                component1.AddAnimOverrides(this.PajamasLocker.interactAnim, 1f);
                component1.Play((HashedString)"working_pre");
                component1.Queue((HashedString)"working_loop");
                component1.Queue((HashedString)"working_pst");
                if (!this.PajamasLocker.HasTag(GameTags.JetSuitBlocker))
                    return;
                KBatchedAnimController component2 = this.PajamasLocker.GetComponent<KBatchedAnimController>();
                component2.Play((HashedString)"working_pre");
                component2.Queue((HashedString)"working_loop");
                component2.Queue((HashedString)"working_pst");
            }

            public override void Update(float dt)
            {
                Facing facing = (bool)reactor ? this.reactor.GetComponent<Facing>() : null;
                if ((bool)facing && (bool)PajamasLocker)
                    facing.SetFacing(this.PajamasLocker.GetComponent<Rotatable>().GetOrientation() == Orientation.FlipH);
                if ((double)Time.time - startTime <= 3)
                    return;
                if (reactor != null && PajamasLocker != null)
                {
                    this.reactor.GetComponent<KBatchedAnimController>().RemoveAnimOverrides(this.PajamasLocker.interactAnim);
                    this.Run();
                }
                this.Cleanup();
            }

            public override void InternalEnd()
            {
                if (!(reactor != null))
                    return;
                this.reactor.GetComponent<KBatchedAnimController>().RemoveAnimOverrides(this.PajamasLocker.interactAnim);
            }

            public override void InternalCleanup()
            {
            }

            public abstract bool MovingTheRightWay(
              GameObject reactor,
              Navigator.ActiveTransition transition);

            public abstract void Run();
        }
    }

}
