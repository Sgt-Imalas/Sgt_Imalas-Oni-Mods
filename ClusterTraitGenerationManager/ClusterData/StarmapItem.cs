using Klei.AI;
using Newtonsoft.Json;
using ObjectCloner;
using ProcGen;
using System;
using System.Collections.Generic;
using System.Linq;
using TUNING;
using UnityEngine;
using UtilLibs;
using static ClusterTraitGenerationManager.ClusterData.CGSMClusterManager;

namespace ClusterTraitGenerationManager.ClusterData
{
	public class StarmapItem
	{
		public string id;
		public StarmapItemCategory category;
		public bool DisablesStoryTraits = false;
		[JsonIgnore] public bool originalPOIGroup => POIGroupUID == string.Empty;

		public string POIGroupUID = string.Empty;

		[JsonIgnore] public Sprite planetSprite;
		[JsonIgnore] public Sprite planetMixingSprite => world_mixing?.planetSprite;

		[JsonIgnore] public ProcGen.World world => world_mixing != null ? world_mixing.world : world_internal;
		[JsonIgnore] public ProcGen.World World_Internal => world_internal;
		[JsonIgnore] public bool IsMixed => world_mixing != null;
		[JsonIgnore] public bool SupportsWorldMixing => placement != null && placement.worldMixing != null;
		[JsonIgnore] public bool SupportsSubworldMixing => SubworldMixingMaxCount > 0;
		[JsonIgnore] public int SubworldMixingMaxCount => world.subworldMixingRules != null ? world.subworldMixingRules.Count : 0;
		[JsonIgnore] public StarmapItem MixingAsteroidSource => world_mixing;


		[JsonIgnore] private ProcGen.World world_internal;
		[JsonIgnore] private StarmapItem world_mixing;
		[JsonIgnore] public Vector2I originalWorldDimensions;
		[JsonIgnore] public float originalWorldTraitScale;
		[JsonIgnore] public string ModName = string.Empty;
		[JsonIgnore] public string DlcID = "";

		public WorldPlacement placement;

		public SpaceMapPOIPlacement placementPOI;

		public bool IsPOI => category == StarmapItemCategory.POI;
		public bool IsRandom => id.Contains(RandomKey);

		public int PredefinedPlacementOrder = -1;

		public string DisplayName
		{
			get
			{
				if (id.Contains(RandomKey))
				{
					switch (category)
					{
						case StarmapItemCategory.Starter:
							return STRINGS.UI.SPACEDESTINATIONS.CGM_RANDOM_STARTER.NAME;
						case StarmapItemCategory.Warp:
							return STRINGS.UI.SPACEDESTINATIONS.CGM_RANDOM_WARP.NAME;
						case StarmapItemCategory.Outer:
							return STRINGS.UI.SPACEDESTINATIONS.CGM_RANDOM_OUTER.NAME;
						case StarmapItemCategory.POI:
							return STRINGS.UI.SPACEDESTINATIONS.CGM_RANDOM_POI.NAME;
					}
				}

				if (world != null && world.name != null)
				{
					if (Strings.TryGet(world.name, out var nameEntry))
					{
						var name = nameEntry.ToString();
						if (IsMixed && World_Internal != null && Strings.TryGet(World_Internal.name, out var namePreMixing))
						{
							name += " (" + namePreMixing.ToString() + ")";
						}

						if (ModName != string.Empty)
							name += " " + UIUtils.ColorText(STRINGS.UI.SPACEDESTINATIONS.MODDEDPLANET, UIUtils.rgb(212, 244, 199));

						return name;
					}
				}
				else if (category == StarmapItemCategory.POI)
				{
					return id.Substring(0, 8);
				}

				//else if (_poiID != null)
				//{
				//    return _poiName;
				//}
				return id;
			}
		}
		public string DisplayDescription
		{
			get
			{
				string desc = string.Empty;
				if (ModName != string.Empty)
				{
					desc += UIUtils.ColorText(string.Format(STRINGS.UI.SPACEDESTINATIONS.MODDEDPLANETDESC, ModName), UIUtils.rgb(212, 244, 199));
					desc += "\n";
					desc += "\n";
				}

				if (id.Contains(RandomKey))
				{
					switch (category)
					{
						case StarmapItemCategory.Starter:
							return STRINGS.UI.SPACEDESTINATIONS.CGM_RANDOM_STARTER.DESCRIPTION;
						case StarmapItemCategory.Warp:
							return STRINGS.UI.SPACEDESTINATIONS.CGM_RANDOM_WARP.DESCRIPTION;
						case StarmapItemCategory.Outer:
							return STRINGS.UI.SPACEDESTINATIONS.CGM_RANDOM_OUTER.DESCRIPTION;
						case StarmapItemCategory.POI:
							return STRINGS.UI.SPACEDESTINATIONS.CGM_RANDOM_POI.DESCRIPTION;
					}
				}

				if (world != null && world.description != null)
				{
					if (Strings.TryGet(world.description, out var description))
					{
						desc += description.String;
						return desc;
					}
				}
				//else if (_poiID != null)
				//{
				//    return _poiDesc;
				//}
				desc += id;
				return desc;
			}
		}

