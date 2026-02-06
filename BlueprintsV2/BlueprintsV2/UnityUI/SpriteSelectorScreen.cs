using BlueprintsV2.BlueprintsV2.BlueprintData.NoteToolPlacedEntities;
using BlueprintsV2.BlueprintsV2.Tools;
using BlueprintsV2.BlueprintsV2.UnityUI.Components;
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
	internal class SpriteSelectorScreen : KScreen
	{
#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable CS0414 // Remove unused private members
		new bool ConsumeMouseScroll = true; // do not remove!!!!
#pragma warning restore CS0414 // Remove unused private members
#pragma warning restore IDE0051 // Remove unused private members

		public static SpriteSelectorScreen Instance = null;
		FButton ClearSearchBar, Close;
		FInputField2 FilterInput;
		Dictionary<string, GameObject> SpriteIcons = [];
		GameObject Container;
		IconSelectionEntry Prefab;
		System.Action<string, Color> OnSelectEntry;

		public static void DestroyInstance() { Instance = null; }

		public static void ShowScreen(bool show, System.Action<string,Color> OnSelect)
		{
			if (Instance == null)
			{
				Instance = Util.KInstantiateUI<SpriteSelectorScreen>(ModAssets.IconSelectorGO, ModAssets.ParentScreen, true);
				Instance.Init();
			}
			Instance.OnSelectEntry = OnSelect;
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
			//StartCoroutine
			PopulateGallery();

		}

		void OnTextFilterChange(string text)
		{
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
			SgtLogger.l("Setting icon to: "+iconId);
			OnSelectEntry?.Invoke(iconId, tint);
			Show(false);
		}
		void PopulateGallery()
		{
			var go = Prefab.gameObject;

			void AddIcon(string name, Sprite sprite, Color color, string iconId)
			{
				var entry = Util.KInstantiateUI<IconSelectionEntry>(go, Container, true);
				entry.Init(name, sprite, () => SelectIcon(iconId, color), color);
				entry.gameObject.name = name;
				SpriteIcons[name] = entry.gameObject;
			}

			foreach (var element in ElementLoader.elements)
			{
				if (element.IsSolid)
					AddIcon(element.name, Def.GetUISprite(element).first, Color.white, element.tag.ToString());
				else
				{
					var c = Def.GetUISprite(element);
					Color color = c.second;
					color.a = 1;
					AddIcon(element.name, c.first, color, element.tag.ToString());
				}
			}

			foreach (var entity in Assets.GetPrefabsWithTag(GameTags.Creature))
			{
				string name = entity.GetProperName();
				var spriteColor = Def.GetUISprite(entity);
				Color color = spriteColor.second;
				color.a = 1;
				AddIcon(name, spriteColor.first, color, entity.PrefabID().ToString());
			}
			foreach (var buildingDef in Assets.BuildingDefs)
			{
				string name = buildingDef.BuildingComplete.GetProperName();
				var spriteColor = Def.GetUISprite(buildingDef.BuildingComplete);
				Color color = spriteColor.second;
				color.a = 1;
				AddIcon(name, spriteColor.first, color, buildingDef.PrefabID);
			}




			List<Sprite> icons = Assets.Sprites.Values.ToList();
			while (icons.Any())
			{
				var icon = icons.First();
				var name = icon.name;
				icons.Remove(icon);
				if(SpriteIcons.ContainsKey(name))
					continue;
				AddIcon(name, icon, Color.white, name);
			}
		}
		public override void OnKeyDown(KButtonEvent e)
		{
			if (e.TryConsume(Action.Escape) || e.TryConsume(Action.MouseRight))
			{
				this.Show(false);
			}
			if (e.TryConsume(Action.DebugToggleClusterFX))
			{
				FilterInput.ExternalStartEditing();
			}

			base.OnKeyDown(e);
		}
	}
}

