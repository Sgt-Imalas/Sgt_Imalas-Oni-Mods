using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dupery
{
    class AccessoryPool
    {
        private Dictionary<string, Dictionary<string, string>> pool;

        public AccessoryPool()
        {
            pool = new Dictionary<string, Dictionary<string, string>>
            {
                { "Hair", new Dictionary<string, string>() }
            };
        }

        public string GetId(string slotId, string accessoryKey)
        {
            if (!pool.ContainsKey(slotId))
                return null;

            pool[slotId].TryGetValue(accessoryKey, out string id);
            return id;
        }

        public void AddId(string slotId, string accessoryKey, string accessoryId)
        {
            if(!pool.ContainsKey(slotId)) 
                pool[slotId] = new Dictionary<string, string>();

            pool[slotId][accessoryKey] = accessoryId;
        }
    }
}
