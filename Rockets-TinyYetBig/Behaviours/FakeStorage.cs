//using KSerialization;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;

//namespace Rockets_TinyYetBig.Behaviours
//{
//    internal class FakeStorage : KMonoBehaviour, ISim200ms
//    {
//        [Serialize]
//        public Storage backupStorage;

//        [Serialize]
//        public Storage refStorage;
//        [Serialize]
//        public RocketModuleLinkType LinkType;


//        public enum RocketModuleLinkType
//        {
//            FreezerModule = 0,
//        }


//        public bool shouldShow = true;
//        public bool IsLinked => refStorage != null;

//        public bool allowUIItemRemoval
//        {
//            get => IsLinked ? refStorage.allowUIItemRemoval : false;
//            set
//            {
//                if (IsLinked)
//                    refStorage.allowUIItemRemoval = value;
//            }
//        }

//        public bool ShouldShowInUI() => shouldShow;

//        internal void InitLink(Storage target,Storage backup)
//        {
//            refStorage = target;
//            backupStorage = backup;
//            foreach (var item in backupStorage.items)
//            {
//                refStorage.Store(item, true);
//            }

//        }
//        public void CloseLink()
//        {
//            foreach(var item in refStorage.items)
//            {
//                backupStorage.Store(item, true);
//            }
//            refStorage = null;
//        }

//        public void Sim200ms(float dt)
//        {
//            if (backupStorage == null || refStorage == null)
//                return;
//            foreach (var item in backupStorage.items)
//            {
//                refStorage.Store(item, true);
//            }
//        }
//    }
//}
