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
            int arrayLength = butcherable.drops.Length;
            int repeatCount = multiplier / arrayLength; 

            List<string> outputs = new List<string>();
            foreach(var entry in  butcherable.drops)
            {
                outputs.AddRange(Enumerable.Repeat(entry, repeatCount));
            }
            butcherable.SetDrops(outputs.ToArray());
            return gameObject;
        }
    }
}
