using HarmonyLib;
using Rockets_TinyYetBig.Science;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rockets_TinyYetBig.Patches.ResearchPatches
{
	internal class ArtifactAnalysisStationWorkable_Patch
	{
		[HarmonyPatch(typeof(ArtifactAnalysisStationWorkable), nameof(ArtifactAnalysisStationWorkable.ConsumeCharm))]
		public class AddDeepSpaceResearchOnSpaceArtifactResearch
		{
			public static void Prefix(ArtifactAnalysisStationWorkable __instance)
			{
				GameObject artifactToBeDefrosted = __instance.storage.FindFirst(GameTags.CharmedArtifact);
				if (artifactToBeDefrosted != null)
				{
					if (Config.SpaceStationsPossible)
					{
						DeepSpaceScienceManager.Instance.ArtifactResearched(artifactToBeDefrosted.HasTag(GameTags.TerrestrialArtifact));
					}
					if (Config.Instance.NeutroniumMaterial)
						YeetDust(__instance.gameObject, artifactToBeDefrosted.HasTag(GameTags.TerrestrialArtifact) ? 20f : 10f);
				}
			}
			static void YeetDust(GameObject originGo, float amount)
			{

				GameObject go = ElementLoader.FindElementByHash(ModElements.UnobtaniumDust.SimHash).substance.SpawnResource(originGo.transform.position, amount, UtilLibs.UtilMethods.GetKelvinFromC(20f), byte.MaxValue, 0, false);
				go.transform.SetPosition(Grid.CellToPosCCC(Grid.PosToCell(originGo), Grid.SceneLayer.Ore));
				go.SetActive(true);


				Vector2 initial_velocity = new Vector2(UnityEngine.Random.Range(-2f, 2f) * 1f, (float)((double)UnityEngine.Random.value * 2.5 + 3.0));
				if (GameComps.Fallers.Has((object)go))
					GameComps.Fallers.Remove(go);
				GameComps.Fallers.Add(go, initial_velocity);
			}
		}
	}
}
