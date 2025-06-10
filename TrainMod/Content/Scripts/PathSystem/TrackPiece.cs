using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TrainMod.Content.Scripts.PathSystem
{
	public class TrackPiece : KMonoBehaviour
	{

		[SerializeField]
		public int PathCost = 10;

		[MyCmpReq]
		Rotatable rotatable;

		[SerializeField]
		public CellOffset InputCellOffset;
		[SerializeField]
		public CellOffset[] OutputCellOffsets;

		//[Serialize]
		//private string _guid;

		//public string Guid
		//{
		//	get
		//	{
		//		if (_guid == null)
		//			_guid = new Guid().ToString();
		//		return _guid;
		//	}
		//}

		int inputCell;
		int[] outputCells;
		public int InputCell => inputCell;
		public int[] OutputCells => outputCells;

		TrackPiece InputConnection = null;
		List<TrackPiece> OutputConnections = new();


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
		}
		public void AddConnection(TrackPiece other, int cell)
		{
			if (other == null)
				return;
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
	}
}
