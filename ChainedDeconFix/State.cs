using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainedDeconFix
{
    public static class State
    {
        public static string[] Chainables = new string[]
        {
            "Ladder",
            "LadderFast",
            "SteelLadder",
            "TravelTube",
            "FirePole"
        };


        public static bool ChainAll = false;
    }
}
