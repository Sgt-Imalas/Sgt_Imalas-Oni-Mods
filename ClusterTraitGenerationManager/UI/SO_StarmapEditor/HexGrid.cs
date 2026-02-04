using ClusterTraitGenerationManager.ClusterData;
using ClusterTraitGenerationManager.UI.Screens;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UtilLibs;
using UtilLibs.UIcmp;
using static ClusterTraitGenerationManager.UI.SO_StarmapEditor.StarmapToolkit;

namespace ClusterTraitGenerationManager.UI.SO_StarmapEditor
{
	internal class HexGrid : FScreen
	{

		internal class HexDrag : KMonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
		{
			public StarmapToolkit toolKit;

			[MyCmpGet]
			RectTransform rectTransform;

			bool nameAlwaysActive = false;
			HexGrid parent;
			public Tuple<int, int> InternalPos;
			Image InternalImage;
			Image ownImage;
			string ownId;
			LocText Label;
			public string ID => ownId;
			private bool _isPOI = false;
			public bool IsPOI => _isPOI;

			Transform tParent, dragParent;

			public void Init(HexGrid _parent, int _x, int _y, string starmapItemId)
			{
				//UIUtils.ListAllChildren(transform);
				gameObject.TryGetComponent<Image>(out ownImage);
				parent = _parent;
				InternalPos = new Tuple<int, int>(_x, _y);
				InternalImage = transform.Find("ButtonContainerImage").gameObject.GetComponent<Image>();
				ownId = starmapItemId;
				Label = transform.Find("Label").gameObject.GetComponent<LocText>();
				InitLabelMat();

				SetStarmapItem(ownId);
				Label.gameObject.SetActive(false);
				Label.raycastTarget = false;
				SetNameAlwaysActive(parent.AlwaysShowNames);
				tParent = transform.parent;
				dragParent = _parent.transform.parent.parent.parent.parent;
			}
			static Material UnderlinedGraystroke = null;
			void InitLabelMat()
			{
				if(UnderlinedGraystroke == null)
				{
					UnderlinedGraystroke = new(Label.fontMaterial);
					// Enable underlay
					UnderlinedGraystroke.EnableKeyword("UNDERLAY_ON");
					UnderlinedGraystroke.SetColor("_UnderlayColor", new Color(0, 0, 0, 1));
					UnderlinedGraystroke.SetFloat("_UnderlayDilate", 1f);
					UnderlinedGraystroke.SetFloat("_UnderlaySoftness", 1f);
				}
				Label.fontMaterial = UnderlinedGraystroke;
			}


