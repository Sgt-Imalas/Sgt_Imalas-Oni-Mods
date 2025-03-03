using Imalas_TwitchChaosEvents.Elements;
using System.Collections.Generic;
using UnityEngine;

namespace Imalas_TwitchChaosEvents.Fire
{
	internal class FireManager : KMonoBehaviour, ISim4000ms
	{
		public float[] ignitionStatesSerialize;
		public bool[] activeFires;

		public static FireManager Instance;

		public float ignitionThreshold = 120f;

		public float coolingPerSecond = 20f;


		public override void OnSpawn()
		{
			base.OnSpawn();
			if (ignitionStatesSerialize == null)
			{
				ignitionStatesSerialize = new float[Grid.CellCount];

				for (int i = 0; i < ignitionStatesSerialize.Length; i++)
				{
					ignitionStatesSerialize[i] = 0f;
				}
			}
			if (activeFires == null)
			{
				activeFires = new bool[Grid.CellCount];

				for (int i = 0; i < activeFires.Length; i++)
				{
					activeFires[i] = false;
				}
			}
		}

		Dictionary<SimHashes, float> ElementMultiplier = new Dictionary<SimHashes, float>()
		{
			{ModElements.Creeper.SimHash, 2f},
			{ModElements.CreeperGas.SimHash, 2f},
			{SimHashes.Algae, 2f},
			{SimHashes.SlimeMold, 2f},
			{SimHashes.Carbon, 3f},
			{SimHashes.WoodLog, 4f},
			{SimHashes.RefinedCarbon, 3f},
			{SimHashes.Dirt, 1.5f},
			{SimHashes.Oxygen, 1.5f},
			{SimHashes.Petroleum, 5f},
			{SimHashes.Ethanol, 5f},
			{SimHashes.CrudeOil, 5f},
			{SimHashes.Syngas,5f},
			{SimHashes.Methane, 5f},
			{SimHashes.Hydrogen, 5f},
			{SimHashes.CarbonDioxide, 0f},
			{SimHashes.Helium, 0f},
			{SimHashes.Vacuum, 0f},
		};
		float GetElementMultiplier(Element element)
		{
			if (ElementMultiplier.ContainsKey(element.substance.elementID))
			{
				return ElementMultiplier[element.substance.elementID];
			}
			if (element.IsLiquid)
			{
				return 0.1f;
			}
			if (element.IsGas)
			{
				return 0.25f;
			}

			return 1f;
		}
		public float GetElementMultiplier(int cell) => GetElementMultiplier(Grid.Element[cell]);


		public void ApplyIgnitionHeatToCell(int cell, float heat)
		{
			var element = Grid.Element[cell];

			heat = heat > 0 ? Mathf.Max(0, heat * GetElementMultiplier(element)) : Mathf.Min(0, heat * GetElementMultiplier(element));


			ignitionStatesSerialize[cell] += heat;

			var currentIgnitionForce = ignitionStatesSerialize[cell];
			if (currentIgnitionForce > ignitionThreshold && !activeFires[cell])
			{
				SpawnFire(cell);
			}

		}

		public void SpawnFire(int cell)
		{
			var spawningPosition = Grid.CellToPos(cell);
			spawningPosition.z = Grid.GetLayerZ(Grid.SceneLayer.FXFront2);
			var fire = Util.KInstantiate(ModAssets.FireSpawner, spawningPosition);
			fire.AddOrGet<KPrefabID>().InstanceID = cell;
			var fireCmp = fire.AddComponent<ActiveFire>();


			fire.SetActive(true);


			activeFires[cell] = true;
			//ignitionStatesSerialize[cell] = 0f;

		}



		public void Sim4000ms(float dt)
		{
			for (int i = 0; i < ignitionStatesSerialize.Length; i++)
			{
				if (ignitionStatesSerialize[i] > 0)
				{
					ignitionStatesSerialize[i] = ignitionStatesSerialize[i] < coolingPerSecond * dt ? 0 : ignitionStatesSerialize[i] - coolingPerSecond * dt;
				}
			}
		}

		internal void RemoveFire(int cell)
		{
			ignitionStatesSerialize[cell] = 0;
			activeFires[cell] = false;
		}
	}
	public class OverloadedFire
	{

		//[HarmonyPatch(typeof(BuildingHP), nameof(BuildingHP.OnDoBuildingDamage))]
		//public class SaveGame_OnPrefabInit_Patch
		//{
		//    public static void Postfix(BuildingHP __instance, object data)
		//    {
		//        if(data != null)
		//        {
		//            var converted = (DamageSourceInfo)data;
		//            if(converted.takeDamageEffect == SpawnFXHashes.BuildingSpark)
		//            {
		//                var cell = Grid.PosToCell(__instance);
		//                FireManager.Instance.ApplyIgnitionHeatToCell(cell, FireManager.Instance.ignitionThreshold*10f);
		//            }
		//        }
		//    }
		//}
	}
}
