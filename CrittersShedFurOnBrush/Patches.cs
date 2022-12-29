using Database;
using HarmonyLib;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static CrittersShedFurOnBrush.ModAssets;
using static Grid;
using static KAnim;

namespace CrittersShedFurOnBrush
{
    internal class Patches
    {
        [HarmonyPatch(typeof(RanchStationConfig), "DoPostConfigureComplete")]
        public static class AddSheddingToGroomStation
        {
            public static void Postfix(GameObject go)
            {
                RanchStation.Def def = go.GetDef<RanchStation.Def>();
                def.OnRanchCompleteCb = (System.Action<GameObject>)(creature_go =>
                {
                    RanchStation.Instance targetRanchStation = creature_go.GetSMI<RanchableMonitor.Instance>().TargetRanchStation;
                    RancherChore.RancherChoreStates.Instance smi = targetRanchStation.GetSMI<RancherChore.RancherChoreStates.Instance>();
                    double RancherSkillEffect = (double)targetRanchStation.GetSMI<RancherChore.RancherChoreStates.Instance>().sm.rancher.Get(smi).GetAttributes().Get(Db.Get().Attributes.Ranching.Id).GetTotalValue() * 0.1;
                    float num = (float)(1.0f + RancherSkillEffect);
                    float rancherAmountEffect = 1f + (float)Math.Pow(RancherSkillEffect, 1.25f);
                    creature_go.GetComponent<Effects>().Add("Ranched", true).timeRemaining *= num;
                    if(IsSheddableCreature(creature_go,out var shedAmount))
                    {
                        YeetWool(creature_go, shedAmount* rancherAmountEffect);
                    }
                });
            }

            static void YeetWool(GameObject originGo, float amount)
            {
                PrimaryElement component1 = originGo.GetComponent<PrimaryElement>();
                GameObject go = Util.KInstantiate(Assets.GetPrefab(BasicFabricConfig.ID));

                KBatchedAnimController kBatchedAnimController = go.AddOrGet<KBatchedAnimController>();
                kBatchedAnimController.AnimFiles = new KAnimFile[1] { Assets.GetAnim((HashedString)"bomblet_nuclear_kanim") };
                kBatchedAnimController.initialAnim = "object";

                go.transform.SetPosition(Grid.CellToPosCCC(Grid.PosToCell(originGo), Grid.SceneLayer.Ore));
                PrimaryElement component2 = go.GetComponent<PrimaryElement>();
                component2.Temperature = component1.Temperature;
                component2.Units = amount;
                //component2.AddDisease(component1.DiseaseIdx, component1.DiseaseCount, "Shearing");
                go.SetActive(true);
                Vector2 initial_velocity = new Vector2(UnityEngine.Random.Range(-1f, 1f) * 1f, (float)((double)UnityEngine.Random.value * 2.0 + 4.0));
                if (GameComps.Fallers.Has((object)go))
                    GameComps.Fallers.Remove(go);
                GameComps.Fallers.Add(go, initial_velocity);
            }

            static bool IsSheddableCreature(GameObject go, out float shedValue)
            {
                shedValue = 0f;
                if (go.TryGetComponent<KPrefabID>(out var kPrefabID))
                {
                    Debug.Log(kPrefabID.PrefabTag);
                    if (ModAssets.SheddableCritters.ContainsKey(kPrefabID.PrefabTag))
                    {
                        shedValue = ModAssets.SheddableCritters[kPrefabID.PrefabTag];
                        return true;
                    }
                }
                return false;
            }
        }
    }
}
