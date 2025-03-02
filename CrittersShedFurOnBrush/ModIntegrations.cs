using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace CrittersShedFurOnBrush
{
	internal class ModIntegrations
	{
		//https://steamcommunity.com/sharedfiles/filedetails/?id=3329512249
		public static class UpgradableCritters_Integration
		{
			private static bool _initialized = false;
			public static bool Initialized => _initialized;
			public static Type _critterUpgradeTracker;

			public static void Init()
			{
				//UtilMethods.ListAllTypesWithAssemblies();

				SgtLogger.l("Initializing UpgradableCritters integration");
				_critterUpgradeTracker = Type.GetType("Upgradeable_Dupes_And_Critters.CritterUpgradeTracker, Upgradeable_Dupes_And_Critters", false, false);
				if (_critterUpgradeTracker == null)
				{
					SgtLogger.l("Upgradeable_Dupes_And_Critters.CritterUpgradeTracker not found, UpgradableCritters_Integration going to sleep. zzzzz");
					return;
				}

				var m_GetUpgradeLevel = AccessTools.Method(_critterUpgradeTracker, "GetUpgradeLevel");

				if (m_GetUpgradeLevel == null)
				{
					Debug.LogWarning("Method `GetUpgradeLevel` not found on type CritterUpgradeTracker, aborting...");
					return;
				}

				//var UpgradeGetter = InjectionMethods.CreateGetter<type.GetType(),int>(m_Upgrades);

				//todo: add proper delegate if stuffydoll adds a getter method
				SgtLogger.l("Upgradable critters integration initialized!");
				_initialized = true;
			}
			public static int GetCritterUpgradeMultiplier(GameObject critter)
			{
				if (!Initialized)
					return 1;

				var upgradeCmp = critter.GetComponent(_critterUpgradeTracker);
				if (upgradeCmp == null)
					return 1;

				var _upgradesValue = (int?)Traverse.Create(upgradeCmp)?.Method("GetUpgradeLevel")?.GetValue();

				if (_upgradesValue == null)
					return 1;

				return 1 + _upgradesValue.Value;

			}

		}
	}
}
