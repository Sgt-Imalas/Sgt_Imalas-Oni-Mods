using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace Dupery
{
    [HarmonyPatch(typeof(MinionStartingStats))]
    [HarmonyPatch("CreateBodyData")]
    internal class MinionStartingStats_CreateBodyData
    {
        [HarmonyPostfix]
        static void Postfix(ref KCompBuilder.BodyData __result, Personality p)
        {


            HashedString bodyId = FindNewId(Db.Get().AccessorySlots.Hair, p.nameStringKey);
            if (bodyId != null)
            {
                SgtLogger.l("overriding body: " + __result.body + " -> " + bodyId);
                __result.body = bodyId;
            }
            HashedString hairId = FindNewId(Db.Get().AccessorySlots.Hair, p.nameStringKey);
            if (hairId != null)
            {
                SgtLogger.l("overriding hair: " + __result.hair + " -> " + hairId);
                __result.hair = hairId;
            }


            HashedString headId = FindNewId(Db.Get().AccessorySlots.HeadShape, p.nameStringKey);
            if (headId != null)
            {
                UtilMethods.ListAllFieldValues(__result);
                SgtLogger.l("overriding body: " + __result.headShape + " -> " + headId);
                __result.headShape = headId;
            }

            HashedString eyesId = FindNewId(Db.Get().AccessorySlots.Eyes, p.nameStringKey);
            if (eyesId != null)
                __result.eyes = eyesId;

            HashedString mouthId = FindNewId(Db.Get().AccessorySlots.Mouth, p.nameStringKey);
            if (mouthId != null)
                __result.mouth = mouthId;
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
