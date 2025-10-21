using Dupes_Industrial_Overhaul.Chemical_Processing.Chemicals;
using Dupes_Industrial_Overhaul.Chemical_Processing.Space;
using HarmonyLib;
using Mineral_Processing_Mining.Buildings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;
using UtilLibs.MarkdownExport;
using static ResearchTypes;
using static UtilLibs.SupplyClosetUtils;

namespace RonivansLegacy_ChemicalProcessing.Content.ModDb
{
	internal class WikiDataExport
	{

		[HarmonyPatch(typeof(MainMenu), nameof(MainMenu.OnSpawn))]
		public class MainMenu_OnPrefabInit_Patch
		{
			[HarmonyPrepare]
			public static bool Prepare() => Mod.WriteWikiData;
			static bool written = false;
			public static void Postfix(MainMenu __instance)
			{
				if(written) 
					return;
				written = true;
				WriteWikiData();
			}
		}

		internal static void WriteWikiData()
		{
			var exportPath = "E:\\ONIModding\\Wiki\\docs\\Ronivans Legacy\\Content";
			if (!Directory.Exists(exportPath))
				return;

			var exporter = UtilLibs.MarkdownExport.Exporter.Create(exportPath);
			Dictionary<SourceModInfo, MD_Page> buildingPages = [];
			Dictionary<SourceModInfo, MD_Directory> submodFolders = [];
			SgtLogger.l($"Exporting building data to {exportPath}");

			exporter.RandomRecipeOccurences = RandomRecipeProducts.GetRandomDict(true);
			exporter.RandomRecipeResults = RandomRecipeProducts.GetRandomDict(false);

			foreach (var entry in BuildingManager.BuildingInjections.OrderBy(iten => UtilLibs.MarkdownExport.MarkdownUtil.StrippedBuildingName(iten.Value.BuildingID)))
			{
				var building = entry.Value;
				foreach (var sourceMod in building.SourceMods)
				{
					if (!buildingPages.TryGetValue(sourceMod, out MD_Page page))
					{
						var ModName = Strings.Get($"STRINGS.AIO_MODSOURCE.{sourceMod.ToString().ToUpperInvariant()}").ToString();
						var folder = exporter.root.SubDir(ModName);
						submodFolders[sourceMod] = folder;
						page = folder.File("Buildings");
						buildingPages[sourceMod] = page;
					}

					if (!Config.SubModEnabled(sourceMod))
					{
						continue;
					}

					var id = building.BuildingID;

					//overlap buildings have different recipes without chemproc, only write those when its disabled!
					var buildingEntry = page.AddBuilding(id);
					buildingEntry.Tech(building.TechID).WriteUISprite("E:\\ONIModding\\Wiki\\docs\\assets\\images\\buildings");
					
				}
				foreach(var skinSet in SkinCollection.SkinSets)
				{
					foreach(var skin in skinSet.Skins)
					{
						var kanim = Assets.GetAnim(skin.KanimFile);
						if (kanim == null)
							continue;
						Exporter.WriteUISprite("E:\\ONIModding\\Wiki\\docs\\assets\\images\\buildings", skin.ID, kanim);
					}
				}
			}
			foreach(var modRoots in submodFolders)
			{
				if(!Config.SubModEnabled(modRoots.Key))
				{
					modRoots.Value.WriteDirectory = false;
				}
			}
			submodFolders[SourceModInfo.MineralProcessing_Metallurgy].WriteDirectory = !Config.SubModEnabled(SourceModInfo.ChemicalProcessing_IO);

			buildingPages[SourceModInfo.ChemicalProcessing_IO].AddBuilding(MetalRefineryConfig.ID).VanillaModified();
			buildingPages[SourceModInfo.ChemicalProcessing_IO].AddBuilding(OilWellCapConfig.ID).VanillaModified();
			buildingPages[SourceModInfo.ChemicalProcessing_IO].AddBuilding(MethaneGeneratorConfig.ID).VanillaModified();


			///skins

			var skins = exporter.root
				.File("blueprints", new TranslationGroup("Building Skins").Add("zh", "建筑皮肤"))
				;
			foreach(var building in SkinCollection.SkinIds.Keys)
			{
				SgtLogger.l("Adding wiki skin entries for: " + building);
				skins.Add(new MD_BlueprintCollectionEntry(building));
			}


			var elementTitle = new MD_Text(new TranslationGroup("Enabling this mod in the config will add several new elements to the game or reenable disabled vanilla ones.\n\nA few of them will also be added to the starmap pois for mining or spawn during worldgen.").Add("zh", "在配置文件中启用此模组将为游戏添加多个新元素，或重新启用被禁用的原版元素。\n\n其中部分元素还将被添加到星图的POI中用于采矿，或在世界生成时随机生成。"));
			var newElementCategoriesHeader = new MD_Header(new TranslationGroup("New Element Categories").Add("zh", "新元素类别"), 2);
			var newElements = new TranslationKeyBuilder("NEW_ITEM", ["STRINGS.UI.CODEX.SUBWORLDS.ELEMENTS"]);
			var reenabledElements = new TranslationKeyBuilder("REENABLED_ITEM", ["STRINGS.UI.CODEX.SUBWORLDS.ELEMENTS"]);
			var reenabledElementsText = new TranslationGroup("These vanilla elements have been reenabled or partially adjusted").Add("zh", "这些元素已被重新激活或部分调整。");
			var critterFoodHeader = new MD_Header(new TranslationGroup("Critter Diet Expansions").Add("zh", "动物饮食扩展"), 2);

			///elements
			submodFolders[SourceModInfo.ChemicalProcessing_IO]
				.File("elements", "STRINGS.UI.CODEX.SUBWORLDS.ELEMENTS")
				.Add(elementTitle)
				.Add(newElementCategoriesHeader)
				.Add(new MD_TagsTable([ModAssets.Tags.RandomSand, ModAssets.Tags.AIO_HardenedAlloy, ModAssets.Tags.AIO_CarrierGas]))
				.Add(new MD_Header(newElements, 2))
				.Add(new MD_SubstanceTable(ModElements.ChemicalProcessing_IO_Elements))
				.Add(new MD_Header(reenabledElements, 2))
				.Add(new MD_Text(reenabledElementsText))
				.Add(new MD_SubstanceTable([SimHashes.Naphtha, SimHashes.Syngas, SimHashes.Propane, SimHashes.LiquidPropane, SimHashes.SolidPropane, SimHashes.Electrum, SimHashes.PhosphateNodules, SimHashes.CrushedRock]))
				.Add(critterFoodHeader)
				.Add(new MD_CritterConsumptionsTable(CritterDietsInfo.GetCritterInfo(SourceModInfo.ChemicalProcessing_IO)));
			;

			submodFolders[SourceModInfo.ChemicalProcessing_BioChemistry]
				.File("elements", "STRINGS.UI.CODEX.SUBWORLDS.ELEMENTS")
				.Add(elementTitle)
				.Add(new MD_Header(newElements, 2))
				.Add(new MD_SubstanceTable(ModElements.ChemicalProcessing_BioChem_Elements))
				.Add(critterFoodHeader)
				.Add(new MD_CritterConsumptionsTable(CritterDietsInfo.GetCritterInfo(SourceModInfo.ChemicalProcessing_BioChemistry)));

				;

			submodFolders[SourceModInfo.DupesEngineering]
				.File("elements", "STRINGS.UI.CODEX.SUBWORLDS.ELEMENTS")
				.Add(elementTitle)
				.Add(new MD_Header(reenabledElements, 2))
				.Add(new MD_Text(reenabledElementsText))
				.Add(new MD_SubstanceTable([SimHashes.Brick, SimHashes.Cement, SimHashes.CrushedRock]));

			submodFolders[SourceModInfo.NuclearProcessing]
				.File("elements", "STRINGS.UI.CODEX.SUBWORLDS.ELEMENTS")
				.Add(elementTitle)
				.Add(new MD_Header(reenabledElements, 2))
				.Add(new MD_Text(reenabledElementsText))
				.Add(new MD_SubstanceTable([SimHashes.Radium, SimHashes.Yellowcake]));
				
			///items
			submodFolders[SourceModInfo.ChemicalProcessing_IO]
				.File("items", new TranslationKeyBuilder("NEW_ITEM", ["STRINGS.UI.KLEI_INVENTORY_SCREEN.COLUMN_HEADERS.ITEMS_HEADER"]))
				.Add(new MD_EntityEntry(RayonFabricConfig.ID))
				;
			submodFolders[SourceModInfo.ChemicalProcessing_IO]
				.File("meteors", new TranslationKeyBuilder("NEW_ITEM", ["STRINGS.UI.SANDBOXTOOLS.FILTERS.ENTITIES.COMETS"]))
				.Add(new MD_Text(new TranslationGroup("This mod adds several new meteors to the existing showers.").Add("zh", "TBA, localize several new meteors.")))
				.Add(new MD_EntityEntry(HeavyCometConfig.ID))
				.Add(new MD_EntityEntry(SilverCometConfig.ID))
				.Add(new MD_EntityEntry(ZincCometConfig.ID))
				;
			///geysers
			var geyserPage = submodFolders[SourceModInfo.ChemicalProcessing_IO]
				.File("geysers", new TranslationKeyBuilder("NEW_ITEM", ["STRINGS.UI.SANDBOXTOOLS.FILTERS.ENTITIES.GEYSERS"]))
				.Add(new MD_Text(new TranslationGroup("Enabling this mod in the config adds several new random geysers to the game.\nThese will affect worldgen, so if you intent on playing a dedicated seed, turn off the geyser config option to not have that happen.\nDo note that these geysers will then become unavailable unless you have a way of readding them via other mods or debug templates").Add("zh", "启用此模组配置将为游戏添加数种随机间歇泉，这将影响世界生成。若需使用指定种子地图，请关闭间歇泉配置选项以避免干扰。注意：关闭后这些间歇泉将不可用，除非通过其他模组或调试模板重新添加。")));
			foreach (var geyser in ModGeysers.GeyserIDs)
				geyserPage.Add(new MD_GeyserEntry(geyser));

			var miningItems = submodFolders[SourceModInfo.MineralProcessing_Mining]
				.File("items", new TranslationKeyBuilder("NEW_ITEM", ["STRINGS.UI.KLEI_INVENTORY_SCREEN.COLUMN_HEADERS.ITEMS_HEADER"]))
				.Add(new MD_EntityEntry(Mining_Drillbits_Basic_ItemConfig.ID))
				.Add(new MD_EntityEntry(Mining_Drillbits_Steel_ItemConfig.ID))
				.Add(new MD_EntityEntry(Mining_Drillbits_Tungsten_ItemConfig.ID))
				.Add(new MD_EntityEntry(Mining_Drillbits_GuidanceDevice_ItemConfig.ID))
				;
			foreach (var programmed in Mining_Drillbits_GuidanceDevice_ItemConfig.ProgrammedGuidanceModules)
				miningItems.Add(new MD_EntityEntry(programmed.ToString(),
					new TranslationKeyBuilder("STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.MINING_DRILLBITS_GUIDANCEDEVICE_ITEM.NAME_PROGRAMMED", [ Mining_Drillbits_GuidanceDevice_ItemConfig.GetTargetNameKey(programmed.ToString()) ]),
					new TranslationKeyBuilder("STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.MINING_DRILLBITS_GUIDANCEDEVICE_ITEM.DESC_PROGRAMMED", [ Mining_Drillbits_GuidanceDevice_ItemConfig.GetTargetNameKey(programmed.ToString()) ]
					)));


			exporter.Export();
			exporter.Export("zh");
			exporter.EntityIconPath("E:\\ONIModding\\Wiki\\docs\\assets\\images\\entities");
			exporter.ExportEntityIcons();
		}
	}
}
