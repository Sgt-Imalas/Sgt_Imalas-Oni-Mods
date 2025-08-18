using ClipperLib;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs.BuildingPortUtils;
using static LogicGate.LogicGateDescriptions;
using static ResearchTypes;
using static STRINGS.DUPLICANTS.PERSONALITIES;
using static UtilLibs.MarkdownExport.MarkdownUtil;
using static UtilLibs.MarkdownExport.MD_Localization;

namespace UtilLibs.MarkdownExport
{
	public class MD_BuildingEntry : IMD_Entry
	{
		public bool IsVanillaModified = false;
		string ID;
		public int PowerConsumption;
		public int PowerProduction;
		public int Width, Height;
		public string ResearchKey;
		public int StorageCapacity;
		private BuildingDef def;

		public List<IMD_Entry> Children;
		public List<Tuple<string, int>> Costs = [];
		const string EmptyFiller = "&#8288 {: style=\"padding:0\"}";
		static StringBuilder sb = new StringBuilder();

		static string FontSizeIncrease(string text, int increase = 1)
		{
			return $"<font size=\"+{increase}\">{text}</font>";
		}
		public string FormatAsMarkdown()
		{
			sb.Clear();
			sb.Append($"## {Strip(L($"STRINGS.BUILDINGS.PREFABS.{ID.ToUpperInvariant()}.NAME"))}");
			if (IsVanillaModified)
			{
				sb.Append(" ");
				sb.Append(L("MODIFIED_SUFFIX"));
			}
			sb.AppendLine();

			sb.AppendLine(Strip(L($"STRINGS.BUILDINGS.PREFABS.{ID.ToUpperInvariant()}.DESC")));
			sb.AppendLine();
			sb.AppendLine(Strip(L($"STRINGS.BUILDINGS.PREFABS.{ID.ToUpperInvariant()}.EFFECT")));
			sb.AppendLine();
			sb.AppendLine("| | | |");
			sb.AppendLine("|-|-|-|");

			sb.Append($"| ![{ID}](/assets/images/buildings/{ID}.png){{height=\"100\"}} {{rowspan=\"3\"}}");
			sb.AppendLine($"|**{L("BUILDING_DIMENSIONS_LABEL")}** | {string.Format(L("BUILDING_DIMENSIONS_INFO"), Width, Height)}|");
			if (PowerProduction > 0)
				sb.AppendLine($"|**{L("BUILDING_POWER_GENERATION")}**| {PowerProduction} W|{EmptyFiller}|");
			else
				sb.AppendLine($"|**{L("BUILDING_POWER_CONSUMPTION")}**| {PowerConsumption} W|{EmptyFiller}|");
			if (!ResearchKey.IsNullOrWhiteSpace())
				sb.AppendLine($"|**{L("BUILDING_RESEARCH_REQUIREMENT")}**| {Strip(L(ResearchKey))}|{EmptyFiller}| ");
			else
				sb.AppendLine($"|**{L("BUILDING_RESEARCH_REQUIREMENT")}**| - |{EmptyFiller}| ");

			AppendMaterialCostsTable(sb);

			if (StorageCapacity > 0)
				sb.AppendLine($"|**{FontSizeIncrease(L("BUILDING_STORAGE_CAPACITY"))}**| {GameUtil.GetFormattedMass(StorageCapacity)}|{EmptyFiller}|");

			AppendBuildingPortsTable(sb);
			sb.AppendLine();

			foreach (var child in Children)
			{
				sb.AppendLine(child.FormatAsMarkdown());
			}

			return sb.ToString();

		}

