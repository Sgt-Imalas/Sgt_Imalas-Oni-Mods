using Klei.CustomSettings;
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
using static AnimEventHandler;
using static Database.MonumentPartResource;
using static NameDisplayScreen;

namespace ClusterTraitGenerationManager.SO_StarmapEditor
{
    internal class HexGrid : FScreen, IPointerEnterHandler, IPointerExitHandler
    {

        internal class HexDropHandler : KMonoBehaviour, IDropHandler
        {
            HexGrid parent;
            Tuple<int,int> HexPos;
            public void Init(int _x, int _y, HexGrid _parent)
            {
                HexPos = new(_x, _y);   
                parent = _parent;
            }


            public void OnDrop(PointerEventData eventData)
            {
                if(!parent.HexOccupied(HexPos) && eventData.pointerDrag.TryGetComponent<HexDrag>(out HexDrag hexDragger))
                {
                    hexDragger.DragStartPosition = transform.position;
                    parent.MovePOIToNewLocation(hexDragger,HexPos);
                }
            }
        }

        private void MovePOIToNewLocation(HexDrag hexDragger, Tuple<int, int> hexPos)
        {
            ActiveItems.Remove(hexDragger.InternalPos);
            ActiveItems.Add(hexDragger.InternalPos, hexDragger);
            hexDragger.InternalPos = hexPos;
            //TODO: logic
        }

        internal class HexDrag : KMonoBehaviour, IDragHandler,IBeginDragHandler, IEndDragHandler
        {

            [MyCmpGet]
            RectTransform rectTransform;

            HexGrid parent;
            public Tuple<int, int> InternalPos;
            Image InternalImage;
            Image ownImage;
            string ownId;
            public void Init(HexGrid _parent, int _x, int _y, string starmapItemId)
            {
                //UIUtils.ListAllChildren(transform);
                gameObject.TryGetComponent<Image>(out ownImage);
                parent = _parent;
                InternalPos = new Tuple<int, int>(_x,_y);
                InternalImage = transform.Find("ButtonContainerImage").gameObject.GetComponent<Image>();
                ownId = starmapItemId;
                SetStarmapItem(ownId);
            }


            public void SetStarmapItem(string ID = null)
            {
                ownId = ID;
                if (ID != null)
                {
                    if (CGSMClusterManager.PlanetoidDict.ContainsKey(ID))
                    {
                        SgtLogger.l("isPlanetoid");
                        SetContainerSprite(CGSMClusterManager.PlanetoidDict[ID].planetSprite);
                    }
                    else if (ModAssets.SO_POIs.ContainsKey(ID))
                    {
                        SgtLogger.l("isPOI");
                        SetContainerSprite(ModAssets.SO_POIs[ID].Sprite);
                    }
                    else
                    {
                        SgtLogger.l("IsNothing");
                        SetContainerSprite(null);
                    }
                }
            }

            public void SetContainerSprite(Sprite _sprite)
            {
                if (_sprite != null)
                {
                    InternalImage.gameObject.SetActive(true);
                    InternalImage.sprite = _sprite;
                    var rectTransform = InternalImage.rectTransform();
                    UnityEngine.Rect rect = InternalImage.sprite.rect;
                    if (rect.width > rect.height)
                    {
                        var size = (rect.height / rect.width) * 60f;
                        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
                        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 60);
                    }
                    else
                    {
                        var size = (rect.width / rect.height) * 60f;
                        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
                        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 60);
                    }
                }
                else
                {
                    InternalImage.gameObject.SetActive(false);
                }
            }
            public Vector3 DragStartPosition;
            public void OnDrag(PointerEventData eventData)
            {
                transform.position = eventData.position;
            }

            public void OnBeginDrag(PointerEventData eventData)
            {
                DragStartPosition = transform.position;
                ownImage.raycastTarget = false;
                InternalImage.raycastTarget = false;
            }

            public void OnEndDrag(PointerEventData eventData)
            {
                transform.position = DragStartPosition;
                ownImage.raycastTarget = true;
                InternalImage.raycastTarget = true;
            }

