using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rockets_TinyYetBig.SpaceStations.Construction
{
    public class ConstructionProjectAssembly
    {
        public bool IsUpgrade = false;

        public string ProjectName = "Space Project";
        public string ProjectDescription = "Space Project Desc.";


        public string RequiredScienceUnlockId = string.Empty;


        public Tag RequiredPrebuilt=null;
        public List<PartProject> Parts = new List<PartProject>();

        public Sprite PreviewSprite = null;
        public Action<GameObject> OnConstructionFinishedAction = null;

    }
}
