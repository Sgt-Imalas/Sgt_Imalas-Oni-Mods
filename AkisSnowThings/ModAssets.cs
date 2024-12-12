using AkisSnowThings.Content.Defs.Buildings;
using Klei.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace AkisSnowThings
{
	public class ModAssets
	{
		public static HashSet<string> GlassCaseSealables { get; set; } = new HashSet<string>()
		{
			SnowSculptureConfig.ID,
			IceSculptureConfig.ID
		};
		public class Hashes
		{
			public static ModHashes Sealed = new ModHashes("SnowSculptures_Sealed");
			public static ModHashes UnSealed = new ModHashes("SnowSculptures_UnSealed");
		}

		public static Tag customHighlightTag = TagManager.Create("SnowThings_CustomHighlightTag");
		public static Tag GlassCaseAttachmentTag = TagManager.Create("SnowThings_GlassCase_AttachmentSlot",Strings.Get("STRINGS.MISC.TAGS.SNOWTHINGS_GLASSCASE_ATTACHMENTSLOT"));
		public static Tag TreeAttachmentTag = TagManager.Create("SnowThings_PineTree_AttachmentSlot",Strings.Get("STRINGS.MISC.TAGS.SNOWTHINGS_PINETREE_ATTACHMENTSLOT"));
		public static class Sounds
		{
			public const string GLASS_SHATTER = "SnowSculptures_ShatterGlass";
			public const string CUICA_DRUM = "SnowSculptures_CuicaDrum";
		}
		public class Prefabs
		{
			public static GameObject snowParticlesPrefab;
			public static GameObject snowmachineSidescreenPrefab;
		}
		public static void LoadAssets()
		{
			var bundle = AssetUtils.LoadAssetBundle("snowsculptures_assets");

			var emitterGo = bundle.LoadAsset<GameObject>("Assets/prefabs/SnowEmitter.prefab");
			Prefabs.snowParticlesPrefab = emitterGo.transform.Find("Particle System").gameObject;
			Prefabs.snowParticlesPrefab.SetLayerRecursively(Game.PickupableLayer);
			Prefabs.snowParticlesPrefab.SetActive(false);

			var material = new Material(Shader.Find("UI/Default"))
			{
				renderQueue = RenderQueues.Liquid, // Sparkle Streaker particles also render here
				mainTexture = bundle.LoadAsset<Texture2D>("Assets/Images/snow_particles 1.png")
			};

			Prefabs.snowParticlesPrefab.GetComponent<ParticleSystemRenderer>().material = material;

			Prefabs.snowmachineSidescreenPrefab = bundle.LoadAsset<GameObject>("Assets/UIs/SnowmachineSidescreen.prefab");

			var tmpConverter = new TMPConverter();
			tmpConverter.ReplaceAllText(Prefabs.snowmachineSidescreenPrefab);

			LoadSounds();
		}

		private static void LoadSounds()
		{

			SoundUtils.LoadSound(Sounds.GLASS_SHATTER, "452667__kyles__window-break-with-axe-glass-shatter-in-trailer.wav");
			SoundUtils.LoadSound(Sounds.CUICA_DRUM, "mus_drumcuica2.wav");
		}
	}
}
