using Rockets_TinyYetBig.SpaceStations.Construction;
using Rockets_TinyYetBig.SpaceStations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static ResearchTypes;
using KSerialization;
using static STRINGS.ROOMS.CRITERIA;
using UtilLibs;
using System.IO;

namespace Rockets_TinyYetBig.Derelicts
{
    internal class DerelictStationConfigs : IMultiEntityConfig
    {

        public static readonly string DerelictSubPath = "derelictInteriors";
        public static readonly string DerelictTemplateName = "_RTB_DerelictInterior";
        List<Tuple<string, Vector2I, string>> POIS = new List<Tuple<string, Vector2I, string>>()
        {
            new Tuple<string, Vector2I, string>("ArtifactSpacePOI_GravitasSpaceStation1",new Vector2I (32,32),"station_1" ),
            new Tuple<string, Vector2I, string>("ArtifactSpacePOI_GravitasSpaceStation2",new Vector2I (32,32),"station_2" ),
            new Tuple<string, Vector2I, string>("ArtifactSpacePOI_GravitasSpaceStation3",new Vector2I (32,32),"station_3" ),
            new Tuple<string, Vector2I, string>("ArtifactSpacePOI_GravitasSpaceStation4",new Vector2I (19,18),"station_4" ),
            new Tuple<string, Vector2I, string>("ArtifactSpacePOI_GravitasSpaceStation5",new Vector2I (32,32),"station_5" ),
            new Tuple<string, Vector2I, string>("ArtifactSpacePOI_GravitasSpaceStation6",new Vector2I (32,32),"station_6" ),
            new Tuple<string, Vector2I, string>("ArtifactSpacePOI_GravitasSpaceStation7",new Vector2I (32,32),"station_7" ),
            new Tuple<string, Vector2I, string>("ArtifactSpacePOI_GravitasSpaceStation8",new Vector2I (32,32),"station_8" ),
                                                    
        };
        public List<GameObject> CreatePrefabs()
        {
            var values = new List<GameObject>();
            foreach(var p in POIS)
            {
                string templatename = Path.Combine(DerelictSubPath, p.first + DerelictTemplateName);
                if (!TemplateCache.TemplateExists(templatename))
                {
                    SgtLogger.l(templatename, "template missing");
                    continue;
                }
                values.Add(CreatePrefab(p.first + DerelictTemplateName, p.second, templatename,p.third));
                
            }
            return values;
        }
        public GameObject CreatePrefab(string ID, Vector2I size, string template, string initialAnim)
        {
            var entity = EntityTemplates.CreateEntity(
                   id: ID,
                   name: ID
             );
            SaveLoadRoot saveLoadRoot = entity.AddOrGet<SaveLoadRoot>();
            saveLoadRoot.DeclareOptionalComponent<WorldInventory>();
            saveLoadRoot.DeclareOptionalComponent<WorldContainer>();
            saveLoadRoot.DeclareOptionalComponent<OrbitalMechanics>();
            entity.AddOrGet<AssignmentGroupController>().generateGroupOnStart = true;

            RocketClusterDestinationSelector destinationSelector = entity.AddOrGet<RocketClusterDestinationSelector>();
            destinationSelector.assignable = false;
            destinationSelector.shouldPointTowardsPath = false;
            destinationSelector.requireAsteroidDestination = false;

            var spst = entity.AddOrGet<DerelictStation>();
            spst.IsDeconstructable = false;
            spst.BuildableInterior = false;
            spst.ShouldDrawBarriers = false;
            spst.IsDerelict = true;
            spst.Upgradeable = false;
            spst.InteriorSize = size;
            spst.InteriorTemplate = template;
            spst.m_Anim = initialAnim;
            spst.InteriorSize = size;
            spst.bottomLeftCorner = new Vector2I(0, 0);
            spst.topRightCorner = new Vector2I(size.x-1, size.y-1);
            spst.poiID = ID.Replace(DerelictTemplateName, string.Empty).Replace("ArtifactSpacePOI_", string.Empty);
            spst.l_name = Strings.Get("STRINGS.UI.SPACEDESTINATIONS.ARTIFACT_POI." + spst.poiID.ToUpperInvariant() + ".NAME"); 

            entity.AddOrGet<CharacterOverlay>().shouldShowName = true;
            entity.AddOrGetDef<AlertStateManager.Def>();
            entity.AddOrGet<Notifier>();

            var traveler = entity.AddOrGet<ClusterTraveler>();
            traveler.stopAndNotifyWhenPathChanges = false;

            entity.AddOrGet<CraftModuleInterface>();
            var desc = Strings.Get("STRINGS.UI.SPACEDESTINATIONS.ARTIFACT_POI." + spst.poiID.ToUpperInvariant() + ".DESC").String;
            var firstLineBreak = desc.IndexOf("\n");
            if (firstLineBreak != -1)
            {
                desc = desc.Substring(0, firstLineBreak);
            }
            entity.AddOrGet<InfoDescription>().description = desc;

            return entity;
        }
        public void OnPrefabInit(GameObject inst)
        {
        }

        public void OnSpawn(GameObject inst)
        {
        }
    }
}