		//private float XYratio = -1f;
		public float ApplySizeMultiplierToValue(float inputValue)
		{
			return (CurrentSizeMultiplier * inputValue);
		}

		[JsonIgnore]
		public Vector2I CustomPlanetDimensions
		{
			get
			{
				if (IsMixed)
					return MixingAsteroidSource.CustomPlanetDimensions;


				if (UsingCustomDimensions)
				{
					return new(CustomX, CustomY);
				}
				var dim = originalWorldDimensions;

				if (world != null)
				{
					if (SizePreset != WorldSizePresets.Normal || RatioPreset != WorldRatioPresets.Normal)
					{
						float sizePercentage = (float)SizePreset / 100f;
						float ratioModifier = 0;

						bool? ChooseHeight = null;

						if (RatioPreset != WorldRatioPresets.Normal)
						{
							int intRatio = (int)RatioPreset;
							if (intRatio < 100)
							{
								ratioModifier = 1f - ((float)RatioPreset / 100f);
								ChooseHeight = false;
							}
							else if (intRatio > 100)
							{
								ratioModifier = ((float)RatioPreset / 100f) - 1;
								ChooseHeight = true;
							}
						}
						float sizeIncreaseMultiplier = Mathf.Sqrt(sizePercentage);

						dim.X = Mathf.RoundToInt(originalWorldDimensions.X * sizeIncreaseMultiplier);
						dim.Y = Mathf.RoundToInt(originalWorldDimensions.Y * sizeIncreaseMultiplier);

						if (ChooseHeight == true)
						{
							dim.Y = dim.Y + Mathf.RoundToInt(dim.Y * ratioModifier);
						}
						if (ChooseHeight == false)
						{
							dim.X = dim.X + Mathf.RoundToInt(dim.X * ratioModifier);
						}
					}
				}
				return dim;
			}
		}

		//int CustomWorldSizeX = -1, CustomWorldSizeY = -1;
		WorldSizePresets SizePreset = WorldSizePresets.Normal;
		WorldRatioPresets RatioPreset = WorldRatioPresets.Normal;
		[JsonIgnore] public bool DefaultDimensions => IsMixed ? MixingAsteroidSource.DefaultDimensions : SizePreset == WorldSizePresets.Normal && RatioPreset == WorldRatioPresets.Normal && !UsingCustomDimensions;
		[JsonIgnore] public WorldSizePresets CurrentSizePreset => IsMixed ? MixingAsteroidSource.SizePreset : SizePreset;
		[JsonIgnore] public WorldRatioPresets CurrentRatioPreset => IsMixed ? MixingAsteroidSource.RatioPreset : RatioPreset;


		[JsonIgnore]
		public float CurrentSizeMultiplier =>
			IsMixed
			? MixingAsteroidSource.CurrentSizeMultiplier
			: UsingCustomDimensions
				? CustomSizeIncrease
				: (float)SizePreset / 100f;

