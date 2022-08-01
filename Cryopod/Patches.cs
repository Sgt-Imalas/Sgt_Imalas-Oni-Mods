using Cryopod.Buildings;
using Database;
using HarmonyLib;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace Cryopod
{
    class Patches
    {
        //needs Changes
        //[HarmonyPatch(typeof(WorldContainer))]
        //[HarmonyPatch(nameof(WorldContainer.SpacePodAllDupes))]
        //public static class ThrowOutFrozenDupesInsideRocket_ToWorld_Patch
        //{
        //    public static void Prefix(Vector3 spawn_pos, WorldContainer __instance)
        //    {
        //        foreach (var worldItem in ModAssets.CryoPods.GetWorldItems(__instance.id))
        //        {
        //            Debug.Log("PATCH !" + worldItem + spawn_pos);
        //            worldItem.ThrowOutDupe(true, spawn_pos);
        //        }
        //    }
        //}

        /// <summary>
        /// Sickness should be Stored
        /// </summary>
        /// 
        [HarmonyPatch(typeof(Research))]
        [HarmonyPatch("SetActiveResearch")]
        public class Research_SetActiveResearch_Patch
        {
            public static void Prefix(ref Tech tech)
            {
                if (tech == null)
                    return;
                Debug.Log(tech.Id);
                // Can use last check status - req was checked while opening research screen to generate tooltips
                //if (tech.Id == ModAssets.Techs.FrostedDupeResearchID)
                //tech = null;

            }
        }
        [HarmonyPatch(typeof(ResourceTreeLoader<ResourceTreeNode>), MethodType.Constructor, typeof(TextAsset))]
        public class ResourceTreeLoader_Load_Patch
        {
            public static void Postfix(ResourceTreeLoader<ResourceTreeNode> __instance, TextAsset file)
            {
                //Debug.Log("ADDNEWTECH");
                AddNode(__instance);
            }

            private static void AddNode(ResourceTreeLoader<ResourceTreeNode> tech_tree_nodes)
            {
                ResourceTreeNode tempModNode = null;
                var x = 0f;
                var y = 0f;

                foreach (var item in tech_tree_nodes)
                {
                    if (item.Id == GameStrings.Technology.ColonyDevelopment.RoboticTools)
                    {
                        tempModNode = item;
                    }
                    else if (item.Id == GameStrings.Technology.ColonyDevelopment.ArtificialFriends)
                    {
                        y = item.nodeY;
                    }
                    else if (item.Id == GameStrings.Technology.ColonyDevelopment.DurableLifeSupport)
                    {
                        x = item.nodeX;
                    }
                }
                Debug.Log(x + " - " + y + " -> " + tempModNode);
                if (tempModNode == null)
                {
                    return;
                }

                var id = ModAssets.Techs.FrostedDupeResearchID;
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

                //tempModNode.references.Add(node);
                tech_tree_nodes.resources.Add(node);
            }
        }

        [HarmonyPatch(typeof(Techs), "Init")]
        public class Techs_TargetMethod_Patch
        {
            public static void Postfix(Techs __instance)
            {
                var CryoTech = new Tech(ModAssets.Techs.FrostedDupeResearchID, new List<string>
                {
                    BuildableCryopodConfig.ID,
                },
                __instance);

                CryoTech.costsByResearchTypeID.Clear();
                CryoTech.costsByResearchTypeID.Add("basic", 0f);
                CryoTech.costsByResearchTypeID.Add("space", 1f);
            }
        }
        


        [HarmonyPatch(typeof(CryoTank), "DropContents")]
        public class AddTechUnlock_Patch
        {
            public static void Postfix()
            {
                var frostedResearch = Research.Instance.GetTechInstance(ModAssets.Techs.FrostedDupeResearchID);
                //frostedResearch.progressInventory.AddResearchPoints("space", 1f);
                frostedResearch.Purchased();
                Game.Instance.Trigger((int)GameHashes.ResearchComplete, frostedResearch.tech);
                //Game.Instance.Trigger(-107300940, (object)this.activeResearch.tech);

            }
        }
        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
        public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {

            public static void Prefix()
            {
                ModUtil.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Utilities, BuildableCryopodConfig.ID);
            }
        }

        
        [HarmonyPatch(typeof(Localization), "Initialize")]
        public static class Localization_Initialize_Patch
        {
            public static void Postfix()
            {
                LocalisationUtil.Translate(typeof(STRINGS), true);
            }
        }

        [HarmonyPatch(typeof(Database.BuildingStatusItems), "CreateStatusItems")]
        public static class Database_BuildingStatusItems_CreateStatusItems_Patch
        {
            public static void Postfix()
            {
                ModAssets.StatusItems.Register();
            }
        }
    }
}
