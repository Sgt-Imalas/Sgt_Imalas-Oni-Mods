using Database;
using FMOD;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Assertions.Must;
using UtilLibs;
using static HoverTextDrawer;
using static KCompBuilder;
using static STRINGS.DUPLICANTS;
using static STRINGS.NAMEGEN;

namespace DuperyFixed.Source.Patch
{
    internal class MissingPersonalitiesPatches
    {

        [HarmonyPatch(typeof(Accessorizer), nameof(Accessorizer.OnDeserialized))]
        public class Accessorizer_OnDeserialized_Patch
        {
            public static void Prefix(Accessorizer __instance)
            {
                if (__instance.TryGetComponent<MinionIdentity>(out var identity))
                {
                    var personality = Db.Get().Personalities.TryGet(identity.personalityResourceId);
                    if (personality != null)
                    {
                        return;
                    }
                    SgtLogger.l("Personality of "+__instance.gameObject.name+" is missing, defaulting to jorge. for ui keys");
                    personality = Db.Get().Personalities.GetPersonalityFromNameStringKey("JORGE");
                    identity.personalityResourceId = personality.Id;
                    identity.nameStringKey = LonelyMinionConfig.PERSONALITY_ID;
                    SwitchToBackupBodyParts(__instance, personality);
                    //SwitchToBackupBodyParts__instance.bodyData = Accessorizer.UpdateAccessorySlots(identity.nameStringKey, ref __instance.accessories);           
                    //__instance.accessories.RemoveAll((ResourceRef<Accessory> x) => x.Get() == null);
                    //SwitchToBackupBodyParts(__instance, personality);
                }
            }
            static void SwitchToBackupBodyParts(Accessorizer __instance, Personality fallback)
            {
                //for (int i = __instance.accessories.Count - 1; i >= 0; i--)
                //{
                //    if (__instance.accessories[i].Get() == null)
                //    {
                //        SgtLogger.l("removing missing accessory");
                //    }
                //}
                __instance.accessories.RemoveAll(x => x.Get() == null);
                KCompBuilder.BodyData result = MinionStartingStats.CreateBodyData(fallback);
                {
                    foreach (AccessorySlot resource in Db.Get().AccessorySlots.resources)
                    {
                        if (resource.accessories.Count == 0)
                        {
                            SgtLogger.warning(resource.Name + " had not valid resource");
                            continue;
                        }
                        if (__instance.accessories.Any(accessory => accessory.Get().slot == resource))
                        {
                            continue;
                        }


                        var accessory = resource.accessories.GetRandom();
                        if (accessory == null)
                        {
                            SgtLogger.warning(resource.Name + " had not valid resource");
                        }
                        ResourceRef<Accessory> item = new ResourceRef<Accessory>(accessory);
                        SgtLogger.l(resource.Name + " was not found, adding " + item.Get().Name);
                        __instance.accessories.Add(item);
                    }
                }

                __instance.bodyData = result;
            }

        }

        private static void Switch(Accessorizer __instance, AccessorySlot slot, HashedString id)
        {
            if (__instance.GetAccessory(slot) == null)
                __instance.AddAccessory(slot.Lookup(id));
        }
    }

}

