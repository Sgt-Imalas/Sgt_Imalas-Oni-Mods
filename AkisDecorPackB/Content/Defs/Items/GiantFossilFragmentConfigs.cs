using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AkisDecorPackB.Content.Defs.Items
{
	internal
	class GiantFossilFragmentConfigs : IMultiEntityConfig
	{
		public const string
			TREX = "DecorPackB_TrexFragment",
			LIVAYATAN = "DecorPackB_LivayatanFragment",
			TRICERATOPS = "DecorPackB_TriceratopsFragment",
			BRONTO = "DecorPackB_BrontoFragment";

		public List<GameObject> CreatePrefabs()
		{
			return
			[
				CreatePrefab(TREX, "dpii_trexfragment_kanim", 0.8f, 0.8f),
				CreatePrefab(LIVAYATAN, "dpii_livayatanfragment_kanim", 0.8f, 0.8f),
				CreatePrefab(TRICERATOPS, "dpii_triceratopsfragment_kanim", 0.8f, 0.8f),
				CreatePrefab(BRONTO, "dpii_brontofragment_kanim", 0.8f, 0.8f),
			];
		}

		private static GameObject CreatePrefab(string id, string animFile, float w = 1f, float h = 1f)
		{
			var name = Strings.Get($"STRINGS.ITEMS.{id.ToUpperInvariant()}.NAME");
			var description = Strings.Get($"STRINGS.ITEMS.{id.ToUpperInvariant()}.DESCRIPTION").String;
			description = $"{description}\n\n{STRINGS.MISC.DECORPACKB.FOSSIL_FRAGMENT}";

			var prefab = EntityTemplates.CreateLooseEntity(
				id,
				name,
				description,
				100f,
				true,
				Assets.GetAnim("barbeque_kanim"),//animFile),
				"object",
				Grid.SceneLayer.Ore,
				EntityTemplates.CollisionShape.RECTANGLE,
				w,
				h,
				true,
				-1,
				SimHashes.Creature,
				[
					GameTags.PedestalDisplayable,
					ModAssets.Tags.BuildingFossilNodule
					]);

			prefab.AddOrGet<OccupyArea>().SetCellOffsets(EntityTemplates.GenerateOffsets(1, 1));
			prefab.AddOrGet<DecorProvider>().SetValues(TUNING.DECOR.BONUS.TIER2);

			return prefab;
		}

		public void OnPrefabInit(GameObject inst) { }

		public void OnSpawn(GameObject inst) { }
	}
}
