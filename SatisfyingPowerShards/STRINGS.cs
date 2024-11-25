using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatisfyingPowerShards
{
	internal class STRINGS
	{
		public class CREATURES
		{
			public class SPECIES
			{
				public class STATERPILLAR
				{
					public static LocString BASESLUG_POWERSHARDS = "From a certain age, they can be harvested for a power shard.";

					public class VARIANT_YELLOW
					{
						public static LocString NAME = UI.FormatAsLink("Yellow Plug Slug", nameof(STATERPILLAR));
						public static LocString DESC = global::STRINGS.CREATURES.SPECIES.STATERPILLAR.DESC + "\nYellow Variants produce more power of than regular slugs and yield more power shards on harvest after sufficient growth";
						public static LocString EGG_NAME = UI.FormatAsLink("Yellow Slug Egg", nameof(STATERPILLAR));

						public class BABY
						{
							public static LocString NAME = UI.FormatAsLink("Yellow Plug Slug", nameof(STATERPILLAR));
							public static LocString DESC = ("A chubby little Yellow Plug Sluglet.\n\nIn time it will mature into a fully grown " + UI.FormatAsLink("Yellow Plug Slug", nameof(STATERPILLAR)) + ".");
						}
					}
					public class VARIANT_PURPLE
					{
						public static LocString NAME = UI.FormatAsLink("Purple Plug Slug", nameof(STATERPILLAR));
						public static LocString DESC = global::STRINGS.CREATURES.SPECIES.STATERPILLAR.DESC+"\nPurple Variants produce the most power of all and yield more power shards on harvest after sufficient growth";
						public static LocString EGG_NAME = UI.FormatAsLink("Purple Slug Egg", nameof(STATERPILLAR));

						public class BABY
						{
							public static LocString NAME = UI.FormatAsLink("Smog Sluglet", nameof(STATERPILLAR));
							public static LocString DESC = ("A chubby little Purple Plug Sluglet.\n\nIn time it will mature into a fully grown " + UI.FormatAsLink("Purple Plug Slug", nameof(STATERPILLAR)) + ".");
						}
					}
				}

			}
		}
	}
}
