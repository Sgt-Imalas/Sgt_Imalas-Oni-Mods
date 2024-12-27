using AkisSnowThings.Content.Defs.Entities;
using AkisSnowThings.Content.Defs.Plants;
using AkisSnowThings.Content.Scripts.Buildings;
using AkisSnowThings.Content.Scripts.Elements;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using TemplateClasses;
using UtilLibs;
using static CodexEntryGenerator_Elements;

namespace AkisSnowThings.Patches
{
	internal class CodexEntryGenerator_ElementsPatch
	{
		[HarmonyPatch(typeof(CodexEntryGenerator_Elements),nameof(CodexEntryGenerator_Elements.GetElementEntryContext))]
		public class CodexEntryGenerator_Elements_GetElementEntryContext_Patch
		{
			static bool init = false;
			public static void Postfix(ElementEntryContext __result)
			{
				if (init)
					return;

				init = true;

				Tag treeId = EvergreenTreeConfig.ID;

				if (!__result.usedMap.map.ContainsKey(treeId))
				{
					__result.usedMap.map[treeId] = new();
				}
				__result.usedMap.map[treeId].Add(new()
				{
					title = STRINGS.CREATURES.SPECIES.SNOWSCULPTURES_EVERGREEN_TREE.NAME,
					prefab = Assets.GetPrefab(treeId),
					inSet = [new ElementUsage(SimHashes.DirtyWater.CreateTag(), EvergreenTreeConfig.WATER_PER_SECOND, true)],
					outSet = [new ElementUsage(SnowModElements.EvergreenTreeSap.Tag, EvergreenTreeConfig.SAP_PER_SECOND, true)]
				});

				__result.madeMap.map[SimHashes.WoodLog.CreateTag()].Add(new()
				{
					title = STRINGS.CREATURES.SPECIES.SNOWSCULPTURES_EVERGREEN_TREE.NAME,
					prefab = Assets.GetPrefab(treeId),
					inSet = [new ElementUsage(SimHashes.DirtyWater.CreateTag(), EvergreenTreeConfig.WATER_PER_SECOND, true)],
					outSet = [new ElementUsage(SimHashes.WoodLog.CreateTag(), EvergreenTreeConfig.WOOD_PER_SECOND, true)]
				});
			}
		}
	}
}
