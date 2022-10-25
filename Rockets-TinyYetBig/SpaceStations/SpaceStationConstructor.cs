using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rockets_TinyYetBig.SpaceStations
{
    class SpaceStationConstructor : KMonoBehaviour, ISidescreenButtonControl
    {
        public string SidescreenButtonText => "Make Station";

        public string SidescreenButtonTooltip => "Make Station Tooltip";

        private void SpawnStation(AxialI location, string prefab)
        {
            Vector3 position = new Vector3(-1f, -1f, 0.0f);
            GameObject sat = Util.KInstantiate(Assets.GetPrefab((Tag)prefab), position);

            sat.GetComponent<ClusterGridEntity>().Location = location;
            sat.SetActive(true);
        }

        public int ButtonSideScreenSortOrder()
        {
            return 21;
        }

        public void OnSidescreenButtonPressed()
        {

            Clustercraft component = this.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>();
            SpawnStation(component.Location, SmallOrbitalSpaceStationConfig.ID);
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
