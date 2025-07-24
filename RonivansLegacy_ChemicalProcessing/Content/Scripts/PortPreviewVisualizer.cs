using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs.BuildingPortUtils;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	class PortPreviewVisualizer : KMonoBehaviour
	{
		[MyCmpReq]
		PortDisplayController PortController;

		//public void DrawUtilityIcon(int cell, Sprite icon_img, ref GameObject visualizerObj, Color tint, float scaleMultiplier = 1.5f, bool hideBG = false)
		//{
		//	Vector3 position = Grid.CellToPosCCC(cell, Grid.SceneLayer.Building);
		//	if (visualizerObj == null)
		//	{
		//		visualizerObj = Util.KInstantiate(Assets.UIPrefabs.ResourceVisualizer, GameScreenManager.Instance.worldSpaceCanvas);
		//		visualizerObj.transform.SetAsFirstSibling();
		//		icons.Add(visualizerObj, visualizerObj.transform.GetChild(0).GetComponent<Image>());
		//	}

		//	if (!visualizerObj.gameObject.activeInHierarchy)
		//	{
		//		visualizerObj.gameObject.SetActive(value: true);
		//	}

		//	visualizerObj.GetComponent<Image>().enabled = !hideBG;
		//	icons[visualizerObj].raycastTarget = enableRaycast;
		//	icons[visualizerObj].sprite = icon_img;
		//	visualizerObj.transform.GetChild(0).gameObject.GetComponent<Image>().color = tint;
		//	visualizerObj.transform.SetPosition(position);
		//	if (visualizerObj.GetComponent<SizePulse>() == null)
		//	{
		//		visualizerObj.transform.localScale = Vector3.one * scaleMultiplier;
		//	}
		//}
	}
}
