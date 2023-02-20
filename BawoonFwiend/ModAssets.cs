using Klei.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BawoonFwiend
{
    public class ModAssets
    {
        public static Effect JustAMachine;
        public static string JustAMachineId = "NotATrueFriend";
        public class Tags
        {
            public static Tag BalloonGas = TagManager.Create(nameof(BalloonGas));
        }
    }
}
