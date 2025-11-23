using AkisDecorPackB.Content.Defs.Items;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkisDecorPackB.Patches
{
	internal class FossilBits_Patches
	{

        [HarmonyPatch(typeof(FossilBits), nameof(FossilBits.DropLoot))]
        public class FossilBits_DropLoot_Patch
        {
            public static void Postfix(FossilBits __instance)
            {
				if (__instance.TryGetComponent(out PrimaryElement primaryElement))
				{
					
					var nodule = Util.KInstantiate(Assets.GetPrefab(FossilNoduleConfig.ID), __instance.transform.position);
					nodule.SetActive(true);
					if (nodule.TryGetComponent(out PrimaryElement primaryElement2))
					{
						primaryElement2.SetTemperature(primaryElement.Temperature);
						primaryElement2.AddDisease(primaryElement.DiseaseIdx, (int)(primaryElement2.DiseaseCount * 0.1f), "Spawn");
					}
				}
			}
        }
	}
}
