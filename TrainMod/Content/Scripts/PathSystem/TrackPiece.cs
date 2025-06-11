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
		public List<TrackPiece> GetInputConnections() => [InputConnection];
		public bool GetReachableConnectionsFrom(TrackPiece connectedSource, out List<TrackPiece> others)
		{
			SgtLogger.l(message: "MMMM");
			others = null;
			if (connectedSource == InputConnection)
			{
				others = OutputConnections;
				return others.Any();
			}
			if (OutputConnections.Contains(connectedSource))
			{
				others = [InputConnection];
				return others.Any();
			}
			return false;
		}

		public override void OnSpawn()
		{
			base.OnSpawn();
			CacheConnectionCells();
			TrackManager.RegisterTrack(this);
		}
		public override void OnCleanUp()
		{
			base.OnCleanUp();
			TrackManager.UnregisterTrack(this);
		}
		void CacheConnectionCells()
		{
			int cell = Grid.PosToCell(this);
			inputCell = Grid.OffsetCell(cell, rotatable.GetRotatedCellOffset(InputCellOffset));
			outputCells = OutputCellOffsets.Select(offset => Grid.OffsetCell(cell, rotatable.GetRotatedCellOffset(offset))).ToArray();
			inputConnectionCell = Grid.OffsetCell(cell, rotatable.GetRotatedCellOffset(InputCellOffsetConnectsTo));
			outputConnectionCells = OutputCellOffsetsConnectsTo.Select(offset => Grid.OffsetCell(cell, rotatable.GetRotatedCellOffset(offset))).ToArray();
		}
		public void AddConnection(TrackPiece other, int cell)
		{
			if (other == null || other == this)
				return;
			if (cell == InputCell || cell == inputConnectionCell)
				InputConnection = other;

			for (int i = 0; i < outputCells.Length; i++)
			{
				var outputCell = OutputCells[i];
				var outputConnectionCel = OutputConnectionCells[i];
				if (outputCell == cell || outputConnectionCel == cell)
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
			if (InputCell == othersConnection && InputConnectionCell == othersInput)
				return true;

			for (int i = 0; i < OutputCells.Length; ++i)
			{
				var outputCell = OutputCells[i];
				var outputConnectionCel = OutputConnectionCells[i];
				if (outputCell == othersConnection && outputConnectionCel == othersInput)
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
