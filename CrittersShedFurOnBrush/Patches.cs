using HarmonyLib;
using Klei.AI;
using System;
using UnityEngine;


namespace CrittersShedFurOnBrush
{
	internal class Patches
	{
		[HarmonyPatch(typeof(BasicFabricConfig), "CreatePrefab")]
		public static class AddColourInfoToReeds
		{
			public static void Postfix(GameObject __result)
			{
				__result.AddComponent<FloofColourHolder>();
			}
		}
		//[HarmonyPatch(typeof(BasicFabricConfig), "OnSpawn")]
		//public static class ApplyFloofInfo
		//{
		//    public static void Postfix(GameObject inst)
		//    {
		//        if(inst.TryGetComponent<FloofColourHolder>(out var floofy))
		//        {
		//            inst.SetActive(false);
		//            floofy.ApplyStyleChanges();
		//            inst.SetActive(true);
		//        }
		//    }
		//}


		[HarmonyPatch(typeof(RanchStationConfig), "DoPostConfigureComplete")]
		public static class AddSheddingToGroomStation
		{
			public static void Postfix(GameObject go)
			{
				RanchStation.Def def = go.GetDef<RanchStation.Def>();
				def.OnRanchCompleteCb = (creature_go, _) =>
				{
					RanchStation.Instance targetRanchStation = creature_go.GetSMI<RanchableMonitor.Instance>().TargetRanchStation;
					RancherChore.RancherChoreStates.Instance smi = targetRanchStation.GetSMI<RancherChore.RancherChoreStates.Instance>();
					double RancherSkillEffect = (double)targetRanchStation.GetSMI<RancherChore.RancherChoreStates.Instance>().sm.rancher.Get(smi).GetAttributes().Get(Db.Get().Attributes.Ranching.Id).GetTotalValue() * 0.1;
					float num = (float)(1.0f + RancherSkillEffect);
					float rancherAmountEffect = 1f + (float)Math.Pow(RancherSkillEffect, 1.25f);
					creature_go.GetComponent<Effects>().Add("Ranched", true).timeRemaining *= num;
					if (IsSheddableCreature(creature_go, out var shedAmount, out Tag CreatureTag))
					{
						YeetWool(creature_go, shedAmount * rancherAmountEffect, CreatureTag);
					}
				};
			}

			static void YeetWool(GameObject originGo, float amount, Tag CreatureTag)
			{
				PrimaryElement component1 = originGo.GetComponent<PrimaryElement>();
				GameObject go = Util.KInstantiate(Assets.GetPrefab(BasicFabricConfig.ID));
				go.AddOrGet<FloofColourHolder>().SetCritterTagAndGORef(CreatureTag, originGo);


				go.transform.SetPosition(Grid.CellToPosCCC(Grid.PosToCell(originGo), Grid.SceneLayer.Ore));
				PrimaryElement component2 = go.GetComponent<PrimaryElement>();
				component2.Temperature = component1.Temperature;
				component2.Units = amount;
				go.SetActive(true);


				Vector2 initial_velocity = new Vector2(UnityEngine.Random.Range(-1f, 1f) * 1f, (float)((double)UnityEngine.Random.value * 2.0 + 4.0));
				if (GameComps.Fallers.Has((object)go))
					GameComps.Fallers.Remove(go);
				GameComps.Fallers.Add(go, initial_velocity);
			}

			static bool IsSheddableCreature(GameObject go, out float shedValue, out Tag CreatureTag)
			{
				shedValue = 0f;
				CreatureTag = null;
				if (go.TryGetComponent<KPrefabID>(out var kPrefabID))
				{
					//Debug.Log(kPrefabID.PrefabTag);
					if (ModAssets.SheddableCritters.ContainsKey(kPrefabID.PrefabTag))
					{
						shedValue = ModAssets.SheddableCritters[kPrefabID.PrefabTag].FloofPerCycle;
						CreatureTag = kPrefabID.PrefabTag;
						return true;
					}
				}
				return false;
			}
		}
	}
}
