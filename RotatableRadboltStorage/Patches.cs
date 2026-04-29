using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UtilLibs;
using static LogicGateBase;
using static STRINGS.LORE.BUILDINGS;


namespace RotatableRadboltStorage
{
	internal class Patches
	{

		/// <summary>
		/// add destination selection sidescreen to rad battery
		/// </summary>
		[HarmonyPatch(typeof(HighEnergyParticleDirectionSideScreen))]
		[HarmonyPatch(nameof(HighEnergyParticleDirectionSideScreen.IsValidForTarget))]
		public static class AddTargetValidityForRadBattery
		{
			public static bool Prefix(GameObject target, ref bool __result)
			{
				if (target.TryGetComponent<BatteryDirectionAddon>(out var targetComponent))
				{
					__result = true;
					return false;
				}
				return true;
			}
		}

		static void MakeFirstInputPortDirectionRibbon(BuildingDef def)//, HashedString portID, CellOffset offset, string desc, string desc_on, string desc_off)
		{
			LogicPorts.Port port;
			if (def.LogicInputPorts == null)
				def.LogicInputPorts = [];
			if (!def.LogicInputPorts.Any())
			{
				port = LogicPorts.Port.RibbonInputPort(LogicOperationalController.PORT_ID, new(0, 0), STRINGS.UI.LOGIC_PORTS.CONTROL_OPERATIONAL, STRINGS.UI.LOGIC_PORTS.CONTROL_OPERATIONAL_ACTIVE, STRINGS.UI.LOGIC_PORTS.CONTROL_OPERATIONAL_INACTIVE);
			}
			else
				port = def.LogicInputPorts.FirstOrDefault();
			port.spriteType = LogicPortSpriteType.RibbonInput;
			port.activeDescription += "\n";
			port.activeDescription += MOD_STRINGS.STATUSITEMS.DIRECTION_ADDON;
			if(def.LogicInputPorts.Any())
				def.LogicInputPorts.RemoveAt(0);
			def.LogicInputPorts.Insert(0, port);

			//def.LogicInputPorts = [LogicPorts.Port.RibbonInputPort(portID, offset, desc, desc_on, desc_off)];

		}


		[HarmonyPatch(typeof(HEPBattery.Instance), nameof(HEPBattery.Instance.OnLogicValueChanged))]
		public class HEPBattery_Instance_OnLogicValueChanged_Patch
		{
			public static bool Prefix(HEPBattery.Instance __instance, object data)
			{
				LogicValueChanged logicValueChanged = (LogicValueChanged)data;
				if (!(logicValueChanged.portID == HEPBattery.FIRE_PORT_ID))
					return false;
				__instance.isLogicActive = LogicCircuitNetwork.IsBitActive(0, logicValueChanged.newValue);
				__instance.hasLogicWire = __instance.GetNetwork() != null;
				return false;
			}
		}


		[HarmonyPatch(typeof(HEPBattery), nameof(HEPBattery.Fire))]
		public static class TargetCellForRadBattery
		{
			public static bool Prefix(HEPBattery.Instance smi)
			{
				if (smi.gameObject.TryGetComponent<BatteryDirectionAddon>(out var DirectionAddon))
				{
					int particleOutputCell = DirectionAddon.GetCircularHEPOutputCell();
					GameObject radboltGO = GameUtil.KInstantiate(Assets.GetPrefab((Tag)"HighEnergyParticle"), Grid.CellToPosCCC(particleOutputCell, Grid.SceneLayer.FXFront2), Grid.SceneLayer.FXFront2);
					radboltGO.SetActive(true);
					if (radboltGO == null)
						return false;
					HighEnergyParticle component = radboltGO.GetComponent<HighEnergyParticle>();
					component.payload = smi.particleStorage.ConsumeAndGet(smi.particleThreshold);
					component.SetDirection(DirectionAddon.Direction);

					return false;
				}
				return true;
			}
		}

		static void AddThresholdLogicPortDesc(BuildingDef def)
		{
			var fullnesOutput = def.LogicOutputPorts.First();
			fullnesOutput.description = STRINGS.BUILDINGS.PREFABS.SMARTRESERVOIR.LOGIC_PORT;
			fullnesOutput.activeDescription = STRINGS.BUILDINGS.PREFABS.SMARTRESERVOIR.LOGIC_PORT_ACTIVE;
			fullnesOutput.inactiveDescription = STRINGS.BUILDINGS.PREFABS.SMARTRESERVOIR.LOGIC_PORT_INACTIVE;

			def.LogicOutputPorts[0] = fullnesOutput;
		}


		static void AddThresholdLogic(GameObject gameObject) => gameObject.AddOrGet<HEPStorageThreshold>();
		#region Railgun
		[HarmonyPatch(typeof(RailGunConfig), nameof(RailGunConfig.ConfigureBuildingTemplate))]
		public class RailGunConfig_ConfigureBuildingTemplate_Patch
		{
			public static void Postfix(GameObject go)
			{
				AddThresholdLogic(go);
			}
		}

		[HarmonyPatch(typeof(RailGunConfig), nameof(RailGunConfig.CreateBuildingDef))]
		public class RailGunConfig_CreateBuildingDef_Patch
		{
			public static void Postfix(ref BuildingDef __result)
			{
				AddThresholdLogicPortDesc(__result);
			}
		}
		#endregion
		#region NuclearResearchCenter
		[HarmonyPatch(typeof(NuclearResearchCenterConfig), nameof(NuclearResearchCenterConfig.ConfigureBuildingTemplate))]
		public class NuclearResearchCenterConfig_ConfigureBuildingTemplate_Patch
		{
			public static void Postfix(GameObject go)
			{
				AddThresholdLogic(go);
			}
		}

