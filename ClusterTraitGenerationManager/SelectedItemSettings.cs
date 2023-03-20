using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.UI;
using UtilLibs;
using UtilLibs.UIcmp;
using static ClusterTraitGenerationManager.CGSMClusterManager;
using static ClusterTraitGenerationManager.STRINGS.UI.CGM.INDIVIDUALSETTINGS;
using static ClusterTraitGenerationManager.STRINGS.UI.CGM.INDIVIDUALSETTINGS.ASTEROIDTRAITS;
using static STRINGS.UI.UISIDESCREENS.AUTOPLUMBERSIDESCREEN;

namespace ClusterTraitGenerationManager
{
    internal class SelectedItemSettings : FScreen
    {
        private FToggle2 StarmapItemEnabled;

        private FSlider NumberToGenerate;

        private FSlider MinimumDistance;

        private FSlider MaximumDistance;

        private FSlider BufferDistance;

        private FSlider ClusterSize;
        
        
        public FButton AddTraitButton;


        public FButton ResetButton;
        public FButton ResetAllButton;
        public FButton ReturnButton;
        public FButton GenerateClusterButton;

        public System.Action OnClose;

        public void NewItemSelected(StarmapItem item)
        {

        }
        private bool init;
        private void Init()
        {
            StarmapItemEnabled = transform.Find("StarmapItemEnabled").FindOrAddComponent<FToggle2>();
            StarmapItemEnabled.SetCheckmark("Background/Checkmark");

            NumberToGenerate = transform.Find("AmountSlider").FindOrAddComponent<FSlider>();
            UIUtils.FindAndDisable(NumberToGenerate.transform, "Descriptor/Input");

            MinimumDistance = transform.Find("MinDistanceSlider").FindOrAddComponent<FSlider>();
            UIUtils.FindAndDisable(MinimumDistance.transform, "Descriptor/Input");

            MaximumDistance = transform.Find("MaxDistanceSlider").FindOrAddComponent<FSlider>();
            UIUtils.FindAndDisable(MaximumDistance.transform, "Descriptor/Input");

            BufferDistance = transform.Find("BufferSlider").FindOrAddComponent<FSlider>();
            UIUtils.FindAndDisable(BufferDistance.transform, "Descriptor/Input");

            ClusterSize = transform.Find("ClusterSize").FindOrAddComponent<FSlider>();
            UIUtils.FindAndDisable(ClusterSize.transform, "Descriptor/Input");

            AddTraitButton = transform.Find("AsteroidTraits/AddTraitButton").FindOrAddComponent<FButton>();

            ReturnButton = transform.Find("Buttons/ReturnButton").FindOrAddComponent<FButton>();
            ReturnButton.OnClick += OnClose ;

            GenerateClusterButton = transform.Find("Buttons/GenerateClusterButton").FindOrAddComponent<FButton>();
            GenerateClusterButton.OnClick += () => CGSMClusterManager.InitializeGeneration();
            
            ResetButton = transform.Find("Buttons/ResetSelectionButton").FindOrAddComponent<FButton>(); 

            ResetAllButton = transform.Find("Buttons/ResetClusterButton").FindOrAddComponent<FButton>();
            ResetAllButton.OnClick += () =>
            {
                CGSMClusterManager.ResetToLastPreset();
                //RefreshView();
            };

            UIUtils.AddSimpleTooltipToObject(ResetAllButton.transform, STRINGS.UI.CUSTOMCLUSTERUI.RESET.DESC, true);




            //NumberToGenerate.transform.Find("Descriptor/Input").FindOrAddComponent<FInputField2>();
            //NumberToGenerate.AttachInputField(NumberToGenerate.transform.Find("Descriptor/Input").FindOrAddComponent<FNumberInputField>());

            //colorInput = transform.Find("Content/Color/Input").gameObject.AddOrGet<FInputField2>();
            //patternInput = transform.Find("Content/Pattern/Input").gameObject.AddOrGet<FInputField2>();

            //renderCycle = transform.Find("Content/RenderLayerPreset").gameObject.AddOrGet<FCycle>();
            //renderCycle.Initialize(
            //    renderCycle.transform.Find("Left").gameObject.AddOrGet<FButton>(),
            //    renderCycle.transform.Find("Right").gameObject.AddOrGet<FButton>(),
            //    renderCycle.transform.Find("ChoiceLabel").gameObject.AddOrGet<LocText>());

            //transform.Find("VersionLabel").GetComponent<LocText>().text = $"v{Log.GetVersion()}";

            init = true;
        }
        public override void OnShow(bool show)
        {
            base.OnShow(show);
            if (!init)
            {
                Init();
            }
        }
    }
}
