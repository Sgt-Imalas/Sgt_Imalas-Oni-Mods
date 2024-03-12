using HarmonyLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;
using static STRINGS.DUPLICANTS.ROLES;

namespace Dupery
{
    class AccessoryManager
    {
        public AccessoryPool Pool { get { return this.accessoryPool; } }

        private AccessoryPool accessoryPool;

        public AccessoryManager()
        {
            accessoryPool = new AccessoryPool();
        }

        public string TryGetAccessoryId(string slotId, string accessoryKey)
        {
            return accessoryPool.GetId(slotId, accessoryKey);
        }


        //[HarmonyPatch(typeof(KAnimGroupFile), nameof(KAnimGroupFile.AddGroup))]
        //public class KAnimGroupFileGroup_TargetMethod_Patch
        //{
        //    public static void Postfix(KAnimGroupFile.GroupFile gf,KAnimFile file)
        //    {
        //            SgtLogger.l(gf.groupID  , "GroupDumping");
        //    }
        //}

        public int LoadAccessories(string animName, bool saveToCache = false)
        {
            ResourceSet accessories = Db.Get().Accessories;

            KAnimFile anim = Assets.GetAnim(animName);
            //HashedString groupId = new HashedString(animName);
            KAnim.Build build = anim.GetData().build;
            //var oldGroup = KAnimGroupFile.GetGroup(groupId);
            //var swapAnimsGroup = KAnimGroupFile.GetGroup(new HashedString(InjectionMethods.BATCH_TAGS.SWAPS));

            //// remove the wrong group
            //oldGroup.animFiles.RemoveAll(g => anim == g);
            //oldGroup.animNames.RemoveAll(g => anim.name == g);
            //// readd to correct group
            //swapAnimsGroup.animFiles.Add(anim);
            //swapAnimsGroup.animNames.Add(anim.name);


            int numLoaded = 0;
            int numCached = 0;
            for (int index = 0; index < build.symbols.Length; ++index)
            {
                string id = HashCache.Get().Get(build.symbols[index].hash);

                AccessorySlot slot;
                bool cachable = true;

                if (id.StartsWith("hair_"))
                {
                    slot = Db.Get().AccessorySlots.Hair;
                }
                else if (id.StartsWith("hat_hair_"))
                {
                    slot = Db.Get().AccessorySlots.HatHair;
                    cachable = false;
                }
                //else if (id.StartsWith("head_"))
                //{
                //    slot = Db.Get().AccessorySlots.HeadShape;
                //    cachable = false;
                //}
                //else if (id.StartsWith("eyes_"))
                //{
                //    slot = Db.Get().AccessorySlots.Eyes;
                //    cachable = false;
                //}
                else
                {
                    continue;
                }

                Accessory accessory = new Accessory(id, accessories, slot, anim.batchTag, build.symbols[index], anim);
                slot.accessories.Add(accessory);
                Db.Get().ResourceTable.Add(accessory);

                if (cachable && saveToCache)
                {
                    accessoryPool.AddId(slot.Id, id, id);
                    numCached++;
                }

                numLoaded++;
            }

            if (numCached > 0)
                Logger.Log($"Added {numCached} new accessories IDs to the cache.");

            return numLoaded;
        }
    }
}
