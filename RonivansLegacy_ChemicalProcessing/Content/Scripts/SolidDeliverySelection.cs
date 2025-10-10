using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	class SolidDeliverySelection : KMonoBehaviour, FewOptionSideScreen.IFewOptionSideScreen
	{
		[Serialize] public Tag SelectedOption = Tag.Invalid;

		public List<Tag> Options = new();

		[MyCmpReq] protected ManualDeliveryKG manualDelivery;

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

		protected virtual void OverrideDeliveryRequest()
		{
			if (SelectedOption != Tag.Invalid)
			{
				manualDelivery.RequestedItemTag = SelectedOption;
				List<GameObject> dropItems = new();
				foreach(var item in manualDelivery.storage.items)
				{
					///Remove any items that are not the selected option, but are in the options list
					if (item.TryGetComponent<KPrefabID>(out var kPrefabID) 
						&& Options.Contains(kPrefabID.PrefabTag) && kPrefabID.PrefabTag != SelectedOption)
					{
						dropItems.Add(item);
					}
				}
				foreach(var item in dropItems)
				{
					manualDelivery.storage.Drop(item);
				}
			}
		}

		public Tag GetSelectedOption() => SelectedOption;

		public virtual void OnOptionSelected(FewOptionSideScreen.IFewOptionSideScreen.Option option)
		{
			SelectedOption = option.tag;
			OverrideDeliveryRequest();
		}
	}
}
