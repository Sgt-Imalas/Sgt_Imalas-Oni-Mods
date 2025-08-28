using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	class SolidDeliverySelection : KMonoBehaviour, FewOptionSideScreen.IFewOptionSideScreen
	{
		public List<Tag> Options = new();

		[MyCmpReq] ManualDeliveryKG manualDelivery;

		public FewOptionSideScreen.IFewOptionSideScreen.Option[] GetOptions() => Options.Select(o =>
			new FewOptionSideScreen.IFewOptionSideScreen.Option(o, o.ProperName(), Def.GetUISprite(o))).ToArray();



		public Tag GetSelectedOption() => manualDelivery.RequestedItemTag;

		public void OnOptionSelected(FewOptionSideScreen.IFewOptionSideScreen.Option option)
		{
			manualDelivery.requestedItemTag = option.tag;
		}
	}
}
