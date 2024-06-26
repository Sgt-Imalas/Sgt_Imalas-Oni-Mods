using ProcGen;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ClusterTraitGenerationManager.ClusterData;

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
        /// Copied from the ProcGen.Cluster Class
        /// </summary>
        public bool AssignClusterLocations(int seed)
        {
            _failedGenerationPlanetId = string.Empty;
            ClusterLayout clusterLayout = CGSMClusterManager.GeneratedLayout;
            var myRandom = new SeededRandom(seed);
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
                    //worldGen.SetClusterLocation(axialI);
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
                //ProcGenGame.Cluster.AssignClusterLocations

                availableWorldLocations = (from location in AxialUtil.GetRings(AxialI.ZERO, worldPlacement.allowedRings.min, Mathf.Min(worldPlacement.allowedRings.max, clusterLayout.numRings - 1))
                         where !assignedLocations.Contains(location) && !minBuffers.Contains(location) //
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
                _failedGenerationPlanetId = worldPlacement.world;
                return false;
            }
            if (DlcManager.FeatureClusterSpaceEnabled() && poiPlacements != null)
            {
                HashSet<AxialI> poiClumpLocations = new HashSet<AxialI>();
                HashSet<AxialI> poiForbiddenLocations = new HashSet<AxialI>();
                float num = 0.5f;
                int maxRange = 3;
                int minRange = 0;
                foreach (SpaceMapPOIPlacement spaceMapPoiPlacement in poiPlacements)
                {
                    List<string> availablePOITypes = new List<string>(spaceMapPoiPlacement.pois);
                    for (int index = 0; index < spaceMapPoiPlacement.numToSpawn; ++index)
                    {
                        if (availablePOITypes.Count == 0)
                            break;

                        bool randRangeSmaller = myRandom.RandomRange(0f, 1f) <= num;
                        List<AxialI> availableLocations = null;
                        if (randRangeSmaller && minRange < maxRange && !spaceMapPoiPlacement.avoidClumping)
                        {
                            minRange++;
                            availableLocations = (from location in AxialUtil.GetRings(AxialI.ZERO, spaceMapPoiPlacement.allowedRings.min, Mathf.Min(spaceMapPoiPlacement.allowedRings.max, clusterLayout.numRings - 1))
                                     where !assignedLocations.Contains(location) && poiClumpLocations.Contains(location) && !poiWorldAvoidance.Contains(location)
                                     select location).ToList();
                        }

                        if (availableLocations == null || availableLocations.Count <= 0)
                        {
                            minRange = 0;
                            poiClumpLocations.Clear();
                            availableLocations = (from location in AxialUtil.GetRings(AxialI.ZERO, spaceMapPoiPlacement.allowedRings.min, Mathf.Min(spaceMapPoiPlacement.allowedRings.max, clusterLayout.numRings - 1))
                                     where !assignedLocations.Contains(location) && !poiWorldAvoidance.Contains(location) && !poiForbiddenLocations.Contains(location)
                                     select location).ToList();
                        }

                        if (availableLocations != null && availableLocations.Count > 0)
                        {
                            AxialI axialI3 = availableLocations[myRandom.RandomRange(0, availableLocations.Count)];
                            string selectedPoiType = availablePOITypes[myRandom.RandomRange(0, availablePOITypes.Count)];
                            if (!spaceMapPoiPlacement.canSpawnDuplicates)
                            {
                                availablePOITypes.Remove(selectedPoiType);
                            }

                            OverridePlacements[axialI3] = selectedPoiType;
                            poiForbiddenLocations.UnionWith(AxialUtil.GetRings(axialI3, 1, 3));
                            poiClumpLocations.UnionWith(AxialUtil.GetRings(axialI3, 1, 1));
                            assignedLocations.Add(axialI3);
                        }
                        else
                        {
                           // Debug.LogWarning($"There is no room for a Space POI in ring range [{poiPlacement.allowedRings.min}, {poiPlacement.allowedRings.max}]");
                        }
                    }
                }
            }

            return true;
        }

    }
}
