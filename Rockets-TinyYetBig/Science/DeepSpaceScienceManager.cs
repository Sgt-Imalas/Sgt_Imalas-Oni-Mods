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
        List<String> DeepSpaceTechs = new List<String>();


        [Serialize]
        float CurrentScienceValue = 0;

        void ApplySciencePoints()
        {
            if(CurrentScienceValue > 1f)
            {
                --CurrentScienceValue;
                //++deepscienceresearch
            }
        }

        public void ScienceResearched(string researchType)
        {
            if (DeepSpaceScienceUnlocked)
            {
                switch (researchType)
                {
                    case ResearchTypes.ID.BASIC:
                        CurrentScienceValue += 0.1f;
                        break;
                    case ResearchTypes.ID.ADVANCED:
                        CurrentScienceValue += 0.2f;
                        break;
                    case ResearchTypes.ID.NUCLEAR:
                        CurrentScienceValue += 0.4f;
                        break;
                    case ResearchTypes.ID.ORBITAL:
                        CurrentScienceValue += 0.4f;
                        break;
                }
                ApplySciencePoints();
            }
        }
        public bool DeepSpaceScienceUnlocked => ModAssets.Techs.DockingTech.IsComplete();
    }
}
