using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static ModInfo;

namespace Dupery
{
    class PersonalityOutline
    {
        // Properties for toggling stuff
        [JsonProperty]
        public bool Printable { get; set; } = true;
        [JsonProperty]
        public bool Randomize { get; set; } = false;
        [JsonProperty]
        public bool StartingMinion { get; set; } = true;

        // Personality properties
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Gender { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string PersonalityType { get; set; } // Doesn't seem to do anything
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string StressTrait { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string JoyTrait { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string StickerType { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string HeadShape { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Mouth { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Eyes { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Hair { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Body { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CongenitalTrait { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Belt { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Cuff { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Foot { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Hand { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Pelvis { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Leg { get; set; }

        // Extra not-serlialized properties
        private string sourceModId;
        private bool isModified;

        public PersonalityOutline() { }

        public void OverrideValues(PersonalityOutline overridingPersonality)
        {
            PersonalityOutline p = overridingPersonality;

            Printable = p.Printable;
            StartingMinion = p.StartingMinion;

            ///Potential alternative:

            //var outlineType = typeof(PersonalityOutline);

            //foreach (PropertyInfo srcProp in outlineType.GetProperties())
            //{
            //    if (!srcProp.CanRead)
            //    {
            //        continue;
            //    }
            //    PropertyInfo targetProperty = outlineType.GetProperty(srcProp.Name);
            //    if (targetProperty == null)
            //    {
            //        continue;
            //    }
            //    if (!targetProperty.CanWrite)
            //    {
            //        continue;
            //    }
            //    if (targetProperty.GetSetMethod(true) != null && targetProperty.GetSetMethod(true).IsPrivate)
            //    {
            //        continue;
            //    }
            //    if ((targetProperty.GetSetMethod().Attributes & MethodAttributes.Static) != 0)
            //    {
            //        continue;
            //    }
            //    if (!targetProperty.PropertyType.IsAssignableFrom(srcProp.PropertyType))
            //    {
            //        continue;
            //    }
            //    // Passed all tests, lets set the value
            //    var ownValue = srcProp.GetValue(this, null);
            //    var targetValue = srcProp.GetValue(p, null);
            //    if(ownValue!=targetValue)
            //    {
            //        targetProperty.SetValue(this, targetValue);
            //        isModified =true;
            //    }
            //}


            if (p.Name != null && p.Name != Name) { Name = p.Name; isModified = true; }
            if (p.Description != null && p.Description != Description) { Description = p.Description; isModified = true; }
            if (p.Gender != null && p.Gender != Gender) { Gender = p.Gender; isModified = true; }
            if (p.PersonalityType != null && p.PersonalityType != PersonalityType) { PersonalityType = p.PersonalityType; isModified = true; }
            if (p.StressTrait != null && p.StressTrait != StressTrait) { StressTrait = p.StressTrait; isModified = true; }
            if (p.JoyTrait != null && p.JoyTrait != JoyTrait) { JoyTrait = p.JoyTrait; isModified = true; }
            if (p.StickerType != null && p.StickerType != StickerType) { StickerType = p.StickerType; isModified = true; }
            if (p.HeadShape != null && p.HeadShape != HeadShape) { HeadShape = p.HeadShape; isModified = true; }
            if (p.Mouth != null && p.Mouth != Mouth) { Mouth = p.Mouth; isModified = true; }
            if (p.Eyes != null && p.Eyes != Eyes) { Eyes = p.Eyes; isModified = true; }
            if (p.Hair != null && p.Hair != Hair) { Hair = p.Hair; isModified = true; }
            if (p.Body != null && p.Body != Body) { Body = p.Body; isModified = true; }
            if (p.Belt != null && p.Belt != Belt) { Belt = p.Belt; isModified = true; }
            if (p.Cuff != null && p.Cuff != Cuff) { Cuff = p.Cuff; isModified = true; }
            if (p.Foot != null && p.Foot != Foot) { Foot = p.Foot; isModified = true; }
            if (p.Hand != null && p.Hand != Hand) { Hand = p.Hand; isModified = true; }
            if (p.Pelvis != null && p.Pelvis != Pelvis) { Pelvis = p.Pelvis; isModified = true; }
            if (p.Leg != null && p.Leg != Leg) { Leg = p.Leg; isModified = true; }
            // There's probably a cleverer way of doing all of that but whatever

            if (p.Randomize)
            {
                Randomize = true;
                if (p.PersonalityType == null) PersonalityType = null;
                if (p.StressTrait == null) StressTrait = null;
                if (p.JoyTrait == null) JoyTrait = null;
                if (p.StickerType == null) StickerType = null;
                if (p.HeadShape == null) HeadShape = null;
                if (p.Mouth == null) Mouth = null;
                if (p.Eyes == null) Eyes = null;
                if (p.Hair == null) Hair = null;
                if (p.Body == null) Body = null;
            }
        }

        public Personality ToPersonality(string nameStringKey)
        {
            nameStringKey = nameStringKey.ToUpper();

            // Meaningless attributes
            string congenitalTrait = CongenitalTrait != null ? CongenitalTrait : "None";
            int neck = -1;

            // Name can't be null
            string name = Name;
            if (name == null)
                if (!DuperyPatches.Localizer.TryGet("Dupery.STRINGS.MISSING_DUPLICANT_NAME", out name))
                    name = STRINGS.MISSING_DUPLICANT_NAME;

            // Description can't be null
            string description = Description;
            if (description == null)
                if (!DuperyPatches.Localizer.TryGet("Dupery.STRINGS.MISSING_DUPLICANT_DESCRIPTION", out description))
                    description = STRINGS.MISSING_DUPLICANT_DESCRIPTION;

            // Other values can't be null but can be randomized

            string gender = Gender;
            if (gender == null)
                gender = Randomize ? PersonalityGenerator.RollGender() : PersonalityGenerator.DEFAULT_GENDER;
            gender = gender.ToUpper();

            string personalityType = PersonalityType;
            if (personalityType == null)
                personalityType = Randomize ? PersonalityGenerator.RollPersonalityType() : PersonalityGenerator.DEFAULT_PERSONALITY_TYPE;

            string stressTrait = StressTrait;
            if (stressTrait == null)
                stressTrait = Randomize ? PersonalityGenerator.RollStressTrait() : PersonalityGenerator.DEFAULT_STRESS_TRAIT;

            string joyTrait = JoyTrait;
            if (joyTrait == null)
                joyTrait = Randomize ? PersonalityGenerator.RollJoyTrait() : PersonalityGenerator.DEFAULT_JOY_TRAIT;

            string stickerType = "";
            if (joyTrait == "StickerBomber")
            {
                stickerType = StickerType;
                if (stickerType == null | stickerType == "")
                    stickerType = Randomize ? PersonalityGenerator.RollStickerType() : PersonalityGenerator.DEFAULT_STICKER_TYPE;
            }

            // Localizable attributes
            StringEntry result;
            if (name != null)
                name = Strings.TryGet(new StringKey(name), out result) ? result.ToString() : name;
            if (description != null)
                description = Strings.TryGet(new StringKey(description), out result) ? result.ToString() : description;

            string localizedName = null;
            string localizedDescription = null;
            if (sourceModId != null)
            {
                DuperyPatches.ModLocalizers[sourceModId].TryGet($"{nameStringKey}.NAME", out localizedName);
                DuperyPatches.ModLocalizers[sourceModId].TryGet($"{nameStringKey}.DESCRIPTION", out localizedDescription);
            }

            name = localizedName != null ? localizedName : name;
            description = localizedDescription != null ? localizedDescription : description;

            // Uncustomisable accessories
            int headShape = ChooseAccessoryNumber(Db.Get().AccessorySlots.HeadShape, HeadShape);
            int mouth = Mouth == null ? headShape : ChooseAccessoryNumber(Db.Get().AccessorySlots.Mouth, Mouth);
            int eyes = ChooseAccessoryNumber(Db.Get().AccessorySlots.Eyes, Eyes);

            // Customisable accessories
            int hair = ChooseAccessoryNumber(Db.Get().AccessorySlots.Hair, Hair);
            int body = ChooseAccessoryNumber(Db.Get().AccessorySlots.Body, Body);
            int belt = ChooseAccessoryNumber(Db.Get().AccessorySlots.Belt, Belt);
            int cuff = ChooseAccessoryNumber(Db.Get().AccessorySlots.Cuff, Cuff);
            int foot = ChooseAccessoryNumber(Db.Get().AccessorySlots.Foot, Foot);
            int hand = ChooseAccessoryNumber(Db.Get().AccessorySlots.Hand, Hand);
            int pelvis = ChooseAccessoryNumber(Db.Get().AccessorySlots.Pelvis, Pelvis);
            int leg = ChooseAccessoryNumber(Db.Get().AccessorySlots.Leg, Leg);

            // Remember any custom accessories
            DuperyPatches.PersonalityManager.TryAssignAccessory(nameStringKey, Db.Get().AccessorySlots.Hair.Id, Hair);

            Personality personality = new Personality(
                nameStringKey,
                name,
                gender,
                personalityType,
                stressTrait,
                joyTrait,
                stickerType,
                congenitalTrait,
                headShape,
                mouth,
                neck,
                eyes,
                hair,
                body,
                belt, cuff, foot,hand,pelvis,leg,
                description,
                StartingMinion,
                ""
            );

            return personality;
        }

        public static PersonalityOutline FromStockPersonality(Personality personality)
        {
            string name = string.Format("STRINGS.DUPLICANTS.PERSONALITIES.{0}.NAME", personality.nameStringKey.ToUpper());
            string description = string.Format("STRINGS.DUPLICANTS.PERSONALITIES.{0}.DESC", personality.nameStringKey.ToUpper());

            PersonalityOutline jsonPersonality = new PersonalityOutline
            {
                Printable = true,
                StartingMinion = personality.startingMinion,
                Name = name,
                Description = description,
                Gender = personality.genderStringKey,
                PersonalityType = personality.personalityType,
                StressTrait = personality.stresstrait,
                JoyTrait = personality.joyTrait,
                StickerType = personality.stickerType,
                HeadShape = personality.headShape.ToString(),
                Eyes = personality.eyes.ToString(),
                Hair = personality.hair.ToString(),
                Body = personality.body.ToString(),
                Belt = personality.belt.ToString(),
                Cuff = personality.cuff.ToString(),
                Foot = personality.foot.ToString(),
                Hand = personality.hand.ToString(),
                Pelvis = personality.pelvis.ToString(),
                Leg = personality.leg.ToString(),
                CongenitalTrait = personality.congenitaltrait?.ToString(),
            };

            return jsonPersonality;
        }

        public void SetSourceModId(string sourceModId)
        {
            this.sourceModId = sourceModId;
        }

        public string GetSourceModId()
        {
            return this.sourceModId;
        }

        public bool IsModified()
        {
            return this.isModified;
        }

        private int ChooseAccessoryNumber(AccessorySlot slot, string value)
        {
            int accessoryNumber;

            if (value == null || value == "")
            {
                accessoryNumber = Randomize ? PersonalityGenerator.RollAccessory(slot) : 1;
            }
            else
            {
                if(!int.TryParse(value, out accessoryNumber))
                {
                    accessoryNumber = 1;
                }
            }
            return accessoryNumber;
        }
    }
}
