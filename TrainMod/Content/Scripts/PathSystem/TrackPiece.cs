using KSerialization;
using PeterHan.PLib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using YamlDotNet.Core;

namespace TrainMod.Content.Scripts.PathSystem
{
	public class TrackPiece : KMonoBehaviour
	{
		[MyCmpGet] KBatchedAnimController kbac;

		[SerializeField]
		public int PathCost = 10;

		[MyCmpReq]
		Rotatable rotatable;

		[SerializeField]
		public CellOffset InputCellOffset;
		[SerializeField]
		public CellOffset InputCellOffsetConnectsTo;
		[SerializeField]
		public CellOffset[] OutputCellOffsets;
		[SerializeField]
		public CellOffset[] OutputCellOffsetsConnectsTo;

		[Serialize]
		private string _guid;

		public string Guid
		{
			get
			{
				if (_guid == null)
					_guid = new Guid().ToString();
				return _guid;
			}
		}

		public override string ToString()
		{
			return this.GetProperName() + ": " + Grid.CellToXY(Grid.PosToCell(this));
		}

		public Tuple<int, int> InputCell;
		public Tuple<int, int>[] OutputCells;
		public List<Tuple<int, int>> AllConnectionCells;

		TrackPiece InputConnection = null;
		List<TrackPiece> OutputConnections = new();
		public bool IsDivider => OutputConnections.Count > 1;

		public List<TrackPiece> GetOutputConnections() => OutputConnections;
		public List<TrackPiece> GetInputConnections() => InputConnection != null ? [InputConnection] : new();

		/// <summary>
		/// returns the pieces a train coming from the source can drive towards
		/// </summary>
		/// <param name="connectedSource"></param>
		/// <param name="others"></param>
		/// <returns></returns>
		public bool GetReachableConnectionsFrom(TrackPiece connectedSource, out List<TrackPiece> others)
		{
			others = null;
			if (connectedSource == InputConnection)
			{
				//SgtLogger.l(connectedSource.GetProperName() + " sits as input, returning output connections");
				others = GetOutputConnections();
				return others != null && others.Any();
			}
			if (OutputConnections.Contains(connectedSource))
			{
				//SgtLogger.l(connectedSource.GetProperName() + " sits at output, returning input connections");
				others = GetInputConnections();
				return others != null && others.Any();
			}
			return false;
		}

		public override void OnSpawn()
		{
			base.OnSpawn();
			CacheConnectionCells();
			Subscribe((int)GameHashes.SelectObject, OnSelectionChanged);
			TrackManager.RegisterTrack(this);
		}
		public override void OnCleanUp()
		{
			base.OnCleanUp();
			Unsubscribe((int)GameHashes.SelectObject, OnSelectionChanged);
			CleanupRenderers();
			TrackManager.UnregisterTrack(this);
		}
		#region debugSpawner
		bool isSelected = false;
		private void OnSelectionChanged(object data)
		{
			bool enabled = ((Boxed<bool>)data).value;
			isSelected = enabled;
			if (enabled)
				MakeRenderers();
			else
				CleanupRenderers();

		}
		void CleanupRenderers()
		{
			foreach (var connected in this.GetAllConnections())
				connected.Tint(Color.white);


			for (int i = indicators.Count - 1; i >= 0; i--)
			{
				var indicator = indicators[i];
				if (indicator != null && indicator.gameObject)
					Destroy(indicator.gameObject);
			}
			indicators.Clear();
		}
		void MakeRenderers()
		{
			foreach (var connected in this.GetInputConnections())
				connected.Tint(Color.cyan);
			foreach (var connected in this.GetOutputConnections())
				connected.Tint(Color.yellow);

			return;
			//indicators.Add(spawnRenderer(InputCell, UIUtils.Darken(Color.green, 50)));
			//indicators.Add(spawnRenderer(InputConnectionCell, Color.green));
			//foreach (var cell in OutputCells)
			//	indicators.Add(spawnRenderer(cell, UIUtils.Darken(Color.red, 50)));
			//foreach (var cell in outputConnectionCells)
			//	indicators.Add(spawnRenderer(cell, Color.red));

		}

