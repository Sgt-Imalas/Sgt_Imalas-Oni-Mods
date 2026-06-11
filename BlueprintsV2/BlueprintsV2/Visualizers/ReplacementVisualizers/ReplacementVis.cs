using BlueprintsV2.BlueprintData;
using KSerialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UtilLibs;
using static BlueprintsV2.STRINGS.BLUEPRINTS_BLUEPRINTNOTE;
using static Grid.Restriction;
using static LogicGateVisualizer;
using static Rendering.BlockTileRenderer;
using static STRINGS.UI.SANDBOXTOOLS.SETTINGS;
using static STRINGS.UI.TOOLS;

namespace BlueprintsV2.BlueprintsV2.Visualizers.ReplacementVisualizers
{

	public class ReplacementVis : KMonoBehaviour
	{
		[Serialize]
		public bool TryReplacing = true;
		[Serialize]
		protected string buildingDefId;
		[Serialize]
		protected Orientation orientation;
		[Serialize]
		protected int conduitFlags = -1;
		[Serialize]
		protected Dictionary<string, string> buildingDataSerialized = new();
		[Serialize]
		protected Tag[] selectedElements = [];

		protected BuildingDef def;
		[Serialize]
		protected int cell;

		protected HashSet<int> occupiedCells = new();
		protected Extents extents;
		protected static Dictionary<Deconstructable, ReplacementVis> queuedDeconstructablesGlobal = new();


		public static VisLayerIndexer Visualizers = new();


		[MyCmpGet] protected KBatchedAnimController kbac;
		private HandleVector<int>.Handle partitionerEntry;
		[MyCmpReq] KSelectable selectable;
		[MyCmpReq] InfoDescription description;
		[MyCmpGet] protected VisualizerRotatable visRot;
		[MyCmpGet] protected SpriteRenderer tileSpriteRenderer;

		List<int> subs = [];
		Coroutine check = null;
		bool replacementInProgress = false;
		bool markedForDeletion = false;
		HashSet<ObjectLayer> layersToReplace = null;

		public void Configure(int cell, BuildingConfig building, Orientation orientation, IEnumerable<Tag> elements)
		{
			this.cell = cell;
			this.buildingDefId = building.BuildingDef.PrefabID;
			this.orientation = orientation;
			building.GetConduitFlags(out int flags);
			this.conduitFlags = flags;
			this.selectedElements = elements.ToArray();
			this.buildingDataSerialized = new();
			if (building.AdditionalBuildingData != null)
				foreach (var data in building.AdditionalBuildingData)
					buildingDataSerialized[data.Key] = JsonConvert.SerializeObject(data.Value);

			InitDefAndAnim();
		}
		void InitDefAndAnim()
		{
			def = Assets.GetBuildingDef(buildingDefId);
			if (def != null && kbac != null)
			{
				kbac.SwapAnims(def.AnimFiles);
				if (visRot != null)
				{
					visRot.Init(def, this.orientation);
					visRot.UpdateRotation();
				}
			}
			layersToReplace = [def.ObjectLayer];
			if (def.TileLayer != ObjectLayer.NumLayers)
				layersToReplace.Add(def.TileLayer);
			if (def.ReplacementLayer != ObjectLayer.NumLayers)
				layersToReplace.Add(def.ReplacementLayer);
			if (def.ReplacementCandidateLayers != null && def.ReplacementCandidateLayers.Any())
				foreach (var layer in def.ReplacementCandidateLayers)
					layersToReplace.Add(layer);
		}
		private void OnRefreshUserMenu(object data)
		{
			Game.Instance.userMenu.AddButton(this.gameObject, new KIconButtonMenu.ButtonInfo("action_cancel", global::STRINGS.UI.USERMENUACTIONS.CANCELCONSTRUCTION.NAME, DestroySelf, tooltipText: global::STRINGS.UI.USERMENUACTIONS.CANCELCONSTRUCTION.TOOLTIP));
		}

