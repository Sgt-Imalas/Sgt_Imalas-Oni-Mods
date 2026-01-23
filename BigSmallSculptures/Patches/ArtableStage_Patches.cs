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

                void GenerateDynamicSculpture(string customId,string targetPrefabId)
				{
					GeneratedIDs.Add(customId);
					ArtableStage newStage = new ArtableStage(
					customId,
					name,
					desc,
					PermitRarity.Universal, //ownership check has been done above, it only gets generated if the original is unlocked
					animFile,
					anim,
					decor_value,
					cheer_on_complete,
					status_item,
					prefabId,
					symbolName,
					requiredDlcIds,
					forbiddenDlcIds
					);
					ArtablesInstance.Add(newStage);
				}
				string customId;
				switch (prefabId)
                {
                    case SmallSculptureConfig.ID:
						customId = MarbleSculptureConfig.ID + "_" + id;
                        prefabId = MarbleSculptureConfig.ID;
						ModAssets.MarbleSculptureScaleableSkins.Add(customId);
						GenerateDynamicSculpture(customId, prefabId);
						customId = SculptureConfig.ID + "_" + id;
						prefabId = SculptureConfig.ID;
						GenerateDynamicSculpture(customId, prefabId);
						break;
					case MarbleSculptureConfig.ID:
						customId = SmallSculptureConfig.ID + "_" + id;
                        prefabId = SmallSculptureConfig.ID;
                        ModAssets.SmallSculptureScaledSkins.Add(customId);
						GenerateDynamicSculpture(customId, prefabId);
						break;
                    default:
                        return;
				}
                
			}
        }

        [HarmonyPatch(typeof(ArtableStages), MethodType.Constructor, [typeof(ResourceSet)])]
        public class ArtableStages_TargetMethod_Patch
		{
            public static void Prefix(ArtableStages __instance)
            {
				ArtablesInstance = __instance;
			}
        }
	}
}