		[HarmonyPatch(typeof(NuclearResearchCenterConfig), nameof(NuclearResearchCenterConfig.CreateBuildingDef))]
		public class NuclearResearchCenterConfig_CreateBuildingDef_Patch
		{
			public static void Postfix(ref BuildingDef __result)
			{
				AddThresholdLogicPortDesc(__result);
			}
		}
		#endregion

		#region DiamondPress
		[HarmonyPatch(typeof(DiamondPressConfig), nameof(DiamondPressConfig.ConfigureBuildingTemplate))]
		public class DiamondPressConfig_ConfigureBuildingTemplate_Patch
		{
			public static void Postfix(GameObject go)
			{
				AddThresholdLogic(go);
			}
		}

		[HarmonyPatch(typeof(DiamondPressConfig), nameof(DiamondPressConfig.CreateBuildingDef))]
		public class DiamondPressConfig_CreateBuildingDef_Patch
		{
			public static void Postfix(ref BuildingDef __result)
			{
				AddThresholdLogicPortDesc(__result);
			}
		}
		#endregion

		#region HEPBattery
		[HarmonyPatch(typeof(HEPBatteryConfig))]
		[HarmonyPatch(nameof(HEPBatteryConfig.ConfigureBuildingTemplate))]
		public static class AdjustNormalHEPBatteryTwo
		{
			public static void Postfix(GameObject go)
			{
				go.AddOrGet<BatteryDirectionAddon>().OuputOffByOne = true;
				go.AddOrGet<LogicHEPDirectionController>();
				AddThresholdLogic(go);
			}
		}
		[HarmonyPatch(typeof(HEPBatteryConfig))]
		[HarmonyPatch(nameof(HEPBatteryConfig.CreateBuildingDef))]
		public static class AdjustNormalHEPBattery
		{
			public static void Postfix(ref BuildingDef __result)
			{
				__result.HighEnergyParticleOutputOffset = new CellOffset(0, 1);
				__result.AnimFiles = new KAnimFile[1]
				{
					Assets.GetAnim((HashedString) "radbolt_battery_rotatable_kanim")
				};

				MakeFirstInputPortDirectionRibbon(__result);
				AddThresholdLogicPortDesc(__result);
			}
		}
		#endregion

		[HarmonyPatch(typeof(HighEnergyParticleRedirectorConfig), nameof(HighEnergyParticleRedirectorConfig.CreateBuildingDef))]
		public class HighEnergyParticleRedirectorConfig_CreateBuildingDef_Patch
		{
			public static void Postfix(ref BuildingDef __result)
			{
				MakeFirstInputPortDirectionRibbon(__result);
			}
		}

		[HarmonyPatch(typeof(HighEnergyParticleRedirector), nameof(HighEnergyParticleRedirector.OnLogicValueChanged))]
		public class HighEnergyParticleRedirector_OnLogicValueChanged_Patch
		{
			public static void Postfix(HighEnergyParticleRedirector __instance, object data)
			{
				LogicValueChanged logicValueChanged = (LogicValueChanged)data;
				SgtLogger.l("HEPPORT: " + (logicValueChanged.portID == HighEnergyParticleRedirector.PORT_ID)+": " + __instance.isLogicActive + " <- " + logicValueChanged.newValue);
				if (logicValueChanged.portID == HighEnergyParticleRedirector.PORT_ID)
				{
					__instance.isLogicActive = LogicCircuitNetwork.IsBitActive(0, logicValueChanged.newValue);
				}
			}
		}


		/// <summary>
		/// Init. auto translation
		/// </summary>
		[HarmonyPatch(typeof(Localization), "Initialize")]
		public static class Localization_Initialize_Patch
		{
			public static void Postfix()
			{
				LocalisationUtil.Translate(typeof(MOD_STRINGS), true);
			}
		}

		[HarmonyPatch(typeof(HighEnergyParticleSpawnerConfig), nameof(HighEnergyParticleSpawnerConfig.CreateBuildingDef))]
		public class HighEnergyParticleSpawnerConfig_CreateBuildingDef_Patch
		{
			public static void Postfix(ref BuildingDef __result)
			{
				MakeFirstInputPortDirectionRibbon(__result);

			}
		}
		[HarmonyPatch]
		public static class AddLogicDirectionControl
		{
			[HarmonyPrefix]
			public static void Prefix(GameObject go)
			{
				go.AddOrGet<LogicHEPDirectionController>();
			}
			[HarmonyTargetMethods]
			internal static IEnumerable<MethodBase> TargetMethods()
			{
				const string name = nameof(IBuildingConfig.ConfigureBuildingTemplate);
				yield return typeof(HighEnergyParticleRedirectorConfig).GetMethod(name);
				yield return typeof(HighEnergyParticleSpawnerConfig).GetMethod(name);
			}
		}



		[HarmonyPatch(typeof(HighEnergyParticleStorage), nameof(HighEnergyParticleStorage.UpdateLogicPorts))]
		public static class ReplaceNormalPortLogic
		{
			public static bool Prefix(HighEnergyParticleStorage __instance)
			{
				if (__instance.TryGetComponent<HEPStorageThreshold>(out var t))
				{
					return false;
				}
				return true;
			}
		}
	}
}
