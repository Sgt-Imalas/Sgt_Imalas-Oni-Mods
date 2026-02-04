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
	internal class StoryTraitAsteroidBlacklistToggle : KMonoBehaviour
	{
		FToggle toggle;
		LocText Label;
		Image Image;
		public Action<string, bool> onToggled;
		public string AsteroidId;

		public void Init(string asteroidId)
		{
			toggle = transform.Find("Background").gameObject.AddOrGet<FToggle>();
			toggle.SetCheckmark("Checkmark");
			Label = transform.Find("Label").GetComponent<LocText>();
			Image = transform.Find("Image").GetComponent<Image>();
			toggle.OnClick += OnClick;

			this.AsteroidId = asteroidId;
			if (CGSMClusterManager.PlanetoidDict.TryGetValue(AsteroidId, out var planetoid))
			{
				Label.SetText(planetoid.OriginalDisplayName);
				Image.sprite = planetoid.planetSprite;
			}
		}

		public override void OnSpawn()
		{
			base.OnSpawn();
		}
		void OnClick(bool change)
		{
			onToggled?.Invoke(AsteroidId, change);
		}
		public void Refresh(bool on, bool interactable, Action<string, bool> onToggleChanged)
		{
			onToggled = onToggleChanged;
			toggle?.SetOnFromCode(on);
			SetInteractable(interactable);
			if (AsteroidId.IsNullOrWhiteSpace())
				return;
			if (CGSMClusterManager.PlanetoidDict.TryGetValue(AsteroidId, out var planetoid))
			{
				Label.SetText(planetoid.DisplayName);
				Image.sprite = planetoid.DisplaySprite;
			}
		}

		public void SetInteractable(bool second)
		{
			toggle?.SetInteractable(second);
		}
	}
}
