using Klei.AI;
using System.Collections.Generic;

namespace UtilLibs
{
	//Courtesy of Akis FUtility
	public class EffectBuilder
	{
		private readonly string ID;
		private string name;
		private string description;
		private readonly float duration;
		private bool triggerFloatingText;
		private bool showInUI;
		private readonly bool isBad;
		private List<AttributeModifier> modifiers;
		private string emoteAnim;
		private float emoteCooldown;
		private string customIcon;
		private string stompGroup;
		private List<Reactable.ReactablePrecondition> emotePreconditions;

		public EffectBuilder(string ID, float duration, bool isBad)
		{
			name = Strings.Get("STRINGS.DUPLICANTS.MODIFIERS." + ID.ToUpper() + ".NAME");
			description = Strings.Get("STRINGS.DUPLICANTS.MODIFIERS." + ID.ToUpper() + ".TOOLTIP");
			triggerFloatingText = true;
			showInUI = true;
			this.duration = duration;
			this.isBad = isBad;
			this.ID = ID;
			customIcon = "";
		}
		public EffectBuilder CustomIcon(string icon)
		{
			this.customIcon = icon;
			return this;
		}
		public EffectBuilder StompGroup(string Group)
		{
			this.stompGroup = Group;
			return this;
		}
		public EffectBuilder Name(string name)
		{
			this.name = name;

			return this;
		}

		public EffectBuilder Description(string description)
		{
			this.description = description;

			return this;
		}

		public EffectBuilder Modifier(string id, float value, bool isMultiplier, bool uiOnly = false, bool readOnly = true)
		{
			modifiers = modifiers ?? new List<AttributeModifier>();
			modifiers.Add(new AttributeModifier(id, value, name, isMultiplier, uiOnly, readOnly));

			return this;
		}

		public EffectBuilder Modifier(string id, float value)
		{
			modifiers = modifiers ?? new List<AttributeModifier>();
			modifiers.Add(new AttributeModifier(id, value, name));

			return this;
		}

		public EffectBuilder Emote(string emoteAnim, float emoteCooldown)
		{
			this.emoteAnim = emoteAnim;
			this.emoteCooldown = emoteCooldown;

			return this;
		}

		public EffectBuilder EmotePrecondition(Reactable.ReactablePrecondition condition)
		{
			emotePreconditions = emotePreconditions ?? new List<Reactable.ReactablePrecondition>();
			emotePreconditions.Add(condition);

			return this;
		}

		public EffectBuilder HideFloatingText()
		{
			triggerFloatingText = false;

			return this;
		}
		public EffectBuilder HideInUI()
		{
			showInUI = false;

			return this;
		}

		public void Add(ModifierSet set)
		{
			var effect = new Effect(ID, name, description, duration, showInUI, triggerFloatingText, isBad, emoteAnim, emoteCooldown, stompGroup, customIcon);

			if (modifiers != null)
			{
				effect.SelfModifiers = modifiers;
			}

			if (emotePreconditions != null)
			{
				effect.emotePreconditions = emotePreconditions;
			}

			set.effects.Add(effect);
		}
	}
}
