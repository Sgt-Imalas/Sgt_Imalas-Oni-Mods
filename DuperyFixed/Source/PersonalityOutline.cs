using HarmonyLib;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using TUNING;
using UtilLibs;
using static STRINGS.DUPLICANTS;

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
		public string Model { get; set; }


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
		public string Neck { get; set; }
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

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string ArmSkin { get; set; }
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string LegSkin { get; set; }

		/// <summary>
		/// Conversations dont use regular mouth overrides, instead, the animation has a dedicated extra anim for mouth_006 (sonjar).
		/// setting this value to true will make the dupe use that mouth_006 anim instead of the regular mouth anim.
		/// </summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public bool RoboMouthConversation { get; set; } = false;

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string SpeechMouth { get; set; }

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
			if (p.CongenitalTrait != null && p.CongenitalTrait != CongenitalTrait) { CongenitalTrait = p.CongenitalTrait; isModified = true; }
			if (p.JoyTrait != null && p.JoyTrait != JoyTrait) { JoyTrait = p.JoyTrait; isModified = true; }
			if (p.StickerType != null && p.StickerType != StickerType) { StickerType = p.StickerType; isModified = true; }
			if (p.HeadShape != null && p.HeadShape != HeadShape) { HeadShape = p.HeadShape; isModified = true; }
			if (p.Mouth != null && p.Mouth != Mouth) { Mouth = p.Mouth; isModified = true; }
			if (p.SpeechMouth != null && p.SpeechMouth != SpeechMouth) { SpeechMouth = p.SpeechMouth; isModified = true; }
			if (p.Neck != null && p.Neck != Neck) { Neck = p.Neck; isModified = true; }
			if (p.Eyes != null && p.Eyes != Eyes) { Eyes = p.Eyes; isModified = true; }
			if (p.Hair != null && p.Hair != Hair) { Hair = p.Hair; isModified = true; }
			if (p.Body != null && p.Body != Body) { Body = p.Body; isModified = true; }
			if (p.Belt != null && p.Belt != Belt) { Belt = p.Belt; isModified = true; }
			if (p.Cuff != null && p.Cuff != Cuff) { Cuff = p.Cuff; isModified = true; }
			if (p.Foot != null && p.Foot != Foot) { Foot = p.Foot; isModified = true; }
			if (p.Hand != null && p.Hand != Hand) { Hand = p.Hand; isModified = true; }
			if (p.Pelvis != null && p.Pelvis != Pelvis) { Pelvis = p.Pelvis; isModified = true; }
			if (p.Leg != null && p.Leg != Leg) { Leg = p.Leg; isModified = true; }
			if (p.ArmSkin != null && p.ArmSkin != Leg) { ArmSkin = p.ArmSkin; isModified = true; }
			if (p.LegSkin != null && p.LegSkin != LegSkin) { LegSkin = p.LegSkin; isModified = true; }
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
				if (p.SpeechMouth == null) SpeechMouth = null;
				if (p.Neck == null) Neck = null;
				if (p.Eyes == null) Eyes = null;
				if (p.Hair == null) Hair = null;
				if (p.Body == null) Body = null;
				if (p.Belt == null) Belt = null;
				if (p.Cuff == null) Cuff = null;
				if (p.Foot == null) Foot = null;
				if (p.Hand == null) Hand = null;
				if (p.Pelvis == null) Pelvis = null;
				if (p.Leg == null) Leg = null;
				if (p.ArmSkin == null) ArmSkin = null;
				if (p.LegSkin == null) LegSkin = null;
			}
		}

		static HashSet<string> ValidModels = new() { "Minion", "BionicMinion" }; //tags arent initialized yet so they are null, update from GameTags.Minions.Models.AllModels when new minion type releases
		public bool ToPersonality(string nameStringKey, out Personality outPersonality, out string failReason)
		{
			failReason = null;

			if (Model == null || !ValidModels.Contains(Model))
				Model = "Minion";
			Tag model = Model;

			bool isBionic = Model == "BionicMinion";


			if (!DlcManager.IsContentSubscribed(DlcManager.DLC3_ID) && isBionic)
			{
				failReason = "BionicMinion model is not available without the Bionic Booster DLC";
				outPersonality = null;
				return false;
			}


			nameStringKey = nameStringKey.ToUpper();

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
			if (!PersonalityGenerator.GENDERS.Contains(gender))
			{
				gender = PersonalityGenerator.DEFAULT_GENDER;
			}

			if (gender == null)
				gender = Randomize ? PersonalityGenerator.RollGender() : PersonalityGenerator.DEFAULT_GENDER;
			gender = gender.ToUpper();

			string personalityType = PersonalityType;
			if (personalityType == null)
				personalityType = Randomize ? PersonalityGenerator.RollPersonalityType() : PersonalityGenerator.DEFAULT_PERSONALITY_TYPE;

			if (CongenitalTrait != null && Db.Get().traits.TryGet(CongenitalTrait) == null)
			{
				SgtLogger.warning("invalid CongenitalTrait on dupery dupe: " + CongenitalTrait);
				CongenitalTrait = null;
			}

			if (Db.Get().traits.TryGet(StressTrait) == null)
			{
				SgtLogger.warning("invalid StressTrait on dupery dupe: " + StressTrait);
				StressTrait = null;
			}

			string stressTrait = StressTrait;
			if (stressTrait == null)
				stressTrait = Randomize ? PersonalityGenerator.RollStressTrait() : PersonalityGenerator.DEFAULT_STRESS_TRAIT;

			if (Db.Get().traits.TryGet(JoyTrait) == null)
			{
				SgtLogger.warning("invalid JoyTrait on dupery dupe: " + StressTrait);
				StressTrait = null;
			}

			string joyTrait = JoyTrait;
			if (joyTrait == null)
				joyTrait = Randomize ? PersonalityGenerator.RollJoyTrait() : PersonalityGenerator.DEFAULT_JOY_TRAIT;

			string stickerType = "";
			if (joyTrait == "StickerBomber")
			{
				stickerType = StickerType;
				if (stickerType.IsNullOrWhiteSpace())
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


			int headShape = ChooseAccessoryNumber(Db.Get().AccessorySlots.HeadShape, HeadShape);
			int mouth = Mouth == null ? headShape : ChooseAccessoryNumber(Db.Get().AccessorySlots.Mouth, Mouth);
			int speechMouth = (SpeechMouth == null || !int.TryParse(SpeechMouth, out int speechMouthParsed)) ? 0 : speechMouthParsed;
			//SgtLogger.l(Name + " - SpeechMouth: " + SpeechMouth+ "parsed: " + speechMouth);

			if (RoboMouthConversation)
				speechMouth = 6;

			int eyes = ChooseAccessoryNumber(Db.Get().AccessorySlots.Eyes, Eyes);

			//legacy personalities without arms defined
			if (HeadShape != null && !Randomize && ArmSkin == null)
			{
				SgtLogger.l("legacy dupe without arms, using headshape: " + HeadShape);
				if (ArmSkin == null) ArmSkin = HeadShape;
				if (LegSkin == null) LegSkin = HeadShape;
			}
			if (Mouth != null && Mouth.Length > 1)// && !int.TryParse(Mouth, out _))
			{
				string genericMouthFlap = $"anim_{Mouth}_flap_kanim";
				if (Assets.TryGetAnim(genericMouthFlap, out _))
				{
					SgtLogger.l("adding SpeechMonitorKanimOverride: " + genericMouthFlap + " to personality: " + nameStringKey);
					PersonalityManager.RegisterCustomSpeechMonitorKanim(nameStringKey, genericMouthFlap);
										
					//InjectionMethods.RegisterCustomInteractAnim(KAnimGroupFile.GetGroupFile(), genericMouthFlap);
				}
				else
				{
					SgtLogger.l("no custom speech monitor anim found for " + Mouth);

				}
			}
			if (Eyes != null && Eyes.Length > 1)// && !int.TryParse(Eyes, out _))
			{
				string customBlinkAnim = $"anim_{Eyes}_blinks_kanim";
				if (Assets.TryGetAnim(customBlinkAnim, out _))
				{
					SgtLogger.l("adding CustomBlinkMonitorKanimOverride: " + customBlinkAnim + " to personality: " + nameStringKey);
					PersonalityManager.RegisterCustomBlinkMonitorKanim(nameStringKey, customBlinkAnim);
				}
				else
				{
					SgtLogger.l("no custom blink monitor anim found for " + Eyes);
				}

			}

			// Customisable accessories
			int hair = ChooseAccessoryNumber(Db.Get().AccessorySlots.Hair, Hair);
			int body = ChooseAccessoryNumber(Db.Get().AccessorySlots.Body, Body);
						
			if (body == 7)
				body = 5;

			int neck = ChooseAccessoryNumber(Db.Get().AccessorySlots.Neck, Neck, 0);
			int belt = ChooseAccessoryNumber(Db.Get().AccessorySlots.Belt, Belt, 0);
			int cuff = ChooseAccessoryNumber(Db.Get().AccessorySlots.Cuff, Cuff, 0);
			int foot = ChooseAccessoryNumber(Db.Get().AccessorySlots.Foot, Foot, 0);
			int hand = ChooseAccessoryNumber(Db.Get().AccessorySlots.Hand, Hand, 0);
			int pelvis = ChooseAccessoryNumber(Db.Get().AccessorySlots.Pelvis, Pelvis, 0);
			int leg = ChooseAccessoryNumber(Db.Get().AccessorySlots.Leg, Leg, 0);
			int armSkin = ChooseAccessoryNumber(Db.Get().AccessorySlots.ArmUpperSkin, ArmSkin);
			int legSkin = ChooseAccessoryNumber(Db.Get().AccessorySlots.LegSkin, LegSkin);


			//arms are split into arm_lower_[X] and arm_upper_[X] in the anims, so we need to interpolate
			string armUpper = ArmSkin, armLower = ArmSkin;
			if(ArmSkin != null && ArmSkin.Length > 1 && ArmSkin.Contains ("arm_"))
			{
				armLower = ArmSkin.Replace("arm_", "arm_lower_");
				armUpper = ArmSkin.Replace("arm_", "arm_upper_");
			}


			// Remember any custom accessories
			DuperyPatches.PersonalityManager.TryAssignAccessory(nameStringKey, Db.Get().AccessorySlots.Hair.Id, Hair);
			DuperyPatches.PersonalityManager.TryAssignAccessory(nameStringKey, Db.Get().AccessorySlots.Eyes.Id, Eyes);
			DuperyPatches.PersonalityManager.TryAssignAccessory(nameStringKey, Db.Get().AccessorySlots.Mouth.Id, Mouth);

			if(Body != null && Body.Length > 1 && Body.Contains("torso"))
			{
				DuperyPatches.PersonalityManager.TryAssignAccessory(nameStringKey, Db.Get().AccessorySlots.Body.Id, Body);
				string armId = Body.Replace("torso", "arm_sleeve");
				DuperyPatches.PersonalityManager.TryAssignAccessory(nameStringKey, Db.Get().AccessorySlots.Arm.Id, armId);
				string armlowerId = Body.Replace("torso", "arm_lower_sleeve");
				DuperyPatches.PersonalityManager.TryAssignAccessory(nameStringKey, Db.Get().AccessorySlots.ArmLower.Id, armlowerId);
			}

			DuperyPatches.PersonalityManager.TryAssignAccessory(nameStringKey, Db.Get().AccessorySlots.HeadShape.Id, HeadShape);
			DuperyPatches.AccessoryManager.RegisterPersonalityForCustomCheeks(nameStringKey, Mouth);

			DuperyPatches.PersonalityManager.TryAssignAccessory(nameStringKey, Db.Get().AccessorySlots.Neck.Id, Neck);
			DuperyPatches.PersonalityManager.TryAssignAccessory(nameStringKey, Db.Get().AccessorySlots.Belt.Id, Belt);
			DuperyPatches.PersonalityManager.TryAssignAccessory(nameStringKey, Db.Get().AccessorySlots.Cuff.Id, Cuff);
			DuperyPatches.PersonalityManager.TryAssignAccessory(nameStringKey, Db.Get().AccessorySlots.Foot.Id, Foot);
			DuperyPatches.PersonalityManager.TryAssignAccessory(nameStringKey, Db.Get().AccessorySlots.Hand.Id, Hand);
			DuperyPatches.PersonalityManager.TryAssignAccessory(nameStringKey, Db.Get().AccessorySlots.Pelvis.Id, Pelvis);
			DuperyPatches.PersonalityManager.TryAssignAccessory(nameStringKey, Db.Get().AccessorySlots.Leg.Id, Leg);
			DuperyPatches.PersonalityManager.TryAssignAccessory(nameStringKey, Db.Get().AccessorySlots.ArmUpperSkin.Id, armUpper);
			DuperyPatches.PersonalityManager.TryAssignAccessory(nameStringKey, Db.Get().AccessorySlots.ArmLowerSkin.Id, armLower);
			DuperyPatches.PersonalityManager.TryAssignAccessory(nameStringKey, Db.Get().AccessorySlots.LegSkin.Id, LegSkin);

			SgtLogger.l("Speechmouth for " + nameStringKey + ": " + SpeechMouth + " parsed: " + speechMouth);

			Personality personality = new Personality(
				nameStringKey,
				name,
				gender,
				personalityType,
				stressTrait,
				joyTrait,
				stickerType,
				CongenitalTrait,
				headShape,
				mouth,
				neck,
				eyes,
				hair,
				body,
				belt, cuff, foot, hand, pelvis, leg,
				armSkin,
				legSkin,
				description,
				StartingMinion,
				"",
				model,
				speechMouth
			);

			if (isBionic)
				personality.requiredDlcId = DlcManager.DLC3_ID;
			outPersonality = personality;
			return true;
		}

		public static PersonalityOutline FromStockPersonality(Personality personality)
		{

			string name = string.Format("STRINGS.DUPLICANTS.PERSONALITIES.{0}.NAME", personality.nameStringKey.ToUpper());
			string description = string.Format("STRINGS.DUPLICANTS.PERSONALITIES.{0}.DESC", personality.nameStringKey.ToUpper());

			PersonalityOutline jsonPersonality = new PersonalityOutline
			{
				Printable = true,
				StartingMinion = personality.startingMinion,
				Model = personality.model.ToString(),
				Name = name,
				Description = description,
				Gender = personality.genderStringKey,
				PersonalityType = personality.personalityType,
				StressTrait = personality.stresstrait,
				JoyTrait = personality.joyTrait,
				StickerType = personality.stickerType,
				HeadShape = personality.headShape.ToString(),
				Neck = personality.neck.ToString(),
				Mouth = personality.mouth.ToString(),
				SpeechMouth = personality.speech_mouth.ToString(),
				Eyes = personality.eyes.ToString(),
				Hair = personality.hair.ToString(),
				Body = personality.body.ToString(),
				Belt = personality.belt.ToString(),
				Cuff = personality.cuff.ToString(),
				Foot = personality.foot.ToString(),
				Hand = personality.hand.ToString(),
				Pelvis = personality.pelvis.ToString(),
				Leg = personality.leg.ToString(),
				ArmSkin = personality.arm_skin.ToString(),
				LegSkin = personality.leg_skin.ToString(),
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

		private int ChooseAccessoryNumber(AccessorySlot slot, string value, int defaultValue = 1)
		{
			int accessoryNumber = defaultValue;

			if (value == null || value == "")
			{
				accessoryNumber = Randomize ? PersonalityGenerator.RollAccessory(slot) : defaultValue;
			}
			else
			{
				if (int.TryParse(value, out int parsed))
				{
					accessoryNumber = parsed;
				}
				else
				{
					accessoryNumber = defaultValue;
				}
			}
			return accessoryNumber;
		}
	}
}
