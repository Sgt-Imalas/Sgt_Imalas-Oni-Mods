using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Components;
using static STRINGS.UI.SANDBOXTOOLS.SETTINGS;
using UnityEngine;
using UtilLibs;

namespace Cheese.ModElements
{
    internal class ElementDissolver:KMonoBehaviour
    {
        [MyCmpGet] PrimaryElement primElement;

        static Dictionary<SimHashes, List<Tuple<SimHashes, float>>> ElementDissolveLookup = new Dictionary<SimHashes, List<Tuple<SimHashes, float>>>()
        {
            { ModElementRegistration.SaltyMilkFat.SimHash, new ()
            {
                new(SimHashes.Salt,1f),
                new(SimHashes.MilkFat,59f),
            }
            }
        };
        internal static List<Tuple<SimHashes, float>> GetBreakupElements(SimHashes elementID)
        {
            if (ElementDissolveLookup.ContainsKey(elementID))
            {
                return ElementDissolveLookup[elementID];
            }
            return new();
        }

        void DissolveElement()
        {
            List<Tuple<SimHashes, float>> BreakupElements = GetBreakupElements(primElement.ElementID);
            if (BreakupElements.Count == 0)
                return;

            float totalsum = 0;
            foreach (var entry in BreakupElements)
                totalsum += entry.second;

            float totalMass = primElement.Mass;
            float totalDiseaseCount = primElement.DiseaseCount;
            Element elementByHash;
            var position = this.transform.GetPosition();
            float partialMass, partialDiseaseCount;
            int cell = Grid.PosToCell(position);

            if (totalMass > 1f / 1000f)
            {
                foreach (var entry in BreakupElements)
                {
                    elementByHash = ElementLoader.FindElementByHash(entry.first);

                    partialMass = totalMass * (entry.second / totalsum);
                    partialDiseaseCount = totalDiseaseCount * (entry.second / totalsum);

                    if (elementByHash.IsSolid)
                    {
                        GameObject gameObject = elementByHash.substance.SpawnResource(position, partialMass, primElement.Temperature, primElement.DiseaseIdx, Mathf.RoundToInt(partialDiseaseCount), true, manual_activation: true);
                        elementByHash.substance.ActivateSubstanceGameObject(gameObject, primElement.DiseaseIdx, Mathf.RoundToInt(partialDiseaseCount));
                    }
                    else
                        SimMessages.AddRemoveSubstance(cell, elementByHash.id, CellEventLogger.Instance.OreMelted, partialMass, primElement.Temperature, primElement.DiseaseIdx, Mathf.RoundToInt(partialDiseaseCount));

                }
            }
            Destroy(this.gameObject);
        }

        public override void OnSpawn()
        {
            base.OnSpawn(); DissolveElement();
        }
    }
}
