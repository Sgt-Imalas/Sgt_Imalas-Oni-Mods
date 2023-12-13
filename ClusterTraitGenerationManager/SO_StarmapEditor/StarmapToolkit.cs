using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UtilLibs;
using UtilLibs.UIcmp;
using static ClusterTraitGenerationManager.STRINGS.UI.CGM_MAINSCREENEXPORT.ITEMSELECTION;
using static ResearchTypes;
using static STRINGS.ELEMENTS;
using static STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS;

namespace ClusterTraitGenerationManager.SO_StarmapEditor
{
    internal class StarmapToolkit : KMonoBehaviour
    {
        public class ToolkitDraggable : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
        {
            public Transform tParent, dragParent;
            public string poiId;
            public Image ownImage;
            public Vector3 DragStartPosition;
            public void Init(string Id)
            {
                this.poiId = Id;
                this.tParent = transform;
                this.dragParent = transform.parent.parent.parent.parent.parent.parent;
                TryGetComponent(out ownImage);
            }

            public void OnDrag(PointerEventData eventData)
            {
                transform.SetParent(dragParent);
                transform.position = eventData.position;
            }

            public void OnBeginDrag(PointerEventData eventData)
            {
                DragStartPosition = transform.position;
                ownImage.raycastTarget = false;
               // InternalImage.raycastTarget = false;
            }

            public void OnEndDrag(PointerEventData eventData)
            {
                transform.SetParent(tParent);
                transform.position = DragStartPosition;
                ownImage.raycastTarget = true;
               // InternalImage.raycastTarget = true;
            }
        }



        static Color toolkitEntryNormal = UIUtils.rgb(34, 39, 60), toolkitEntryMissing = UIUtils.rgb(153, 123, 17), tooltipWarningColor = UIUtils.rgb(255, 237, 30);

        class ToolkitItem: KMonoBehaviour
        {
            Image bgImage, fgImage;
            LocText Label;
            string desc;
            ToolTip toolTip;
            bool currentlyMissing = false;
            public bool IsMissing => currentlyMissing;
            public void SetMissing(bool isMissing)
            {
                currentlyMissing = isMissing;
                bgImage.color = isMissing ? toolkitEntryMissing : toolkitEntryNormal;
                toolTip.SetSimpleTooltip(isMissing ? desc + "\n\n" + UIUtils.ColorText(FOOTER.TOOLBOX.BOXOFPOI.POINOTINSTARMAP, tooltipWarningColor) : desc);
            }
            public void Init(string poiID)
            {
                bgImage = gameObject.GetComponent<Image>();
                fgImage = transform.Find("Image").gameObject.GetComponent<Image>();
                fgImage.gameObject.AddOrGet<ToolkitDraggable>().Init(poiID);


                Label = transform.Find("Label").gameObject.GetComponent<LocText>();
                if (ModAssets.SO_POIs.ContainsKey(poiID))
                {
                    //SgtLogger.l("isPOI");
                    var data = ModAssets.SO_POIs[poiID];
                    fgImage.sprite = data.Sprite;
                    var rectTransform = fgImage.rectTransform();
                    UnityEngine.Rect rect = fgImage.sprite.rect;
                    float width = rectTransform.rect.width;

                    if (rect.width > rect.height)
                    {
                        var size = (rect.height / rect.width) * width;
                        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
                        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
                    }
                    else
                    {
                        var size = (rect.width / rect.height) * width;
                        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
                        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, width);
                    }
                    Label.SetText(data.Name);
                    toolTip = UIUtils.AddSimpleTooltipToObject(gameObject, data.Description);
                    desc = data.Description;
                }
            }
        }


        private GameObject ToolboxPOIContainer;
        private GameObject ToolboxPOIPrefab;
        private FInputField2 POIFilter;
        private FToggle2 alwaysShowNames;
        private HexGrid Grid;
        private GameObject TrashCan;
        Dictionary<string,ToolkitItem> ToolboxItems;

        private GameObject HexGridGO;
        private GameObject FooterGO;

        private bool _init =false;
        private bool wasActive = false; 


        public void SetActive(bool active, bool isInit = false)
        {
            if(wasActive!= active || isInit)
            {
                wasActive = active;
                Init();
                Show(active);
            }
        }

        private void Show(bool active)
        {
            HexGridGO.SetActive(active);
            FooterGO.SetActive(active);
            var currentLayout = CGSMClusterManager.GeneratedLayout;
            Grid.MapRadius = currentLayout.numRings;
            Grid.InitGrid();
            Grid.InitPositions();
            foreach (var item in Grid.ActiveItems.Values)
            {
                if (ToolboxItems.ContainsKey(item.ID) && ToolboxItems[item.ID].IsMissing)
                {
                    ToolboxItems[item.ID].SetMissing(false);
                }
            }
        }

        public void Init()
        {
            if (_init) return;
            _init = true;

            FooterGO = transform.Find("Footer").gameObject;
            HexGridGO = transform.Find("SpacedOutStarmapContent").gameObject;

            ToolboxPOIContainer = transform.Find("Footer/Toolbox/BoxOfPoi/ScrollArea/Content").gameObject;
            ToolboxPOIPrefab = transform.Find("Footer/Toolbox/BoxOfPoi/ScrollArea/Content/Item").gameObject;
            ToolboxPOIPrefab.SetActive(false);
            Grid = transform.Find("SpacedOutStarmapContent/ItemInfo/ScrollArea/Content").gameObject.AddOrGet<HexGrid>();
            var pointerHandler = transform.Find("SpacedOutStarmapContent/ItemInfo").gameObject.AddOrGet<StarmapCursorHandler>();
            pointerHandler.grid = Grid;

            Grid.EntryPrefab = transform.Find("SpacedOutStarmapContent/ItemInfo/ScrollArea/Content/HexagonBG").gameObject;
            Grid.DraggablePrefab = transform.Find("SpacedOutStarmapContent/ItemInfo/ScrollArea/Content/HexDrag").gameObject;
            Grid.EntryPrefab.SetActive(false);
            Grid.DraggablePrefab.SetActive(false);

            TrashCan = transform.Find("Footer/Toolbox/TrashCanContainer/Trashcan").gameObject;
            var handler = TrashCan.AddOrGet<DeleteDragHandler>();
            handler.secondaryHighlight = TrashCan.transform.Find("TrashcanIcon").gameObject.GetComponent<Image>(); 
            handler.hexGrid = Grid;

            alwaysShowNames = transform.Find("Footer/Toolbox/TrashCanContainer/ShowPermaLabels").gameObject.AddOrGet<FToggle2>();
            alwaysShowNames.SetCheckmark("Background/Checkmark");
            alwaysShowNames.On = Grid.AlwaysShowNames;
            alwaysShowNames.OnClick += () => Grid.AlwaysShowNames = alwaysShowNames.On;

            ToolboxItems = new Dictionary<string, ToolkitItem>();

            foreach(var poi in ModAssets.SO_POIs)
            {
                var toolKitGO = Util.KInstantiateUI(ToolboxPOIPrefab, ToolboxPOIContainer,true);
                var toolkitItem = toolKitGO.AddOrGet<ToolkitItem>();
                toolkitItem.Init(poi.Key);

                ToolboxItems.Add(poi.Key, toolkitItem);
                toolkitItem.SetMissing(true);
            }
        }
    }
}
