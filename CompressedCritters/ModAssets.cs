using CompressedCritters.Critters;
using Klei.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace CompressedCritters
{
    internal class ModAssets
    {
        public static GameObject MultiplyButcherable(GameObject gameObject, int multiplier)
        {
            if(!gameObject.TryGetComponent<Butcherable>(out var butcherable))
            {
                SgtLogger.error("butcherable not found on critter");
                return null;
            }
            Dictionary<string, float> outputs = new Dictionary<string, float>();

			foreach (var entry in  butcherable.drops)
            {
                outputs[entry.Key] = entry.Value * multiplier;

			}
            butcherable.SetDrops(outputs);
            return gameObject;
        }
    }
}
