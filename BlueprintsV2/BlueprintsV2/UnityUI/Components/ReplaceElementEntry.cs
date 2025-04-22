using BlueprintsV2.BlueprintData;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using UtilLibs.UIcmp;

namespace BlueprintsV2.UnityUI.Components
{
	internal class ReplaceElementEntry : KMonoBehaviour
	{
		public Tag targetTag;
		public System.Action<Tag> OnSelectElement;
		LocText ElementName;
		Image ElementIcon;
		Image buttonBg;
		FButton button;
		ToolTip toolTip;
		public string Name = string.Empty;

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			ElementName = transform.Find("Label").gameObject.GetComponent<LocText>();
			ElementIcon = transform.Find("CarePackageSprite").gameObject.GetComponent<Image>();
			buttonBg = transform.Find("Background").gameObject.GetComponent<Image>();
			button = gameObject.AddComponent<FButton>();
			if (targetTag != null)
			{
				this.gameObject.name = targetTag.name;
				var prefab = Assets.TryGetPrefab(targetTag);

				var icoSprite = Def.GetUISprite(prefab);
				if (icoSprite != null)
				{
					ElementIcon.sprite = icoSprite.first;
					ElementIcon.color = icoSprite.second;
				}
				else
					SgtLogger.warning("no ui sprite found for " + prefab.name);

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

				ElementName?.SetText(prefab.GetProperName());
				Name = prefab.GetProperName();
				button.OnClick += OnClick;
				toolTip = UIUtils.AddSimpleTooltipToObject(this.gameObject, GameUtil.GetMaterialTooltips(targetTag));
			}
		}
		void OnClick()
		{
			OnSelectElement?.Invoke(targetTag);
		}
		public void Refresh(Blueprint current, float requiredAmount, Tag original, Tag replacement = default)
		{
			float catrequirement = requiredAmount;
			if (current.CachedAbsTagCost.TryGetValue(targetTag, out float totalCachedCosts))
			{
				if (targetTag != original)
					requiredAmount += totalCachedCosts;
			}
			if(targetTag == replacement)//replacement material is currently selected, dont calculate requiredAmount increase here
			{
				requiredAmount -= catrequirement;
			}

			float currentWorldAmount = ClusterManager.Instance.activeWorld.worldInventory.GetAmount(targetTag, true);

			bool materialUnlocked = BlueprintState.InstantBuild || DiscoveredResources.Instance.IsDiscovered(targetTag);
			bool enoughMaterial = BlueprintState.InstantBuild || requiredAmount <= currentWorldAmount;

			//button.SetInteractable(materialUnlocked);
			StringBuilder sb = new StringBuilder();
			sb.AppendLine(GameUtil.GetMaterialTooltips(targetTag));
			if (materialUnlocked)
			{
				if (enoughMaterial)
				{
					buttonBg.color = UIUtils.rgb(138, 140, 152);
				}
				else
				{
					buttonBg.color = UIUtils.Lerp(Color.yellow, Color.black, 60);
					sb.AppendLine();
					sb.AppendLine(string.Format(STRINGS.UI.BLUEPRINTSELECTOR.MATERIALREPLACER.SCROLLAREA.CONTENT.ELEMENTSTATE.NOTENOUGH, GameUtil.GetFormattedMass(currentWorldAmount), GameUtil.GetFormattedMass(requiredAmount)));
				}
			}
			else
			{
				buttonBg.color = UIUtils.Lerp(Color.red, Color.black, 60);
				sb.AppendLine();
				sb.AppendLine(STRINGS.UI.BLUEPRINTSELECTOR.MATERIALREPLACER.SCROLLAREA.CONTENT.ELEMENTSTATE.NOTFOUND);
			}
			toolTip.SetSimpleTooltip(sb.ToString());
		}
		public override void OnSpawn()
		{
			base.OnSpawn();
		}
	}
}
