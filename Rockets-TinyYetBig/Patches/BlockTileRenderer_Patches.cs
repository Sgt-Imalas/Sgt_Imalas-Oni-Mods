//using HarmonyLib;
//using Rendering;
//using Rockets_TinyYetBig.Elements;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;

//namespace Rockets_TinyYetBig.Patches
//{

/////Disabled, because it makes all tiles rainbow if TrueTiles is not enabled

//	internal class BlockTileRenderer_Patches
//    {
//        [HarmonyPatch(typeof(Rendering.BlockTileRenderer.RenderInfo), MethodType.Constructor, [typeof(BlockTileRenderer), typeof(int), typeof(int), typeof(BuildingDef), typeof(SimHashes )])]
//        public class Rendering_BlockTileRenderer_RenderInfo_TargetMethod_Patch
//        {
//            public static void Postfix(Rendering.BlockTileRenderer.RenderInfo __instance, BuildingDef def, SimHashes element)
//            {
//                if(element == ModElements.UnobtaniumAlloy && def.PrefabID == MetalTileConfig.ID)
//                {
//                    RainbowSpec.NeutroniumAlloyTileMaterial = __instance.material;
//                }
//            }
//        }
//	}
//}
