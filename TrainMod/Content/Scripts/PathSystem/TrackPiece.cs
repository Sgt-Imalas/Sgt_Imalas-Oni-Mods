using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

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

		int inputCell;
		int[] outputCells;
		public int InputCell => inputCell;
		public int[] OutputCells => outputCells;

		int inputConnectionCell;
		int[] outputConnectionCells;
		public int InputConnectionCell => inputConnectionCell;
		public int[] OutputConnectionCells => outputConnectionCells;

		TrackPiece InputConnection = null;
		List<TrackPiece> OutputConnections = new();
		public bool IsDivider => OutputConnections.Count > 1;

		public List<TrackPiece> GetOutputConnections() => OutputConnections;
		public List<TrackPiece> GetInputConnections() => InputConnection != null ? [InputConnection] : new();
		public bool GetReachableConnectionsFrom(TrackPiece connectedSource, out List<TrackPiece> others)
		{
			others = null;
			if (connectedSource == InputConnection)
			{
				others = GetOutputConnections();
				return others != null && others.Any();
			}
			if (OutputConnections.Contains(connectedSource))
			{
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
			var enabled = (bool)data;
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
			indicators.Add(spawnRenderer(InputCell, UIUtils.Darken(Color.green, 50)));
			indicators.Add(spawnRenderer(InputConnectionCell, Color.green));
			foreach (var cell in OutputCells)
				indicators.Add(spawnRenderer(cell, UIUtils.Darken(Color.red, 50)));
			foreach (var cell in outputConnectionCells)
				indicators.Add(spawnRenderer(cell, Color.red));

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
			inputCell = Grid.OffsetCell(cell, rotatable.GetRotatedCellOffset(InputCellOffset));
			outputCells = OutputCellOffsets.Select(offset => Grid.OffsetCell(cell, rotatable.GetRotatedCellOffset(offset))).ToArray();

			inputConnectionCell = Grid.OffsetCell(cell, rotatable.GetRotatedCellOffset(InputCellOffsetConnectsTo));
			outputConnectionCells = OutputCellOffsetsConnectsTo.Select(offset => Grid.OffsetCell(cell, rotatable.GetRotatedCellOffset(offset))).ToArray();
		}
		public void AddConnection(TrackPiece other, int cell, int connectionCell)
		{
			if (other == null || other == this)
				return;
			if (cell == InputCell && connectionCell == InputConnectionCell || cell == InputConnectionCell && connectionCell == InputCell)
				InputConnection = other;

			for (int i = 0; i < outputCells.Length; i++)
			{
				var outputCell = OutputCells[i];
				var outputConnectionCel = OutputConnectionCells[i];
				if (outputCell == cell && outputConnectionCel == connectionCell || outputConnectionCel == cell && outputCell == connectionCell)
				{
					OutputConnections.Add(other);
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

		internal bool ConnectsFromTo(int othersConnection, int othersInput)
		{
			if (InputCell == othersConnection && InputConnectionCell == othersInput || InputCell == othersInput && InputConnectionCell == othersConnection)
				return true;

			for (int i = 0; i < OutputCells.Length; ++i)
			{
				var outputCell = OutputCells[i];
				var outputConnectionCel = OutputConnectionCells[i];
				if (outputCell == othersConnection && outputConnectionCel == othersInput || outputCell == othersInput && outputConnectionCel == othersConnection)
					return true;
			}
			return false;
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
