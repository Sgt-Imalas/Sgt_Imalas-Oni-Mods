using Rockets_TinyYetBig.Buildings;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Rockets_TinyYetBig.Behaviours
{
    class NoseConeHEPHarvestSideScreen : SideScreenContent, ISimEveryTick
    {
        private Clustercraft targetCraft;
        public GameObject moduleContentContainer;
        public GameObject modulePanelPrefab;

        private CraftModuleInterface craftModuleInterface => this.targetCraft.GetComponent<CraftModuleInterface>();

        protected override void OnShow(bool show)
        {
            base.OnShow(show);
            this.ConsumeMouseScroll = true;
        }

        public override float GetSortKey() => 21f;

        public override bool IsValidForTarget(GameObject target) => target.GetComponent<Clustercraft>() != null && this.GetResourceHarvestModule(target.GetComponent<Clustercraft>()) != null;

        public override void SetTarget(GameObject target)
        {
            base.SetTarget(target);
            this.targetCraft = target.GetComponent<Clustercraft>();
            this.RefreshModulePanel((StateMachine.Instance)this.GetResourceHarvestModule(this.targetCraft));
        }

        private NoseConeHEPHarvest.StatesInstance GetResourceHarvestModule(
          Clustercraft craft)
        {
            foreach (Ref<RocketModuleCluster> clusterModule in (IEnumerable<Ref<RocketModuleCluster>>)craft.GetComponent<CraftModuleInterface>().ClusterModules)
            {
                GameObject gameObject = clusterModule.Get().gameObject;
                if (gameObject.GetDef<NoseConeHEPHarvest.Def>() != null)
                    return gameObject.GetSMI<NoseConeHEPHarvest.StatesInstance>();
            }
            return (NoseConeHEPHarvest.StatesInstance)null;
        }

        private void RefreshModulePanel(StateMachine.Instance module)
        {
            HierarchyReferences component = this.GetComponent<HierarchyReferences>();
            component.GetReference<Image>("icon").sprite = Def.GetUISprite((object)module.gameObject).first;
            component.GetReference<LocText>("label").SetText(module.gameObject.GetProperName());
        }

        public void SimEveryTick(float dt)
        {
            if (this.targetCraft.IsNullOrDestroyed())
                return;
            HierarchyReferences component1 = this.GetComponent<HierarchyReferences>();
            NoseConeHEPHarvest.StatesInstance resourceHarvestModule = this.GetResourceHarvestModule(this.targetCraft);
            if (resourceHarvestModule == null)
                return;
            GenericUIProgressBar reference1 = component1.GetReference<GenericUIProgressBar>("progressBar");
            float num1 = 4f;
            float num2 = resourceHarvestModule.timeinstate % num1;
            if (resourceHarvestModule.sm.canHarvest.Get(resourceHarvestModule))
            {
                reference1.SetFillPercentage(num2 / num1);
                reference1.label.SetText((string)global::STRINGS.UI.UISIDESCREENS.HARVESTMODULESIDESCREEN.MINING_IN_PROGRESS);
            }
            else
            {
                reference1.SetFillPercentage(0.0f);
                reference1.label.SetText((string)global::STRINGS.UI.UISIDESCREENS.HARVESTMODULESIDESCREEN.MINING_STOPPED);
            }
            GenericUIProgressBar reference2 = component1.GetReference<GenericUIProgressBar>("diamondProgressBar");
            HighEnergyParticleStorage component2 = resourceHarvestModule.GetComponent<HighEnergyParticleStorage>();
            reference2.SetFillPercentage(component2.Particles / component2.Capacity());
            reference2.label.SetText(UI.UNITSUFFIXES.HIGHENERGYPARTICLES.PARTRICLES + ": " + component2.Particles);
        }
    }
}
