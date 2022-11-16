using HarmonyLib;
using RoboRockets.LearningBrain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace RoboRockets.Patches
{
    internal class ResearchTreePatches
    {/// <summary>
     /// add research card to research screen
     /// </summary>
        [HarmonyPatch(typeof(ResourceTreeLoader<ResourceTreeNode>), MethodType.Constructor, typeof(TextAsset))]
        public class ResourceTreeLoader_Load_Patch
        {
            public static void Postfix(ResourceTreeLoader<ResourceTreeNode> __instance, TextAsset file)
            {
                AddNode(__instance);
            }

            private static void AddNode(ResourceTreeLoader<ResourceTreeNode> tech_tree_nodes)
            {
                ResourceTreeNode tempModNode = null;
                var x = 0f;
                var y = 0f;

                foreach (var item in tech_tree_nodes)
                {
                    if (item.Id == GameStrings.Technology.SolidMaterial.HighVelocityDestruction)
                    {
                        tempModNode = item;
                        y = item.nodeY;
                    }
                    else if (item.Id == GameStrings.Technology.ColonyDevelopment.CryoFuelPropulsion)
                    {
                        x = item.nodeX;
                    }
                }
                if (tempModNode == null)
                {
                    return;
                }

                var id = ModAssets.Techs.AiBrainsTech;
                var node = new ResourceTreeNode
                {
                    height = tempModNode.height,
                    width = tempModNode.width,
                    nodeX = x,
                    nodeY = y,
                    edges = new List<ResourceTreeNode.Edge>(tempModNode.edges),
                    references = new List<ResourceTreeNode>() { },
                    Disabled = false,
                    Id = id,
                    Name = id

                };

                tempModNode.references.Add(node);
                tech_tree_nodes.resources.Add(node);
            }
        }

        /// <summary>
        /// Add research node to tree
        /// </summary>
        [HarmonyPatch(typeof(Database.Techs), "Init")]
        public class Techs_TargetMethod_Patch
        {
            public static void Postfix(Database.Techs __instance)
            {
                var tech = new Tech(ModAssets.Techs.AiBrainsTech, new List<string>
                {
                    AIControlModuleLearningConfig.ID,
                    BrainConfig.ID,
                },
                __instance, 
                
                new Dictionary<string, float>()
                {
                    {"basic", 50f },
                    {"advanced", 50f},
                    {"orbital", 400f},
                    {"nuclear", 50f}
                }
                );

            }
        }

    }
}
