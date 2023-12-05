using ProcGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ResearchTypes;
using UnityEngine;
using Klei.CustomSettings;

namespace ClusterTraitGenerationManager.SO_StarmapEditor
{
    internal class SO_StarmapLayout
    {
        public Dictionary<AxialI,string> OverridePlacements = new();
        public bool GenerationPossible=false;
        public bool UsingCustomLayout=false;

        public void ResetCustomPlacements(int seed)
        {
            UsingCustomLayout = false;
            GeneratePlacementOverrides(seed);
        }

        public void MovePOI(string id, AxialI original, AxialI newPlace)
        {
            OverridePlacements.Remove(original);
            OverridePlacements.Add(newPlace, id);  
        }

        void GeneratePlacementOverrides(int seed)
        {
            OverridePlacements.Clear();
            GenerationPossible = AssignClusterLocations(seed);
        }

        /// <summary>
        /// Copied from the ProcGen.Cluster Class
        /// </summary>
        public bool AssignClusterLocations(int seed)
        {
            var myRandom = new SeededRandom(seed);
            ClusterLayout clusterLayout = CGSMClusterManager.GeneratedLayout;
            List<WorldPlacement> asteroidPlacements = new List<WorldPlacement>(clusterLayout.worldPlacements);
            List<SpaceMapPOIPlacement> poiPlacements = ((clusterLayout.poiPlacements == null) ? new List<SpaceMapPOIPlacement>() : new List<SpaceMapPOIPlacement>(clusterLayout.poiPlacements));
           // currentWorld.SetClusterLocation(AxialI.ZERO);
            HashSet<AxialI> assignedLocations = new HashSet<AxialI>();
            HashSet<AxialI> worldForbiddenLocations = new HashSet<AxialI>();
            new HashSet<AxialI>();
            HashSet<AxialI> poiWorldAvoidance = new HashSet<AxialI>();
            int maxRadius = 2;
            for (int i = 0; i < asteroidPlacements.Count; i++)
            {
                //WorldGen worldGen = worlds[i];
                WorldPlacement worldPlacement = asteroidPlacements[i];
                HashSet<AxialI> antiBuffer = new HashSet<AxialI>();
                foreach (AxialI item in assignedLocations)
                {
                    antiBuffer.UnionWith(AxialUtil.GetRings(item, 1, worldPlacement.buffer));
                }

                List<AxialI> availableWorldLocations = (from location in AxialUtil.GetRings(AxialI.ZERO, worldPlacement.allowedRings.min, Mathf.Min(worldPlacement.allowedRings.max, clusterLayout.numRings - 1))
                                      where !assignedLocations.Contains(location) && !worldForbiddenLocations.Contains(location) && !antiBuffer.Contains(location)
                                      select location).ToList();
                if (availableWorldLocations.Count > 0)
                {
                    AxialI axialI = availableWorldLocations[myRandom.RandomRange(0, availableWorldLocations.Count)];
                    OverridePlacements[axialI]=worldPlacement.world;
                    assignedLocations.Add(axialI);
                    worldForbiddenLocations.UnionWith(AxialUtil.GetRings(axialI, 1, worldPlacement.buffer));
                    poiWorldAvoidance.UnionWith(AxialUtil.GetRings(axialI, 1, maxRadius));
                    continue;
                }

                //DebugUtil.DevLogError("Could not find a spot in the cluster for " + worldGen.Settings.world.filePath + ". Check the placement settings in " + Id + ".yaml to ensure there are no conflicts.");
                HashSet<AxialI> minBuffers = new HashSet<AxialI>();
                foreach (AxialI item2 in assignedLocations)
                {
                    minBuffers.UnionWith(AxialUtil.GetRings(item2, 1, 2));
                }

                availableWorldLocations = (from location in AxialUtil.GetRings(AxialI.ZERO, worldPlacement.allowedRings.min, Mathf.Min(worldPlacement.allowedRings.max, clusterLayout.numRings - 1))
                         where !assignedLocations.Contains(location) && !minBuffers.Contains(location)
                         select location).ToList();
                if (availableWorldLocations.Count > 0)
                {
                    AxialI axialI2 = availableWorldLocations[myRandom.RandomRange(0, availableWorldLocations.Count)];
                    OverridePlacements[axialI2]=worldPlacement.world;
                    //worldGen.SetClusterLocation(axialI2);
                    assignedLocations.Add(axialI2);
                    worldForbiddenLocations.UnionWith(AxialUtil.GetRings(axialI2, 1, worldPlacement.buffer));
                    poiWorldAvoidance.UnionWith(AxialUtil.GetRings(axialI2, 1, maxRadius));
                    continue;
                }

               // string text = "Could not find a spot in the cluster for " + worldGen.Settings.world.filePath + " EVEN AFTER REDUCING BUFFERS. Check the placement settings in " + Id + ".yaml to ensure there are no conflicts.";
               // DebugUtil.LogErrorArgs(text);
                //if (!worldGen.isRunningDebugGen)
                //{
                //    currentWorld.ReportWorldGenError(new Exception(text));
                //}

                return false;
            }

            if (DlcManager.FeatureClusterSpaceEnabled() && poiPlacements != null)
            {
                HashSet<AxialI> poiClumpLocations = new HashSet<AxialI>();
                HashSet<AxialI> poiForbiddenLocations = new HashSet<AxialI>();
                float num = 0.5f;
                int num2 = 3;
                int num3 = 0;
                foreach (SpaceMapPOIPlacement item3 in poiPlacements)
                {
                    List<string> list4 = new List<string>(item3.pois);
                    for (int j = 0; j < item3.numToSpawn; j++)
                    {
                        bool num4 = myRandom.RandomRange(0f, 1f) <= num;
                        List<AxialI> list5 = null;
                        if (num4 && num3 < num2 && !item3.avoidClumping)
                        {
                            num3++;
                            list5 = (from location in AxialUtil.GetRings(AxialI.ZERO, item3.allowedRings.min, Mathf.Min(item3.allowedRings.max, clusterLayout.numRings - 1))
                                     where !assignedLocations.Contains(location) && poiClumpLocations.Contains(location) && !poiWorldAvoidance.Contains(location)
                                     select location).ToList();
                        }

                        if (list5 == null || list5.Count <= 0)
                        {
                            num3 = 0;
                            poiClumpLocations.Clear();
                            list5 = (from location in AxialUtil.GetRings(AxialI.ZERO, item3.allowedRings.min, Mathf.Min(item3.allowedRings.max, clusterLayout.numRings - 1))
                                     where !assignedLocations.Contains(location) && !poiWorldAvoidance.Contains(location) && !poiForbiddenLocations.Contains(location)
                                     select location).ToList();
                        }

                        if (list5 != null && list5.Count > 0)
                        {
                            AxialI axialI3 = list5[myRandom.RandomRange(0, list5.Count)];
                            string text2 = list4[myRandom.RandomRange(0, list4.Count)];
                            if (!item3.canSpawnDuplicates)
                            {
                                list4.Remove(text2);
                            }

                            OverridePlacements[axialI3] = text2;
                            poiForbiddenLocations.UnionWith(AxialUtil.GetRings(axialI3, 1, 3));
                            poiClumpLocations.UnionWith(AxialUtil.GetRings(axialI3, 1, 1));
                            assignedLocations.Add(axialI3);
                        }
                        else
                        {
                           // Debug.LogWarning($"There is no room for a Space POI in ring range [{item3.allowedRings.min}, {item3.allowedRings.max}]");
                        }
                    }
                }
            }

            return true;
        }

    }
}
