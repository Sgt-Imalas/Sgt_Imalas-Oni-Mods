using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;
using static STRINGS.BUILDINGS.PREFABS.EXTERIORWALL.FACADES;
using static STRINGS.CODEX;

namespace ForceFieldWallTile.Content.ModDb
{
	internal class ModEffects
	{
		public const string FFT_StuckInBarrier_ID = "ForceFieldTile_StuckInBarrier";
		public static void Register()
		{
			var db = Db.Get();
			var attributes = db.Attributes;
			var athlethics = db.Attributes.Athletics.Id;


			new EffectBuilder(FFT_StuckInBarrier_ID, 0, true)
				.Modifier(athlethics, -0.5f, true)
				.Modifier(athlethics, -5f)
				.Add(db, out _);

		}
	}
}
