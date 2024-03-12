using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace DuperyFixed.Source.Patch
{
    internal class MissingPersonalitiesPatches
    {

        [HarmonyPatch(typeof(Accessorizer), "OnDeserialized")]
        public class Accessorizer_OnDeserialized_Patch
        {
            public static void Prefix(Accessorizer __instance)
            {
                if (__instance.TryGetComponent<MinionIdentity>(out var identity))
                {
                    var personality = Db.Get().Personalities.TryGet(identity.personalityResourceId);
                    if(personality != null)
                    {
                        return;
                    }
                    SgtLogger.l("Personality missing, defaulting to jorge...");
                    personality = Db.Get().Personalities.GetPersonalityFromNameStringKey(LonelyMinionConfig.PERSONALITY_ID);
                    identity.personalityResourceId = personality.Id;
                    identity.nameStringKey = null;

                    __instance.bodyData = MinionStartingStats.CreateBodyData(personality);

                    //__instance.accessories.RemoveAll(x => x.Get() == null);

                    //var accessorySlots = Db.Get().AccessorySlots;
                    //Switch(__instance, accessorySlots.HeadShape, __instance.bodyData.headShape);
                    //Switch(__instance, accessorySlots.Mouth, __instance.bodyData.mouth);
                    //Switch(__instance, accessorySlots.Eyes, __instance.bodyData.eyes);
                    //Switch(__instance, accessorySlots.Body, __instance.bodyData.body);
                    //Switch(__instance, accessorySlots.Arm, __instance.bodyData.arms);
                    //Switch(__instance, accessorySlots.ArmLower, __instance.bodyData.armslower);
                    //Switch(__instance, accessorySlots.ArmLowerSkin, __instance.bodyData.armLowerSkin);
                    //Switch(__instance, accessorySlots.ArmUpperSkin, __instance.bodyData.armUpperSkin);
                    //Switch(__instance, accessorySlots.Leg, __instance.bodyData.legs);
                    //Switch(__instance, accessorySlots.LegSkin, __instance.bodyData.legSkin);
                    //Switch(__instance, accessorySlots.Pelvis, __instance.bodyData.pelvis);
                    //Switch(__instance, accessorySlots.Cuff, __instance.bodyData.cuff);
                    //Switch(__instance, accessorySlots.Hand, __instance.bodyData.hand);
                    //Switch(__instance, accessorySlots.Belt, __instance.bodyData.belt);
                    //Switch(__instance, accessorySlots.Neck, __instance.bodyData.neck);
                }
            }

            private static void Switch(Accessorizer __instance, AccessorySlot slot, HashedString id)
            {
                if (__instance.GetAccessory(slot) == null)
                    __instance.AddAccessory(slot.Lookup(id));
            }
        }

    }
}
