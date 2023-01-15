using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rockets_TinyYetBig.Science
{
    internal class DeepSpaceScienceManager : KMonoBehaviour, ISaveLoadable
    {
        public static DeepSpaceScienceManager Instance
        {
            get
            {
                DeepSpaceScienceManager manager = null;
                SaveGame.Instance.TryGetComponent(out manager);
                return manager;
            }
        }

        [Serialize]
        float CurrentScienceValue = 0;

        public void ScienceResearched(string researchType)
        {
            if (DeepSpaceScienceUnlocked)
            {
                switch (researchType)
                {
                    case ResearchTypes.ID.BASIC:
                        break;
                    case ResearchTypes.ID.ADVANCED:
                        break;
                    case ResearchTypes.ID.NUCLEAR:
                        break;
                    case ResearchTypes.ID.ORBITAL:
                        break;
                }
            }

        }
        public bool DeepSpaceScienceUnlocked => ModAssets.Techs.DockingTech.IsComplete();
    }
}
