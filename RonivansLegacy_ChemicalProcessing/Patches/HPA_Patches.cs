using HarmonyLib;
using Klei.AI;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
    class HPA_Patches
    {

		static void IncreaseRocketConduitTarget(RocketConduitStorageAccess cmp)
        {
			float tweakedCapacity;

			switch (cmp.cargoType)
            {
                case CargoBay.CargoType.Liquids:
                    tweakedCapacity = Config.Instance.HPA_Capacity_Liquid;
					cmp.storage.capacityKg = tweakedCapacity;
					if(cmp.targetLevel > 0)
						cmp.targetLevel = tweakedCapacity;
					break;
                case CargoBay.CargoType.Gasses:
					tweakedCapacity = Config.Instance.HPA_Capacity_Gas;
					cmp.storage.capacityKg = tweakedCapacity;
					if (cmp.targetLevel > 0)
						cmp.targetLevel = tweakedCapacity;
					break;
			}
        }
		public static void IncreaseConsumerInput(ConduitConsumer __instance, bool increaseStorage)
		{
			if (__instance.conduitType == ConduitType.Gas)
			{
				float flowRate = Config.Instance.HPA_Capacity_Gas / 1;
				if (increaseStorage)
					__instance.Storage.capacityKg *= flowRate;
				__instance.consumptionRate *= flowRate;
			}
			else if (__instance.conduitType == ConduitType.Liquid)
			{
				float flowRate = Config.Instance.HPA_Capacity_Liquid / 10;
				if (increaseStorage)
					__instance.Storage.capacityKg *= flowRate;
				__instance.consumptionRate *= flowRate;
			}
		}


		[HarmonyPatch(typeof(LiquidConditionerConfig), nameof(LiquidConditionerConfig.ConfigureBuildingTemplate))]
		public class LiquidConditionerConfig_ConfigureBuildingTemplate_Patch
		{
			public static void Postfix(GameObject go)
			{
				IncreaseConsumerInput(go.GetComponent<ConduitConsumer>(),true);
			}
		}

		[HarmonyPatch(typeof(AirConditionerConfig), nameof(AirConditionerConfig.ConfigureBuildingTemplate))]
		public class AirConditionerConfig_ConfigureBuildingTemplate_Patch
		{
			public static void Postfix(GameObject go)
			{
				IncreaseConsumerInput(go.GetComponent<ConduitConsumer>(), true);
			}
		}

		[HarmonyPatch(typeof(RocketConduitStorageAccess), nameof(RocketConduitStorageAccess.OnSpawn))]
		public class RocketConduitStorageAccess_OnSpawn_Patch
		{
			public static void Postfix(RocketConduitStorageAccess __instance)
			{
				IncreaseRocketConduitTarget(__instance);
			}
		}

		[HarmonyPatch(typeof(ValveBase), nameof(ValveBase.OnSpawn))]
        public class ValveBase_OnSpawn_Patch
        {
            public static void Postfix(ValveBase __instance)
            {
				if (__instance.conduitType == ConduitType.Gas)
				{
					float flowRate = Config.Instance.HPA_Capacity_Gas;
					__instance.maxFlow *= flowRate;
				}
				else if (__instance.conduitType == ConduitType.Liquid)
				{
					float flowRate = Config.Instance.HPA_Capacity_Liquid / 10;
					__instance.maxFlow *= flowRate;
				}
			}
        }

		[HarmonyPatch(typeof(WarpConduitSender), nameof(WarpConduitSender.OnSpawn))]
		public class WarpConduitSender_OnSpawn_Patch
		{
			public static void Prefix(WarpConduitSender __instance)
			{
				__instance.gasStorage.capacityKg *= Config.Instance.HPA_Capacity_Gas;
				__instance.liquidStorage.capacityKg *= Config.Instance.HPA_Capacity_Liquid / 10;
			}
		}


		[HarmonyPatch(typeof(Game), nameof(Game.OnLoadLevel))]
		public class Game_OnLoadLevel_Patch
		{
			public static void Postfix()
			{
				HPA_Util.ClearStaticInfo();
			}
		}
	}
}
