using ClusterTraitGenerationManager.ClusterData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UtilLibs.UIcmp;

namespace ClusterTraitGenerationManager.UI.Components
{
	internal class MixingTargetSelectable : KMonoBehaviour
	{
		FButton button;
		LocText Label;
		Image Image;
		public Action<string> OnMixingTargetSelected;
		public string AsteroidId;

		public void Init(string asteroidId, Action<string> onMixingTargetSelected)
		{
			button = gameObject.AddOrGet<FButton>();
			Label = transform.Find("Label").GetComponent<LocText>();
			Image = transform.Find("Image").GetComponent<Image>();
			button.OnClick += OnClick;

			this.AsteroidId = asteroidId;
			OnMixingTargetSelected = onMixingTargetSelected;
			if(CGSMClusterManager.PlanetoidDict.TryGetValue(AsteroidId, out var planetoid))
			{
				Label.SetText(planetoid.OriginalDisplayName);
				Image.sprite = planetoid.planetSprite;
			}
		}

		public override void OnSpawn()
		{
			base.OnSpawn();
		}
		void OnClick()
		{
			OnMixingTargetSelected?.Invoke(AsteroidId);
		}

		internal void AllowSwapping(bool second)
		{
			button.SetInteractable(second);
		}
	}
}