		public void SetPlanetSizeToPreset(WorldSizePresets preset)
		{
			if (IsMixed)
			{
				MixingAsteroidSource.SetPlanetSizeToPreset(preset);
				return;
			}

			CustomX = -1;
			CustomY = -1;
			UsingCustomDimensions = false;
			SizePreset = preset;
		}
		public void SetPlanetRatioToPreset(WorldRatioPresets preset)
		{
			if (IsMixed)
			{
				MixingAsteroidSource.SetPlanetRatioToPreset(preset);
				return;
			}

			CustomX = -1;
			CustomY = -1;
			UsingCustomDimensions = false;
			RatioPreset = preset;
		}
		public int CustomX = -1, CustomY = -1;
		private bool UsingCustomDimensions = false;
		public void ApplyCustomDimension(int value, bool heightTrueWidthFalse)
		{
			if (IsMixed)
			{
				MixingAsteroidSource.ApplyCustomDimension(value, heightTrueWidthFalse);
				return;
			}


			var currentDims = CustomPlanetDimensions;
			UsingCustomDimensions = true;
			if (CustomX == -1)
			{
				CustomX = currentDims.X;
			}
			if (CustomY == -1)
			{
				CustomY = currentDims.Y;
			}
			if (world != null)
			{
				if (heightTrueWidthFalse)
				{
					var rounded = Mathf.RoundToInt(Mathf.Clamp(value, originalWorldDimensions.Y * 0.55f, originalWorldDimensions.Y * 2.6f));
					if (rounded != CustomY)
						CustomY = rounded;
				}
				else
				{
					var rounded = Mathf.RoundToInt(Mathf.Clamp(value, originalWorldDimensions.X * 0.55f, originalWorldDimensions.X * 2.6f));

					if (rounded != CustomX)
						CustomX = rounded;
				}
				CustomSizeIncrease = (float)(CustomX * CustomY) / (float)(originalWorldDimensions.X * originalWorldDimensions.Y);
			}
		}
		public float SizeMultiplierX()
		{
			if (UsingCustomDimensions && world != null)
			{
				return CustomX / originalWorldDimensions.X;
			}
			else
			{
				float sizePercentage = (float)SizePreset / 100f;
				return Mathf.Sqrt(sizePercentage);
			}

		}
		public float SizeMultiplierY()
		{
			if (UsingCustomDimensions && world != null)
			{
				return CustomY / originalWorldDimensions.Y;
			}
			else
			{
				float sizePercentage = (float)SizePreset / 100f;
				return Mathf.Sqrt(sizePercentage);
			}
		}

		float CustomSizeIncrease = -1f;
		public float InstancesToSpawn = 1;
		public bool MoreThanOnePossible = false;
		//public float MaxNumberOfInstances = 1;
		[JsonIgnore] public int minRing => placement != null ? placement.allowedRings.min : placementPOI != null ? placementPOI.allowedRings.min : -1;
		[JsonIgnore] public int maxRing => placement != null ? placement.allowedRings.max : placementPOI != null ? placementPOI.allowedRings.max : -1;
		[JsonIgnore] public int buffer => placement != null ? placement.buffer : -1;

		[JsonIgnore]
		public bool SupportsGeyserOverride
		{
			get
			{
				if (_geyserOverrideCount == -1)
				{
					if (world != null)
					{
						_geyserOverrideCount = 0;
						foreach (var poiRule in world.worldTemplateRules)
						{
							if (poiRule.names != null && poiRule.names.Count == 1 && poiRule.names.First() == "geysers/generic")
							{
								_geyserOverrideCount += poiRule.times;
							}
						}
					}
				}
				return _geyserOverrideCount > 0;
			}
		}

		private int _geyserOverrideCount = -1;

		#region SetterMethods

		public void RefresDuplicateState()
		{
			if (placementPOI != null && InstancesToSpawn < placementPOI.pois.Count)
			{
				placementPOI.canSpawnDuplicates = true;
			}
		}

		public void SetSpawnNumber(float newNumber, bool force = false)
		{
			if (newNumber < 0)
				newNumber = 0;
			if (this.IsPOI)
			{
				if (newNumber <= MaxPOICount || force)
				{
					InstancesToSpawn = newNumber;
				}
				else
					InstancesToSpawn = MaxPOICount;

				RefresDuplicateState();
			}
			else
			{
				if (newNumber <= MaxAmountRandomPlanet || force)
				{
					InstancesToSpawn = newNumber;
				}
				else
					InstancesToSpawn = MaxAmountRandomPlanet;
			}
		}
		public void ResetPOI()
		{
			if (this.category == StarmapItemCategory.POI)
			{
				if (originalMaxPOI != null)
				{
					SetOuterRing((int)originalMaxPOI);
				}
				if (originalMinPOI != null)
				{
					SetInnerRing((int)originalMinPOI);
				}
			}
		}