		public override void OnSpawn()
		{
			base.OnSpawn();
			InitDefAndAnim();
			SgtLogger.l("Spawning ID: " + buildingDefId);
			var elementTag = selectedElements.Any() ? selectedElements[0] : SimHashes.COMPOSITION.CreateTag();
			var element = ElementLoader.GetElement(elementTag);
			string elementName = element != null ? element.nameUpperCase : global::STRINGS.UI.ELEMENTAL.AGE.UNKNOWN;

			string nameWithMaterial = StringFormatter.Replace(StringFormatter.Replace(global::STRINGS.UI.TOOLS.GENERIC.BUILDING_HOVER_NAME_FMT, "{Name}", def.Name), "{Element}", elementName);
			selectable.SetName(global::STRINGS.UI.ROLES_SCREEN.SLOTS.ASSIGNMENT_PENDING + " " + nameWithMaterial);


			description.description = STRINGS.BLUEPRINTS_FORCE_REPLACER.PENDING_INFO + "\n\n" + def.Desc + "\n\n" + def.Effect;

			if (def == null)
			{
				SgtLogger.warning("Could not find building def for id " + buildingDefId);
				Destroy(this.gameObject);
				return;
			}
			UpdateVisualState();
			SeatVis();


			subs.Add(Subscribe((int)GameHashes.RefreshUserMenu, OnRefreshUserMenu));
			subs.Add(Subscribe((int)GameHashes.Cancel, OnCancel));
		}
		public override void OnCleanUp()
		{
			foreach (var sub in subs)
				Unsubscribe(sub);
			if (check != null)
				StopCoroutine(check);
			UnseatVis();
			base.OnCleanUp();
		}
		void DestroySelf()
		{
			markedForDeletion = true;
			UnityEngine.Object.Destroy(gameObject);
		}
		void UnseatVis()
		{
			foreach (var occupiedCell in occupiedCells)
			{
				Visualizers[occupiedCell, (int)def.ObjectLayer] = null;
			}

			if (TryReplacing)
			{
				RefreshPendingDeconstructs(false);
				GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
			}
		}
		void SeatVis()
		{
			DetermineOccupiedCells();
			foreach (var occupiedCell in occupiedCells)
			{
				var existingVisOnCell = Visualizers[occupiedCell, (int)def.ObjectLayer];
				if (existingVisOnCell != null && existingVisOnCell != this)
				{
					existingVisOnCell.DestroySelf();
				}
				Visualizers[occupiedCell, (int)def.ObjectLayer] = this;
			}
			if (TryReplacing)
			{
				RefreshPendingDeconstructs(true);
				partitionerEntry = GameScenePartitioner.Instance.Add("ReplacementVis.ReCheckPlacement", (object)this.gameObject, new Extents(this.extents.x - 1, this.extents.y - 1, this.extents.width + 2, this.extents.height + 2), GameScenePartitioner.Instance.objectLayers[(int)def.ObjectLayer], OnPreoccupiedCellChanged);
				GameScheduler.Instance.ScheduleNextFrame("ReplacementVisInitialCheck", OnPreoccupiedCellChanged);
			}
		}
		void RefreshPendingDeconstructs(bool deconstruct)
		{
			foreach (var cell in occupiedCells)
			{
				foreach (var layer in layersToReplace)
					DoDeconstrucThingsAt(cell, layer, deconstruct);

				if (deconstruct)
					CancelPlannedOccupyingBuildings(cell);
			}
		}

