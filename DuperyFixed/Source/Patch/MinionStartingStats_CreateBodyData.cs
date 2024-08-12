using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Dupery
{
    [HarmonyPatch(typeof(MinionStartingStats))]
    [HarmonyPatch("CreateBodyData")]
    internal class MinionStartingStats_CreateBodyData
    {
        [HarmonyPostfix]
        static void Postfix(ref KCompBuilder.BodyData __result, Personality p)
        {
            HashedString hairId = FindNewId(Db.Get().AccessorySlots.Hair, p.nameStringKey);
            if (hairId != null)
                __result.hair = hairId;
        }

        private static HashedString FindNewId(AccessorySlot slot, string duplicantId)
        {
            string id = DuperyPatches.PersonalityManager.FindOwnedAccessory(duplicantId, slot.Id);
            if (id != null)
                return HashCache.Get().Add(id);
            else
                return null;
        }
    }
}