		void AppendBuildingPortsTable(StringBuilder sb) => AppendBuildingPorts(sb, true);
		private void AppendBuildingPorts(StringBuilder sb, bool htmlTable = false)
		{
			List<string> inputs = [], outputs = [];


			if (def.InputConduitType != ConduitType.None)
			{
				Tag consumption = GameTags.Any;
				bool anyConsumer = false;
				if (def.BuildingComplete.TryGetComponent<ConduitConsumer>(out var consumer))
				{
					consumption = consumer.capacityTag;
					anyConsumer = true;
				}
				else if (def.BuildingComplete.TryGetComponent<SolidConduitConsumer>(out var solidconsumer))
				{
					consumption = solidconsumer.capacityTag;
					anyConsumer = true;
				}

				if (anyConsumer)
				{
					string mat = consumption == GameTags.Any ? null : MarkdownUtil.GetTagString(consumption);
					inputs.Add(MarkdownUtil.GetPortDescription(def.InputConduitType, true, mat));
				}
			}
			var modConsumers = def.BuildingComplete.GetComponents<PortConduitConsumer>();
			if (modConsumers.Any())
			{
				foreach (var consumer2 in modConsumers)
				{
					string mat = consumer2.capacityTag == GameTags.Any ? null : MarkdownUtil.GetTagString(consumer2.capacityTag);
					inputs.Add(MarkdownUtil.GetPortDescription(consumer2.conduitType, true, mat));
				}
			}

			if (def.OutputConduitType != ConduitType.None)
			{
				outputs.Add(MarkdownUtil.GetPortDescription(def.OutputConduitType, false));
			}
			var modDispensers = def.BuildingComplete.GetComponents<PortConduitDispenserBase>();
			if (modDispensers.Any())
			{
				foreach (var dispenser in modDispensers)
				{
					Tag filter = GameTags.Any;
					if (dispenser.elementFilter != null && dispenser.elementFilter.Any())
						filter = dispenser.elementFilter[0].CreateTag();
					else if (dispenser.tagFilter != null && dispenser.tagFilter.Any())
						filter = dispenser.tagFilter[0];

					string mat = filter == GameTags.Any ? null : MarkdownUtil.GetTagString(filter);
					outputs.Add(MarkdownUtil.GetPortDescription(dispenser.conduitType, false, mat));
				}
			}


			if (!inputs.Any() && !outputs.Any())
				return;
			if (!htmlTable)
			{
				sb.AppendLine();
				sb.AppendLine($"### {L("BUILDING_PORTS_HEADER")}");
				sb.AppendLine($"|{L("INPUTS_HEADER")}|{L("OUTPUTS_HEADER")}|");
				sb.AppendLine("|-|-|");

				int max = Math.Max(inputs.Count, outputs.Count);
				for (int i = 0; i < max; i++)
				{
					string input = inputs.Count > i ? inputs[i] : "-";
					string output = outputs.Count > i ? outputs[i] : "-";
					sb.Append('|');
					sb.Append(input);
					sb.Append('|');
					sb.Append(output);
					sb.AppendLine("|");
				}
			}
			else { 
				sb.Append($"| **<font size=\"+1\">{L("BUILDING_PORTS_HEADER")}:</font>** |");
				sb.Append("<table>");
				sb.Append("<tr>");
				sb.Append($"<th>{L("INPUTS_HEADER")}</th>");
				sb.Append($"<th>{L("OUTPUTS_HEADER")}</th>");
				sb.Append("</tr>");
				int max = Math.Max(inputs.Count, outputs.Count);
				for (int i = 0; i < max; i++)
				{
					sb.Append("<tr>");
					sb.Append("<td>");
					sb.Append(inputs.Count > i ? inputs[i] : "-");
					sb.Append("</td>");
					sb.Append("<td>");
					sb.Append(outputs.Count > i ? outputs[i] : "-");
					sb.Append("</td>");
					sb.Append("</tr>");
				}
				sb.Append("</table>");
				sb.Append(" {colspan=\"2\"}");
				sb.Append('|');
				sb.Append(EmptyFiller);
				sb.AppendLine("|");
			}
		}
		private void AppendMaterialCostsTable(StringBuilder sb)
		{
			sb.Append($"|**<font size=\"+1\">{L("BUILDING_MATERIAL_COST_HEADER")}</font>**|");
			sb.Append("<table>");
			foreach (var mat in Costs)
			{
				sb.Append("<tr>");
				var cost = mat.second;
				string[] mats = mat.first.Split('&');

				string or = " " + L("SEPARATOR_OR") + " ";
				sb.Append("<td>");
				sb.Append(string.Join(or, mats.Select(m => MarkdownUtil.GetTagString(m))));
				sb.Append("</td>");
				sb.Append("<td>");
				sb.Append(GameUtil.GetFormattedMass(cost));
				sb.Append("</td>");
				sb.Append("</tr>");
			}
			sb.Append("</table>");
			sb.Append(" {colspan=\"2\"} |");
			sb.Append(EmptyFiller);
			sb.AppendLine("|");

		}
		private void AppendMaterialCosts(StringBuilder sb)
		{
			sb.AppendLine();
			sb.AppendLine($"|**<font size=\"+1\">{L("BUILDING_MATERIAL_COST_HEADER")}</font>**| |");
			sb.AppendLine("|-|-|");
			foreach (var mat in Costs)
			{
				sb.Append('|');
				var cost = mat.second;
				string[] mats = mat.first.Split('&');

				string or = " " + L("SEPARATOR_OR") + " ";
				sb.Append(string.Join(or, mats.Select(m => MarkdownUtil.GetTagString(m))));
				sb.Append('|');
				sb.Append(GameUtil.GetFormattedMass(cost));
				sb.AppendLine("|");
			}
		}
		public MD_BuildingEntry WriteUISprite(string path)
		{
			KAnimFile kanim = def.AnimFiles.First();
			Exporter.WriteUISprite(path, ID, kanim);
			return this;
		}

