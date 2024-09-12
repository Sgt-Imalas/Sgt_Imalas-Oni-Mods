using KSerialization;
using UnityEngine;

namespace AmogusMorb.TwitchEvents.TwitchEventAddons
{
	internal class CrewColoursSetter : KMonoBehaviour
	{
		[Serialize]
		Color CrewColour;
		public override void OnSpawn()
		{
			base.OnSpawn();
			if (CrewColour == null)
			{
				CrewColour = GetBrightColor();
			}
			this.GetComponent<KAnimControllerBase>().TintColour = CrewColour;
		}

		Color GetBrightColor()
		{
			return Random.ColorHSV(0, 1, 1, 1, 1, 1);
		}
	}
}
