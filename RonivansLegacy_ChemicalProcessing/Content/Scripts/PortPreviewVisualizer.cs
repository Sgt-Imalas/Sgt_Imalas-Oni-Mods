using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using UtilLibs.BuildingPortUtils;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	class PortPreviewVisualizer : KMonoBehaviour
	{
		[MyCmpReq]
		Building building;

		[MyCmpReq]
		PortDisplayController PortController;

		Dictionary<CellOffset, GameObject> PortPreviews = []; 
		Dictionary<CellOffset, Image> PortPreviewImagess = [];

		[SerializeField]
		public HashedString DisableInOverlay = HashedString.Invalid;

		void OnOverlaySwitched(HashedString overlay)
		{
			TogglePorts(overlay != DisableInOverlay);
		}
		public override void OnSpawn()
		{
			base.OnSpawn();
			OverlayScreen.Instance.OnOverlayChanged += OnOverlaySwitched;
			SpawnPortPreviews();
			OnOverlaySwitched(OverlayScreen.Instance.mode);
		}
		public override void OnCleanUp()
		{
			CleanupPorts();
			OverlayScreen.Instance.OnOverlayChanged -= OnOverlaySwitched;
			base.OnCleanUp();
		}
		void CleanupPorts()
		{
			foreach(var port in PortPreviews.Values)
			{
				if (port != null)
				{
					Util.KDestroyGameObject(port.gameObject);
				}
			}
			PortPreviewImagess.Clear();
			PortPreviews.Clear();
		}

		void TogglePorts(bool enabled)
		{
			foreach(var port in PortPreviews.Values)
			{
				port.SetActive(enabled);
			}
		}

		void SpawnPortPreviews()
		{
			int ownCell = building.NaturalBuildingCell();
			Sprite previewSpriteIn = Assets.GetSprite("aio_conduit_input_preview");
			Sprite previewSpriteOut = Assets.GetSprite("aio_conduit_output_preview");

			var ports = PortController.GetAllPorts();
			//SgtLogger.l("port count: " + ports.Count);	

			foreach (PortDisplay2 port in ports)
			{
				var offset = port.GetUtilityCellOffset(building);
				Sprite sprite = port.Input ? previewSpriteIn : previewSpriteOut;
				CreatePortPreview(offset, sprite);
			}
			if(building.Def.InputConduitType != ConduitType.None)
			{
				CellOffset inputOffset = building.GetUtilityInputOffset();
				CreatePortPreview(inputOffset, previewSpriteIn);				
			}
			if(building.Def.OutputConduitType != ConduitType.None)
			{
				CellOffset outputOffset = building.GetUtilityOutputOffset();
				CreatePortPreview(outputOffset, previewSpriteOut);
			}
		}
		public void MovePortPreviews()
		{
			foreach(var port in PortPreviews)
			{
				if (port.Value != null)
				{
					Vector3 position = Grid.CellToPosCCC(building.GetCellWithOffset(port.Key), Grid.SceneLayer.Building);
					port.Value.transform.SetPosition(position);
				}
			}
		}
		public void TintPortPreviews(Color color)
		{
			foreach (var port in PortPreviewImagess)
			{
				if (port.Value != null)
				{
					port.Value.color = color;
				}
			}
		}
		void CreatePortPreview(CellOffset offset, Sprite sprite)
		{
			if (PortPreviews.ContainsKey(offset))
			{
				return;
			}
			GameObject previewObj = Util.KInstantiate(Assets.UIPrefabs.ResourceVisualizer, GameScreenManager.Instance.worldSpaceCanvas);
			previewObj.SetActive(true);
			previewObj.transform.SetAsFirstSibling();
			Image previewImage = previewObj.transform.GetChild(0).GetComponent<Image>();
			previewImage.sprite = sprite;
			previewImage.raycastTarget = false;
			Vector3 position = Grid.CellToPosCCC(building.GetCellWithOffset(offset), Grid.SceneLayer.Building);
			previewObj.transform.SetPosition(position);
			PortPreviews[offset] = previewObj;
			PortPreviewImagess[offset] = previewImage;
		}
	}
}