		public MD_BuildingEntry(string id)
		{
			ID = id;
			CollectInfo(id);
		}
		public MD_BuildingEntry Tech(string techId)
		{
			ResearchKey = $"STRINGS.RESEARCH.TECHS.{techId.ToUpperInvariant()}.NAME";
			return this;
		}
		void CollectInfo(string id)
		{
			var buildingDef = Assets.GetBuildingDef(id);
			def = buildingDef;
			if (buildingDef == null)
			{
				SgtLogger.error("No BuildingDef found for " + id);
				return;
			}
			PowerConsumption = Mathf.RoundToInt(buildingDef.EnergyConsumptionWhenActive);
			PowerProduction = Mathf.RoundToInt(buildingDef.GeneratorWattageRating);

			Width = buildingDef.WidthInCells;
			Height = buildingDef.HeightInCells;
			Children = new List<IMD_Entry>();
			//foreach (var child in buildingDef.GetComponents<MD_BuildingEntry>())
			//	Children.Add(child);
			for (int i = 0; i < buildingDef.MaterialCategory.Length; i++)
			{
				string material = buildingDef.MaterialCategory[i];
				float amount = buildingDef.Mass[i];
				Costs.Add(new(material, (int)amount));
			}

			if (buildingDef.BuildingComplete.TryGetComponent<EnergyGenerator>(out var generator))
			{
				Children.Add(new MD_Header("CONVERSION_GENERATOR_HEADER", 4));
				Children.Add(new MD_EnergyGenerator(generator));
			}
			foreach(var techItem in Db.Get().TechItems.resources)
			{
				if (techItem.Id==(id))
				{
					ResearchKey = $"STRINGS.RESEARCH.TECHS.{techItem.ParentTech.Id.ToUpperInvariant()}.NAME";
					break;
				}
			}


			var converters = buildingDef.BuildingComplete.GetComponents<ElementConverter>();

			if (converters != null && converters.Any())
			{
				Children.Add(new MD_Header("CONVERSION_ELEMENT_HEADER", 4));

				foreach (var converter in converters)
				{
					Children.Add(new MD_ElementConverter(converter));
				}
			}

			if (buildingDef.BuildingComplete.GetComponent<ComplexFabricator>() != null)
			{
				Children.Add(new MD_Header("RECIPES_HEADER", 3));
				var recipes = ComplexRecipeManager.Get().recipes.FindAll(recipe => recipe.fabricators[0] == id);
				Children.Add(new MD_ComplexRecipes(recipes));
			}
			if ((buildingDef.BuildingComplete.TryGetComponent<SmartReservoir>(out _) || buildingDef.BuildingComplete.TryGetComponent<TreeFilterable>(out _))
				&& buildingDef.BuildingComplete.TryGetComponent<Storage>(out var storage) && storage.showInUI)
			{
				StorageCapacity = Mathf.RoundToInt(storage.Capacity());
			}
			else if (buildingDef.BuildingComplete.TryGetComponent<StorageLocker>(out var storageLocker))
			{
				StorageCapacity = Mathf.RoundToInt(storageLocker.MaxCapacity);
			}

			if (SupplyClosetUtils.TryGetCollectionFor(id, out var collection))
			{
				Children.Add(new MD_Header("STRINGS.UI.UISIDESCREENS.TABS.SKIN", 3));
				foreach (var item in collection)
					Children.Add(new MD_BlueprintEntry(item));
			}

		}

		public MD_BuildingEntry VanillaModified()
		{
			IsVanillaModified = true;
			return this;
		}
	}
}
