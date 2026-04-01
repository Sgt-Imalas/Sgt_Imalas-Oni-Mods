using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Rockets_TinyYetBig.Content.Scripts.Buildings.SpaceStationConstruction
{
	public struct ConstructionProjectAssembly
	{
		public ConstructionProjectAssembly(List<string> parts)
		{
			Parts = [.. parts];
		}

		public string ProjectName = "Space Project";
		public string ProjectDescription = "Space Project Desc.";


		public string RequiredScienceUnlockId = string.Empty;


		public Tag RequiredPrebuilt = null;
		public List<string> Parts = new List<string>();

		public Sprite PreviewSprite = null;
	}
}
