using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	class SolidDeliverySelection : KMonoBehaviour, FewOptionSideScreen.IFewOptionSideScreen
	{
		[Serialize] public Tag SelectedOption = Tag.Invalid;

		public List<Tag> Options = new();

		[MyCmpReq] ManualDeliveryKG manualDelivery;

		public FewOptionSideScreen.IFewOptionSideScreen.Option[] GetOptions() => Options.Select(o =>
			new FewOptionSideScreen.IFewOptionSideScreen.Option(o, o.ProperName(), Def.GetUISprite(o))).ToArray();

		public override void OnSpawn()
		{
			base.OnSpawn();
			if (SelectedOption == Tag.Invalid)
				SelectedOption = manualDelivery.RequestedItemTag;
			else
				OverrideDeliveryRequest();

		}

		void OverrideDeliveryRequest()
		{
			if (SelectedOption != Tag.Invalid)
				manualDelivery.RequestedItemTag = SelectedOption;
		}

		public Tag GetSelectedOption() => SelectedOption;

		public void OnOptionSelected(FewOptionSideScreen.IFewOptionSideScreen.Option option)
		{
			SelectedOption = option.tag;
			OverrideDeliveryRequest();
		}
	}
}
