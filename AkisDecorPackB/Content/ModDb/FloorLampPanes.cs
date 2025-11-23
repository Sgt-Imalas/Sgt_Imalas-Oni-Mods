using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AkisDecorPackB.Content.ModDb
{
	public class FloorLampPane(string id, string name, string animFile, Color lightColor) : Resource(id, name)
	{
		public string animFile = animFile;
		public Color lightColor = lightColor;

		public static FloorLampPane FromElement(SimHashes element, Color lightColor) => FromElement(element.ToString(), lightColor);

		public static string GetIdFromElement(string elementName) => $"floorlamp_{elementName.ToLowerInvariant()}";

		public static FloorLampPane FromElement(string element, Color lightColor)
		{
			return new FloorLampPane(
				GetIdFromElement(element),
				$"{element} pane",
				$"dpii_floorlamppane_{element.ToLowerInvariant()}_kanim",
				lightColor);
		}
	}
	public class FloorLampPanes : ResourceSet<FloorLampPane>
	{
		public static Dictionary<string, Color> entries = new()
		{
			{ SimHashes.Glass.ToString(), new Color(2.0f, 1.5f, 0.7f)},
			{ SimHashes.SedimentaryRock.ToString(), Util.ColorFromHex("20dced")},
			{ SimHashes.Salt.ToString(), Util.ColorFromHex("d65f5c")},
			{ SimHashes.Granite.ToString(), Util.ColorFromHex("ff94fb")},
			{ SimHashes.Amber.ToString(), Util.ColorFromHex("be7c2a")},
			{ SimHashes.Algae.ToString(), Util.ColorFromHex("d2ff5e")}
		};

		public FloorLampPanes()
		{
			foreach (var entry in entries)
				Add(FloorLampPane.FromElement(entry.Key, entry.Value));
		}
	}
}