		[JsonIgnore] public int? originalMinPOI = null, originalMaxPOI = null;
		public void SetInnerRing(int newRing, bool original = false)
		{
			if (original)
				originalMinPOI = newRing;

			newRing = Math.Min(newRing, CustomCluster.Rings);
			if (newRing >= 0)
			{
				if (placement != null)
				{
					var rings = placement.allowedRings;
					rings.min = newRing;
					if (newRing > rings.max)
					{
						rings.max = newRing;
					}
					placement.allowedRings = rings;
				}
				else if (placementPOI != null)
				{
					var rings = placementPOI.allowedRings;
					rings.min = newRing;
					if (newRing > rings.max)
					{
						rings.max = newRing;
					}
					placementPOI.allowedRings = rings;
				}
				else
					SgtLogger.warning(this.id + ": no placement component found!");
			}
		}
		public void SetOuterRing(int newRing, bool original = false)
		{
			newRing = Math.Min(newRing, CustomCluster.Rings);
			if (original)
				originalMaxPOI = newRing;
			if (newRing >= 0)
			{
				if (placement != null)
				{
					var rings = placement.allowedRings;
					rings.max = newRing;
					if (newRing < rings.min)
					{
						rings.min = newRing;
					}
					placement.allowedRings = rings;
				}
				else if (placementPOI != null)
				{
					var rings = placementPOI.allowedRings;
					rings.max = newRing;
					if (newRing < rings.min)
					{
						rings.min = newRing;
					}
					placementPOI.allowedRings = rings;
				}
				else
					SgtLogger.warning(this.id + ": no placement component found!");
			}
		}

		public void SetBuffer(int newBuffer)
		{
			newBuffer = Math.Min(newBuffer, CustomCluster.Rings);
			if (newBuffer >= 0)
			{
				if (placement != null)
				{
					//var buffer = placement.buffer;
					//rings.max = newRing;
					//if (newRing < rings.min)
					//{
					//    rings.min = newRing;
					//}
					placement.buffer = newBuffer;
				}
				else
					SgtLogger.warning(this.id + ": no placement component found!");
			}
		}

		#endregion

		public StarmapItem(string id, StarmapItemCategory category, Sprite sprite)
		{
			this.id = id;
			this.category = category;
			this.planetSprite = sprite;
		}
		public StarmapItem AssignDlc(string dlcId)
		{
			this.DlcID = dlcId;
			return this;
		}

		public StarmapItem MakeItemPlanet(ProcGen.World world)
		{
			this.world_internal = world;
			this.originalWorldDimensions = world.worldsize;
			this.originalWorldTraitScale = world.worldTraitScale;
			//this.InitGeyserInfo();

			//XYratio = (float)world.worldsize.X / (float)world.worldsize.Y;


			string worldId = world.filePath;
			///Dynamically created planets, check for original instead
			if (ModAssets.ModPlanetOriginPaths.ContainsKey(worldId))
				worldId = ModAssets.ModPlanetOriginPaths[worldId];


			if (ModAssets.IsModdedAsteroid(worldId, out var sourceMod))
			{
				this.ModName = sourceMod.title;
				SgtLogger.l($"Created Starmap item for {worldId} from {ModName}");
			}

			SettingsCache.GetDlcIdAndPath(worldId, out var dlcId, out _);
			this.AssignDlc(dlcId);
			return this;
		}

		//private void InitGeyserInfo()
		//{
		//    var Templates = this.world.worldTemplateRules;
		//    foreach (var Template in Templates)
		//    {

		//    }
		//}
		//[JsonIgnore]public List<GeyserInfo> GeyserInfos;
		//public class GeyserInfo
		//{
		//    public Sprite PreviewImage;
		//    public string Name, Description;
		//    public int minCount, maxCount;
		//}


