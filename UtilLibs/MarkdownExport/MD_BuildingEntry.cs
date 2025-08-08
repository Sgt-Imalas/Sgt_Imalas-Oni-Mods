using ClipperLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs.BuildingPortUtils;
using static ResearchTypes;

namespace UtilLibs.MarkdownExport
{
	public class MD_BuildingEntry : IMD_Entry
	{
		string ID;
		public string Name;
		public string Description;
		public string Effect;
		public int PowerConsumption;
		public int PowerProduction;
		public int Width, Height;
		public string Research;
		public int StorageCapacity;
		private BuildingDef def;

		public List<IMD_Entry> Children;
		public List<Tuple<string, int>> Costs = [];

		static StringBuilder sb = new StringBuilder();
		public string FormatAsMarkdown()
		{
			sb.Clear();
			sb.AppendLine($"## {Name}");
			sb.AppendLine(Description);
			sb.AppendLine();
			sb.AppendLine(Effect);
			sb.AppendLine("### Info");
			//sb.AppendLine("|Parameter|Value|");
			sb.AppendLine($"| <img width=\"200\"src=\"../../images/buildings/{ID}.png\"> | |");
			//sb.Append(Description.Replace("\n","<br/>"));
			//sb.Append("<br/>");
			//sb.Append(Effect.Replace("\n", "<br/>"));
			//sb.AppendLine("|");
			sb.AppendLine("|-|-|");
			sb.AppendLine($"|**Dimensions:** |{Width} wide x {Height} high|");
			if (PowerConsumption > 0)
				sb.AppendLine($"|**Power Consumption:**| {PowerConsumption} W|");
			if (PowerProduction > 0)
				sb.AppendLine($"|**Power Generation:**| {PowerProduction} W|");
			if (!Research.IsNullOrWhiteSpace())
				sb.AppendLine($"|**Research Required:**| {Research}|");
			if (StorageCapacity > 0)
				sb.AppendLine($"|**Storage Capacity:**| {GameUtil.GetFormattedMass(StorageCapacity)}|");

			AppendMaterialCosts(sb);
			AppendBuildingPorts(sb);
			sb.AppendLine();

			foreach (var child in Children)
			{
				sb.AppendLine(child.FormatAsMarkdown());
			}

			return sb.ToString();

		}

		private void AppendBuildingPorts(StringBuilder sb)
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
					string mat = consumption == GameTags.Any ? null : MarkdownUtil.GetTagName(consumption);
					inputs.Add(MarkdownUtil.GetPortDescription(def.InputConduitType, true, mat));
				}
			}
			var modConsumers = def.BuildingComplete.GetComponents<PortConduitConsumer>();
			if (modConsumers.Any())
			{
				foreach (var consumer2 in modConsumers)
				{
					string mat = consumer2.capacityTag == GameTags.Any ? null : MarkdownUtil.GetTagName(consumer2.capacityTag);
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

					string mat = filter == GameTags.Any ? null : MarkdownUtil.GetTagName(filter);
					outputs.Add(MarkdownUtil.GetPortDescription(dispenser.conduitType, false, mat));
				}
			}


			if (!inputs.Any() && !outputs.Any())
				return;

			sb.AppendLine();
			sb.AppendLine("### Building Ports");
			sb.AppendLine("|Inputs:|Outputs:|");
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

		private void AppendMaterialCosts(StringBuilder sb)
		{
			sb.AppendLine();
			sb.AppendLine("|**<font size=\"+1\">Material Costs:</font>**| |");
			sb.AppendLine("|-|-|");
			foreach (var mat in Costs)
			{
				sb.Append('|');
				var cost = mat.second;
				string[] mats = mat.first.Split('&');

				sb.Append(string.Join(" or ", mats.Select(m => MarkdownUtil.GetTagName(m))));
				sb.Append('|');
				sb.Append(GameUtil.GetFormattedMass(cost));
				sb.AppendLine("|");
			}
		}

		public MD_BuildingEntry WriteUISprite(string path)
		{
			var kanim = def.AnimFiles.First();
			if (kanim == null) return this;

			var UISprite = Def.GetUISpriteFromMultiObjectAnim(kanim);

			if (UISprite != null && UISprite != Assets.GetSprite("unknown"))
			{
				MarkdownUtil.WriteUISpriteToFile(UISprite, path, ID);
			}
			return this;
		}

		public MD_BuildingEntry(string id)
		{
			ID = id;
			Name = STRINGS.UI.StripLinkFormatting(Strings.Get($"STRINGS.BUILDINGS.PREFABS.{id.ToUpperInvariant()}.NAME").ToString());
			Description = STRINGS.UI.StripLinkFormatting(Strings.Get($"STRINGS.BUILDINGS.PREFABS.{id.ToUpperInvariant()}.DESC").ToString());
			Effect = STRINGS.UI.StripLinkFormatting(Strings.Get($"STRINGS.BUILDINGS.PREFABS.{id.ToUpperInvariant()}.EFFECT").ToString());
			CollectInfo(id);
		}
		public MD_BuildingEntry Tech(string techId)
		{
			Research = STRINGS.UI.StripLinkFormatting(Strings.Get($"STRINGS.RESEARCH.TECHS.{techId.ToUpperInvariant()}.NAME").ToString());
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
				Children.Add(new MD_Header("Generator Conversion", 3));
				Children.Add(new MD_EnergyGenerator(generator));
			}


			var converters = buildingDef.BuildingComplete.GetComponents<ElementConverter>();

			if (converters != null && converters.Any())
			{
				Children.Add(new MD_Header("Element Conversion", 3));

				foreach (var converter in converters)
				{
					Children.Add(new MD_ElementConverter(converter));
				}
			}

			if (buildingDef.BuildingComplete.GetComponent<ComplexFabricator>() != null)
			{
				Children.Add(new MD_Header("Recipes", 3));
				var recipes = ComplexRecipeManager.Get().recipes.FindAll(recipe => recipe.fabricators[0] == id);
				Children.Add(new MD_ComplexRecipes(recipes));
			}
			if((buildingDef.BuildingComplete.TryGetComponent<SmartReservoir>(out _) || buildingDef.BuildingComplete.TryGetComponent<TreeFilterable>(out _))
				&& buildingDef.BuildingComplete.TryGetComponent<Storage>(out var storage) && storage.showInUI)
			{
				StorageCapacity = Mathf.RoundToInt(storage.Capacity());
			}
			else if(buildingDef.BuildingComplete.TryGetComponent<StorageLocker>(out var storageLocker) )
			{
				StorageCapacity = Mathf.RoundToInt(storageLocker.MaxCapacity);
			}

		}
	}
}
