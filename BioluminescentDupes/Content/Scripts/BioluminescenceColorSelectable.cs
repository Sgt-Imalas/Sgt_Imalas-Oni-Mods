using Klei.AI;
using KSerialization;
using Satsuma;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace BioluminescentDupes.Content.Scripts
{
	internal class BioluminescenceColorSelectable : KMonoBehaviour
	{
		[MyCmpGet]
		Light2D light;


		[MyCmpReq]
		Traits traits;

		[Serialize]
		[SerializeField]
		public string ColorTintHex;

		public Color TintColor => ColorTintHex != null && !ColorTintHex.IsNullOrWhiteSpace() ? Util.ColorFromHex(ColorTintHex) : Color.green;

		public bool HasLightTrait()
		{
			return traits.HasTrait(BD_Bioluminescence.ID)||traits.HasTrait(nameof(GlowStick));
		}

		public override void OnSpawn()
		{
			base.OnSpawn();
			GameScheduler.Instance.ScheduleNextFrame("BioluminescenceColorSelectable.OnSpawn", (_)=>RefreshLamp());
		}

		internal void SetColor(Color color)
		{
			ColorTintHex = Util.ToHexString(color);
			RefreshLamp();
		}
		void RefreshLamp()
		{
			if(this.IsNullOrDestroyed()||this.gameObject.IsNullOrDestroyed())
			{
				return;
			}
			if (light == null || ColorTintHex == null || ColorTintHex.IsNullOrWhiteSpace())
				return;
			if (HasLightTrait())
			{
				light.Color = TintColor;
			}
		}
	}
}
