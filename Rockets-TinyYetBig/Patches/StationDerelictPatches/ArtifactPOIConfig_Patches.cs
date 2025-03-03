using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rockets_TinyYetBig.Patches.StationDerelictPatches
{
	internal class ArtifactPOIConfig_Patches
	{
		/// <summary>
		/// Fixes description not existing on artifact POIs, also removes the incorrect "requires drillcone" part from the description
		/// </summary>
		[HarmonyPatch(typeof(ArtifactPOIConfig), nameof(ArtifactPOIConfig.CreateArtifactPOI))]
		public static class AddDerelictInteriorToArtifactPOIs
		{
			public static void Postfix(string id,
				string anim,
				string name,
				string desc,
				HashedString poiType,
				ref GameObject __result)
			{

				var firstLineBreak = desc.IndexOf("\n");
				if (firstLineBreak != -1)
				{
					desc = desc.Substring(0, firstLineBreak);
				}

				__result.AddOrGet<InfoDescription>().description = desc;// Strings.Get("STRINGS.UI.SPACEDESTINATIONS.ARTIFACT_POI." + spst.poiID.ToUpperInvariant() + ".DESC");
			}
		}
	}
}
