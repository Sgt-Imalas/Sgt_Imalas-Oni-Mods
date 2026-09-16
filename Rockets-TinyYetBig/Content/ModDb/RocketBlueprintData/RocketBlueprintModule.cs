using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rockets_TinyYetBig.Content.ModDb.ModIntegrations;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig.Content.ModDb.RocketBlueprintData
{
	[Serializable]
	internal class RocketBlueprintModule
	{
		public string ID;
		public string Name;
		public int width, height;
		public List<string> SelectedElements = [];
		public string Facade = "";
		[JsonIgnore]
		public bool Valid;
		[JsonIgnore]
		public BuildingDef def;

		public Dictionary<string, JObject> AdditionalBuildingData = null;

		public RocketBlueprintModule() { }
		public RocketBlueprintModule(Building building)
		{
			this.def = building.Def;
			this.ID = building.Def.PrefabID;

			DetermineMaterials(building);
			RefreshValidity();
			AdditionalBuildingData = BlueprintsV2.GetAllAdditionalBuildingData(building.gameObject);
		}
		void DetermineMaterials(Building building)
		{
			if (building.TryGetComponent<Constructable>(out var constructable))
			{
				SelectedElements = constructable.SelectedElementsTags.Select(t => t.Name).ToList();
			}
			else if (building.TryGetComponent<Deconstructable>(out var deconstructable))
			{
				SelectedElements = deconstructable.constructionElements.Select(t => t.Name).ToList();
			}
			else
			{
				SelectedElements = building.Def.DefaultElements().Select(t => t.Name).ToList();
			}
			if (building.TryGetComponent<BuildingFacade>(out var facade) && !facade.IsOriginal)
				Facade = facade.CurrentFacade;
		}

		internal static RocketBlueprintModule From(Building moduleBuilding)
		{
			return new RocketBlueprintModule(moduleBuilding);
		}

		public void RefreshValidity()
		{
			def = Assets.GetBuildingDef(ID);
			if (def == null)
			{
				Valid = false;
				return;
			}

			Valid = true;
			width = def.WidthInCells;
			height = def.HeightInCells;
			Name = def.Name;

			SanitizeSelectedTags();
		}


		void SanitizeSelectedTags()
		{
			bool logd = false;

			if (SelectedElements.Count > def.MaterialCategory.Length)
			{
				SgtLogger.l(ID + " has more selected materials than ingredients. Trimming...");
				while (SelectedElements.Count > def.MaterialCategory.Length)
				{
					SelectedElements.RemoveAt(SelectedElements.Count - 1);
				}
			}

			for (int i = 0; i < def.MaterialCategory.Length; i++)
			{
				var ingredientStep = def.MaterialCategory[i];
				Tag selectedTag = SelectedElements.Count > i ? SelectedElements[i] : Tag.Invalid;
				var validMaterials = MaterialSelector.GetValidMaterials(ingredientStep);

				if (!validMaterials.Contains(selectedTag))
				{
					if (!logd)
					{
						logd = true;
					}

					var element = ElementLoader.FindElementByHash((SimHashes)selectedTag.hash);
					if (element != null)
						selectedTag = element.tag;
					var mat = validMaterials.FirstOrDefault();

					SgtLogger.l(ID + " has invalid material " + selectedTag + " for ingredient " + ingredientStep + ". replacing with default: " + mat);

					if (SelectedElements.Count > i)
						SelectedElements[i] = mat.Name;
					else
						SelectedElements.Add(mat.Name);

				}
			}
		}

		public bool CanConstructModule(GameObject launchPad, out string reason)
		{
			reason = "invalid";
			if (!Valid)
				return false;

			if (!def.BuildingComplete.TryGetComponent<ReorderableBuilding>(out var reorderable))
				return false;

			bool conditionsValid = true;
			reason = string.Empty;
			foreach (var buildCondition in reorderable.buildConditions)
			{
				if (buildCondition.IgnoreInSanboxMode() && SandboxEnabled() || buildCondition.EvaluateCondition(launchPad, def, SelectModuleCondition.SelectionContext.AddModuleAbove))
					continue;

				conditionsValid = false;
				if (!string.IsNullOrEmpty(reason))
					reason += "\n";
				reason += buildCondition.GetStatusTooltip(false, launchPad, def);
			}
			return conditionsValid;
		}
		bool SandboxEnabled() => DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive;
	}
}
