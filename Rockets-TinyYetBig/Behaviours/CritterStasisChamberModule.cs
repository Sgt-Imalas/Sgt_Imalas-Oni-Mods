using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rockets_TinyYetBig.Behaviours
{
    internal class CritterStasisChamberModule : KMonoBehaviour
    {
        [Serialize]
        List<CritterStorageInfo> storedCritters = new List<CritterStorageInfo> ();

        public void SpawnCrittersFromStorage()
        {
            Vector3 position = this.transform.GetPosition();
            position.z = Grid.GetLayerZ(Grid.SceneLayer.Creatures);
            foreach (var critterInfo in storedCritters)
            {
                GameObject spawnedCritter = Util.KInstantiate(Assets.GetPrefab(critterInfo.CreatureTag), position);
                spawnedCritter.SetActive(true);
                spawnedCritter.GetSMI<AnimInterruptMonitor.Instance>().PlayAnim((HashedString)"growup_pst");
                spawnedCritter.GetSMI<AgeMonitor.Instance>().age.SetValue(critterInfo.CreatureAge);
                var wild = spawnedCritter.GetSMI<WildnessMonitor.Instance>();
                if(wild != null)
                {
                    wild.wildness.SetValue(critterInfo.WildnessPercentage);
                }
            }
        }
        public void AddCritterToStorage(GameObject critter)
        {
            var CritterInfoToStore = new CritterStorageInfo();
            CritterInfoToStore.CreatureTag = critter.GetComponent<CreatureBrain>().species;
            CritterInfoToStore.CreatureAge = critter.GetSMI<AgeMonitor.Instance>().age.value;
            CritterInfoToStore.WildnessPercentage = critter.GetSMI<WildnessMonitor.Instance>().wildness.value;
            storedCritters.Add(CritterInfoToStore); 

            critter.gameObject.DeleteObject();
        }

        struct CritterStorageInfo
        {
            public Tag CreatureTag;
            public float CreatureAge;
            public float WildnessPercentage;
            //public float EggPercentage;

            public CritterStorageInfo(Tag _tag, float _age, float _wildPerc)//, float _egg)
            {
                CreatureTag = _tag;    
                CreatureAge = _age;
                WildnessPercentage = _wildPerc;
                //EggPercentage = _egg;   
            }
        }

    }
}
