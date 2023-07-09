using Klei.AI;
using ONITwitchLib.Logger;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static STRINGS.CREATURES.STATS;

namespace Imalas_TwitchChaosEvents
{
    internal class ModAssets
    {
        public static class SOUNDS
        {
            public const string
                TACORAIN = "ICT_TACORAIN",
                NUKE_DETONATION = "ICT_NUKEDETONATION";
        }


        public static void LoadAll()
        {
            SoundUtils.LoadSound(SOUNDS.TACORAIN, "ICT_TACORAIN.wav");
        }
    }
}
