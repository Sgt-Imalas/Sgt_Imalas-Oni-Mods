using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace _WorldGenStateCapture.WorldStateData
{
	public class biomePolygon
	{
		public List<SerializableVector2> points = new();
		//json serialisation dies in an endless loop on normal vector2s
		public struct SerializableVector2
		{
			public SerializableVector2(Vector2 source)
			{
				x = Mathf.RoundToInt(source.x); y = Mathf.RoundToInt(source.y);
			}
			public SerializableVector2(float _x, float _y)
			{
				x = Mathf.RoundToInt(_x); y = Mathf.RoundToInt(_y);
			}
			public SerializableVector2(int _x, int _y)
			{
				x = _x; y = _y;
			}
			public int x, y;
		}
	}
}