		public StarmapItem AddItemWorldPlacement(WorldPlacement placement2, bool morethanone = false)
		{
			//this.MaxNumberOfInstances = morethanone;
			this.MoreThanOnePossible = morethanone;
			this.placement = new WorldPlacement();
			placement.startWorld = placement2.startWorld;
			placement.world = placement2.world;
			placement.y = placement2.y;
			placement.x = placement2.x;
			placement.height = placement2.height;
			placement.width = placement2.width;
			placement.allowedRings = new(placement2.allowedRings.min, placement2.allowedRings.max);
			placement.buffer = placement2.buffer;
			placement.locationType = placement2.locationType;
			placement.worldMixing = SerializingCloner.Copy(placement2.worldMixing);
			placement.worldMixing = new()
			{
				requiredTags = new List<string>(placement2.worldMixing.requiredTags),
				forbiddenTags = new List<string>(placement2.worldMixing.forbiddenTags),
				additionalWorldTemplateRules = new(placement2.worldMixing.additionalWorldTemplateRules),
				additionalUnknownCellFilters = new(placement2.worldMixing.additionalUnknownCellFilters),
				additionalSubworldFiles = new(placement2.worldMixing.additionalSubworldFiles),
				additionalSeasons = new List<string>(placement2.worldMixing.additionalSeasons),

				mixingWasApplied = placement2.worldMixing.mixingWasApplied,
				previousWorld = placement2.worldMixing.previousWorld
			};


			return this;
		}
		public StarmapItem MakeItemPOI(SpaceMapPOIPlacement placement2)
		{
			MoreThanOnePossible = true;
			placementPOI = new SpaceMapPOIPlacement();
			placementPOI.pois = new(placement2.pois);
			placementPOI.canSpawnDuplicates = placement2.canSpawnDuplicates;
			placementPOI.avoidClumping = placement2.avoidClumping;
			placementPOI.numToSpawn = placement2.numToSpawn;
			placementPOI.allowedRings = new(placement2.allowedRings.min, placement2.allowedRings.max);
			placementPOI.guarantee = placement2.guarantee;
			originalMaxPOI = placement2.allowedRings.max;
			originalMinPOI = placement2.allowedRings.min;
			//MaxNumberOfInstances = placement2.numToSpawn * 5f; 
			InstancesToSpawn = placement2.numToSpawn;
			return this;
		}
		public StarmapItem SetWorldMixing(StarmapItem mix)
		{
			world_mixing = mix;
			if (placement != null)
			{
				//clearing previous mixing
				placement.UndoWorldMixing();

				if (mix != null)//applying mixing
				{
					SgtLogger.l(DisplayName + " is getting mixing replacement " + mix.DisplayName);
					placement.worldMixing.previousWorld = placement.world;
					placement.worldMixing.mixingWasApplied = true;
					placement.world = mix.world.filePath;

					mix.PredefinedPlacementOrder = this.PredefinedPlacementOrder;
				}
			}
			return this;
		}


		#region GeyserBlacklist

		private bool _geyserBlacklistAffectsNonGenerics = false;
		public bool GeyserBlacklistAffectsNonGenerics => IsMixed ? MixingAsteroidSource._geyserBlacklistAffectsNonGenerics : _geyserBlacklistAffectsNonGenerics;

		private List<string> _geyserBlacklistIDs = new();
		public List<string> GeyserBlacklistIDs => IsMixed ? MixingAsteroidSource.GeyserBlacklistIDs : _geyserBlacklistIDs;

		public void SetGeyserBlacklist(List<string> NEWs)
		{
			if (IsMixed)
			{
				MixingAsteroidSource.SetGeyserBlacklist(NEWs);
				return;
			}
			if (NEWs == null) NEWs = new List<string>();
			_geyserBlacklistIDs = NEWs;
		}
		public void SetGeyserBlacklistAffectsNonGenerics(bool affectsNongenerics)
		{
			if(IsMixed)
				MixingAsteroidSource.SetGeyserBlacklistAffectsNonGenerics(affectsNongenerics);
			else
			 _geyserBlacklistAffectsNonGenerics = affectsNongenerics;
		}

		public void AddGeyserBlacklist(string geyserID)
		{
			if (IsMixed)
				MixingAsteroidSource.AddGeyserBlacklist(geyserID);
			else
				_geyserBlacklistIDs.Add(geyserID);
		}
		public void RemoveGeyserBlacklist(string geyserID)
		{
			if (IsMixed)
				MixingAsteroidSource.RemoveGeyserBlacklist(geyserID);
			else
				_geyserBlacklistIDs.Remove(geyserID);
		}
		public bool HasGeyserBlacklisted(string geyserID)
		{
			if (IsMixed)
				return MixingAsteroidSource.HasGeyserBlacklisted(geyserID);
			else
				return GeyserBlacklistIDs.Contains(geyserID);
		}

		#endregion

		#region GeyserOverrides

