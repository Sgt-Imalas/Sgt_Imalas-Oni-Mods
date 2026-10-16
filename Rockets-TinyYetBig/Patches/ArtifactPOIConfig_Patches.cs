using HarmonyLib;
using Rockets_TinyYetBig.Content.Scripts.StarmapEntities;
using UnityEngine;

namespace Rockets_TinyYetBig.Patches
{
	internal class ArtifactPOIConfig_Patches
	{
		/// <summary>
		/// Fixes description not existing on artifact POIs, also removes the incorrect "requires drillcone" part from the description
		/// </summary>
		[HarmonyPatch(typeof(ArtifactPOIConfig),
			nameof(ArtifactPOIConfig.CreateArtifactPOI), 
			[typeof(string), typeof(string), typeof(string), typeof(string), typeof(HashedString), typeof(int)])]
		public static class AddDerelictInteriorToArtifactPOIs
		{
			[HarmonyPrepare]
			static bool Prepare() => Config.Derelicts;
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
				__result.AddOrGet<DerelictSpawner>();
			}
		}
	}
}
