using BlueprintsV2.BlueprintsV2.BlueprintData.NoteToolPlacedEntities;
using BlueprintsV2.BlueprintsV2.Tools;
using BlueprintsV2.BlueprintsV2.UnityUI.Components;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using UtilLibs.UI.FUI;
using UtilLibs.UIcmp;

namespace BlueprintsV2.BlueprintsV2.UnityUI
{
	internal class SpriteSelectorScreen : FScreen
	{
		public SpriteSelectorScreen() : base()
		{
			ConsumeMouseScroll = true;
			pause = false;
			lockCam = false;
		}
		public override float GetSortKey()
		{
			return base.GetSortKey() + 2;
		}

#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable CS0414 // Remove unused private members
		new bool ConsumeMouseScroll = true; // do not remove!!!!
#pragma warning restore CS0414 // Remove unused private members
#pragma warning restore IDE0051 // Remove unused private members

		public static SpriteSelectorScreen Instance = null;
		FButton ClearSearchBar, Close;
		FInputField2 FilterInput;
		Dictionary<string, GameObject> SpriteIcons = [];
		GameObject Container, PrefabGO;
		IconSelectionEntry Prefab;
		System.Action<string, Color> OnSelectEntry;
		System.Action OnClose;

		bool populating = false;

		public static void DestroyInstance() { Instance = null; }

		public static void ShowScreen(bool show, System.Action<string, Color> OnSelect, System.Action OnClose)
		{
			if (Instance == null)
			{
				Instance = Util.KInstantiateUI<SpriteSelectorScreen>(ModAssets.IconSelectorGO, ModAssets.ParentScreen, true);
				Instance.Init();
			}
			Instance.OnSelectEntry = OnSelect;
			Instance.OnClose = OnClose;
			Instance.gameObject.SetActive(show);
			if (show)
			{
				Instance.transform.SetAsLastSibling();
				//Instance.StartCoroutine(Instance.RefreshSize());
			}
		}
		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			Init();
		}
		bool init;
		void Init()
		{
			if (init)
				return;
			init = true;

			FilterInput = transform.Find("IconList/SearchBar/Input").gameObject.AddOrGet<FInputField2>();
			FilterInput.OnValueChanged.AddListener(OnTextFilterChange);
			//TitleInput.OnValueChanged.AddListener(ApplyBlueprintFilter);
			FilterInput.Text = string.Empty;
			Container = transform.Find("IconList/ScrollArea/Content").gameObject;
			Prefab = Container.transform.Find("Item").gameObject.AddOrGet<IconSelectionEntry>();
			Prefab.CollectReferences();
			Prefab.gameObject.SetActive(false);
			PrefabGO = Prefab.gameObject;
			ClearSearchBar = transform.Find("IconList/SearchBar/DeleteButton").gameObject.AddOrGet<FButton>();
			ClearSearchBar.OnClick += () => FilterInput.Text = string.Empty;
			Close = transform.Find("CloseButton").gameObject.AddOrGet<FButton>();
			Close.OnClick += () => Show(false);
			PopulateGallery();

		}

		void OnTextFilterChange(string text)
		{
			if (populating)
				return;

			text = text.Trim().ToLowerInvariant();
			bool hasFilter = text.Any();
			foreach (var entry in SpriteIcons)
			{
				bool shouldBeActive = !hasFilter || entry.Key.ToLowerInvariant().Contains(text);
				entry.Value.SetActive(shouldBeActive);
			}
		}
		void SelectIcon(string iconId, Color tint)
		{
			SgtLogger.l("Setting icon to: " + iconId);
			OnSelectEntry?.Invoke(iconId, tint);
			Show(false);
		}
		public override void OnShow(bool show)
		{
			base.OnShow(show);
			if (!show)
				Instance.OnClose?.Invoke();
		}

