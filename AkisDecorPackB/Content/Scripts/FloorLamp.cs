using AkisDecorPackB.Content.ModDb;
using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AkisDecorPackB.Content.Scripts
{
	public class FloorLamp : KMonoBehaviour
	{
		[Serialize] public string paneId;

		[MyCmpReq] private Deconstructable deconstructable;
		[MyCmpReq] private KBatchedAnimController kbac;

		public Color overrideColor;

		public static Dictionary<SimHashes, Color> colors = new()
		{
			{ SimHashes.Gold, Util.ColorFromHex("c8a256") },
			{ SimHashes.GoldAmalgam, Util.ColorFromHex("c8a256") },
			{ SimHashes.FoolsGold, Util.ColorFromHex("c8a256") },
			{ SimHashes.Cinnabar, Util.ColorFromHex("cc6a6a") },
			{ SimHashes.Cuprite, Util.ColorFromHex("d66c49") },
			{ SimHashes.Copper, Util.ColorFromHex("d66c49") },
			{ SimHashes.Cobaltite, Util.ColorFromHex("4777c9") },
			{ SimHashes.Cobalt, Util.ColorFromHex("4777c9") },
			{ SimHashes.IronOre, Util.ColorFromHex("9c9c9c") },
			{ SimHashes.Iron, Util.ColorFromHex("9c9c9c") },
		};

		public static Dictionary<SimHashes, Func<Vector3, Color>> colorFns = new()
		{
			{ (SimHashes)Hash.SDBMLower("UnobtaniumAlloy"), GetRainbowColor },
			{ (SimHashes)Hash.SDBMLower("UnobtaniumDust"), GetRainbowColor }
		};

		public override void OnSpawn()
		{
			base.OnSpawn();
			kbac.Offset = new Vector3(0f, 0.5f);

			UpdatePane();


			var primaryElement = GetComponent<PrimaryElement>();

			var elementId = primaryElement.ElementID;

			if (colorFns.TryGetValue(primaryElement.ElementID, out var colorFn))
				overrideColor = colorFn.Invoke(transform.position);
			else if (colors.TryGetValue(primaryElement.ElementID, out var color))
				overrideColor = color;
			else
				overrideColor = (Color)primaryElement.Element.substance.colour with { a = byte.MaxValue };

			Subscribe((int)GameHashes.DeconstructComplete, OnDeconstructed);
		}

		private void OnDeconstructed(object obj)
		{
			kbac.enabled = false;
		}

		private void UpdatePane()
		{
			if ((deconstructable?.constructionElements == null || deconstructable?.constructionElements.Length == 1))
				return;
			UpdateAnim();
		}
		private void UpdateAnim()
		{
			kbac.enabled = true;

			var currentAnim = kbac.CurrentAnim.name;
			if (paneId.IsNullOrWhiteSpace())
			{
				string element = deconstructable.constructionElements.Last().ToString();
				paneId = FloorLampPane.GetIdFromElement(element);
			}

			var pane = Mod_Db.FloorLampPanes.TryGet(paneId);
			if (pane != null)
			{
				kbac.SwapAnims([Assets.GetAnim(pane.animFile)]);
				kbac.Play(currentAnim);

				GetComponent<Light2D>().Color = pane.lightColor;
			}
		}


		private static Color GetRainbowColor(Vector3 position)
		{
			var hue = ((Mathf.Abs(position.x + position.y) / 2f) % 10f) / 10f;
			var saturation = 0.7f;
			var value = 0.9f;

			return Color.HSVToRGB(hue, saturation, value);
		}
	}
}
