using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Rockets_TinyYetBig.Content.Scripts.Buildings.RocketModules
{
	internal class EngineFuelFilter : KMonoBehaviour, FewOptionSideScreen.IFewOptionSideScreen
	{
		[Serialize] public Tag SelectedFuel = Tag.Invalid;

		[SerializeField]
		public Tag AnyTag = Tag.Invalid;

		[MyCmpReq] protected RocketEngineCluster engine;
		[MyCmpGet] protected ConduitConsumer fuelConsumer;

		List<Tag> _options = null;

		public FewOptionSideScreen.IFewOptionSideScreen.Option[] GetOptions()
		{
			if (_options == null)
			{
				_options = ElementLoader.elements.FindAll(e => e.HasTag(AnyTag)).Select(e => e.tag).ToList();
			}

			var options = _options.Select(o =>
			new FewOptionSideScreen.IFewOptionSideScreen.Option(o, o.ProperName(), Def.GetUISprite(o))).ToList();
			if (AnyTag != Tag.Invalid)
			{
				options.Insert(0, new FewOptionSideScreen.IFewOptionSideScreen.Option(AnyTag, GameTags.Any.ProperName() + " " + AnyTag.ProperName(), new(Assets.GetSprite("ui_buildable_any"), Color.white)));
			}

			return [.. options];
		}

		public override void OnSpawn()
		{
			AnyTag = engine.fuelTag;
			base.OnSpawn();
			if (SelectedFuel == Tag.Invalid)
				SelectedFuel = AnyTag;
			RefreshConsumer();
		}

		public Tag GetSelectedOption() => SelectedFuel;

		public virtual void OnOptionSelected(FewOptionSideScreen.IFewOptionSideScreen.Option option)
		{
			SelectedFuel = option.tag;
			RefreshConsumer();
		}
		void RefreshConsumer()
		{
			if (fuelConsumer != null)
				fuelConsumer.capacityTag = SelectedFuel;
		}
	}
}
