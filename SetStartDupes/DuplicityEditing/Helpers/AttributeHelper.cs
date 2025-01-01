using Klei.AI;
using System.Collections.Generic;
using System.Linq;

namespace SetStartDupes.DuplicityEditing.Helpers
{
    public static class AttributeHelper
    {
        private static List<Attribute> _editableAttributesBase;
        private static List<string> _editableAttributeIDsBase;
        private static List<Attribute> _editableAttributeBionics;
        private static List<string> _editableAttributeIDsBionics;

        private static List<Attribute> _validMinionAttributes;
        private static List<string> _validMinionAttributeIDs;
        public static List<Attribute> GetEditableAttributes(Tag Model)
        {

            bool init = Model == null;
            var attributes = Db.Get().Attributes;

            if (_editableAttributeIDsBase == null)
            {
                _editableAttributesBase = new List<Attribute>
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
                    _editableAttributesBase.Add(attributes.SpaceNavigation);
                if (ModAssets.BeachedActive)
                {
                    if (attributes.TryGet("Beached_Precision") != null)
                        _editableAttributesBase.Add(attributes.Get("Beached_Precision"));
                }
                _editableAttributeBionics = new();
                //_editableAttributeBionics = new List<Attribute>()
                //{
                //    attributes.BionicBoosterSlots,
                //    attributes.BionicBatteryCountCapacity
                //};
            }
            var items = new List<Attribute>(_editableAttributesBase);
            //if(init ||Model == GameTags.Minions.Models.Standard)
            //{

            //         }

            if (init || Model == GameTags.Minions.Models.Bionic && DlcManager.IsContentSubscribed(DlcManager.DLC3_ID))
            {
                return _editableAttributesBase.Concat(_editableAttributeBionics).ToList();
            }

            return _editableAttributesBase;
        }
        public static List<string> GetEditableAttributeIDs(Tag Model)
        {
            if (_editableAttributeIDsBase == null)
            {
                GetEditableAttributes(null);
                _editableAttributeIDsBionics = _editableAttributesBase.Select(a => a.Id).ToList(); 
                _editableAttributeIDsBase = _editableAttributeBionics.Select(a => a.Id).ToList(); 
            }
            if (Model == null || Model == GameTags.Minions.Models.Bionic && DlcManager.IsContentSubscribed(DlcManager.DLC3_ID))
            {
                return _editableAttributeIDsBase.Concat(_editableAttributeIDsBionics).ToList();
            }
            return _editableAttributeIDsBase;
        }
        public static List<Attribute> GetValidMinionAttributes()
        {
            if (_validMinionAttributes == null)
            {
                var attributes = Db.Get().Attributes;
                var amounts = Db.Get().Amounts;
                _validMinionAttributes = GetEditableAttributes(null)
                    .Concat(new List<Attribute>
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
                    amounts.BionicGunk.deltaAttribute,
                    amounts.BionicInternalBattery.deltaAttribute,
                    amounts.BionicOil.deltaAttribute,
                    amounts.BionicOxygenTank.deltaAttribute,
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
