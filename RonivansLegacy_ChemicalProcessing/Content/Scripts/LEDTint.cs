using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	public class LEDTint : KMonoBehaviour
	{
		public static Dictionary<string, Color> TintColorsPerSkinID = [];
		public static void AddSkinLightTint(string skinID, Color color) => TintColorsPerSkinID.Add(skinID, color);

		public Color Default = Color.white;

		[MyCmpReq] BuildingFacade facade;
		[MyCmpReq] Light2D light;

		public override void OnSpawn()
		{
			base.OnSpawn();
			Subscribe(ModAssets.OnBuildingFacadeChanged, OnFacadeChanged);
			OnFacadeChanged(null);
		}
		public override void OnCleanUp()
		{
			Unsubscribe(ModAssets.OnBuildingFacadeChanged, OnFacadeChanged);
			base.OnCleanUp();
		}
		void OnFacadeChanged(object _)
		{
			var currentskin = facade.CurrentFacade;
			if (!currentskin.IsNullOrWhiteSpace() && TintColorsPerSkinID.TryGetValue(currentskin, out var tintColor))
			{
				light.Color = tintColor;
				light.overlayColour = tintColor;
			}
			else
			{
				light.Color = Default;
				light.overlayColour = Default;
			}
		}
	}
}
