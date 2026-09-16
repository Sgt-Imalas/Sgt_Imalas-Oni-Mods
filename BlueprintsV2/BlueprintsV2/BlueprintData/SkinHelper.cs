using HarmonyLib;
using Newtonsoft.Json.Linq;
using System.Linq;
using UnityEngine;
using UtilLibs;

namespace BlueprintsV2.BlueprintData
{
	internal class SkinHelper
	{
		//Akis Backwalls
		internal static void TryApplyBackwall(GameObject arg1, JObject arg2)
		{
			if (!arg1.TryGetComponentMod("Backwall", out var backwallCmp))
				return;

			string colorHex = arg2.GetValue("colorHex").Value<string>();
			string pattern = arg2.GetValue("pattern").Value<string>();

			//GameScheduler.Instance.ScheduleNextFrame("backwall pattern",(_)=>
			Traverse.Create(backwallCmp).Method("TrySetPattern", new[] { typeof(string) }).GetValue(pattern);
			//GameScheduler.Instance.ScheduleNextFrame("backwall color",(_)=>
			Traverse.Create(backwallCmp).Method("SetColor", new[] { typeof(string) }).GetValue(colorHex);
			Traverse.Create(backwallCmp).Field("copiedColor").SetValue(true);
		}

		internal static JObject TryStoreBackwall(GameObject arg)
		{
			if (!arg.TryGetComponentMod("Backwall", out var backwallCmp))
				return null;

			var settingsStruct = Traverse.Create(backwallCmp).Field("settings").GetValue();
			var colorHex = Traverse.Create(settingsStruct).Field("colorHex").GetValue() as string;
			var pattern = Traverse.Create(settingsStruct).Field("pattern").GetValue() as string;

			//SgtLogger.l($"Pattern: {pattern}, colorHex: {colorHex}");
			var data = new JObject() {
				{"colorHex", colorHex},
				{"pattern", pattern}
			};

			return data;
		}

		//Akis DecorPackI moodlamp
		internal static void TryApplyMoodLamp(GameObject arg1, JObject arg2)
		{
			if (!arg1.TryGetComponentMod("MoodLamp", out var moodLampCmp))
				return;
			string currentVariantID = arg2.GetValue("currentVariantID")?.Value<string>();

			if (currentVariantID != null)
				Traverse.Create(moodLampCmp).Method("SetVariant", [typeof(string)]).GetValue(currentVariantID);

			string colorHex = arg2.GetValue("colorHex")?.Value<string>();

			if (colorHex != null && arg1.TryGetComponentMod("TintableLamp", out var tintableLampCmp))
			{
				var color = Util.ColorFromHex(colorHex);
				Traverse.Create(tintableLampCmp).Method("SetColor", [typeof(Color)]).GetValue(color);
			}

		}

		internal static JObject TryStoreMoodLamp(GameObject arg)
		{
			JObject data = null;
			string currentVariantID = null;
			string colorHex = null;

			if (arg.TryGetComponentMod("MoodLamp", out var moodLampCmp))
			{
				currentVariantID = Traverse.Create(moodLampCmp).Field("currentVariantID").GetValue() as string;
			}
			if (arg.TryGetComponentMod("TintableLamp", out var tintableLampCmp))
			{
				colorHex = Traverse.Create(tintableLampCmp).Field("colorHex").GetValue() as string;
			}

			if (currentVariantID != null)
			{
				data = new JObject()
				{
					{"currentVariantID", currentVariantID}
				};

				if (colorHex != null)
					data.Add("colorHex", colorHex);
			}


			return data;
		}

		//Artable (painting, statue)
		internal static JObject TryStoreArtableSkin(GameObject arg)
		{
			string skinId = string.Empty;
			if (arg.TryGetComponent<Artable>(out var artable))
			{
				if (!string.IsNullOrEmpty(artable.userChosenTargetStage))
				{
					skinId = artable.userChosenTargetStage;
				}
				else if (!string.IsNullOrEmpty(artable.CurrentStage))
				{
					skinId = artable.CurrentStage;
				}
				else
					skinId = "Default";
			}
			if (!ValidArtableId(skinId, arg))
			{
				return null;
			}
			return new JObject()
			{
				{ "CurrentStage", skinId }
			};
		}
		public static void TryApplyArtableSkin(GameObject building, JObject facadeObj)
		{
			if (facadeObj == null)
				return;

			var token = facadeObj.SelectToken("CurrentStage");
			if (token == null)
				return;

			string facadeID = token.Value<string>();

			if (building.TryGetComponent<Artable>(out var sculpture))
			{
				if (ValidArtableId(facadeID, building))
				{
					if (BlueprintState.InstantBuild)
					{
						sculpture.SetStage(facadeID, true);
						sculpture.userChosenTargetStage = null;
					}
					else
					{
						sculpture.chore?.Cancel("blueprint applied");
						sculpture.SetUserChosenTargetState(facadeID);
					}
				}
			}
		}
		//Facade (building skin)
		internal static JObject TryStoreBuildingSkin(GameObject arg)
		{
			string skinId = string.Empty;
			if (arg.TryGetComponent<BuildingFacade>(out var buildingFacade) && !buildingFacade.IsOriginal)
			{
				skinId = buildingFacade.CurrentFacade;
			}
			if (!ValidFacadeId(skinId, arg))
			{
				return null;
			}
			return new JObject()
			{
				{ "CurrentFacade", skinId }
			};
		}
		public static void TryApplyBuildingSkin(GameObject building, JObject facadeObj)
		{
			if (facadeObj == null)
				return;

			var token = facadeObj.SelectToken("CurrentFacade");
			if (token == null)
				return;

			string facadeID = token.Value<string>();

			if (building.TryGetComponent<BuildingFacade>(out var buildingFacade))
			{
				if (ValidFacadeId(facadeID, building))
				{
					buildingFacade.ApplyBuildingFacade(Db.GetBuildingFacades().Get(facadeID));
					if (building.TryGetComponentMod("FacadeRestorer", out _)
						&& building.TryGetComponent<KBatchedAnimController>(out var kbac)) {
						//refresh the anim on akis facade restorer aero pods
						SgtLogger.l("fixing decor pack aero pod");
						kbac.Play("off");
					}
				}
			}
		}
		static bool ValidFacadeId(string facadeID, GameObject buildingGO)
		{
			if (!buildingGO.TryGetComponent<Building>(out var building)) return false;

			return !facadeID.IsNullOrWhiteSpace() && facadeID != "DEFAULT_FACADE"
				&& Db.GetBuildingFacades().TryGet(facadeID) != null
				&& Db.GetBuildingFacades().TryGet(facadeID).PrefabID == building.Def.PrefabID
				&& Db.GetBuildingFacades().Get(facadeID).IsUnlocked();
		}
		static bool ValidArtableId(string artableID, GameObject buildingGO)
		{
			if (!buildingGO.TryGetComponent<Building>(out var building)) return false;

			var validStagesForBuilding = Db.GetArtableStages().GetPrefabStages(building.Def.PrefabID);

			return
				!artableID.IsNullOrWhiteSpace()
				&& artableID != "Default"
				&& Db.GetArtableStages().TryGet(artableID) != null
				&& Db.GetArtableStages().Get(artableID).IsUnlocked()
				&& validStagesForBuilding.Any(skin => skin.Id == artableID);
		}
	}
}
