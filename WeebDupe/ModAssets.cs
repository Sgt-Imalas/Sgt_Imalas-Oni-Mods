using Database;
using System.Collections.Generic;
using UtilLibs;

namespace WeebDupe
{
	internal class ModAssets
	{
		public static string
			WEEB_ID = "WD_WEEB"
			, SpriteID = "CatEarSprite";

		public class WAccessories
		{
			public static void Register(AccessorySlots slots, Accessories accessories)
			{

				var catEars = Assets.GetAnim("hat_role_weeb1_kanim");
				AddAccessories(catEars, slots.Hat, accessories);
			}
			public static void AddAccessories(KAnimFile file, AccessorySlot slot, ResourceSet parent)
			{
				SgtLogger.l(slot.Id);

				var build = file.GetData().build;
				var id = slot.Id.ToLower();

				for (var i = 0; i < build.symbols.Length; i++)
				{
					var symbolName = HashCache.Get().Get(build.symbols[i].hash);
					SgtLogger.l(symbolName);

					if (symbolName.StartsWith(id))
					{
						var accessory = new Accessory(symbolName, parent, slot, file.batchTag, build.symbols[i]);
						slot.accessories.Add(accessory);
						HashCache.Get().Add(accessory.IdHash.HashValue, accessory.Id);

						SgtLogger.l("Added accessory: " + accessory.Id);
					}
					else
					{
						SgtLogger.l($"Symbol {symbolName} in file {file.name} is not starting with {id}");
					}
				}
			}
		}

		public class WSkills
		{
			public static Skill weeb;

			public static void Register(Skills __instance)
			{
				weeb = __instance.Add(new Skill(
						WEEB_ID,
						STRINGS.DUPLICANTS.ROLES.WEEB.NAME,
						STRINGS.DUPLICANTS.ROLES.WEEB.DESCRIPTION,
						"",
						0,
						"hat_role_weeb1",
						"skillbadge_role_research1",
						Db.Get().SkillGroups.Research.Id,
						new List<SkillPerk>
						{
							WSkillPerks.Weeb1,
							WSkillPerks.Weeb2

						}, new List<string>()
						{
						}));

			}
		}
		public class WSkillPerks
		{
			public static SkillPerk Weeb1;
			public static SkillPerk Weeb2;

			public const string WeebPerk1ID = "WD_WeebPerk1";
			public const string WeebPerk2ID = "WD_WeebPerk2";

			public static void Register(SkillPerks __instance)
			{
				Weeb1 = __instance.Add(new SkillAttributePerk(
					WeebPerk1ID,
					Db.Get().Attributes.Learning.Id,
					1,
					STRINGS.DUPLICANTS.ROLES.WEEB.NAME));
				Weeb2 = __instance.Add(new SkillAttributePerk(
					WeebPerk2ID,
					Db.Get().Attributes.Decor.Id,
					15,
					STRINGS.DUPLICANTS.ROLES.WEEB.NAME));

			}
		}
	}
}
