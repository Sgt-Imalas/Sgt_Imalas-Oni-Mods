using Rockets_TinyYetBig.Buildings.Nosecones;
using Rockets_TinyYetBig.Buildings.Utility;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Rockets_TinyYetBig.Content.Scripts.UI.Sidescreens
{
    class ResourceHarvestModuleHEPSideScreen : SideScreenContent, ISim200ms
    {
        private Clustercraft targetCraft = null;

        public GameObject moduleContentContainer;
        public GameObject modulePanelPrefab;
        public void Flush()
        {
            targetCraft = null;
            lastPercentageState = -2f;
            lastMassStored = -2f;
            drillerStorage = null;
            moduleInstance = null;
            hepStorages = null;
        }

        float lastPercentageState = -1f;
        float lastMassStored = -1f;
        public HighEnergyParticleStorage drillerStorage = null;
        public List<HighEnergyParticleStorage> hepStorages = null;
        public ResourceHarvestModuleHEP.StatesInstance moduleInstance;


        GenericUIProgressBar progressBar;
        GenericUIProgressBar diamondProgressBar;

        public HierarchyReferences hierarchyReferences = null;

        private CraftModuleInterface craftModuleInterface => targetCraft.GetComponent<CraftModuleInterface>();

        public override void OnShow(bool show)
        {
            base.OnShow(show);
            ConsumeMouseScroll = true;
        }

        public override float GetSortKey() => 21f;

        public override bool IsValidForTarget(GameObject target) => target.TryGetComponent<Clustercraft>(out var clustercraft) && HasResourceHarvestModule(clustercraft);

        public override void OnSpawn()
        {
            base.OnSpawn();
            TryGetComponent(out hierarchyReferences);
            progressBar = hierarchyReferences.GetReference<GenericUIProgressBar>("progressBar");
            diamondProgressBar = hierarchyReferences.GetReference<GenericUIProgressBar>("diamondProgressBar");
        }
        public override void SetTarget(GameObject target)
        {
            Flush();
            base.SetTarget(target);
            TryGetComponent(out hierarchyReferences);
            target.TryGetComponent(out targetCraft);
            GetResourceHarvestModule(targetCraft);
            RefreshModulePanel(moduleInstance);
        }

        private bool HasResourceHarvestModule(
          Clustercraft craft)
        {
            foreach (Ref<RocketModuleCluster> clusterModule in craft.GetComponent<CraftModuleInterface>().ClusterModules)
            {
                GameObject gameObject = clusterModule.Get().gameObject;
                if (gameObject.GetDef<ResourceHarvestModuleHEP.Def>() != null)
                    return gameObject.GetSMI<ResourceHarvestModuleHEP.StatesInstance>() != null;
            }
            return false;
        }
        private void GetResourceHarvestModule(
          Clustercraft craft)
        {
            hepStorages = new List<HighEnergyParticleStorage>();
            foreach (Ref<RocketModuleCluster> clusterModule in craft.GetComponent<CraftModuleInterface>().ClusterModules)
            {
                GameObject gameObject = clusterModule.Get().gameObject;
                if (gameObject.GetDef<ResourceHarvestModuleHEP.Def>() != null)
                {
                    moduleInstance = gameObject.GetSMI<ResourceHarvestModuleHEP.StatesInstance>();
                    moduleInstance.gameObject.TryGetComponent(out drillerStorage);
                    hepStorages.Add(drillerStorage);
                }
                if (gameObject.TryGetComponent<DrillConeAssistentModuleHEP>(out var moduleHEP))
                {
                    hepStorages.Add(moduleHEP.HEPStorage);
                }
            }
        }
        private void RefreshModulePanel(StateMachine.Instance module)
        {
            hierarchyReferences.GetReference<Image>("icon").sprite = Def.GetUISprite(module.gameObject).first;
            hierarchyReferences.GetReference<LocText>("label").SetText(module.gameObject.GetProperName());
        }

        public void Sim200ms(float dt)
        {
            if (targetCraft.IsNullOrDestroyed())
                return;

            float miningProgress = moduleInstance.sm.canHarvest.Get(moduleInstance) ? moduleInstance.timeinstate % 4f / 4f : -1f;

            if (!Mathf.Approximately(miningProgress, lastPercentageState))
            {
                progressBar.SetFillPercentage(miningProgress > -1f ? miningProgress : 0f);
                progressBar.label.SetText(miningProgress > -1f ? (string)global::STRINGS.UI.UISIDESCREENS.HARVESTMODULESIDESCREEN.MINING_IN_PROGRESS : (string)global::STRINGS.UI.UISIDESCREENS.HARVESTMODULESIDESCREEN.MINING_STOPPED);
                lastPercentageState = miningProgress;
            }
            GetParticleCounts(out var capacity, out var currentBolts);
            if (!Mathf.Approximately(currentBolts, lastMassStored))
            {
                diamondProgressBar.SetFillPercentage(currentBolts / capacity);
                diamondProgressBar.label.SetText(global::STRINGS.UI.UNITSUFFIXES.HIGHENERGYPARTICLES.PARTRICLES + ": " + currentBolts.ToString("0.#"));
                lastMassStored = currentBolts;
            }
        }
        public void GetParticleCounts(out float max, out float current)
        {
            current = 0;
            max = 0;

            if (hepStorages == null)
                return;

            foreach (var hepStorage in hepStorages)
            {
                current += hepStorage.Particles;
                max += hepStorage.Capacity();
            }
        }
    }
}
