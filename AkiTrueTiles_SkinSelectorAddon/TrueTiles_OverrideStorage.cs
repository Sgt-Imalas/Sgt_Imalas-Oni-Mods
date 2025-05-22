using KSerialization;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static Components;
using static UnityEngine.UI.Image;

namespace AkiTrueTiles_SkinSelectorAddon
{
	class TrueTiles_OverrideStorage : KMonoBehaviour
	{
		
		public static Dictionary<int, TrueTiles_OverrideStorage> Cmps = new Dictionary<int, TrueTiles_OverrideStorage>();
		public static bool TryGetElement(int cell, out SimHashes element)
		{
			if (Cmps.TryGetValue(cell, out var cmp))
			{
				element = cmp.HasOverride ? cmp.OverrideElement : cmp.OriginalElement;
				return true;
			}
			element = SimHashes.Vacuum;
			return false;
		}


		int cell;
		public int Cell => cell;
		[Serialize]
		public SimHashes OverrideElement = SimHashes.Void;

		public bool HasOverride => OverrideElement != SimHashes.Void;

		[MyCmpGet]
		private Building building;
		[MyCmpGet]
		private PrimaryElement primaryElement;
		public SimHashes OriginalElement => primaryElement.ElementID;

		[MyCmpGet]
		private Constructable constructable;
		public bool IsReplacementTile => constructable != null && constructable.IsReplacementTile;

		public BuildingDef Def => building.Def;

		[MyCmpAdd] CopyBuildingSettings copyBuildingSettings;
		private static readonly EventSystem.IntraObjectHandler<TrueTiles_OverrideStorage> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<TrueTiles_OverrideStorage>((System.Action<TrueTiles_OverrideStorage, object>)((component, data) => component.OnCopySettings(data)));

		public override void OnSpawn()
		{
			cell = Grid.PosToCell(this);
			Cmps[cell] = this;
			base.OnSpawn();

			if (HasOverride)
			{
				SetOverride(OverrideElement, true);
			}
			this.Subscribe(-905833192, OnCopySettingsDelegate);
		}
		public override void OnCleanUp()
		{
			base.OnCleanUp();
			Cmps.Remove(cell);
			ModAssets.ScheduleCellRefresh(cell);
			this.Unsubscribe(-905833192, OnCopySettingsDelegate);
		}
		private void OnCopySettings(object data)
		{
			TrueTiles_OverrideStorage component = ((GameObject)data).GetComponent<TrueTiles_OverrideStorage>();
			if (component == null)
				return;
			var targetElement = component.HasOverride ? component.OverrideElement : component.OriginalElement;
			this.SetOverride(targetElement, false);
		}

		public List<SimHashes> GetAvailableElementOverrides()
		{
			List<SimHashes> validMaterials = new List<SimHashes>();
			var actualTags = building.Def.MaterialCategory.First()?.ToString().Split('&');
			foreach (var actualTag in actualTags)
			{
				foreach (Element element in ElementLoader.elements)
				{
					if (element.IsSolid && (element.tag == actualTag || element.HasTag(actualTag)))
					{
						validMaterials.Add(element.id);
					}
				}
			}
			return validMaterials;
		}

		internal bool IsCurrentOverride(SimHashes element)
		{
			if (!HasOverride)
				return OriginalElement == element;
			return OverrideElement == element;
		}

		public SimHashes CurrentDisplayElement => HasOverride ? OverrideElement : OriginalElement;
		
		public void ResetOverride()
		{
			if (HasOverride)
			{
				SetOverride(OriginalElement);
			}
		}

		internal void SetOverride(SimHashes newElement, bool init = false)
		{
			//dont do anything if the new element is the same as the current one
			if (!init)
			{
				if ((!HasOverride && newElement == OriginalElement) || (HasOverride && OverrideElement == newElement))
				{
					return;
				}
			}

			//remove old block
			SimHashes toRemoveBlock = HasOverride && !init ? OverrideElement : OriginalElement;
			SgtLogger.l("Removing block for " + toRemoveBlock.ToString() + " at " + Cell);
			World.Instance.blockTileRenderer.RemoveBlock(Def, IsReplacementTile, toRemoveBlock, Cell);
			SgtLogger.l("Adding block for " + newElement.ToString() + " at " + Cell);
			World.Instance.blockTileRenderer.AddBlock(gameObject.layer, Def, IsReplacementTile, newElement, cell);

			if (newElement == OriginalElement)
			{
				OverrideElement = SimHashes.Void;
			}
			else
				OverrideElement = newElement;

			TileVisualizer.RefreshCell(cell, ObjectLayer.FoundationTile, ObjectLayer.ReplacementTile);
			ModAssets.ScheduleCellRefresh(Cell);
		}
		///BlueprintsV2 integration
		public static JObject Blueprints_GetData(GameObject arg)
		{
			if (arg.TryGetComponent<TrueTiles_OverrideStorage>(out var component) && component.HasOverride)
			{
				return new JObject()
					{
						{ "OverrideElement", (int)component.CurrentDisplayElement}
					};
			}
			return null;
		}
		public static void Blueprints_SetData(GameObject building, JObject jObject)
		{
			if (jObject == null)
				return;
			if (building.TryGetComponent<TrueTiles_OverrideStorage>(out var targetComponent))
			{
				var t1 = jObject.GetValue("OverrideElement");
				if (t1 == null)
					return;
				var DefaultPermissionBionics = t1.Value<int>();
				var elementSimhash = (SimHashes)DefaultPermissionBionics;
				if(ElementLoader.GetElement(elementSimhash.CreateTag()) == null)
				{
					SgtLogger.l("Element " + elementSimhash.ToString() + " not found");
					return;
				}


				//applying values
				targetComponent.SetOverride(elementSimhash);
			}
		}
	}
}
