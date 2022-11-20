using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rockets_TinyYetBig.SpaceStations
{
    class SpaceStationBuilder : KMonoBehaviour, ISidescreenButtonControl
    {
        public string SidescreenButtonText => "Make Station";

        public string SidescreenButtonTooltip => "Make Station Tooltip";

        private void SpawnStation(AxialI location, string prefab)
        {
            Vector3 position = new Vector3(-1f, -1f, 0.0f);
            GameObject sat = Util.KInstantiate(Assets.GetPrefab((Tag)prefab), position);
            sat.SetActive(true);
            var spaceStation = sat.GetComponent<SpaceStation>();
            spaceStation.Location = location;
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

        public bool SidescreenButtonInteractable()
        {
            return true;
        }

        public bool SidescreenEnabled()
        {
            return true;
        }
    }
}
