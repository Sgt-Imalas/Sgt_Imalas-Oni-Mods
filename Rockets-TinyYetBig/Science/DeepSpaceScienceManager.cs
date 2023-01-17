using Database;
using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UnityEngine.GraphicsBuffer;

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

        void ApplySciencePoints()
        {
            if (CurrentScienceValue > 1f)
            {
                --CurrentScienceValue;
                //++deepscienceresearch
                if (GetCurrentDeepSpaceReserach(out var target))
                {
                    var techInstance = Research.Instance.Get(target);
                    if (techInstance != null)
                    {
                        techInstance.progressInventory.AddResearchPoints(ModAssets.DeepSpaceScienceID, 1);
                    }
                }
            }
        }
        List<string> DeepSpaceTechs = new List<string>()
        {
            ModAssets.Techs.SpaceStationTechMediumID,
            ModAssets.Techs.SpaceStationTechLargeID
        };

        bool GetCurrentDeepSpaceReserach(out Tech Target)
        {
            Target = null;
            if (DeepSpaceScienceUnlocked)
            {
                foreach (var Tech in DeepSpaceTechs)
                {
                    var potentialDeepSpaceTech = Db.Get().Techs.TryGetTechForTechItem(Tech);

                    if (potentialDeepSpaceTech.IsComplete())
                        continue;

                    if (potentialDeepSpaceTech.RequiresResearchType(ModAssets.DeepSpaceScienceID) &&
                        potentialDeepSpaceTech.ArePrerequisitesComplete() &&
                        Research.Instance.Get(potentialDeepSpaceTech).PercentageCompleteResearchType(ModAssets.DeepSpaceScienceID) < 1f)
                    {
                        Target = potentialDeepSpaceTech;
                        return true;
                    }
                }
            }
            return false;
        }

        public void ArtifactResearched (bool terrestial)
        {
            if (terrestial)
                CurrentScienceValue += 10;
            else
                CurrentScienceValue += 5;
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
        public bool DeepSpaceScienceUnlocked => ModAssets.Techs.SpaceStationTech.IsComplete();
    }
}