		private List<string> _geyserOverrideIDs = new();
		public List<string> GeyserOverrideIDs => IsMixed ? MixingAsteroidSource.GeyserOverrideIDs : _geyserOverrideIDs;
		public void SetGeyserOverrides(List<string> NEWs)
		{
			if (NEWs == null) NEWs = new List<string>();

			if (IsMixed)
			{
				MixingAsteroidSource.SetGeyserOverrides(NEWs);
				return;
			}


			_geyserOverrideIDs = NEWs;
			for (int i = _geyserOverrideCount - 1; i >= 0; i--)
			{
				var currentGeyser = _geyserOverrideIDs[i];
				if (!ModAssets.AllGeysers.TryGetValue(currentGeyser, out _))
				{
					SgtLogger.l("invalid geyser found: " + currentGeyser);
					_geyserOverrideIDs.RemoveAt(i);
				}
			}
		}
		public void ClearGeyserOverrides()
		{
			GeyserOverrideIDs.Clear();
			GeyserBlacklistIDs.Clear();
			_geyserBlacklistAffectsNonGenerics = false;
		}

		public void AddGeyserOverride(string geyserID)
		{
			if (IsMixed)
				MixingAsteroidSource.AddGeyserOverride(geyserID);
			else
				GeyserOverrideIDs.Add(geyserID);
		}
		public void RemoveGeyserOverrideAt(int index)
		{
			if (IsMixed)
				MixingAsteroidSource.RemoveGeyserOverrideAt(index);
			else
				GeyserOverrideIDs.RemoveAt(index);
		}
		public int GetCurrentGeyserOverrideCount()
		{
			return IsMixed ? MixingAsteroidSource.GetCurrentGeyserOverrideCount() : GeyserOverrideIDs.Count;
		}
		public bool CanAddGeyserOverrides() => IsMixed ? MixingAsteroidSource.CanAddGeyserOverrides() : GetMaxGeyserOverrideCount() > 0;

		public int GetMaxGeyserOverrideCount()
		{
			if (IsMixed)
				return MixingAsteroidSource.GetMaxGeyserOverrideCount();


			int totalCountRules = 0;
			int totalCountTraits = 0;

			if (world != null && world.worldTemplateRules != null) //Grabbing geyserGenericSpawns
			{
				foreach (var rule in world.worldTemplateRules)
				{
					if (rule.names != null && rule.names.Count() == 1 && rule.names[0] == "geysers/generic")
					{
						totalCountRules += Mathf.FloorToInt(rule.times * CurrentSizeMultiplier);
					}
				}
			}
			foreach (var traitID in CurrentTraits) //grabbing generics from traits
			{
				if (SettingsCache.worldTraits.TryGetValue(traitID, out var trait))
				{
					if (trait != null && trait.removeWorldTemplateRulesById != null && trait.removeWorldTemplateRulesById.Count() > 0)
					{
						if (trait.removeWorldTemplateRulesById.Contains("GenericGeysers")) //Geodormant removes the main rule and replaces it with a 9 geyser rule
							totalCountRules = 0;
					}

					if (trait != null && trait.additionalWorldTemplateRules != null)
					{
						foreach (var rule in trait.additionalWorldTemplateRules)
						{
							if (rule.names != null && rule.names.Count() == 1 && rule.names[0] == "geysers/generic")
							{
								totalCountTraits += Mathf.FloorToInt(rule.times);
							}
						}
					}
				}
			}
			///TODO!!!!
			//Implement Size Scaling!
			//is 
			//potentially 
			//implemented, todo test!
			return totalCountTraits + totalCountRules;
		}

		#endregion

		#region SkyFixedTraits


		private void BackupOriginalWorldSkyFixedTraits()
		{
			if (world != null)
			{
				if (world.fixedTraits == null)
					world.fixedTraits = new();

				if (!ModAssets.ChangedSkyFixedTraits.ContainsKey(world))
				{
					ModAssets.ChangedSkyFixedTraits[world] = new List<string>(world.fixedTraits);
				}
			}
		}
		public void SetFixedSkyTraits(List<string> fixedSkyTraits)
		{
			if (fixedSkyTraits == null || fixedSkyTraits.Count == 0 || world == null)
				return;


			string lightTrait = fixedSkyTraits.FirstOrDefault(t => ModAssets.SunlightFixedTraits.ContainsKey(t));
			if (lightTrait != null)
			{
				SetSunlightValue(lightTrait);
			}

			string radTrait = fixedSkyTraits.FirstOrDefault(t => ModAssets.CosmicRadiationFixedTraits.ContainsKey(t));
			if (radTrait != null && DlcManager.IsExpansion1Active())
			{
				SetRadiationValue(radTrait);
			}

			string northernLightTrait = fixedSkyTraits.FirstOrDefault(t => ModAssets.NorthernLightsFixedTraits.ContainsKey(t));
			if (northernLightTrait != null)
			{
				SetNorthernLightsValue(northernLightTrait);
			}

		}
		public string GetSunlightValue()
		{
			if (world == null)
				return null;


			string currentValue = world.fixedTraits.FirstOrDefault(t => ModAssets.SunlightFixedTraits.ContainsKey(t));
			if (currentValue != null)
				return currentValue;

			return FIXEDTRAITS.SUNLIGHT.NAME.DEFAULT;
		}
		public string GetRadiationValue()
		{
			if (world == null)
				return null;


			string currentValue = world.fixedTraits.FirstOrDefault(t => ModAssets.CosmicRadiationFixedTraits.ContainsKey(t));
			if (currentValue != null)
				return currentValue;

			return FIXEDTRAITS.COSMICRADIATION.NAME.DEFAULT;
		}
		public string GetNorthernLightsValue()
		{
			if (world == null)
				return null;


			string currentValue = world.fixedTraits.FirstOrDefault(t => ModAssets.NorthernLightsFixedTraits.ContainsKey(t));
			if (currentValue != null)
				return currentValue;

			return FIXEDTRAITS.NORTHERNLIGHTS.NAME.DEFAULT;
		}