		GameObject spawnRenderer(int cell, Color color)
		{
			////testGo = Instantiate(ModAssets.Prefabs.slimyPulse);
			//var textureGO = new GameObject();
			//textureGO.SetActive(false);
			//var renderer = textureGO.AddComponent<KBatchedAnimController>();
			//renderer.AnimFiles = [Assets.GetAnim("baby_lightbug_kanim")];
			//textureGO.SetActive(true);
			//renderer.Play("idle_loop");

			var pos = Grid.CellToPos(cell);
			var bebi = Util.KInstantiate(Assets.GetPrefab("LightBugBaby"), pos);
			var kbac = bebi.GetComponent<KBatchedAnimController>();


			pos.x += 0.5f;
			pos.y += 0.5f;
			pos.z = Grid.GetLayerZ(Grid.SceneLayer.FXFront2);
			bebi.transform.position = pos; //z value is one layer above liquid
			kbac.TintColour = color; // set to biome color + bit of extra dark for blending better. use KAnimBatchedController.OnHighlightChanged on the main kbac to listen to changes for building selection

			bebi.SetActive(true);
			return bebi;
		}

		List<GameObject> indicators = new();
		#endregion


		void CacheConnectionCells()
		{
			int cell = Grid.PosToCell(this);
			InputCell =
				new(Grid.OffsetCell(cell, rotatable.GetRotatedCellOffset(InputCellOffset)),
				Grid.OffsetCell(cell, rotatable.GetRotatedCellOffset(InputCellOffsetConnectsTo)));


			List<Tuple<int, int>> outputCells = new(3);
			for (int i = 0; i < OutputCellOffsets.Length; i++)
			{
				var offset = OutputCellOffsets[i];
				var offsetConnectsTo = OutputCellOffsetsConnectsTo[i];
				outputCells.Add(new Tuple<int, int>(
					Grid.OffsetCell(cell, rotatable.GetRotatedCellOffset(offset)),
					Grid.OffsetCell(cell, rotatable.GetRotatedCellOffset(offsetConnectsTo))
					));
			}
			OutputCells = outputCells.ToArray();
			outputCells.Add(InputCell);

			AllConnectionCells = outputCells;

		}
		public void TryAddConnection(TrackPiece other)
		{
			if (other == null || other == this)
				return;
			if (other.AllConnectionCells.Any(connection => connection.second == InputCell.first && connection.first == InputCell.second))
			{
				InputConnection = other;
				SgtLogger.l("adding edge from " + this + " to " + other);
			}
			foreach (var outputCell in OutputCells)
			{
				if (other.AllConnectionCells.Any(connection => connection.second == outputCell.first && connection.first == outputCell.second))
				{
					OutputConnections.Add(other);
					SgtLogger.l("adding edge from " + this + " to " + other);
					break;
				}
			}
		}
		public void RemoveConnection(TrackPiece other)
		{
			if (other == null)
				return;
			
			if (InputConnection == other)
				InputConnection = null;

			if (OutputConnections.Contains(other))
				OutputConnections.Remove(other);

		}

		public List<TrackPiece> GetAllConnections()
		{
			if (InputConnection != null)
				return OutputConnections.Append(InputConnection).ToList();
			return OutputConnections;
		}
		internal void RemoveAllConnections()
		{
			InputConnection?.RemoveConnection(this);
			foreach (var connection in OutputConnections)
				connection.RemoveConnection(this);

			OutputConnections.Clear();
		}
		public bool InvertedConnection(TrackPiece other)
		{
			bool InputOwn = InputConnection == other;
			bool InputOther = other.InputConnection == this;
			return InputOwn == InputOther;
		}

		public void Tint(Color? color)
		{
			if (color.HasValue)
			{
				kbac.TintColour = color.Value;
			}
			else
				kbac.TintColour = Color.white;
		}
	}
}
