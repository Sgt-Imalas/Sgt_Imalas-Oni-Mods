using Klei.AI;
using System.Collections.Generic;
using System.Linq;

namespace SetStartDupes.DuplicityEditing.Helpers
{
	public static class AttributeHelper
	{
		private static List<Attribute> _editableAttributes;
		private static List<string> _editableAttributeIDs;

		private static List<Attribute> _validMinionAttributes;
		private static List<string> _validMinionAttributeIDs;
		public static List<Attribute> GetEditableAttributes()
		{
			if (_editableAttributes == null)
			{
				var attributes = Db.Get().Attributes;
				_editableAttributes = new List<Attribute>
				{
					attributes.Construction,
					attributes.Digging,
					attributes.Machinery,
					attributes.Athletics,
					attributes.Learning,
					attributes.Cooking,
					attributes.Caring,
					attributes.Strength,
					attributes.Art,
					attributes.Botanist,
					attributes.Ranching,
				};
				if (DlcManager.IsExpansion1Active())
					_editableAttributes.Add(attributes.SpaceNavigation);
				if (ModAssets.BeachedActive)
				{
					if (attributes.TryGet("Beached_Precision") != null)
						_editableAttributes.Add(attributes.Get("Beached_Precision"));
				}

			}
			return _editableAttributes;
		}

		public static List<string> GetEditableAttributeIDs()
		{
			if (_editableAttributeIDs == null)
				_editableAttributeIDs = GetEditableAttributes().Select(a => a.Id).ToList();
			return _editableAttributeIDs;
		}
		public static List<Attribute> GetValidMinionAttributes()
		{
			if (_validMinionAttributes == null)
			{
				var attributes = Db.Get().Attributes;
				var amounts = Db.Get().Amounts;
				_validMinionAttributes = GetEditableAttributes().Concat(new List<Attribute>
				{

                    //hidden attributes
                    attributes.QualityOfLife,
					attributes.QualityOfLifeExpectation,
					attributes.AirConsumptionRate,
					attributes.CarryAmount,
					attributes.RadiationResistance,
					attributes.RadiationRecovery,
					attributes.ToiletEfficiency,
					attributes.DiseaseCureSpeed,
					attributes.Decor,
					attributes.DecorExpectation,
					attributes.Sneezyness,

                    //delta attributes                    
                    amounts.Bladder.deltaAttribute,
					amounts.Stamina.deltaAttribute,
					amounts.Stress.deltaAttribute,
					amounts.Calories.deltaAttribute,
					amounts.Toxicity.deltaAttribute,
					amounts.HitPoints.deltaAttribute,
					amounts.ImmuneLevel.deltaAttribute,
				}).ToList();

			}
			return _validMinionAttributes;
		}

		public static List<string> GetValidMinionAttributeIDs()
		{
			var db = Db.Get().Amounts;
			if (_validMinionAttributeIDs == null)
			{
				_validMinionAttributeIDs = GetValidMinionAttributes().Select(a => a.Id).ToList();
			}
			return _validMinionAttributeIDs;
		}
	}
}