			public void SetStarmapItem(string ID = null)
			{
				ownId = ID;
				if (ID != null)
				{
					if (CGSMClusterManager.PlanetoidDict.ContainsKey(ID))
					{
						//SgtLogger.l("isPlanetoid");
						var data = CGSMClusterManager.PlanetoidDict[ID];
						SetContainerSprite(data.IsMixed ? data.planetMixingSprite : data.planetSprite);
						transform.Find("OrbitRing")?.gameObject.SetActive(true);
						Label.SetText(data.DisplayName);
					}
					else if (ModAssets.SO_POIs.ContainsKey(ID))
					{
						//SgtLogger.l("isPOI");
						var data = ModAssets.SO_POIs[ID];
						SetContainerSprite(data.Sprite);
						Label.SetText(data.Name);
						_isPOI = true;
					}
					else
					{
						//SgtLogger.l("IsNothing");
						SetContainerSprite(null);
						Label.SetText("");
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
				transform.SetParent(dragParent);

				if (eventData != null)
					parent.DoubleClickCanceledByDraggingHandler();
			}

			public void OnEndDrag(PointerEventData eventData)
			{
				transform.SetParent(tParent);
				transform.localScale = Vector3.one;
				transform.position = DragStartPosition;
				ownImage.raycastTarget = true;
				InternalImage.raycastTarget = true;

				CGM_MainScreen_UnityScreen.Instance.SelectHexItem(ID, InternalPos);
			}

			public void SetPositionXY(Tuple<int, int> pos)
			{
				rectTransform.anchoredPosition = parent.GetPosForHex(pos.first, pos.second);
			}

			public void OnPointerExit(PointerEventData eventData)
			{
				if (!nameAlwaysActive)
					Label.gameObject.SetActive(false);
			}

			public void OnPointerEnter(PointerEventData eventData)
			{
				if (!nameAlwaysActive)
					Label.gameObject.SetActive(true);
				transform.SetAsLastSibling();
			}

			internal void SetNameAlwaysActive(bool value)
			{
				nameAlwaysActive = value;
				Label.gameObject.SetActive(value);
			}

			public void OnPointerClick(PointerEventData eventData)
			{
				if (eventData.button == PointerEventData.InputButton.Left)
				{
					if (eventData.clickCount == 2)
					{
						parent.OnDoubleClickSimDragStartedHandler(this);
					}
					else if (eventData.clickCount == 1)
					{
						PlaySound(UISoundHelper.ClickOpen);
						CGM_MainScreen_UnityScreen.Instance.SelectHexItem(ID, InternalPos);
					}
				}
			}
		}
		internal class HexDropHandler : KMonoBehaviour, IDropHandler, IPointerClickHandler
		{
			HexGrid parent;
			public Tuple<int, int> HexPos;
			public void Init(int _x, int _y, HexGrid _parent)
			{
				HexPos = new(_x, _y);
				parent = _parent;
			}


			public void OnDrop(PointerEventData eventData)
			{
				if (!parent.HexOccupied(HexPos))
				{
					if (eventData.pointerDrag.TryGetComponent(out HexDrag hexDragger))
					{
						hexDragger.DragStartPosition = transform.position;
						parent.MovePOIToNewLocation(hexDragger, HexPos);
					}
					else if (eventData.pointerDrag.TryGetComponent(out ToolkitDraggable newPOI))
					{
						parent.AddNewPoi(HexPos, newPOI.poiId);
					}
				}
			}

			public void OnPointerClick(PointerEventData eventData)
			{
				if (eventData.button == PointerEventData.InputButton.Left)
				{
					if (eventData.clickCount == 2)
					{
						parent.OnDoubleClickSimDragEndedHandler(this);
					}
				}
			}
		}

		public override void OnBeginDrag(PointerEventData eventData)
		{

			transform.parent.TryGetComponent<ScrollRect>(out var scrollRect);
			scrollRect.OnBeginDrag(eventData);
			base.OnBeginDrag(eventData);
		}
		public override void OnDrag(PointerEventData eventData)
		{
			transform.parent.TryGetComponent<ScrollRect>(out var scrollRect);
			scrollRect.OnDrag(eventData);
			base.OnDrag(eventData);
		}
		public override void OnEndDrag(PointerEventData eventData)
		{
			transform.parent.TryGetComponent<ScrollRect>(out var scrollRect);
			scrollRect.OnEndDrag(eventData);
			base.OnEndDrag(eventData);
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
		public Vector2 GetPosForHex(int r, int q) => new(r * RStep + (0.5f * q * RStep), (-q * QStep));
		public Tuple<int, int> GetHexPosForPos(Vector2 pos)
		{
			var q = (Mathf.Sqrt(3f) / 3f * pos.x - 1f / 3f * pos.y) / QStep;
			var r = (2f / 3f * pos.y) / RStep;

			return new Tuple<int, int>(Mathf.RoundToInt(r), Mathf.RoundToInt(q));
		}

		[SerializeField]
		public float CellRadius = 50;
		[SerializeField]
		public float BufferDistance = 4;
		[SerializeField]
		public int MapRadius = 5;
		[MyCmpGet] RectTransform RectTransform;


		Dictionary<Tuple<int, int>, GameObject> GridItems = new Dictionary<Tuple<int, int>, GameObject>();
		public Dictionary<Tuple<int, int>, HexDrag> ActiveItems = new Dictionary<Tuple<int, int>, HexDrag>();
		//GameObject[,] GridItems;
		//Button[,] GridButtons;

		[SerializeField]
		public GameObject EntryPrefab;
		public GameObject DraggablePrefab;
		public GameObject EntryParent;

		ToolkitDraggable _currentlySimDraggedToolkit = null;
		public ToolkitDraggable CurrentlySimDraggedNew => _currentlySimDraggedToolkit;

		HexDrag _currentlySimDragged = null;
		public HexDrag CurrentlySimDragged => _currentlySimDragged;


		public void DoubleClickCanceledByDraggingHandler()
		{
			if (_currentlySimDragged != null)
			{
				_currentlySimDragged.OnEndDrag(null);
				_currentlySimDragged = null;
			}

			if (_currentlySimDraggedToolkit != null)
			{
				_currentlySimDraggedToolkit.OnEndDrag(null);
				_currentlySimDraggedToolkit = null;
			}
		}

		public void OnDoubleClickSimDragStartedHandler(HexDrag _clickedItem)
		{
			if (_currentlySimDragged != null || _currentlySimDraggedToolkit != null)
				return;

			_currentlySimDraggedToolkit = null;
			_currentlySimDragged = _clickedItem;
			_currentlySimDragged.OnBeginDrag(null);

			//SgtLogger.l("start virt drag @" + _clickedItem.name );
		}
		public void OnDoubleClickSimDragStartedHandler(ToolkitDraggable _clickedItem)
		{
			if (_currentlySimDragged != null || _currentlySimDraggedToolkit != null)
				return;

			_currentlySimDragged = null;
			_currentlySimDraggedToolkit = _clickedItem;
			_currentlySimDraggedToolkit.OnBeginDrag(null);
		}
		public void OnDoubleClickSimDragEndedHandler(HexDropHandler dropHandler)
		{
			if (_currentlySimDragged != null)
			{
				//SgtLogger.l("end virt drag @" + _currentlySimDragged.name);
				if (!HexOccupied(dropHandler.HexPos))
				{
					_currentlySimDragged.DragStartPosition = dropHandler.transform.position;
					MovePOIToNewLocation(_currentlySimDragged, dropHandler.HexPos);
				}
				_currentlySimDragged.OnEndDrag(null);

			}
			else if (_currentlySimDraggedToolkit != null)
			{
				if (!HexOccupied(dropHandler.HexPos))
				{;
					AddNewPoi(dropHandler.HexPos, _currentlySimDraggedToolkit.poiId);
				}
				_currentlySimDraggedToolkit.OnEndDrag(null);
			}
			_currentlySimDraggedToolkit = null;
			_currentlySimDragged = null;
		}
		public void OnDoubleClickSimDragDeletedHandler()
		{
			if (_currentlySimDragged != null)
			{
				if (_currentlySimDragged.IsPOI)
				{
					RemovePOI(_currentlySimDragged);
					Destroy(_currentlySimDragged.gameObject);
				}
				else
					_currentlySimDragged.OnEndDrag(null);
			}
			else if (_currentlySimDraggedToolkit != null)
			{
				_currentlySimDraggedToolkit.OnEndDrag(null);
			}
			_currentlySimDraggedToolkit = null;
			_currentlySimDragged = null;
		}

		public override void ScreenUpdate(bool topLevel)
		{
			this.m_currentZoomScale = Mathf.Lerp(this.m_currentZoomScale, this.m_targetZoomScale, Mathf.Min(4f * Time.unscaledDeltaTime, 1f));
			Vector2 mousePos = (Vector2)KInputManager.GetMousePos();
			Vector3 vector3_1 = RectTransform.InverseTransformPoint((Vector3)mousePos);
			RectTransform.localScale = new Vector3(this.m_currentZoomScale, this.m_currentZoomScale, 1f);
			Vector3 vector3_2 = RectTransform.InverseTransformPoint((Vector3)mousePos);
			RectTransform content = RectTransform;
			content.localPosition = content.localPosition + (vector3_2 - vector3_1) * this.m_currentZoomScale;
			if (_currentlySimDragged != null) _currentlySimDragged.transform.SetPosition(mousePos);
			if (_currentlySimDraggedToolkit != null) _currentlySimDraggedToolkit.transform.SetPosition(mousePos);
		}


		public override void OnKeyDown(KButtonEvent e)
		{
			if (!e.Consumed && CursorInside && (e.IsAction(Action.ZoomIn) || e.IsAction(Action.ZoomOut)))
			{
				if (e.IsAction(Action.ZoomIn) && currentZoomStep < zoomStepMax)
					currentZoomStep += 1;
				else if (e.IsAction(Action.ZoomOut) && currentZoomStep > zoomStepMin)
					currentZoomStep -= 1;
				else
					return;

				var B = Mathf.Log(lowerZoomBound / upperZoomBound, 2.718f) / 17f;
				var A = upperZoomBound;

				//this.m_targetZoomScale = Mathf.Clamp(this.m_targetZoomScale + (!KInputManager.currentControllerIsGamepad ? UnityEngine.Input.mouseScrollDelta.y * 0.1f : (float)((e.IsAction(Action.ZoomIn) ? -0.003 : 0.003))), 0.15f, 2f);
				this.m_targetZoomScale = A * Mathf.Exp(B * currentZoomStep);

				//this.rectTransform().localScale = new Vector3(this.m_targetZoomScale, this.m_targetZoomScale, 1f);

				e.TryConsume(Action.ZoomIn);
				if (!e.Consumed)
					e.TryConsume(Action.ZoomOut);

			}
		}

		public bool AlwaysShowNames
		{
			get
			{
				return _alwaysShowNames;
			}
			set
			{
				_alwaysShowNames = value;
				foreach (var active in ActiveItems.Values)
				{
					active.SetNameAlwaysActive(value);
				}

			}
		}

		public System.Action OnActiveItemCompositionChanged;

		private bool _alwaysShowNames = true;

		float lowerZoomBound = 2.5f, upperZoomBound = 0.2f;
		float currentZoomStep = 1f;
		float zoomStepMin = -2, zoomStepMax = 15;
		float m_targetZoomScale = 0.25f, m_currentZoomScale = 0.25f;
		int lastRadius;


		float RStep, QStep;
		public void UpdateBgGrid()
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


				RStep = (CellRadius) * Mathf.Sqrt(3f) + BufferDistance;
				QStep = (3f / 2f) * (CellRadius + BufferDistance);

				RectTransform.sizeDelta = new Vector2((float)(MapRadius * 220), (float)(MapRadius * 200));
				RectTransform.localPosition = new Vector2(0, 0);

				RectTransform.localScale = new Vector2(0.25f, 0.25f);

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

								rect.anchoredPosition = GetPosForHex(x, y);
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
		public bool CursorInside = false;

		public bool CurrentlySelectingNewPosition = true;

		public void AddNewPoiToStarmap(Tuple<int, int> key, string itemId, bool calledDuringInit = false)
		{
			if (!ActiveItems.ContainsKey(key))
			{
				int r = key.first, q = key.second;

				var StarmapItemEntry = Util.KInstantiateUI(DraggablePrefab, EntryParent);
				StarmapItemEntry.SetActive(true);
				StarmapItemEntry.TryGetComponent<RectTransform>(out var rect);
				StarmapItemEntry.TryGetComponent<Image>(out var img);
				rect.anchoredPosition = GetPosForHex(r, q); //x = r, y = q

				var dragLogic = StarmapItemEntry.AddOrGet<HexDrag>();
				dragLogic.Init(this, r, q, itemId);

				ActiveItems.Add(new Tuple<int, int>(r, q), dragLogic);

				if (calledDuringInit)
					return;

				if (OnActiveItemCompositionChanged != null)
					OnActiveItemCompositionChanged();

				CGM_MainScreen_UnityScreen.Instance.SelectHexItem(dragLogic.ID, dragLogic.InternalPos);
			}
			else
			{
				SgtLogger.warning(key.first + ", " + key.second + ": " + itemId, "Coordinate Key already in dictionary");
			}
		}
		public void AddNewPoi(Tuple<int,int> hexPos, string poiId)
		{
			AddNewPoiToStarmap(hexPos, poiId);
			AxialI targetPos = new(hexPos.first, hexPos.second);
			CGSMClusterManager.CustomCluster.SO_Starmap.AddPOI(poiId, targetPos);
		}
		public void RemovePOI(HexDrag hexDragger)
		{
			ActiveItems.Remove(hexDragger.InternalPos);

			int x = hexDragger.InternalPos.first,
				y = hexDragger.InternalPos.second;

			CGSMClusterManager.CustomCluster.SO_Starmap.RemovePOI(new(x, y));
			if (OnActiveItemCompositionChanged != null)
				OnActiveItemCompositionChanged();

			CGM_MainScreen_UnityScreen.Instance.SelectHexItem(null, null);
		}
		private void MovePOIToNewLocation(HexDrag hexDragger, Tuple<int, int> hexPos)
		{
			int x1 = hexDragger.InternalPos.first, y1 = hexDragger.InternalPos.second;
			int x2 = hexPos.first, y2 = hexPos.second;

			CGSMClusterManager.CustomCluster.SO_Starmap.MovePOI(hexDragger.ID, new(x1, y1), new(x2, y2));

			ActiveItems.Remove(hexDragger.InternalPos);
			hexDragger.InternalPos = hexPos;
			ActiveItems.Add(hexDragger.InternalPos, hexDragger);

			if (OnActiveItemCompositionChanged != null)
				OnActiveItemCompositionChanged();

			CGM_MainScreen_UnityScreen.Instance.SelectHexItem(hexDragger.ID, hexDragger.InternalPos);
		}


		bool HexOccupied(Tuple<int, int> key) => ActiveItems.ContainsKey(key);

		internal void UpdateActiveItemsPositions()
		{
			foreach (var item in ActiveItems)
			{
				Destroy(item.Value.gameObject);
			}
			ActiveItems.Clear();

			foreach (var dataEntry in CGSMClusterManager.CustomCluster.SO_Starmap.OverridePlacements)
			{
				var key = new Tuple<int, int>(dataEntry.Key.R, dataEntry.Key.Q);
				AddNewPoiToStarmap(key, dataEntry.Value, true);
			}
			if (OnActiveItemCompositionChanged != null)
				OnActiveItemCompositionChanged();
		}
	}

}
