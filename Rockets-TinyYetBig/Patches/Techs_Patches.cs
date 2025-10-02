using HarmonyLib;
using Rockets_TinyYetBig.Buildings.CargoBays;
using Rockets_TinyYetBig.Buildings.Habitats;
using Rockets_TinyYetBig.Buildings;
using Rockets_TinyYetBig.RocketFueling;
using Rockets_TinyYetBig.SpaceStations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;
using Rockets_TinyYetBig.Content.Defs.Buildings.Research;
using Rockets_TinyYetBig.Content.ModDb;

namespace Rockets_TinyYetBig.Patches.ResearchPatches
{
	internal class Techs_Patches
	{

		/// <summary>
		/// Add research node to tree
		/// </summary>
		[HarmonyPatch(typeof(Database.Techs), "Init")]
		public class Techs_TargetMethod_Patch
		{
			public static void Postfix(Database.Techs __instance) => ModTechsDB.RegisterTechs(__instance);		
		}
	}
}
