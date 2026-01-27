using Database;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;
using static ResearchTypes;
using static STRINGS.BUILDINGS.PREFABS.CONDUIT;

namespace BigSmallSculptures.Patches
{
	internal class ArtableStage_Patches
	{
		static ArtableStages ArtablesInstance;
		static HashSet<string> GeneratedIDs = new HashSet<string>();
		[HarmonyPatch(typeof(ArtableStage), MethodType.Constructor, [typeof(string), typeof(string), typeof(string), typeof(PermitRarity), typeof(string), typeof(string), typeof(int), typeof(bool), typeof(ArtableStatusItem), typeof(string), typeof(string), typeof(string[]), typeof(string[])])]
		public class ArtableStage_Constructor_Patch
		{
			public static void Postfix(ArtableStage __instance, string id, string name, string desc, PermitRarity rarity, string animFile, string anim, int decor_value, bool cheer_on_complete, ArtableStatusItem status_item, string prefabId, string symbolName, string[] requiredDlcIds, string[] forbiddenDlcIds)
			{
				if (GeneratedIDs.Contains(id) || !__instance.IsUnlocked())
					return;
				//SgtLogger.l("Generating dynamic sculpture skin variants for sculpture: " + id +" for building: "+prefabId);

				void GenerateDynamicSculpture(string customId, string targetPrefabId, ArtableStatusItem customStatusItem, int customDecorValue)
				{
					GeneratedIDs.Add(customId);
					ArtableStage newStage = new ArtableStage(
					customId,
					name,
					desc,
					PermitRarity.Universal, //ownership check has been done above, it only gets generated if the original is unlocked
					animFile,
					anim,
					customDecorValue,
					cheer_on_complete,
					customStatusItem,
					targetPrefabId,
					symbolName,
					requiredDlcIds,
					forbiddenDlcIds
					);
					ArtablesInstance.Add(newStage);
				}
				string customSkinId;

				if (ModAssets.BuildingSkinsAllowedOnOtherSculptures(prefabId, out List<Tuple<string, float>> allowedOtherSculptures))
				{
					foreach (var sculptureWithModifier in allowedOtherSculptures)
					{
						var prefabIdOfOtherSculpture = sculptureWithModifier.first;
						float scaleMultiplier = sculptureWithModifier.second;
						int customDecorValue = decor_value;
						ArtableStatusItem customArtableRarityStatus = status_item;

						if (ModAssets.TryGetRarityRemap(prefabId, prefabIdOfOtherSculpture, customArtableRarityStatus, out var remapped, out int decorVal))
						{
							customArtableRarityStatus = remapped;
							customDecorValue = decorVal;
							SgtLogger.l("remapped rarity of " + prefabId +" skin for "+prefabIdOfOtherSculpture+" from " + status_item.StatusType + " to " + remapped.StatusType);
						}

						customSkinId = prefabIdOfOtherSculpture + "_" + id;
						ModAssets.RegisterSkinScaleModifier(customSkinId, scaleMultiplier);
						GenerateDynamicSculpture(customSkinId, prefabIdOfOtherSculpture, customArtableRarityStatus, customDecorValue);
					}
				}
			}
		}

		[HarmonyPatch(typeof(ArtableStages), MethodType.Constructor, [typeof(ResourceSet)])]
		public class ArtableStages_Constructor_Patch
		{
			public static void Prefix(ArtableStages __instance)
			{
				ArtablesInstance = __instance;
			}
		}
	}
}