		void AddGalleryItem(string name, Sprite sprite, Color color, string iconId)
		{
			if (name.IsNullOrWhiteSpace() || SpriteIcons.ContainsKey(name))
				return;

			var entry = Util.KInstantiateUI<IconSelectionEntry>(PrefabGO, Container, true);
			entry.Init(name, sprite, () => SelectIcon(iconId, color), color);
			entry.gameObject.name = name;
			SpriteIcons[name] = entry.gameObject;
		}

		List<GalleryItemDataEntry> toPopulate = [];
		public record GalleryItemDataEntry(string name, Sprite sprite, Color tint, string iconId);

		IEnumerator PopulateAsync()
		{
			SetPopulating(true);
			int count = toPopulate.Count;
			for (int i = 0; i < count; ++i)
			{
				var entry = toPopulate[i];
				AddGalleryItem(entry.name, entry.sprite, entry.tint, entry.iconId);
				if (i % 50 == 0) // every 50 items, yield to avoid freezing
					yield return null; // wait a frame to avoid freezing the game
			}
			SgtLogger.l("Populated icon gallery with " + count + " items");
			toPopulate.Clear();
			SetPopulating(false);
		}
		void SetPopulating(bool populating)
		{
			this.populating = populating;
			Close.SetInteractable(!populating);
			ClearSearchBar.SetInteractable(!populating);
		}
		void PopulateGallery()
		{
			toPopulate.Clear();
			PopulateElements(false);
			PopulateCritters(false);
			PopulateBuildings(false);
			PopulateSprites(false);
			StartCoroutine(PopulateAsync());
		}



		void PopulateElements(bool executeCoroutine = true)
		{
			foreach (var element in ElementLoader.elements)
			{
				if (element.IsSolid)
					toPopulate.Add(new(element.name, Def.GetUISprite(element).first, Color.white, element.tag.ToString()));
				else
				{
					var c = Def.GetUISprite(element);
					Color color = c.second;
					color.a = 1;
					toPopulate.Add(new(element.name, c.first, color, element.tag.ToString()));
				}
			}
			if (executeCoroutine)
				StartCoroutine(PopulateAsync());
		}
		void PopulateCritters(bool executeCoroutine = true)
		{
			foreach (var entity in Assets.GetPrefabsWithTag(GameTags.Creature))
			{
				string name = entity.GetProperName();
				var spriteColor = Def.GetUISprite(entity);
				Color color = spriteColor.second;
				color.a = 1;
				toPopulate.Add(new(name, spriteColor.first, color, entity.PrefabID().ToString()));
			}
			if (executeCoroutine)
				StartCoroutine(PopulateAsync());
		}
		void PopulateBuildings(bool executeCoroutine = true)
		{
			foreach (var buildingDef in Assets.BuildingDefs)
			{
				string name = buildingDef.BuildingComplete.GetProperName();
				if (name.Contains("MISSING.") || name.IsNullOrWhiteSpace())
					name = buildingDef.PrefabID;
				if (name.IsNullOrWhiteSpace())
					continue;
				toPopulate.Add(new(name, buildingDef.GetUISprite(), Color.white, buildingDef.PrefabID));
			}
			if (executeCoroutine)
				StartCoroutine(PopulateAsync());
		}

		void PopulateSprites(bool executeCoroutine = true)
		{
			foreach (var sprite in Assets.Sprites.Values)
			{
				toPopulate.Add(new(sprite.name, sprite, Color.white, sprite.name));
			}
			if (executeCoroutine)
				StartCoroutine(PopulateAsync());
		}

		public override void Show(bool show = true)
		{
			if (populating)
				return;

			base.Show(show);
		}

		public override void OnKeyDown(KButtonEvent e)
		{
			if (e.TryConsume(Action.Escape) || e.TryConsume(Action.MouseRight))
			{
				this.Show(false);
			}
			if (e.TryConsume(Action.Find))
			{
				FilterInput.ExternalStartEditing();
			}

			base.OnKeyDown(e);
		}
	}
}