		public void SetSunlightValue(string traitId)
		{
			if (world == null)
				return;
			BackupOriginalWorldSkyFixedTraits();

			SgtLogger.l(traitId, "Trait LIGHT");
			string currentValue = world.fixedTraits.FirstOrDefault(t => ModAssets.SunlightFixedTraits.ContainsKey(t));
			if (currentValue != null)
				world.fixedTraits.Remove(currentValue);
			world.fixedTraits.Add(traitId);

		}
		public void SetRadiationValue(string traitId)
		{
			if (!DlcManager.IsExpansion1Active())
				return;

			if (world == null)
				return;
			BackupOriginalWorldSkyFixedTraits();
			SgtLogger.l(traitId, "Trait RAD");

			string currentValue = world.fixedTraits.FirstOrDefault(t => ModAssets.CosmicRadiationFixedTraits.ContainsKey(t));
			if (currentValue != null)
				world.fixedTraits.Remove(currentValue);
			world.fixedTraits.Add(traitId);
		}
		public void SetNorthernLightsValue(string traitId)
		{
			if (world == null)
				return;
			BackupOriginalWorldSkyFixedTraits();
			SgtLogger.l(traitId, "Trait NL");
			string currentValue = world.fixedTraits.FirstOrDefault(t => ModAssets.NorthernLightsFixedTraits.ContainsKey(t));
			if (currentValue != null)
				world.fixedTraits.Remove(currentValue);
			world.fixedTraits.Add(traitId);
		}

		#endregion


		#region PlanetMeteors

		public List<MeteorShowerSeason> CurrentMeteorSeasons
		{
			get
			{
				var seasons = new List<MeteorShowerSeason>();
				var db = Db.Get();
				if (world != null)
				{
					foreach (var season in world.seasons)
					{
						var seasonData = (db.GameplaySeasons.TryGet(season) as MeteorShowerSeason);
						if (seasonData != null)
							seasons.Add(seasonData);
					}
				}
				if (IsMixed
					&& placement.worldMixing != null
					&& placement.worldMixing.additionalSeasons != null
					)
				{
					foreach (var season in placement.worldMixing.additionalSeasons)
					{
						var seasonData = (db.GameplaySeasons.TryGet(season) as MeteorShowerSeason);
						if (seasonData != null)
							seasons.Add(seasonData);
					}
				}

				return seasons;
			}
		}

		public List<MeteorShowerEvent> CurrentMeteorShowerTypes
		{
			get
			{
				var showers = new List<MeteorShowerEvent>();

				var db = Db.Get();
				foreach (var seasonData in CurrentMeteorSeasons)
				{
					if (seasonData != null)
					{
						foreach (var shower in seasonData.events)
						{
							var showerEvent = shower as MeteorShowerEvent;
							if (!showers.Contains(showerEvent))
								showers.Add(showerEvent);
						}
					}
				}
				return showers;
			}
		}

		private void BackupOriginalWorldMeteors()
		{
			if (world != null)
			{
				if (!ModAssets.ChangedMeteorSeasons.ContainsKey(world))
				{
					ModAssets.ChangedMeteorSeasons[world] = new List<string>(world.seasons);
				}
			}
		}

