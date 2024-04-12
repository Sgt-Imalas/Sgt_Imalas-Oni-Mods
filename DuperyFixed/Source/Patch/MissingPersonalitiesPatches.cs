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
                    SgtLogger.l("Personality of "+__instance.gameObject.name+" is missing, defaulting to jorge for ui keys");
                    personality = Db.Get().Personalities.GetPersonalityFromNameStringKey("JORGE");
                    identity.personalityResourceId = personality.Id;
                    identity.nameStringKey = LonelyMinionConfig.PERSONALITY_ID;
                   // SwitchToBackupBodyParts(__instance, personality);
                }
            }
            public static void Postfix(Accessorizer __instance)
            {
                //PurgeDuplicateEntries(__instance);
            }
            static void PurgeDuplicateEntries(Accessorizer __instance)
            {
                HashSet<string> addedParts = new HashSet<string>();
                SgtLogger.l("Checking for duplicate symbols for:" + __instance.gameObject.name);
                for (int i = __instance.accessories.Count - 1; i >= 0; i--)
                {
                    var item = __instance.accessories[i].Get();
                    if (item == null)
                    {
                        SgtLogger.l("removing missing accessory");
                        __instance.accessories.RemoveAt(i);
                    }
                    else
                    {
                        if (addedParts.Contains(item.Id))
                        {
                            SgtLogger.l("removing duplicate accessory: "+item.Name+ " in "+item.slot.Name);
                            __instance.accessories.RemoveAt(i);
                        }
                        else
                        {
                            SgtLogger.l("accessory: " + item.Name + " in " + item.slot.Name);
                            addedParts.Add(item.Id);
                        }
                    }
                }
            }
            static void SwitchToBackupBodyParts(Accessorizer __instance, Personality fallback)
            {
                
                __instance.accessories.RemoveAll(x => x.Get() == null);
                KCompBuilder.BodyData result = MinionStartingStats.CreateBodyData(fallback);
                {
                    var slots = Db.Get().AccessorySlots;
                    string[] possibleMissings =
                    {
                        slots.Eyes.Id,
                        slots.Hair.Id,
                        slots.HatHair.Id,
                        slots.HeadShape.Id,
                        slots.Body.Id

                    };
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
                        if(!possibleMissings.Contains(resource.Id))
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

                        __instance.accessories.RemoveAll(a => a.Get()!=null && a.Get().slot == accessory.slot);
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

