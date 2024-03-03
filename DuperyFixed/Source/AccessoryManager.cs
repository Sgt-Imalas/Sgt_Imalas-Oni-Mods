using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public int LoadAccessories(string animName, bool saveToCache = false)
        {
            ResourceSet accessories = Db.Get().Accessories;

            KAnimFile anim = Assets.GetAnim(animName);
            KAnim.Build build = anim.GetData().build;

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
