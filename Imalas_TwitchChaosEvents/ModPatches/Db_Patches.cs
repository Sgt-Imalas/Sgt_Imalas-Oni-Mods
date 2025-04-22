using Database;
using HarmonyLib;
using Imalas_TwitchChaosEvents.Meteors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imalas_TwitchChaosEvents.ModPatches
{
	internal class Db_Patches
	{
		[HarmonyPatch(typeof(Db), "Initialize")]
		public static class Db_Initialize_Patch
		{
			public static void Postfix(Db __instance)
			{
				Util_TwitchIntegrationLib.EventRegistration.InitializeTwitchEventsInNameSpace("Imalas_TwitchChaosEvents.Events");
				TacoMeteorPatches.Register(__instance.GameplayEvents);
				RegisterTacoAsMeat(__instance);
				ModAssets.WaterCoolerDrinks.Register(__instance);

				ModAssets.StatusItems.CreateStatusItems();
			}
			public static void RegisterTacoAsMeat(Db __instance)
			{
				var items = __instance.ColonyAchievements.EatkCalFromMeatByCycle100.requirementChecklist;
				foreach (var requirement in items)
				{
					if (requirement is EatXCaloriesFromY foodRequirement)
					{
						foodRequirement.fromFoodType.Add(TacoConfig.ID);
						break;
					}
				}
			}
		}
	}
}
