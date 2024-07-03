using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OniRetroEdition.Entities
{
    internal class OxygenMaskRetroConfig: IEquipmentConfig
    {
        public static string ID = "Oxygen_Mask_Retro";
        private const PathFinder.PotentialPath.Flags suit_flags = PathFinder.PotentialPath.Flags.HasOxygenMask;
        private AttributeModifier expertAthleticsModifier;

        public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

        public EquipmentDef CreateEquipmentDef()
        {
            List<AttributeModifier> AttributeModifiers = new List<AttributeModifier>();
            AttributeModifiers.Add(new AttributeModifier(TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.ATHLETICS, (float)TUNING.EQUIPMENT.SUITS.OXYGEN_MASK_ATHLETICS, global::STRINGS.EQUIPMENT.PREFABS.OXYGEN_MASK.NAME));
            this.expertAthleticsModifier = new AttributeModifier(TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.ATHLETICS, (float)-TUNING.EQUIPMENT.SUITS.OXYGEN_MASK_ATHLETICS, Db.Get().Skills.Suits1.Name);
            EquipmentDef equipmentDef = EquipmentTemplates.CreateEquipmentDef(ID, TUNING.EQUIPMENT.SUITS.SLOT, SimHashes.Dirt, 15f, "oxygen_mask_kanim", "mask_oxygen", "", 6, AttributeModifiers, additional_tags: new Tag[2]
            {
                GameTags.Suit,
                GameTags.Clothes
            });
            equipmentDef.wornID = "Worn_Oxygen_Mask";
            equipmentDef.RecipeDescription = global::STRINGS.EQUIPMENT.PREFABS.OXYGEN_MASK.RECIPE_DESC;
            equipmentDef.OnEquipCallBack = (System.Action<Equippable>)(eq =>
            {
                Ownables soleOwner = eq.assignee.GetSoleOwner();
                if (!((UnityEngine.Object)soleOwner != (UnityEngine.Object)null))
                    return;
                GameObject targetGameObject = soleOwner.GetComponent<MinionAssignablesProxy>().GetTargetGameObject();
                Navigator component1 = targetGameObject.GetComponent<Navigator>();
                if ((UnityEngine.Object)component1 != (UnityEngine.Object)null)
                    component1.SetFlags(PathFinder.PotentialPath.Flags.HasOxygenMask);
                MinionResume component2 = targetGameObject.GetComponent<MinionResume>();
                if (!((UnityEngine.Object)component2 != (UnityEngine.Object)null) || !component2.HasPerk((HashedString)Db.Get().SkillPerks.ExosuitExpertise.Id))
                    return;
                targetGameObject.GetAttributes().Get(Db.Get().Attributes.Athletics).Add(this.expertAthleticsModifier);
            });
            equipmentDef.OnUnequipCallBack = (System.Action<Equippable>)(eq =>
            {
                if (eq.assignee == null)
                    return;
                Ownables soleOwner = eq.assignee.GetSoleOwner();
                if (!((UnityEngine.Object)soleOwner != (UnityEngine.Object)null))
                    return;
                GameObject targetGameObject = soleOwner.GetComponent<MinionAssignablesProxy>().GetTargetGameObject();
                if (!(bool)(UnityEngine.Object)targetGameObject)
                    return;
                targetGameObject.GetAttributes()?.Get(Db.Get().Attributes.Athletics).Remove(this.expertAthleticsModifier);
                Navigator component = targetGameObject.GetComponent<Navigator>();
                if (!((UnityEngine.Object)component != (UnityEngine.Object)null))
                    return;
                component.ClearFlags(PathFinder.PotentialPath.Flags.HasOxygenMask);

                if(eq.TryGetComponent<PrimaryElement>(out var primaryElement))
                {
                    if(eq.TryGetComponent<SuitTank>(out var suitTank))
                    {
                        float mass = suitTank.GetTankAmount();
                        SimHashes element = SimHashes.Oxygen;

                    }

                    UnityEngine.Object.Destroy(eq.gameObject);
                }

            });
            GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SuitIDs, ID);
            GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SuitIDs, "Helmet");
            return equipmentDef;
        }

        public void DoPostConfigure(GameObject go)
        {
            Storage storage = go.AddComponent<Storage>();
            storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
            storage.showInUI = true;
            SuitTank suitTank = go.AddComponent<SuitTank>();
            suitTank.element = "Oxygen";
            suitTank.capacity = 20f;
            suitTank.elementTag = GameTags.Breathable;
            KPrefabID component = go.GetComponent<KPrefabID>();
            component.AddTag(GameTags.Clothes);
            component.AddTag(GameTags.PedestalDisplayable);
            go.AddComponent<SuitDiseaseHandler>();
        }
    }
}
