using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using UtilLibs.UIcmp;
using static BlueprintsV2.STRINGS.UI.BLUEPRINTSELECTOR.MATERIALSWITCH.SCROLLAREA.CONTENT;
using BlueprintsV2.BlueprintsV2.BlueprintData;
using UnityEngine.UI;
using static UnityEngine.UI.Image;

namespace BlueprintsV2.BlueprintsV2.UnityUI.Components
{
    internal class BlueprintElementEntry : KMonoBehaviour
    {
        public BlueprintSelectedMaterial SelectedAndCategory;
        public System.Action<BlueprintSelectedMaterial, float> OnEntryClicked;
        LocText ElementName;
        LocText ElementAmount;
        float amount;
        LocText ReplaceElementName;
        GameObject warningIndicator, severeWarningIndicator;
        FButton button;
        Image ElementIcon, ReplacementElementIcon;
        ToolTip tooltip;
        bool staticTag = false;
        string staticTooltip;

        void OnClick()
        {
            OnEntryClicked?.Invoke(SelectedAndCategory, amount);
        }
        static bool init = false;
        public override void OnPrefabInit()
        {
            if (!init)
            {
                init = true;
                UIUtils.ListAllChildren(this.transform);
            }

            base.OnPrefabInit();
            ElementName = transform.Find("Label").gameObject.GetComponent<LocText>();
            ElementAmount = transform.Find("MassLabel").gameObject.GetComponent<LocText>();
            ReplaceElementName = transform.Find("ReplacementElement").gameObject.GetComponent<LocText>();
            warningIndicator = transform.Find("Warning").gameObject;
            severeWarningIndicator = transform.Find("WarningSevere").gameObject;
            ElementIcon = transform.Find("ElementIcon").gameObject.GetComponent<Image>();
            ReplacementElementIcon = transform.Find("ReplaceElementIcon").gameObject.GetComponent<Image>();

            button = gameObject.AddOrGet<FButton>();

            if (SelectedAndCategory != null)
            {
                button.OnClick += OnClick;
                var selectedTag = SelectedAndCategory.SelectedTag;


                if (ModAssets.IsStaticTag(SelectedAndCategory, out string name, out string desc, out Sprite sprite))
                {
                    ElementName?.SetText(name);
                    tooltip = UIUtils.AddSimpleTooltipToObject(this.gameObject, desc);
                    ElementIcon.sprite = sprite;
                    button.SetInteractable(false);
                    staticTag = true;
                    staticTooltip = desc;
                }
                else
                {
                    var prefab = Assets.TryGetPrefab(selectedTag);

                    if (prefab != null)
                    {

                        var icoSprite = Def.GetUISprite(prefab);
                        if (icoSprite!=null) 
                        { 
                        ElementIcon.sprite = icoSprite.first;
                        ElementIcon.color = icoSprite.second;
                        }
                        else
                            SgtLogger.warning("no ui sprite found for "+prefab.name);
                        

                        ElementName?.SetText(prefab.GetProperName());
                        tooltip = UIUtils.AddSimpleTooltipToObject(this.gameObject, GameUtil.GetMaterialTooltips(selectedTag));
                    }
                    else
                    {
                        ElementName?.SetText(selectedTag.Name);
                    }
                }

                UnityEngine.Rect rect = ElementIcon.sprite.rect;
                if (rect.width > rect.height)
                {
                    var size = (rect.height / rect.width) * 30;
                    ElementIcon.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
                }
                else
                {
                    var size = (rect.width / rect.height) * 30;
                    ElementIcon.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
                }

                SetWarningIndicatorLevel(0);
                Refresh(null);
            }

        }
        public override void OnSpawn()
        {
            base.OnSpawn();
        }


        public void Refresh(Blueprint current)
        {

            if (current == null)
                return;

            Tag targetTag = SelectedAndCategory.SelectedTag;
            
            if(ModAssets.TryGetReplacementTag(SelectedAndCategory, out var replacement))
            {
                targetTag = replacement;

                ReplacementElementIcon.gameObject.SetActive(true);
                var prefab = Assets.TryGetPrefab(replacement);
                if (prefab == null)
                {
                    SgtLogger.warning(replacement + " prefab was null!");
                    return;
                }


                var icoSprite = Def.GetUISprite(prefab);
                ReplacementElementIcon.sprite = icoSprite.first;
                ReplacementElementIcon.color = icoSprite.second;

                UnityEngine.Rect rect = ReplacementElementIcon.sprite.rect;
                if (rect.width > rect.height)
                {
                    var size = (rect.height / rect.width) * 30;
                    ReplacementElementIcon.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
                }
                else
                {
                    var size = (rect.width / rect.height) * 30;
                    ReplacementElementIcon.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
                }
                ReplaceElementName?.SetText(prefab.GetProperName());
            }
            else
            {
                ReplaceElementName?.SetText(PRESETENTRYPREFAB.NONE);
                ReplacementElementIcon.gameObject.SetActive(false);
            }

            float requiredAmount = 0;

            if (current.CachedAbsTagCost.TryGetValue(targetTag, out float costs))
            {
                requiredAmount += costs;
            }

            float currentWorldAmount = ClusterManager.Instance.activeWorld.worldInventory.GetAmount(targetTag, true);
            bool materialUnlocked = BlueprintState.InstantBuild || staticTag || DiscoveredResources.Instance.IsDiscovered(targetTag);
            bool enoughMaterial = BlueprintState.InstantBuild || requiredAmount <= currentWorldAmount;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(staticTag ? staticTooltip : GameUtil.GetMaterialTooltips(targetTag));
            if (!materialUnlocked)
            {
                sb.AppendLine();
                sb.AppendLine(STRINGS.UI.BLUEPRINTSELECTOR.MATERIALREPLACER.SCROLLAREA.CONTENT.ELEMENTSTATE.NOTFOUND);
                SetWarningIndicatorLevel(2);
            }
            else if(!enoughMaterial)
            {
                SetWarningIndicatorLevel(1); 
                sb.AppendLine();
                sb.AppendLine(string.Format(STRINGS.UI.BLUEPRINTSELECTOR.MATERIALREPLACER.SCROLLAREA.CONTENT.ELEMENTSTATE.NOTENOUGH, 
                    GameUtil.GetFormattedMass(currentWorldAmount), 
                    GameUtil.GetFormattedMass(requiredAmount)));

            }
            else
            {
                SetWarningIndicatorLevel(0);
            }
            tooltip?.SetSimpleTooltip(sb.ToString());

        }

        public void SetTotalAmount(float amount)
        {
            this.amount = amount;
            ElementAmount?.SetText(GameUtil.GetFormattedMass(amount));
        }

        public void SetWarningIndicatorLevel(int level)
        {
            switch (level)
            {
                case 0:
                    warningIndicator.SetActive(false);
                    severeWarningIndicator.SetActive(false);
                    break;
                case 1:
                    warningIndicator.SetActive(true);
                    severeWarningIndicator.SetActive(false);
                    break;
                case 2:
                    warningIndicator.SetActive(false);
                    severeWarningIndicator.SetActive(true);
                    break;
            }
        }
    }
}