            public void SetPositionXY(Tuple<int,int> pos)
            {
                rectTransform.anchoredPosition = parent.GetPosForHex(pos.first, pos.second);
            }
        }

        Tuple<int, int>[] axial_direction_vectors = new Tuple<int, int>[]
        {
            new (1,0),
            new (1,-1),
            new (0,-1),
            new (-1,0),
            new (-1,1),
            new (0,1),
        };
        public Vector2 GetPosForHex(int x, int y) => new(x * XStep + (0.5f * y * XStep), (-y * YStep));
        public Tuple<int,int> GetHexPosForPos(Vector2 pos)
        {
            var y = (Mathf.Sqrt(3f) / 3f * pos.x - 1f / 3f * pos.y) / YStep;
            var x = (2f / 3f * pos.y) / XStep ;

            return new Tuple<int, int>(Mathf.RoundToInt(x), Mathf.RoundToInt(y));
        }

        [SerializeField]
        public float CellRadius = 50;
        [SerializeField]
        public float BufferDistance = 4;
        [SerializeField]
        public int MapRadius = 5;



        Dictionary<Tuple<int, int>, GameObject> GridItems = new Dictionary<Tuple<int, int>, GameObject>();
        Dictionary<Tuple<int, int>, HexDrag> ActiveItems = new Dictionary<Tuple<int, int>, HexDrag>();
        //GameObject[,] GridItems;
        //Button[,] GridButtons;

        [SerializeField]
        public GameObject EntryPrefab;
        public GameObject DraggablePrefab;
        public GameObject EntryParent;

        public override void OnKeyDown(KButtonEvent e)
        {
            if (!e.Consumed && (e.IsAction(Action.ZoomIn) || e.IsAction(Action.ZoomOut)))
            {
                //List<RaycastResult> raycastResults = new List<RaycastResult>();
                //PointerEventData eventData = new PointerEventData(UnityEngine.EventSystems.EventSystem.current);
                //eventData.position = (Vector2)KInputManager.GetMousePos();
                //UnityEngine.EventSystems.EventSystem current = UnityEngine.EventSystems.EventSystem.current;
                //if ((UnityEngine.Object)current != (UnityEngine.Object)null)
                //{
                //    current.RaycastAll(eventData, raycastResults);
                //    bool flag = false;
                //    foreach (RaycastResult raycastResult in raycastResults)
                //    {
                //        if (!raycastResult.gameObject.transform.IsChildOf(this.transform))
                //        {
                //            flag = true;
                //            break;
                //        }
                //    }
                //    if (!flag)
                if (true)
                {
                    this.m_targetZoomScale = Mathf.Clamp(this.m_targetZoomScale + (!KInputManager.currentControllerIsGamepad ? UnityEngine.Input.mouseScrollDelta.y * 0.1f : (float)((e.IsAction(Action.ZoomIn) ? -0.003 : 0.003))), 0.15f, 2f);

                    this.rectTransform().sizeDelta = new Vector2((float)(MapRadius * 220), (float)(MapRadius * 200));
                    this.rectTransform().localScale = new Vector3(this.m_targetZoomScale, this.m_targetZoomScale, 1f);

                    e.TryConsume(Action.ZoomIn);
                    if (!e.Consumed)
                        e.TryConsume(Action.ZoomOut);
                }
                // }
            }

            base.OnKeyDown(e);
        }

        float m_targetZoomScale = 1f;
        int lastRadius;
        float XStep, YStep;
        public void InitGrid()
        {
            int radius = MapRadius - 1;
            if (radius != lastRadius)
            {
                lastRadius = radius;
                foreach (var item in GridItems)
                {
                    UnityEngine.Object.Destroy(item.Value.gameObject);
                 //   item.Value.SelfDestruct();
                }

                GridItems.Clear();
                if (EntryParent == null)
                    EntryParent = gameObject;


                XStep = (CellRadius) * Mathf.Sqrt(3f) + BufferDistance;
                YStep = (3f / 2f) * (CellRadius + BufferDistance);


                for (int x = -radius; x <= radius; ++x)
                {
                    for (int y = -radius; y <= radius; ++y)
                    {
                        for (int index = -radius; index <= radius; ++index)
                        {

                            if (x + y + index != 0)
                                continue;
                            if (EntryPrefab != null)
                            {
                                var HexEntry = Util.KInstantiateUI(EntryPrefab, EntryParent);
                                HexEntry.SetActive(true);
                                HexEntry.TryGetComponent<RectTransform>(out var rect);
                                HexEntry.TryGetComponent<Image>(out var img);

                                rect.anchoredPosition = GetPosForHex(x,y);
                                HexEntry.TryGetComponent<Image>(out var hexVis);
                                hexVis.color = UIUtils.rgba(129, 73, 108, 0.75f);
                                var handler = HexEntry.AddOrGet<HexDropHandler>();
                                handler.Init(x, y, this);
                                GridItems.Add(new(x, y), HexEntry);
                            }
                            //GridButtons[x, y] = btn;

                        }
                    }
                }
            }
        }
        public bool CursorInside => cursorInside;

        public bool CurrentlySelectingNewPosition = true;

        private bool cursorInside = false;

        public void OnPointerEnter(PointerEventData eventData)
        {
            cursorInside = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            cursorInside = false;
        }

        bool HexOccupied(Tuple<int,int> key ) => ActiveItems.ContainsKey(key);

        internal void InitPositions()
        {
            foreach(var item in ActiveItems)
            {
                Destroy(item.Value.gameObject);
            }
            ActiveItems.Clear();

             var layout = new SO_StarmapLayout();
            layout.AssignClusterLocations(int.Parse(CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.WorldgenSeed).id));
            foreach (var dataEntry in layout.OverridePlacements)
            {
                //SgtLogger.l(dataEntry.Value.ToString(), dataEntry.Key.Q+","+ dataEntry.Key.R);
                int x = dataEntry.Key.R, y = dataEntry.Key.Q;

                var key = new Tuple<int, int>(x, y);

                if (!ActiveItems.ContainsKey(key))
                {
                    var StarmapItemEntry = Util.KInstantiateUI(DraggablePrefab, EntryParent);
                    StarmapItemEntry.SetActive(true);
                    StarmapItemEntry.TryGetComponent<RectTransform>(out var rect);
                    StarmapItemEntry.TryGetComponent<Image>(out var img);
                    rect.anchoredPosition = GetPosForHex(x, y);

                    var dragLogic = StarmapItemEntry.AddOrGet<HexDrag>();
                    dragLogic.Init(this,x,y, dataEntry.Value);

                    ActiveItems.Add(new Tuple<int, int>(x, y), dragLogic);
                }
                else
                {
                    SgtLogger.warning(key.first+", "+key.second+": "+dataEntry.Value,"Coordinate Key already in dictionary");
                }
            }
        }
    }

}
