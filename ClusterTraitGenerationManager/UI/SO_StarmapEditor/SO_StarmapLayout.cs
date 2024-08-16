using ProcGen;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ClusterTraitGenerationManager.ClusterData;
using static STRINGS.DUPLICANTS.PERSONALITIES;
using UtilLibs;
using static ResearchTypes;
using System;

namespace ClusterTraitGenerationManager.UI.SO_StarmapEditor
{
    public class SO_StarmapLayout
    {
        public Dictionary<AxialI, string> OverridePlacements = new();
        public bool GenerationPossible => _generationPossible;
        private bool _generationPossible = false;
        public string FailedGenerationPlanetId => _failedGenerationPlanetId;
        private string _failedGenerationPlanetId = string.Empty;

        public bool UsingCustomLayout => _usingCustomLayout;
        private bool _usingCustomLayout = false;
        public void SetUsingCustomLayout() => _usingCustomLayout = true;

        public bool EncasedPlanet(out string encasedId)
        {
            HashSet<AxialI> planetPlacements = new HashSet<AxialI>();
            foreach(var entry in OverridePlacements)
            {
                if (!ModAssets.SO_POIs.ContainsKey(entry.Value))
                {
                    planetPlacements.Add(entry.Key);
                }
            }
            var directions = AxialI.DIRECTIONS;

            foreach (AxialI planet in planetPlacements)
            {
                for (int i = 0; i < directions.Count; ++i)
                {
                    if (!planetPlacements.Contains(planet + directions[i]))
                        break;

                    if (i == directions.Count - 1)
                    {
                        encasedId = OverridePlacements[planet];
                        return true;
                    }
                }
            }
            encasedId = string.Empty;
            return false;
        }


        public SO_StarmapLayout(int seed)
        {
            ResetCustomPlacements(seed);
        }

        public void ResetCustomPlacements(int seed)
        {
            _usingCustomLayout = false;
            GeneratePlacementOverrides(seed);
        }

        public void AddPOI(string id, AxialI newPlace)
        {
            _usingCustomLayout = true;
            OverridePlacements.Add(newPlace, id);
        }
        public void MovePOI(string id, AxialI original, AxialI newPlace)
        {
            _usingCustomLayout = true;
            OverridePlacements.Remove(original);
            OverridePlacements.Add(newPlace, id);  
        }
        public void RemovePOI(AxialI original)
        {
            _usingCustomLayout = true;
            OverridePlacements.Remove(original);
        }

        void GeneratePlacementOverrides(int seed)
        {
            OverridePlacements.Clear();
            _generationPossible = AssignClusterLocations(seed);
        }

        /// <summary>
        /// Copied from the ProcGenGame.Cluster Class
        /// </summary>
        /// 


