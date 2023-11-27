using HarmonyLib;
using Imalas_TwitchChaosEvents.Creeper;
using Imalas_TwitchChaosEvents.Fire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Imalas_TwitchChaosEvents.Attachments
{
    internal class AttachmentPatches
    {
        [HarmonyPatch(typeof(Health), nameof(Health.OnSpawn))]
        public class HealthDamageHandler_Addon_Patch
        {
            public static void Postfix(Health __instance)
            {
               __instance.gameObject.AddOrGet<Health_DamageHander>();
            }

        }

        [HarmonyPatch(typeof(MinionConfig), nameof(MinionConfig.OnSpawn))]
        public class Minion_AddFlipper_Patch
        {
            public static void Postfix(GameObject go)
            {
                go.AddOrGet<Minion_Flipper>();
            }

        }
        public class SaveGamePatch
        {
            [HarmonyPatch(typeof(SaveGame), "OnPrefabInit")]
            public class SaveGame_OnPrefabInit_Patch
            {
                public static void Postfix(SaveGame __instance)
                {
                    __instance.gameObject.AddOrGet<ChaosTwitch_SaveGameStorage>();
                    CreeperController.instance = __instance.gameObject.AddOrGet<CreeperController>();
                    FireManager.Instance = __instance.gameObject.AddOrGet<FireManager>();
                }
            }

            //[HarmonyPatch(typeof(VirtualInputModule), "SetCursor")]
            //public class CursorHp
            //{
            //    public static void Postfix(VirtualInputModule __instance)
            //    {
            //        if (__instance.m_VirtualCursor == null)
            //            return;
            //        var hp = Util.KInstantiateUI(ModAssets.CursorHP, __instance.m_VirtualCursor.gameObject, true);
            //        hp.AddComponent<CursorHP>();
            //        hp.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 10, 10);
            //    }
            //}
        }
    }
}
