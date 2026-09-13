using KSerialization;
using Newtonsoft;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using UtilLibs;
using static MaterialSelectionPanel;

namespace Rockets_TinyYetBig.Content.ModDb.RocketBlueprintData
{
	[Serializable]
	internal class RocketBlueprint
	{
		[JsonIgnore]
		static StringBuilder sb = new StringBuilder();
		public string FriendlyName;
		public List<RocketBlueprintModule> RocketModules = [];

		public RocketBlueprint() { }
		public void RefreshValidity()
		{
			foreach(var module in  RocketModules) 
				module.RefreshValidity();
		}

		public string GetDescription()
		{
			sb.Clear();
			sb.AppendLine(UIUtils.EmboldenText(FriendlyName));
			sb.AppendLine();
			int moduleCount = RocketModules.Count;
			for (int i = moduleCount - 1; i >= 0; i--)
			{
				var module = RocketModules[i];
				sb.Append(moduleCount - i);
				sb.Append(": ");
				sb.Append(module.Name);
				sb.Append(", ");
				sb.Append(module.width);
				sb.Append("x");
				sb.Append(module.height);
				sb.Append(" (");
				for (int e = 0; e < module.SelectedElements.Count; e++)
				{
					var selectedElement = module.SelectedElements[e];
					sb.Append(Assets.TryGetPrefab(selectedElement)?.GetProperName() ?? selectedElement);
					if (e < module.SelectedElements.Count - 1)
					{
						sb.Append(", "); 
					}
				}
				sb.Append(")");
				if (!module.Valid)
					sb.Append(" (missing module)");
				sb.AppendLine();
			}
			return sb.ToString();
		}

		public int GetTotalHeight()
		{
			int height = 0;
			for (int i = 0; i < RocketModules.Count; i++)
				height += RocketModules[i].height;
			return height;
		}

		public static RocketBlueprint Generate(IEnumerable<Building> modules)
		{
			var bp = new RocketBlueprint();
			foreach (var module in modules)
			{
				bp.RocketModules.Add(RocketBlueprintModule.From(module));
			}
			return bp;
		}
	}
}
