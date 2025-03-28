﻿using HarmonyLib;
using Rockets_TinyYetBig.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rockets_TinyYetBig.Patches
{
	internal class GeneratedBuildings_Patch
	{
		/// <summary>
		/// This patch registers all buildings to the build screen 
		/// and all rocket modules to the rocket module selection screen and their building categories, added by RE 
		/// </summary>
		[HarmonyPatch(typeof(GeneratedBuildings), nameof(GeneratedBuildings.LoadGeneratedBuildings))]
		public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
		{
			public static void Postfix()
			{
				BuildingInjection.AddBuildingsToPlanscreen();
			}
		}
	}
}
