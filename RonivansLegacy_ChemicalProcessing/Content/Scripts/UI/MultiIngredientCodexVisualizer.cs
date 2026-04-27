using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UtilLibs;
using UtilLibs.UIcmp;
using static STRINGS.DUPLICANTS.MODIFIERS;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts.UI
{
	internal class MultiIngredientCodexVisualizer : KMonoBehaviour
	{
		struct IngredientVariant
		{
			public Sprite sprite;
			public Color color;
			public string tooltip;
			public string link;
			public string amountLabelText;
			public IngredientVariant(Sprite sprite, Color color, string tooltip, string link, string amountLabelText)
			{
				this.sprite = sprite;
				this.color = color;
				this.tooltip = tooltip;
				this.link = link;
				this.amountLabelText = amountLabelText;
			}
		}


		static TMP_FontAsset _textFont = null;
		public static TMP_FontAsset TextFont
		{
			get
			{
				if (_textFont == null)
				{
					_textFont = Localization.GetFont("Economica-Bold-OTF SDF");
				}
				return _textFont;
			}
		}

		bool initialized = false;

		[MyCmpGet]
		LayoutElement le;
 		
		GameObject centerItem;
		Image centerItemImage;
		GameObject rotatableContainer, rotatableItemPrefab;
		Dictionary<Tag, GameObject> rotatingItems = new Dictionary<Tag, GameObject>();
		Dictionary<Tag, IngredientVariant> itemInfo = new Dictionary<Tag, IngredientVariant>();
		ToolTip middleItemTooltip;
		LocText amountLabel;

		Tag[] ingredientVariants;
		Tag _start;

		string centerItemLink;

		bool HasMultipleVariants => ingredientVariants != null && ingredientVariants.Length > 1;	



		//void SetExpanded(bool expanded)
		//{
		//	this.expanded = expanded;
		//	centerItem?.SetActive(!expanded);
		//	//transform.localScale = Vector3.one * (expanded ? 2.25f : 1f);
		//	if (currentlyDisplayed >= 0 && rotatingItems.Any())
		//		rotatingItems[ingredientVariants[currentlyDisplayed]].SetActive(expanded);
		//	RefreshRotatablePosition();
		//}
		void RefreshRotatablePosition()
		{
			if(!rotatingItems.Any())
				return;

			if(rotatingItems.Count == 1)
			{
				le.minWidth = 60;
				rotatingItems.Values.First().gameObject.SetActive(false);
				return;
			}


			int childCount = ingredientVariants.Length;
			//if (!expanded)
			//	childCount--;
			int counter = 0;
			for (int i = 0; i < childCount; i++)
			{
				var tag = ingredientVariants[i];

				Transform child = rotatingItems[tag].transform;
				float angle = (360f / childCount) * counter++;
				child.localPosition = Quaternion.Euler(0, 0, angle) * Vector3.up * 35;
			}
		}

		//void DisplayNextItem()
		//{
		//	if (!HasMultipleVariants)
		//		return;
		//	if (currentlyDisplayed >= 0)
		//	{
		//		var previousIngredient = ingredientVariants[currentlyDisplayed];
		//		rotatingItems[previousIngredient].SetActive(true);
		//	}


		//	currentlyDisplayed++;
		//	currentlyDisplayed = currentlyDisplayed % ingredientVariants.Length;
		//	var currentDisplayIngredient = ingredientVariants[currentlyDisplayed];
			
		//}
		public override void OnSpawn()
		{
			base.OnSpawn();
			if(rotatingItems.TryGetValue(_start, out var go))
			{
				SelectVariant(_start);
			}
		}


		void Init()
		{
			if (initialized)
				return;
			initialized = true;
			centerItem = transform.Find("CenterObject").gameObject;
			centerItemImage = centerItem.GetComponent<Image>();
			rotatableContainer = transform.Find("RotatableContainer").gameObject;
			rotatableItemPrefab = rotatableContainer.transform.Find("RotatablePrefab").gameObject;
			rotatableItemPrefab.SetActive(false);
			middleItemTooltip = UIUtils.AddSimpleTooltipToObject(centerItem, "");
			centerItem.AddComponent<FButton>().OnClick += OnCenterItemClicked;
			amountLabel = transform.Find("AmountLabel").GetComponent<LocText>();
			amountLabel.font = TextFont;
			amountLabel.fontSize = 16;
			rotatableItemPrefab.gameObject.AddComponent<CodexRecipeMultiElementEntry>();
		}


		void OnCenterItemClicked()
		{
			if (centerItemLink.IsNullOrWhiteSpace())
				return;
			ManagementMenu.Instance.codexScreen.ChangeArticle(centerItemLink);
		}

		internal void SetDisplayedIngredients(List<Tuple<Tag, float>> ingredientVariants, Tag start)
		{
			Init();
			_start = start;	
			this.ingredientVariants = ingredientVariants.Select(item => item.first).Distinct().ToArray();
			float min = 0,max=0;

			foreach (var ingredientVariant in ingredientVariants)
			{
				var tag = ingredientVariant.first;
				if (itemInfo.ContainsKey(tag))
				{
					continue;
				}
				float amount = ingredientVariant.second;
				if(min == 0)
					min = amount;

				if(amount > max)
					max = amount;
				if(amount < min)
					min = amount;
				GameObject prefab = Assets.GetPrefab(tag);
				string amountLabelText = GameUtil.GetFormattedByTag(tag, amount);
				string tooltip = tag.ProperName();
				if (prefab.TryGetComponent<Edible>(out var edible))
				{
					tooltip = $"{tooltip}\n    • {string.Format(global::STRINGS.UI.GAMEOBJECTEFFECTS.FOOD_QUALITY, (object)GameUtil.GetFormattedFoodQuality(edible.GetQuality()))}";
				}
				Tuple<Sprite, Color> uiSprite = Def.GetUISprite(tag);
				var link = global::STRINGS.UI.ExtractLinkID(prefab.GetProperName());
				itemInfo[tag] = new IngredientVariant(uiSprite.first, uiSprite.second, tooltip, link, amountLabelText);

				var rotatableItem = Util.KInstantiateUI<CodexRecipeMultiElementEntry>(rotatableItemPrefab, rotatableContainer, true);
				var rotGo = rotatableItem.gameObject;
				rotatableItem.OnHover += ()=> SelectVariant(tag);
				var rotatingImage = rotatableItem.GetComponent<Image>();
				rotatingImage.sprite = uiSprite.first;
				rotatingImage.color = uiSprite.second;
				tooltip = amountLabelText +" "+ tooltip;
				UIUtils.AddSimpleTooltipToObject(rotGo, tooltip);
				rotGo.AddComponent<FButton>().OnClick += () => ManagementMenu.Instance.codexScreen.ChangeArticle(link);
				rotatingItems[tag] = rotGo;
			}
			RefreshRotatablePosition();
		}

		void SelectVariant(Tag tag)
		{
			var info = itemInfo[tag];
			centerItemImage.sprite = info.sprite;
			centerItemImage.color = info.color;
			middleItemTooltip.SetSimpleTooltip(info.tooltip);
			centerItemLink = info.link;
			amountLabel.SetText(info.amountLabelText);
		}
	}
}
