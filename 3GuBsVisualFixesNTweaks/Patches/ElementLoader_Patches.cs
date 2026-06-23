using ElementUtilNamespace;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace _3GuBsVisualFixesNTweaks.Patches
{
	internal class ElementLoader_Patches
	{

        [HarmonyPatch(typeof(ElementLoader), nameof(ElementLoader.Load))]
        public class ElementLoader_Load_Patch
        {
            [HarmonyPriority(Priority.Low)]
            public static void Postfix(ElementLoader __instance)
            {
                foreach(Element element in ElementLoader.elements)
                {
                    var substance = element.substance;
                    if (substance == null)
                        continue;

                    if (element.id == SimHashes.Magma && Config.Instance.OldMagma)
                    {
                        substance.CausticSwirls();
                        substance.Texture(Substance.SubstanceTexture.None);
						substance.Gradient(null);
						substance.Glows(false);
					}

                    else if(substance.Texture == Substance.SubstanceTexture.MoltenMetal && Config.Instance.OldLiquidMetals)
					{
						substance.CausticSwirls();
						substance.Metallic(false);
                        substance.Texture(Substance.SubstanceTexture.None);
                        substance.Gradient(null);
                        substance.Glows(false);
                    }
                }

            }
        }
	}
}
