using RonivansLegacy_ChemicalProcessing;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static ResearchTypes;
using static RonivansLegacy_ChemicalProcessing.STRINGS.ITEMS.INDUSTRIAL_PRODUCTS;
using static RonivansLegacy_ChemicalProcessing.STRINGS.UI;

namespace Mineral_Processing_Mining.Buildings
{
	public class Mining_Drillbits_GuidanceDevice_ItemConfig : IMultiEntityConfig
	{
		public static string ID = "Mining_Drillbits_GuidanceDevice_Item";
		public static Tag TAG => TagManager.Create(ID);

		public static string ProgrammedPrefix = "Mining_GuidanceDevice_";

		public static Tag AquiferTag = TagManager.Create(ProgrammedPrefix + "Aquifer");
		public static Tag CryosphereTag = TagManager.Create(ProgrammedPrefix + "Cryosphere");
		public static Tag HardStratumTag = TagManager.Create(ProgrammedPrefix + "hard_stratum");
		public static Tag MantleTag = TagManager.Create(ProgrammedPrefix + "mantle");
		public static Tag OilReservesTag = TagManager.Create(ProgrammedPrefix + "oil_reserves");
		public static Tag SoftStratumTag = TagManager.Create(ProgrammedPrefix + "soft_stratum");


		public static List<Tag> ProgrammedGuidanceModules = [AquiferTag, CryosphereTag, HardStratumTag, MantleTag, OilReservesTag, SoftStratumTag];


		public GameObject CreatePrefab(string id = null)
		{
			string name, desc, kanim;
			bool isBaseVariant = string.IsNullOrEmpty(id);
			if (isBaseVariant)
			{
				id = ID;
				kanim = "guidance_device_kanim";
				name = MINING_DRILLBITS_GUIDANCEDEVICE_ITEM.NAME;
				desc = MINING_DRILLBITS_GUIDANCEDEVICE_ITEM.DESC;
			}
			else
			{
				string targetID = id.Replace(ProgrammedPrefix, "");

				string targetKey = GetTargetNameKey(id);
				string target = Strings.Get(targetKey);
				name = string.Format(MINING_DRILLBITS_GUIDANCEDEVICE_ITEM.NAME_PROGRAMMED, target);
				desc = string.Format(MINING_DRILLBITS_GUIDANCEDEVICE_ITEM.DESC_PROGRAMMED, target);
				kanim = $"guidance_device_{targetID.ToLowerInvariant()}_kanim";
			}

			GameObject go = EntityTemplates.CreateLooseEntity(id, name, desc, 1f, true, Assets.GetAnim(kanim), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.65f, 0.65f, true, 0, SimHashes.Creature, [GameTags.IndustrialProduct, ModAssets.Tags.MineralProcessing_GuidanceUnit]);
			go.AddOrGet<EntitySplitter>();
			go.AddOrGet<SimpleMassStatusItem>();
			go.AddComponent<Durability>();
			go.AddComponent<ProgrammableGuidanceModule>();
			go.AddOrGet<Prioritizable>();
			Prioritizable.AddRef(go);
			return go;
		}

		public string[] GetDlcIds() => null;

		public void OnPrefabInit(GameObject inst)
		{
		}

		public void OnSpawn(GameObject inst)
		{
		}

		public List<GameObject> CreatePrefabs()
		{
			var list = new List<GameObject>();
			list.Add(CreatePrefab());
			foreach (var tag in ProgrammedGuidanceModules)
			{
				var prefab = CreatePrefab(tag.ToString());
				list.Add(prefab);
			}
			return list;
		}

		internal static string GetTargetNameKey(Tag programmable)
		{
			string targetID = programmable.ToString().Replace(ProgrammedPrefix, "");
			return ("STRINGS.UI.MINING_SMART_DRILL_LOCATIONS." + targetID.ToUpperInvariant());
		}
		public static string GetTargetName(Tag programmable) => Strings.Get(GetTargetNameKey(programmable));

		internal static string GetGuidanceItemName(Tag programmable)
		{
			string target = Strings.Get(GetTargetNameKey(programmable));
			return string.Format(MINING_DRILLBITS_GUIDANCEDEVICE_ITEM.NAME_PROGRAMMED, target);
		}
	}
}