		void CancelPlannedOccupyingBuildings(int cell)
		{
			for (int i = 0; i < (int)ObjectLayer.NumLayers; i++)
			{
				var existingItem = Grid.Objects[cell, i];
				if (existingItem == null
					|| !existingItem.TryGetComponent<Building>(out var building)
					|| !existingItem.TryGetComponent<Constructable>(out var constructable))
					continue;

				var existingDef = building.Def;

				if (layersToReplace.Contains(existingDef.ObjectLayer)
				|| layersToReplace.Contains(existingDef.TileLayer) //defaults to numlayers which is never in the hashset
				|| layersToReplace.Contains(existingDef.ReplacementLayer)
				|| (existingDef.ReplacementCandidateLayers != null && existingDef.ReplacementCandidateLayers.Any(layer => layersToReplace.Contains(layer)))
					)
					existingItem.Trigger((int)GameHashes.Cancel);
			}
		}

		void DoDeconstrucThingsAt(int cell, ObjectLayer layer, bool deconstruct)
		{
			var existingBuilding = Grid.Objects[cell, (int)layer];
			if (existingBuilding == null)
				return;

			if (existingBuilding.TryGetComponent<Deconstructable>(out var decon))
			{
				if (!decon.allowDeconstruction && deconstruct)
				{
					this.DestroySelf();
					return;
				}
				if (deconstruct && decon.allowDeconstruction)
				{
					queuedDeconstructablesGlobal[decon] = this;
					decon.QueueDeconstruction();
				}
				else if (!deconstruct)
				{
					if (queuedDeconstructablesGlobal.TryGetValue(decon, out var registered) && registered == this)
						decon.CancelDeconstruction();
				}
			}
		}

		IEnumerator DelayedPlacementCheck()
		{
			yield return null;
			FinalizePlacementCheck();
		}
		void FinalizePlacementCheck()
		{
			check = null;
			if (!TryReplacing || replacementInProgress)
				return;
			if (TryPlacingQueuedBP())
			{
				UnseatVis();
				DestroySelf();
			}
			else
				UpdateVisualState();
		}
		void OnPreoccupiedCellChanged(object data)
		{
			if (check != null || replacementInProgress || markedForDeletion)
				return;
			check = StartCoroutine(DelayedPlacementCheck());
		}
		bool TryPlacingQueuedBP()
		{
			replacementInProgress = true;
			Vector3 posCbc = Grid.CellToPosCBC(cell, Grid.SceneLayer.Building);
			GameObject builtItem;

			if (!BlueprintState.InstantBuild)
			{
				var placer = Util.KInstantiate(def.BuildingUnderConstruction, Grid.CellToPosCBC(cell, def.SceneLayer));
				builtItem = def.TryPlace(placer, posCbc, orientation, selectedElements, null);
				UnityEngine.Object.DestroyImmediate(placer);
			}
			else
			{
				builtItem = def.Build(cell, orientation, null, selectedElements, ModAssets.GetSpawnTemperature(def, selectedElements), null, false, GameClock.Instance.GetTime());
			}
			replacementInProgress = false;
			if (builtItem == null)
				return false;

			ApplyExtraDataToBuilt(builtItem);

			if (builtItem.TryGetComponent<UnderConstructionDataTransfer>(out var dataTransfer))
			{
				foreach (var kvp in buildingDataSerialized)
				{
					dataTransfer.SetDataToApply(kvp.Key, kvp.Value);
				}
			}
			else
			{
				foreach (var kvp in buildingDataSerialized)
				{
					ModAPI.API_Methods.TryApplyingStoredData(builtItem, kvp.Key, JObject.Parse(kvp.Value));
				}
			}
			return true;
		}
		protected virtual void ApplyExtraDataToBuilt(GameObject built)
		{

		}

		void DetermineOccupiedCells()
		{
			occupiedCells.Clear();
			foreach (var offset in def.PlacementOffsets)
			{
				var rotated = Rotatable.GetRotatedCellOffset(offset, orientation);
				occupiedCells.Add(Grid.OffsetCell(cell, rotated));
			}
			Grid.CellToXY(cell, out int x, out int y);
			int val1_1 = x;
			int val1_2 = y;
			foreach (int placementCell in occupiedCells)
			{
				int val2_1 = 0;
				int val2_2 = 0;
				ref int local1 = ref val2_1;
				ref int local2 = ref val2_2;
				Grid.CellToXY(placementCell, out local1, out local2);
				x = Math.Min(x, val2_1);
				y = Math.Min(y, val2_2);
				val1_1 = Math.Max(val1_1, val2_1);
				val1_2 = Math.Max(val1_2, val2_2);
			}
			this.extents.x = x;
			this.extents.y = y;
			this.extents.width = val1_1 - x + 1;
			this.extents.height = val1_2 - y + 1;
		}

