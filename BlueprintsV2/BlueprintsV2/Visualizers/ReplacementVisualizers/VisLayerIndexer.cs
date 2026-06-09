using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using static Rendering.BlockTileRenderer;

namespace BlueprintsV2.BlueprintsV2.Visualizers.ReplacementVisualizers
{
	/// <summary>
	/// mirror from grid.layerindexer, but instead mapping the ReplacementVis
	/// </summary>
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct VisLayerIndexer
	{
		public VisLayerIndexer()
		{
			VisObjectLayers = new Dictionary<int, ReplacementVis>[(int)ObjectLayer.NumLayers];
			for (int index = 0; index < VisObjectLayers.Length; ++index)
				VisObjectLayers[index] = new Dictionary<int, ReplacementVis>();
		}

		static Dictionary<int, ReplacementVis>[] VisObjectLayers;
		public ReplacementVis this[int cell, int layer]
		{
			get
			{
				if (VisObjectLayers[layer].TryGetValue(cell, out var value))
					return value;
				return null;
			}
			set
			{
				if (value == null)
				{
					VisObjectLayers[layer].Remove(cell);
				}
				else
				{
					VisObjectLayers[layer][cell] = value;
				}
				//GameScenePartitioner.Instance.TriggerEvent(cell, GameScenePartitioner.Instance.objectLayers[layer], value);
			}
		}


	}
}
