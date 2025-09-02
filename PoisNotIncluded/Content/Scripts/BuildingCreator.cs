using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace PoisNotIncluded.Content.Scripts
{
	internal class BuildingCreator
	{
		public const string GeneratedIdPrefix = "PNI_";
		public const string LAEMP_Prefix = "LAEMP_";
		public const string NEWBUILD_Prefix = "NEWBUILD_";
		public const string POI_Category = "PNI_BuildMenuCategory_POI";


		string DerivedFromId;

		string BuildMenuSubcategory;

		string PrefabId;
		string Name;
		string Description;
		string KAnim, InitialAnim;
		string[] AltAnims = null;
		int Width, Height;
		int Wattage;
		BuildLocationRule Rule;
		EffectorValues Decor;

		int ConstructionTime, HP;

		string[] Materials;
		float[] MaterialCosts;


		bool IsLamp = false;
		int Lux;
		int LuxRange;
		string LampOffAnim, LampOnAnim;
		LightShape LampLightShape;

		public bool IsValid = true;

		public BuildingCreator(string derivedId) => DerivedFromId = derivedId;

		public static BuildingCreator Create(string Id)
		{
			return new BuildingCreator(Id);
		}
		void EntityPrefix()
		{
			PrefabId = GeneratedIdPrefix + DerivedFromId;
		}
		void LampPrefix()
		{
			PrefabId = GeneratedIdPrefix + LAEMP_Prefix + DerivedFromId;
		}
		void NewBuildingPrefix()
		{
			PrefabId = GeneratedIdPrefix + NEWBUILD_Prefix + DerivedFromId;
		}

		bool TryHarvestFromEntity()
		{

			var entity = Assets.GetPrefab(DerivedFromId);
			if (entity == null)
			{
				SgtLogger.l("could not find placed entity with id " + DerivedFromId);
				return false;
			}

			if (!entity.TryGetComponent<OccupyArea>(out var occupyArea))
			{
				SgtLogger.l("could not find occupyArea on " + DerivedFromId);
				return false;
			}

			if (!entity.TryGetComponent<PrimaryElement>(out var elementSource))
			{
				SgtLogger.l("could not find PrimaryElement on " + DerivedFromId);
				return false;
			}
			if (!entity.TryGetComponent<KSelectable>(out var selectable))
			{
				SgtLogger.l("could not find KSelectable on " + DerivedFromId);
				return false;
			}
			string desc = string.Empty;
			if (entity.TryGetComponent<InfoDescription>(out var description))
			{
				desc = description.description;
			}

			string name = selectable.GetName();

			this.Name = name;
			this.Description = desc;


			Width = occupyArea.GetWidthInCells();
			Height = occupyArea.GetHeightInCells();

			CalculateBuildingProperties();

			//if (dimensionOverride != null)
			//{
			//	if (dimensionOverride.first > 0)
			//		width = dimensionOverride.first;
			//	if (dimensionOverride.second > 0)
			//		height = dimensionOverride.second;
			//}

			Tag material = elementSource.ElementID.CreateTag();
			var element = ElementLoader.GetElement(material);
			if (element.HasTag(GameTags.BuildableRaw))
				material = GameTags.BuildableRaw;
			else if (material == SimHashes.Creature.CreateTag() || material == SimHashes.Unobtanium.CreateTag())
				material = GameTags.Steel;
			else if (material == SimHashes.Polypropylene.CreateTag())
				material = GameTags.Plastic;
			else if (material == SimHashes.Glass.CreateTag() || material == SimHashes.Diamond.CreateTag())
				material = GameTags.Transparent;

			entity.TryGetComponent<KBatchedAnimController>(out var kbac);

			var anim = kbac.AnimFiles[0];
			KAnim = anim.name;
			InitialAnim = kbac.initialAnim;

			//if (animOverride != null)
			//{
			//initialAnim = animOverride;
			//}

			EffectorValues decorValues = new EffectorValues(0, 0);

			if (entity.TryGetComponent<DecorProvider>(out var decorator))
				decorValues = new EffectorValues((int)decorator.baseDecor, (int)decorator.baseRadius);
			Decor = decorValues;

			float cost = 25 * (Width * Height);
			MaterialCosts = [cost];
			Materials = [material.ToString()];


			//int hitpounts = 5 * (Width * Height);
			//int time = 5 * (width * height);

			//def = BuildingTemplates.CreateBuildingDef(prefabID, width, height, anim.name, hitpounts, time, [cost], [material.ToString()], element.highTemp, rule, decorValues, TUNING.NOISE_POLLUTION.NONE);
			//def.DefaultAnimState = initialAnim;
			//def.ViewMode = OverlayModes.Decor.ID;
			//def.Overheatable = false;
			//def.Floodable = false;
			//if (entity.TryGetComponent<Rotatable>(out var rotatable))
			//def.PermittedRotations = rotatable.permittedRotations;
			//if (def.PermittedRotations == PermittedRotations.Unrotatable || def.PermittedRotations == PermittedRotations.R90)
			//def.PermittedRotations = PermittedRotations.FlipH;

			return true;
		}

		void CalculateBuildingProperties()
		{
			HP = 5 * (Width * Height);
			ConstructionTime = 5 * (Width * Height);
		}


		void RegisterStrings()
		{
			if (!IsValid) return;
			string upperPrefabID = PrefabId.ToUpperInvariant();
			Strings.Add($"STRINGS.BUILDINGS.PREFABS.{upperPrefabID}.NAME", Name);
			Strings.Add($"STRINGS.BUILDINGS.PREFABS.{upperPrefabID}.DESC", Description);
			Strings.Add($"STRINGS.BUILDINGS.PREFABS.{upperPrefabID}.EFFECT", "");
		}

		public BuildingCreator OverrideAnim(string newInitialAnim)
		{
			if (!IsValid) return this;

			InitialAnim = newInitialAnim;

			return this;
		}
		public BuildingCreator DecorName()
		{
			if (!IsValid) return this;
			Name += " (" + STRINGS.DUPLICANTS.ATTRIBUTES.DECOR.NAME + ")";
			return this;
		}
		public BuildingCreator BrokenName()
		{
			if (!IsValid) return this;
			Name = Strings.Get("STRINGS.BUILDING.STATUSITEMS.BROKEN.NAME") + " " + Name;
			return this;
		}

		public BuildingCreator MaterialOverride(string[] newMaterial)
		{
			if (!IsValid) return this;
			Materials = newMaterial;
			return this;
		}
		public BuildingCreator CostOverride(float[] newMaterialCosts)
		{
			if (!IsValid) return this;
			MaterialCosts = newMaterialCosts;
			return this;
		}
		public BuildingCreator MultiAnimSelect(string[] validAnims)
		{
			if (!IsValid) return this;
			if(!validAnims.Contains(InitialAnim))
				validAnims = validAnims.Append(InitialAnim);
			AltAnims = validAnims;
			return this;
		}
		public BuildingCreator SetDimensions(int width, int height, bool recalc = false)
		{
			if (!IsValid) return this;
			Width = width;
			Height = height;
			if(recalc)
			{
				CalculateBuildingProperties();
				if(Decor == default || Decor.amount == 0 && Decor.radius == 0)
				{
					int min = Math.Min(Width, Height);
					Decor = new(min * 5,min/2);
				}
			}
			return this;
		}
		public static BuildingCreator DeriveFromEntity(string Id, string subCategory, BuildLocationRule rule)
		{
			var creator = new BuildingCreator(Id);
			creator.Rule = rule;
			creator.BuildMenuSubcategory = subCategory;
			creator.EntityPrefix();
			creator.IsValid = creator.TryHarvestFromEntity();
			return creator;
		}

		void AddName(string name)
		{
			if(name.Contains("STRINGS."))
				name = Strings.Get(name);
			Name = name;
		}
		void AddDesc(string desc)
		{
			if (desc.Contains("STRINGS."))
				desc = Strings.Get(desc);
			Description = desc;
		}

		public static BuildingCreator DeriveFromAnim(string Id, string subCategory, BuildLocationRule rule, string anim, string initialAnim, string name, string desc, int width, int height, string[] mats, float[] costs, bool decorName = true)
		{
			var creator = new BuildingCreator(Id);
			creator.Rule = rule;
			creator.BuildMenuSubcategory = subCategory;
			creator.NewBuildingPrefix();
			if (Assets.GetAnim(anim) == null)
			{
				creator.IsValid = false;
				return creator;
			}
			creator.InitialAnim = initialAnim;
			creator.KAnim = anim;
			creator.Name = name;
			creator.Description = desc;
			if (decorName)
				creator.DecorName();
			creator.SetDimensions(width, height,true);
			creator.CostOverride(costs);
			creator.MaterialOverride(mats);

			return creator;

		}
	}
}
