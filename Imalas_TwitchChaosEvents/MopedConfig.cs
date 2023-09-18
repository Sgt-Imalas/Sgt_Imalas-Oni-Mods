//using Klei.AI;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;

//namespace Imalas_TwitchChaosEvents
//{
//    internal class MopedConfig : IEquipmentConfig
//    {
//        public const string ID = "ITCE_Moped";

//        public static ComplexRecipe recipe;
//        private const PathFinder.PotentialPath.Flags suit_flags = PathFinder.PotentialPath.Flags.None;
//        private AttributeModifier expertAthleticsModifier;

//        public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

//        public EquipmentDef CreateEquipmentDef()
//        {
//            List<AttributeModifier> AttributeModifiers = new List<AttributeModifier>();
//            AttributeModifiers.Add(new AttributeModifier(TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.ATHLETICS, (float)15, (string)STRINGS.EQUIPMENT.PREFABS.ITCE_MOPED.NAME));
//            AttributeModifiers.Add(new AttributeModifier(TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.THERMAL_CONDUCTIVITY_BARRIER, TUNING.EQUIPMENT.SUITS.ATMOSUIT_THERMAL_CONDUCTIVITY_BARRIER, (string)STRINGS.EQUIPMENT.PREFABS.ITCE_MOPED.NAME));
//            AttributeModifiers.Add(new AttributeModifier(Db.Get().Attributes.CarryAmount.Id, 1000, (string)STRINGS.EQUIPMENT.PREFABS.ITCE_MOPED.NAME));
//            AttributeModifiers.Add(new AttributeModifier(Db.Get().Attributes.ScaldingThreshold.Id, (float)TUNING.EQUIPMENT.SUITS.ATMOSUIT_SCALDING, (string)STRINGS.EQUIPMENT.PREFABS.ITCE_MOPED.NAME));
//            this.expertAthleticsModifier = new AttributeModifier(TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.ATHLETICS, (float)-TUNING.EQUIPMENT.SUITS.ATMOSUIT_ATHLETICS, Db.Get().Skills.Suits1.Name);
//            EquipmentDef equipmentDef = EquipmentTemplates.CreateEquipmentDef("Atmo_Suit", TUNING.EQUIPMENT.SUITS.SLOT, SimHashes.Dirt, (float)TUNING.EQUIPMENT.SUITS.ATMOSUIT_MASS, "suit_oxygen_kanim", "", "body_moped_kanim", 7, AttributeModifiers, IsBody: true, additional_tags: new Tag[]
//            {
//      GameTags.Suit,
//      GameTags.Clothes,
//      GameTags.PedestalDisplayable
//            });
//            equipmentDef.wornID = "Worn_Atmo_Suit";
//            equipmentDef.RecipeDescription = "";
//            equipmentDef.EffectImmunites.Add(Db.Get().effects.Get("WetFeet"));
//            equipmentDef.OnEquipCallBack = (System.Action<Equippable>)(eq =>
//            {
//                Ownables soleOwner = eq.assignee.GetSoleOwner();
//                if (!((UnityEngine.Object)soleOwner != (UnityEngine.Object)null))
//                    return;
//                GameObject targetGameObject = soleOwner.GetComponent<MinionAssignablesProxy>().GetTargetGameObject();
//                Navigator component1 = targetGameObject.GetComponent<Navigator>();
//                if ((UnityEngine.Object)component1 != (UnityEngine.Object)null)
//                    component1.SetFlags(PathFinder.PotentialPath.Flags.HasAtmoSuit);
//                MinionResume component2 = targetGameObject.GetComponent<MinionResume>();
//                if (!((UnityEngine.Object)component2 != (UnityEngine.Object)null) || !component2.HasPerk((HashedString)Db.Get().SkillPerks.ExosuitExpertise.Id))
//                    return;
//                targetGameObject.GetAttributes().Get(Db.Get().Attributes.Athletics).Add(this.expertAthleticsModifier);
//            });
//            equipmentDef.OnUnequipCallBack = (System.Action<Equippable>)(eq =>
//            {
//                if (eq.assignee == null)
//                    return;
//                Ownables soleOwner = eq.assignee.GetSoleOwner();
//                if (!((UnityEngine.Object)soleOwner != (UnityEngine.Object)null))
//                    return;
//                GameObject targetGameObject = soleOwner.GetComponent<MinionAssignablesProxy>().GetTargetGameObject();
//                if ((bool)(UnityEngine.Object)targetGameObject)
//                {
//                    targetGameObject.GetAttributes()?.Get(Db.Get().Attributes.Athletics).Remove(this.expertAthleticsModifier);
//                    Navigator component3 = targetGameObject.GetComponent<Navigator>();
//                    if ((UnityEngine.Object)component3 != (UnityEngine.Object)null)
//                        component3.ClearFlags(PathFinder.PotentialPath.Flags.HasAtmoSuit);
//                    Effects component4 = targetGameObject.GetComponent<Effects>();
//                    if ((UnityEngine.Object)component4 != (UnityEngine.Object)null && component4.HasEffect("SoiledSuit"))
//                        component4.Remove("SoiledSuit");
//                }
//                Tag elementTag = eq.GetComponent<SuitTank>().elementTag;
//                eq.GetComponent<Storage>().DropUnlessHasTag(elementTag);
//            });
//            GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SuitIDs, "Atmo_Suit");
//            GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SuitIDs, "Helmet");
//            return equipmentDef;
//        }

//        public static void SetupVest(GameObject go)
//        {
//            //go.GetComponent<KPrefabID>().AddTag(GameTags.Clothes);
//            //Equippable equippable = go.GetComponent<Equippable>();
//            //if ((UnityEngine.Object)equippable == (UnityEngine.Object)null)
//            //    equippable = go.AddComponent<Equippable>();
//            //equippable.SetQuality(QualityLevel.Poor);
//            //go.GetComponent<KBatchedAnimController>().sceneLayer = Grid.SceneLayer.BuildingBack;
//        }

//        public void DoPostConfigure(GameObject go)
//        {
//            FunkyVestConfig.SetupVest(go);
//            go.GetComponent<KPrefabID>().AddTag(GameTags.PedestalDisplayable);
//        }
//    }

//}
