using AkisDecorPackB.Content.ModDb;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkisDecorPackB.Patches
{
	internal class ElementLoader_Patches
	{

        [HarmonyPatch(typeof(ElementLoader), nameof(ElementLoader.Load))]
        public class ElementLoader_Load_Patch
        {
			public static void Postfix()
			{
				SetFloorLampPaneTags();
			}

			private static void SetFloorLampPaneTags()
			{
				foreach (var elementName in FloorLampPanes.entries.Keys)
				{
					var element = ElementLoader.FindElementByName(elementName);

					if (element != null)
					{
						element.oreTags ??= [];
						element.oreTags = element.oreTags.AddToArray(ModAssets.Tags.FloorLampPaneMaterial);
					}
				}
			}
		}
	}
}