		public void AddMeteorSeason(string id)
		{
			BackupOriginalWorldMeteors();
			if (world != null)
			{
				world.seasons.Add(id);
			}
		}

		public void RemoveMeteorSeason(string id)
		{
			BackupOriginalWorldMeteors();
			if (world != null)
			{
				if (world.seasons.Contains(id))
				{
					world.seasons.Remove(id);
				}
			}
		}




		#endregion
		#region PlanetTraits

		private List<string> currentPlanetTraits = new List<string>();
		[JsonIgnore] public List<string> CurrentTraits => currentPlanetTraits;
		public void SetWorldTraits(List<string> NEWs)
		{
			if (NEWs == null) NEWs = new List<string>();
			currentPlanetTraits = NEWs;
			for (int i = currentPlanetTraits.Count - 1; i >= 0; i--)
			{
				var currentTrait = currentPlanetTraits[i];
				if (!ModAssets.AllTraitsWithRandomDict.TryGetValue(currentTrait, out _))
				{
					currentPlanetTraits.RemoveAt(i);
				}
			}
		}


		public static List<WorldTrait> AllowedWorldTraitsFor(List<string> currentTraits, ProcGen.World world)
		{
			List<WorldTrait> AllTraits = ModAssets.AllTraitsWithRandomValuesOnly;

			if (world == null)
			{
				return new List<WorldTrait>();
			}

			List<WorldTrait> AlwaysAvailableTraits = AllTraits.FindAll((WorldTrait trait) => trait.traitTags.Contains(ModAPI.CGM_TraitTags.OverrideWorldRules_AlwaysAllow));

			List<string> ExclusiveWithTags = new List<string>();

			if (currentTraits.Count > 0 || (world != null && world.disableWorldTraits))
			{
				AllTraits.RemoveAll((WorldTrait trait) => trait.filePath == ModAssets.CGM_RandomTrait);
			}


			foreach (var trait in currentTraits)
			{
				if (SettingsCache.worldTraits.ContainsKey(trait))
				{
					ExclusiveWithTags.AddRange(SettingsCache.worldTraits[trait].exclusiveWithTags);
				}
				if (trait == ModAssets.CGM_RandomTrait) //random trait is mutually exclusive with everything else
					return new List<WorldTrait>();
			}

			foreach (ProcGen.World.TraitRule rule in world.worldTraitRules)
			{

				TagSet requiredTags = ((rule.requiredTags != null) ? new TagSet(rule.requiredTags) : null);
				TagSet forbiddenTags = ((rule.forbiddenTags != null) ? new TagSet(rule.forbiddenTags) : null);

				AllTraits.RemoveAll((WorldTrait trait) =>
					(requiredTags != null && !trait.traitTagsSet.ContainsAll(requiredTags))
					|| (forbiddenTags != null && trait.traitTagsSet.ContainsOne(forbiddenTags))
					|| (rule.forbiddenTraits != null && rule.forbiddenTraits.Contains(trait.filePath))
					|| !trait.IsValid(world, logErrors: true));

			}
			AllTraits = AllTraits.Union(AlwaysAvailableTraits).ToList();
			AllTraits.RemoveAll((WorldTrait trait) =>
				 !trait.IsValid(world, logErrors: true)
				|| trait.exclusiveWithTags.Any(x => ExclusiveWithTags.Any(y => y == x))
				|| currentTraits.Contains(trait.filePath)
				|| trait.exclusiveWith.Any(x => currentTraits.Any(y => y == x))
				);


			return AllTraits;

		}
		[JsonIgnore] public List<WorldTrait> AllowedPlanetTraits => AllowedWorldTraitsFor(currentPlanetTraits, world);

		public bool RemoveWorldTrait(WorldTrait trait)
		{
			//SgtLogger.l(trait.filePath, "TryingToRemove");

			string traitID = trait.filePath;
			bool allowed = currentPlanetTraits.Contains(traitID);
			if (allowed)
				currentPlanetTraits.Remove(traitID);
			return allowed;
		}
		public bool AddWorldTrait(WorldTrait trait)
		{
			string traitID = trait.filePath;
			bool allowed = !currentPlanetTraits.Contains(traitID);
			if (allowed)
				currentPlanetTraits.Add(traitID);
			return allowed;
		}
		public void ClearWorldTraits()
		{
			currentPlanetTraits.Clear();
		}
		public List<string> GetWorldTraits()
		{
			return currentPlanetTraits;
		}
		#endregion
	}
}
