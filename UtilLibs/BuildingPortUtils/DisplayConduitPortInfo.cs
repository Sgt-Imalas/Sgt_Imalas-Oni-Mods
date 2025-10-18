using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UtilLibs.BuildingPortUtils
{

	// can't be stored in components. It somehow gets reset before it's used
	// Serialization doesn't seem to help at all
	public abstract class DisplayConduitPortInfo
	{
		readonly internal ConduitType type;
		readonly internal CellOffset offset;
		readonly internal CellOffset offsetFlipped;
		readonly internal bool input;
		readonly internal Color color;

		public ConduitType Type => type;
		protected DisplayConduitPortInfo(ConduitType type, CellOffset offset, CellOffset? offsetFlipped, bool input, Color? color)
		{
			this.type = type;
			this.offset = offset;
			this.input = input;

			this.offsetFlipped = offsetFlipped ?? offset;

			// assign port colors
			if (color != null)
			{
				this.color = color ?? Color.white;
			}
			else
			{
				// none given. Use defaults
				this.color = SharedConduitUtils.GetIOColor(input,type);
			}
		}
	}
}
