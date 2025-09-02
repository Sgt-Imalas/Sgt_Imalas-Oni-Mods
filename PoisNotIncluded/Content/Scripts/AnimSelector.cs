using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static FewOptionSideScreen;

namespace PoisNotIncluded.Content.Scripts
{
	internal class AnimSelector : KMonoBehaviour,IFewOptionSideScreen
	{
		[SerializeField]
		public string[] AvailableAnimNames;

		[Serialize]
		public string SelectedAnim = null;

		[MyCmpGet]
		KBatchedAnimController kbac;


		public IFewOptionSideScreen.Option[] GetOptions()
		{
			return AvailableAnimNames.Select(anim => new IFewOptionSideScreen.Option(anim, anim, Def.GetUISprite(this.gameObject))).ToArray();
		}

		public Tag GetSelectedOption()
		{
			return SelectedAnim;
		}

		public void OnOptionSelected(IFewOptionSideScreen.Option option)
		{
			SelectedAnim = option.tag.ToString();

			kbac.Play(SelectedAnim, KAnim.PlayMode.Loop);
		}
		public override void OnSpawn()
		{
			base.OnSpawn();
			if(SelectedAnim != null) 
				kbac.Play(SelectedAnim, KAnim.PlayMode.Loop);
		}
	}
}
