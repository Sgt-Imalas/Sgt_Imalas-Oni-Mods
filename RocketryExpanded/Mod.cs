using HarmonyLib;
using KMod;
using RocketryExpanded.entities;
using System;

namespace RocketryExpanded
{
	public class Mod : UserMod2
	{
		public override void OnLoad(Harmony harmony)
		{
			GameTags.MaterialBuildingElements.Add(ModAssets.Tags.BuildableExplosive);
			//GameTags.Other.Add("x");
			base.OnLoad(harmony);
		}
	}
}
