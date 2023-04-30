using HarmonyLib;
using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using TUNING;

namespace KnastoronOniMods
{
    public class AIPassengerModule : PassengerRocketModule
    {
        [Serialize]
        public bool variableSpeed = false;

    }
}
