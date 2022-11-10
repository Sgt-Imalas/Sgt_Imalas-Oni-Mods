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
    class AIPassengerModule : PassengerRocketModule
    {
        public float AiSpeed = 0.5f;
        [Serialize]
        public bool variableSpeed = false;

        public float GiveSpeed()
        {
            if (!variableSpeed)
                return AiSpeed;
            else
                return AiSpeed;///Add logic for variable speed here
        }
    }
}
