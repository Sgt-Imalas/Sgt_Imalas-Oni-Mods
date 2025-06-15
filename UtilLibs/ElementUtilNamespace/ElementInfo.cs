using UnityEngine;
using UtilLibs;

namespace ElementUtilNamespace
{
	// Just a bunch of helper and convenience methods
	public class ElementInfo
	{
		public string id;
		public Element.State state;
		public string anim;
		public Color color;
		public Color uiColor;
		public Color conduitColor;
		public bool isInitialized;

		public SimHashes SimHash { get; private set; }

		public Tag CreateTag() => Tag;
		public Tag Tag { get; private set; }

		public ElementInfo(string id, string anim, Element.State state, Color color)
		{
			this.id = id;
			this.anim = anim;
			this.state = state;
			this.color = color;

			SimHash = SgtElementUtil.RegisterSimHash(id);
			SgtElementUtil.elements.Add(this);

			Tag = id;
			//SgtLogger.l("The elementinfo should have this simhash: "+(SimHashes)SimHash + ", ToString returns: "+SimHash.ToString());
		}

		// be able to reference this class without havng to cast to (SimHashes)
		public static implicit operator SimHashes(ElementInfo info)
		{
			return info.SimHash;
		}

		// GetElement(Tag) is the fastest way to fetch an element, but i can't remember that so here is a shortcut for it
		public Element Get()
		{
			if (ElementLoader.elementTagTable == null)
			{
				SgtLogger.dlogwarn("Trying to fetch element too early, elements are not loaded yet.");
				return null;
			}

			return ElementLoader.GetElement(Tag);
		}

		/// <summary>
		/// creates a solid tile material thats a tinted clone of an existing texture
		/// </summary>
		/// <param name="originalElement"></param>
		/// <param name="color"></param>
		/// <returns></returns>
		public Material CreateTintedMaterialCopy(SimHashes originalElement)
		{
			var baseSubstance = Assets.instance.substanceTable.GetSubstance(originalElement);
			if(baseSubstance == null)
			{
				SgtLogger.error("No substance found for " + originalElement);
				return null;
			}
			var material = new Material(baseSubstance.material);

			Texture2D newTexture = TintTextureWithColor(material.mainTexture, id, color);
			material.mainTexture = newTexture;
			material.name = "mat"+id;

			//var spec = material.GetTexture("_ShineMask");
			//if(spec != null && spec is Texture2D specTex)
			//{
			//	var clone = GetReadableCopy(specTex);
			//	material.SetTexture("_ShineMask", clone);
			//}
			//var normal = material.GetTexture("_NormalNoise");
			//if (normal != null && normal is Texture2D normalTex)
			//{
			//	var clone = GetReadableCopy(normalTex);
			//	material.SetTexture("_NormalNoise", clone);
			//}
			return material;
		}

		/// <summary>
		/// tints an existing texture with a color
		/// </summary>
		/// <param name="sourceTexture"></param>
		/// <param name="name"></param>
		/// <param name="tint"></param>
		/// <returns></returns>
		static Texture2D TintTextureWithColor(Texture sourceTexture, string name, Color tint)
		{
			Texture2D newTexture = GetReadableCopy(sourceTexture as Texture2D);
			var pixels = newTexture.GetPixels32();
			for (int i = 0; i < pixels.Length; ++i)
			{
				var gray = ((Color)pixels[i]).grayscale * 1.5f;
				pixels[i] = tint * gray;
			}
			newTexture.SetPixels32(pixels);
			newTexture.Apply();
			newTexture.name = name;
			return newTexture;
		}

		/// <summary>
		/// Returns a readable copy of the texture, so it can be modified.
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static Texture2D GetReadableCopy(Texture2D source)
		{
			if (source == null || source.width == 0 || source.height == 0) return null;

			RenderTexture renderTex = RenderTexture.GetTemporary(
						source.width,
						source.height,
						0,
						RenderTextureFormat.Default,
						RenderTextureReadWrite.Linear);

			Graphics.Blit(source, renderTex);
			RenderTexture previous = RenderTexture.active;
			RenderTexture.active = renderTex;
			Texture2D readableText = new Texture2D(source.width, source.height);


			readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
			readableText.Apply();
			RenderTexture.active = previous;
			RenderTexture.ReleaseTemporary(renderTex);
			return readableText;
		}

		public Substance CreateSubstanceFromElementTinted(SimHashes clonedMaterial) => CreateSubstance(false, CreateTintedMaterialCopy(clonedMaterial), null, null, null, null, clonedMaterial);

		public Substance CreateSubstance(bool specular = false, Material material = null, Color? uiColor = null, Color? conduitColor = null, Color? specularColor = null, string normal = null,SimHashes cloneMaterialOrigin = SimHashes.Void)
		{
			if (material == null)
			{
				material = state == Element.State.Solid ? Assets.instance.substanceTable.solidMaterial : Assets.instance.substanceTable.liquidMaterial;
				if(cloneMaterialOrigin != SimHashes.Void && state == Element.State.Solid)
				{
					material = CreateTintedMaterialCopy(cloneMaterialOrigin);
				}
			}

			isInitialized = true;

			return SgtElementUtil.CreateSubstance(SimHash, specular, anim, state, color, material, uiColor ?? color, conduitColor ?? color, specularColor, normal);
		}

		public Substance CreateSubstance(Color uiColor, Color conduitColor)
		{
			return CreateSubstance(false, null, uiColor, conduitColor);
		}

		public static ElementInfo Solid(string id, Color color)
		{
			return new ElementInfo(id, id.ToLowerInvariant() + "_kanim", Element.State.Solid, color);
		}
		public static ElementInfo Solid(string id, string anim, Color color)
		{
			return new ElementInfo(id, anim, Element.State.Solid, color);
		}

		public static ElementInfo Liquid(string id, Color color)
		{
			return new ElementInfo(id, "liquid_tank_kanim", Element.State.Liquid, color);
		}

		public static ElementInfo Gas(string id, Color color)
		{
			return new ElementInfo(id, "gas_tank_kanim", Element.State.Gas, color);
		}

		public override string ToString()
		{
			return SimHash.ToString();
		}
	}
}