        public bool AssignClusterLocations(int seed)
        {
            _failedGenerationPlanetId = string.Empty;
            //fields native to Cluster:
            ClusterLayout clusterLayout = CGSMClusterManager.GeneratedLayout;
            var myRandom = new SeededRandom(seed);
            var worlds = new List<WorldPlacement>(clusterLayout.worldPlacements);
            int numRings = clusterLayout.numRings;
            //ProcGenGame.Cluster.AssignClusterLocations



            List<SpaceMapPOIPlacement> list2 = ((clusterLayout.poiPlacements == null) ? new List<SpaceMapPOIPlacement>() : new List<SpaceMapPOIPlacement>(clusterLayout.poiPlacements));
            //currentWorld.SetClusterLocation(AxialI.ZERO);
            HashSet<AxialI> assignedLocations = new HashSet<AxialI>();
            HashSet<AxialI> worldForbiddenLocations = new HashSet<AxialI>();
            new HashSet<AxialI>();
            HashSet<AxialI> poiWorldAvoidance = new HashSet<AxialI>();
            int maxRadius = 2;
            for (int i = 0; i < worlds.Count; i++)
            {
                //WorldGen worldGen = new(worlds[i]);
                WorldPlacement worldPlacement = worlds[i];
                //DebugUtil.Assert(worldPlacement != null, "Somehow we're trying to generate a cluster with a world that isn't the cluster .yaml's world list!", worldGen.Settings.world.filePath);
                HashSet<AxialI> antiBuffer = new HashSet<AxialI>();
                foreach (AxialI item in assignedLocations)
                {
                    antiBuffer.UnionWith(AxialUtil.GetRings(item, 1, worldPlacement.buffer));
                }

                List<AxialI> availableLocations = (from location in AxialUtil.GetRings(AxialI.ZERO, worldPlacement.allowedRings.min, Mathf.Min(worldPlacement.allowedRings.max, numRings - 1))
                                      where !assignedLocations.Contains(location) && !worldForbiddenLocations.Contains(location) && !antiBuffer.Contains(location)
                                      select location).ToList();
                if (availableLocations.Count > 0)
                {
                    AxialI axialI = availableLocations[myRandom.RandomRange(0, availableLocations.Count)];
                    OverridePlacements[axialI] = worldPlacement.world;
                    assignedLocations.Add(axialI);
                    worldForbiddenLocations.UnionWith(AxialUtil.GetRings(axialI, 1, worldPlacement.buffer));
                    poiWorldAvoidance.UnionWith(AxialUtil.GetRings(axialI, 1, maxRadius));
                    continue;
                }

               // DebugUtil.DevLogError("Could not find a spot in the cluster for " + worldPlacement.world + ". Check the placement settings in the custom cluster to ensure there are no conflicts.");
                HashSet<AxialI> minBuffers = new HashSet<AxialI>();
                foreach (AxialI item2 in assignedLocations)
                {
                    minBuffers.UnionWith(AxialUtil.GetRings(item2, 1, 2));
                }

                availableLocations = (from location in AxialUtil.GetRings(AxialI.ZERO, worldPlacement.allowedRings.min, Mathf.Min(worldPlacement.allowedRings.max, numRings - 1))
                         where !assignedLocations.Contains(location) && !minBuffers.Contains(location)
                         select location).ToList();
                if (availableLocations.Count > 0)
                {
                    AxialI axialI2 = availableLocations[myRandom.RandomRange(0, availableLocations.Count)];
                    OverridePlacements[axialI2] = worldPlacement.world;
                    assignedLocations.Add(axialI2);
                    worldForbiddenLocations.UnionWith(AxialUtil.GetRings(axialI2, 1, worldPlacement.buffer));
                    poiWorldAvoidance.UnionWith(AxialUtil.GetRings(axialI2, 1, maxRadius));
                    continue;
                }

                string text = "Could not find a spot in the cluster for " + worldPlacement.world + " EVEN AFTER REDUCING BUFFERS. Check the placement settings in the custom cluster to ensure there are no conflicts.";
                SgtLogger.error(text); 
                _failedGenerationPlanetId = worldPlacement.world;
                return false;
            }

            if (DlcManager.FeatureClusterSpaceEnabled() && list2 != null)
            {
                HashSet<AxialI> poiClumpLocations = new HashSet<AxialI>();
                HashSet<AxialI> poiForbiddenLocations = new HashSet<AxialI>();
                float num = 0.5f;
                int num2 = 3;
                int num3 = 0;
                foreach (SpaceMapPOIPlacement item3 in list2)
                {
                    List<string> list4 = new List<string>(item3.pois);
                    for (int j = 0; j < item3.numToSpawn; j++)
                    {
                        bool num4 = myRandom.RandomRange(0f, 1f) <= num;
                        List<AxialI> axialIList = null;
                        if (num4 && num3 < num2 && !item3.avoidClumping)
                        {
                            num3++;
                            axialIList = (from location in AxialUtil.GetRings(AxialI.ZERO, item3.allowedRings.min, Mathf.Min(item3.allowedRings.max, numRings - 1))
                                     where !assignedLocations.Contains(location) && poiClumpLocations.Contains(location) && !poiWorldAvoidance.Contains(location)
                                     select location).ToList();
                        }

                        if (axialIList == null || axialIList.Count <= 0)
                        {
                            num3 = 0;
                            poiClumpLocations.Clear();
                            axialIList = (from location in AxialUtil.GetRings(AxialI.ZERO, item3.allowedRings.min, Mathf.Min(item3.allowedRings.max, numRings - 1))
                                     where !assignedLocations.Contains(location) && !poiWorldAvoidance.Contains(location) && !poiForbiddenLocations.Contains(location)
                                     select location).ToList();
                        }

                        if (item3.guarantee && (axialIList == null || axialIList.Count <= 0))
                        {
                            num3 = 0;
                            poiClumpLocations.Clear();
                            axialIList = (from location in AxialUtil.GetRings(AxialI.ZERO, item3.allowedRings.min, Mathf.Min(item3.allowedRings.max, numRings - 1))
                                     where !assignedLocations.Contains(location) && !poiWorldAvoidance.Contains(location)
                                     select location).ToList();
                        }

                        if (axialIList != null && axialIList.Count > 0)
                        {
                            AxialI axialI3 = axialIList[myRandom.RandomRange(0, axialIList.Count)];
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
                            Debug.LogWarning(string.Format("There is no room for a Space POI in ring range [{0}, {1}] with pois: {2}", item3.allowedRings.min, item3.allowedRings.max, string.Join("\n - ", item3.pois.ToArray())));
                        }
                    }
                }
            }

            return true;
        }

    }
}
