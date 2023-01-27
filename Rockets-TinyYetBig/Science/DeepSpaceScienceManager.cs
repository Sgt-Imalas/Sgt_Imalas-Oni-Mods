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
            for (float i = CurrentScienceValue; i > 0; i--)
            {
                //++deepscienceresearch
                if (GetCurrentDeepSpaceReserach(out var target))
                {
                    var techInstance = Research.Instance.Get(target);
                    if (techInstance != null)
                    {
                        int pointsToAdd = (int)Math.Min((techInstance.tech.costsByResearchTypeID[ModAssets.DeepSpaceScienceID] - techInstance.progressInventory.PointsByTypeID[ModAssets.DeepSpaceScienceID]), i);
                        techInstance.progressInventory.AddResearchPoints(ModAssets.DeepSpaceScienceID, pointsToAdd);
                        CurrentScienceValue -= pointsToAdd;
                    }
                }
            }
        }
        List<string> DeepSpaceTechs = new List<string>()
        {
            ModAssets.Techs.SpaceStationTechID,
            ModAssets.Techs.SpaceStationTechMediumID,
            ModAssets.Techs.SpaceStationTechLargeID
        };

        bool GetCurrentDeepSpaceReserach(out Tech Target)
        {
            Target = null;
            var Techs = Db.Get().Techs;
            for (int i = 0; i< Techs.Count; ++i )
            {
                var potentialDeepSpaceTech = Techs[i];
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
            for (int i = 0; i < Techs.Count; ++i)
            {
                var potentialDeepSpaceTech2 = Techs[i];
                if (potentialDeepSpaceTech2.IsComplete())
                    continue;

                if (potentialDeepSpaceTech2.RequiresResearchType(ModAssets.DeepSpaceScienceID)&&
                    Research.Instance.Get(potentialDeepSpaceTech2).PercentageCompleteResearchType(ModAssets.DeepSpaceScienceID) < 1f)
                {
                    Target = potentialDeepSpaceTech2;
                    return true;
                }
            }


            return false;
        }

        public void ArtifactResearched(bool terrestial)
        {
            if (terrestial)
                CurrentScienceValue += 2;
            else
                CurrentScienceValue += 1;
            ApplySciencePoints();
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