		protected virtual void UpdateVisualState()
		{
			if (kbac == null)
			{
				this.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Place"));
				return;
			}

			kbac.SetLayer(LayerMask.NameToLayer("Place"));
			if (def.isKAnimTile && conduitFlags >= 0)
			{
				if (PlayUtilityAnim(kbac))
					return;
			}
			else
				kbac.Play("place");
		}
		protected bool PlayUtilityAnim(KBatchedAnimController targetKbac)
		{
			IUtilityNetworkMgr utilityNetworkManager = def.BuildingComplete.GetComponent<IHaveUtilityNetworkMgr>().GetNetworkManager();
			string animation = utilityNetworkManager.GetVisualizerString((UtilityConnections)conduitFlags) + "_place";

			if (utilityNetworkManager != null && targetKbac.HasAnimation(animation))
			{
				targetKbac.Play(animation);
				return true;
			}
			return false;
		}

		public static Bits GetConnectionBits(BuildingDef def, int cell, int query_layer)
		{
			var pos = Grid.CellToPosCCC(cell, Grid.SceneLayer.TileMain);
			return GetConnectionBits(def, (int)pos.x, (int)pos.y, query_layer);
		}
		public static Bits GetConnectionBits(BuildingDef def, int x, int y, int query_layer)
		{
			Bits bits = (Bits)0;
			Tag tileTag = def.PrefabID;
			if (tileTag == null || tileTag == Tag.Invalid)
				return bits;

			if (y > 0)
			{
				int num = (y - 1) * Grid.WidthInCells + x;
				if (x > 0 && MatchesDef(Visualizers[num - 1, query_layer], tileTag))
				{
					bits |= Bits.DownLeft;
				}

				if (MatchesDef(Visualizers[num, query_layer], tileTag))
				{
					bits |= Bits.Down;
				}

				if (x < Grid.WidthInCells - 1 && MatchesDef(Visualizers[num + 1, query_layer], tileTag))
				{
					bits |= Bits.DownRight;
				}
			}

			int num2 = y * Grid.WidthInCells + x;
			if (x > 0 && MatchesDef(Visualizers[num2 - 1, query_layer], tileTag))
			{
				bits |= Bits.Left;
			}

			if (x < Grid.WidthInCells - 1 && MatchesDef(Visualizers[num2 + 1, query_layer], tileTag))
			{
				bits |= Bits.Right;
			}

			if (y < Grid.HeightInCells - 1)
			{
				int num3 = (y + 1) * Grid.WidthInCells + x;
				if (x > 0 && MatchesDef(Visualizers[num3 - 1, query_layer], tileTag))
				{
					bits |= Bits.UpLeft;
				}

				if (MatchesDef(Visualizers[num3, query_layer], tileTag))
				{
					bits |= Bits.Up;
				}

				if (x < Grid.WidthInCells + 1 && MatchesDef(Visualizers[num3 + 1, query_layer], tileTag))
				{
					bits |= Bits.UpRight;
				}
			}

			return bits;
		}
		public static bool MatchesDef(ReplacementVis other, Tag target)
		{
			return other != null && other.buildingDefId == target;
		}

		internal static void CancelToolTriggered(int cell)
		{
			for (int i = 0; i < 45; i++)
			{
				var vis = Visualizers[cell, i];
				if (vis != null)
				{
					vis.DestroySelf();
				}
			}
		}
		public void OnCancel(object _)
		{
			DestroySelf();
		}
	}
}
