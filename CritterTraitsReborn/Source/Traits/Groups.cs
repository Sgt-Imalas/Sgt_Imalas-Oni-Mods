using Klei.AI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CritterTraitsReborn.Traits
{
    public sealed class Group
    {

        public const string SizeGroupId = "SizeGroup";
        public const string NoiseGroupId = "NoiseGroup";
        public const string SmellGroupId = "SmellGroup";
        public const string GlowGroupId = "GlowGroup";
        public const string SpeedGroupId = "SpeedGroup";
        public const string LifespanGroupId = "LifespanGroup";
        public const string FertilityGroupId = "FertilityGroup";

        public Group(string id, float probability, Predicate<GameObject> requirement = null)
        {
            Id = id;
            Probability = probability;
            HasRequirements = requirement ?? (_ => true);
        }


        public static Dictionary<string, Group> Groups;

        public static Group GetGroup(string id)
        {
            if (Groups == null)
                Init();
            if(Groups.TryGetValue(id, out Group group)) 
                return group;
            return null;
        }
        private static void Init()
        {
            Groups = new Dictionary<string, Group>();
            Groups[SizeGroupId] = new Group(SizeGroupId, 0.3f);
            Groups[NoiseGroupId] = new Group(NoiseGroupId, 0.05f);
            Groups[SmellGroupId] = new Group(SmellGroupId, 0.05f, inst => !inst.HasTag(GameTags.Creatures.Swimmer));
            Groups[GlowGroupId] = new Group(GlowGroupId, DlcManager.IsExpansion1Active() ? 0.16f : 0.08f, inst => inst.GetComponent<Light2D>() == null);
            Groups[SpeedGroupId] = new Group(SpeedGroupId, 0.2f, inst => inst.GetComponent<Navigator>() != null);
            Groups[LifespanGroupId] = new Group(LifespanGroupId, 0.15f, inst => HasAmount(inst, Db.Get().Amounts.Age));
            Groups[FertilityGroupId] = new Group(FertilityGroupId, 0.1f, inst => HasAmount(inst, Db.Get().Amounts.Fertility));
    }

        public string Id { get; private set; }
        public float Probability { get; private set; }
        public Predicate<GameObject> HasRequirements { get; private set; }


        private static bool HasAmount(GameObject go, Amount amount)
        {
            return go.GetComponent<Modifiers>()?.amounts.Has(amount) ?? false;
        }
    }
}
