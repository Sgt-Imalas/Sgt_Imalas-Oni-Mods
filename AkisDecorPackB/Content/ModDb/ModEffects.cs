using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace AkisDecorPackB.Content.ModDb
{
	internal class ModEffects
	{
		public const string INSPIRED_LOW = "DecorpackB_Inspired_Low";
		public const string INSPIRED_OKAY = "DecorpackB_Inspired_Okay";
		public const string INSPIRED_GREAT = "DecorpackB_Inspired_Great";
		public const string INSPIRED_GIANT = "DecorpackB_Inspired_Giant";
		public const float INSPIRED_DURATION = 60f;

		public static void Register(ModifierSet instance)
		{
			var learning = global::Db.Get().Attributes.Learning.Id;

			new EffectBuilder(INSPIRED_LOW, INSPIRED_DURATION, false)
				.Modifier(learning, 1)
				.Add(instance);

			new EffectBuilder(INSPIRED_OKAY, INSPIRED_DURATION, false)
				.Modifier(learning, 2)
				.Add(instance);

			new EffectBuilder(INSPIRED_GREAT, INSPIRED_DURATION, false)
				.Modifier(learning, 4)
				.Add(instance);

			new EffectBuilder(INSPIRED_GIANT, 600f, false)
				.Modifier(learning, 6)
				.Add(instance);
		}
	}
}
