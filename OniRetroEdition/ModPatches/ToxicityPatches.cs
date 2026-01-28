using ElementUtilNamespace;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UtilLibs;
using static OverlayLegend;

namespace OniRetroEdition.ModPatches
{
	internal class ToxicityPatches
	{
		public static Color32[] ToxicityColors = new Color32[2]
		{
			UIUtils.rgb(206, 135, 29), //Slightly Toxic
            UIUtils.rgb(227, 228, 94)  //Very Toxic
        };
		[HarmonyPatch(typeof(OverlayLegend), "OnSpawn")]
		public static class OverlayLegend_OnSpawn
		{
			public static void Prefix(List<OverlayLegend.OverlayInfo> ___overlayInfoList)
			{
				var oxygenOverlay = ___overlayInfoList
					.Find(info => info.mode == OverlayModes.Oxygen.ID);
				if (oxygenOverlay == null)
				{
					SgtLogger.error("oxygen overlay not found!");
					return;
				}

				Sprite icon = oxygenOverlay.infoUnits[0].icon;
				var data = oxygenOverlay.infoUnits[0].formatData;
				List<OverlayInfoUnit> toxicityValues = new List<OverlayInfoUnit>()
				{
					new OverlayInfoUnit(icon,"STRINGS.UI.RETRO_OVERLAY.TOXICITY.SLIGHTLYTOXIC", ToxicityColors[0], Color.white, data)
					{
						tooltip = "STRINGS.UI.OVERLAYS.OXYGEN.TOOLTIPS.LEGEND5"
					},
					new OverlayInfoUnit(icon,"STRINGS.UI.RETRO_OVERLAY.TOXICITY.VERYYTOXIC" ,ToxicityColors[1],Color.white,data){
						tooltip = "STRINGS.UI.OVERLAYS.OXYGEN.TOOLTIPS.LEGEND6"
					},
				};
				oxygenOverlay.infoUnits.AddRange(toxicityValues);
			}
		}

		[HarmonyPatch(typeof(SimDebugView), nameof(SimDebugView.GetOxygenMapColour))]
		public static class OxygenOverlay_Add_ToxicityColor
		{
			public static void Postfix(SimDebugView instance, int cell, ref Color __result)
			{
				if (__result == instance.unbreathableColour && Grid.Element[cell].toxicity > 1f)
				{
					float t = Mathf.Clamp((Grid.Pressure[cell] - instance.minPressureExpected) / (instance.maxPressureExpected - (instance.minPressureExpected)), 0.0f, 1f);
					__result = Color.Lerp(ToxicityColors[0], ToxicityColors[1], t);
				}
			}
		}
		[HarmonyPatch(typeof(ElementLoader), "Load")]
		public static class Patch_ElementLoader_Load
		{
			public static List<SimHashes> ToxicElements = new List<SimHashes>()
			{
				SimHashes.Hydrogen,
				SimHashes.ChlorineGas,
				SimHashes.EthanolGas,
                //SimHashes.Methane,
                SimHashes.SourGas
			};

			public static void Postfix()
			{
				GasLiquidExposureMonitor.InitializeCustomRates();
				foreach (var element in ElementLoader.elements)
				{
					if (element.id == SimHashes.Void || element.id == SimHashes.Vacuum)
						continue;

					element.disabled = false;
					if (element.oreTags != null)
					{
						var list = element.oreTags.ToList();
						list.Remove(GameTags.HideFromSpawnTool);
						list.Remove(GameTags.HideFromCodex);
						element.oreTags = list.ToArray();
					}

					if (!GasLiquidExposureMonitor.customExposureRates.TryGetValue(element.id, out var multiplier))
					{
						multiplier = 1f;
					}
					if (multiplier > 0)
					{
						element.toxicity += 1.1f;
					}
				}


				//var metalMaterial = ElementLoader.GetElement(SimHashes.Steel.CreateTag()).substance.material;

				// fix lead specular
				//var lead = ElementLoader.FindElementByHash(SimHashes.Lead);
				//lead.substance.material.SetTexture("_ShineMask", AssetUtils.LoadTexture("lead_mask_fixed", texturePath));


				var aluminium = ElementLoader.GetElement(SimHashes.Aluminum.CreateTag()).substance.material;
				SgtElementUtil.SetTexture_Main(aluminium, "aluminium_retro");
				SgtElementUtil.SetTexture_ShineMask(aluminium, "aluminium_retro_ShineMask");

				var co2Solid = ElementLoader.GetElement(SimHashes.SolidCarbonDioxide.CreateTag()).substance.material;
				SgtElementUtil.SetTexture_Main(co2Solid, "solid_carbon_dioxide_retro");


				var depletedU = ElementLoader.GetElement(SimHashes.DepletedUranium.CreateTag()).substance.material;
				SgtElementUtil.SetTexture_Main(depletedU, "depleted_uranium_retro");

				var enrichedU = ElementLoader.GetElement(SimHashes.EnrichedUranium.CreateTag()).substance.material;
				SgtElementUtil.SetTexture_Main(enrichedU, "enriched_uranium_retro");

				var ironORe = ElementLoader.GetElement(SimHashes.IronOre.CreateTag()).substance.material;
				SgtElementUtil.SetTexture_Main(ironORe, Config.Instance.IronOreTexture == Config.EarlierVersion.Beta ? "hematite_(t)_retro" : "hematite_(alpha)_retro");
				if (Config.Instance.IronOreTexture == Config.EarlierVersion.Alpha)
					SgtElementUtil.SetTexture_ShineMask(ironORe, "hematite_(alpha)_retro_ShineMask");
				else
					SgtElementUtil.SetTexture_ShineMask(ironORe, "hematite_(t)_retro_ShineMask.png");


				var bleachstone = ElementLoader.GetElement(SimHashes.BleachStone.CreateTag()).substance.material;
				SgtElementUtil.SetTexture_Main(bleachstone, "bleach_stone_retro");

				var radEle = ElementLoader.GetElement(SimHashes.Radium.CreateTag()).substance.material;
				SgtElementUtil.SetTexture_Main(radEle, "radium_retro");
			}

		}

	}
}
