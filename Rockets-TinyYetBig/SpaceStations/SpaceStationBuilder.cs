using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Rockets_TinyYetBig.ModAssets;

namespace Rockets_TinyYetBig.SpaceStations
{
    class SpaceStationBuilder : KMonoBehaviour//, ISidescreenButtonControl
    {
        [Serialize]
        public SpaceStationWithStats CurrentSpaceStationType = ModAssets.SpaceStationTypes[0];

        private void SpawnStation(AxialI location, string prefab)
        {
            Vector3 position = new Vector3(-1f, -1f, 0.0f);
            GameObject sat = Util.KInstantiate(Assets.GetPrefab((Tag)prefab), position);
            sat.SetActive(true);
            var spaceStation = sat.GetComponent<SpaceStation>();
            spaceStation.Location = location;
            spaceStation.CurrentSpaceStationType = CurrentSpaceStationType;
        }

        public int ButtonSideScreenSortOrder()
        {
            return 21;
        }

        public void OnSidescreenButtonPressed()
        {

            Clustercraft component = this.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>();
            var locationToCheck = component.Location;
            //location.q += 2;

            int worldId = SpaceStationManager.GetSpaceStationWorldIdAtLocation(locationToCheck);
            if (worldId!=-1)
            {
                ///other transfering items ?...
                SpaceStationManager.Instance.GetSpaceStationFromWorldId(worldId).DestroySpaceStation();
            }
            else
            {
                ///Add Ressource Check
                SpawnStation(locationToCheck, SmallOrbitalSpaceStationConfig.ID);
            }

        }

    }
}
