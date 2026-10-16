using AkisDecorPackB.Content.Defs.Buildings;
using AkisDecorPackB.Content.ModDb;
using KSerialization;
using UnityEngine;
using UtilLibs;

namespace AkisDecorPackB.Content.Scripts.BigFossil
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class BigFossil : KMonoBehaviour, IRenderEveryTick
	{
		[SerializeField] public bool alwaysUpdate;

		[MyCmpReq] private BigFossilCablesRenderer renderer;
		[MyCmpReq] private AnchorMonitor anchorMonitor;
		[MyCmpGet] private Operational operational;

		private static int materialIndex = -1;

		public static bool isActivePreviewHangable;

		public int lastPosition;

		// TODO: material change

		public override void OnSpawn()
		{
			if (materialIndex == -1)
			{
				var def = Assets.GetBuildingDef(GiantFossilDisplayConfig.ID);

				for (int i = 0; i < def.MaterialCategory.Length; i++)
				{
					if ((Tag)def.MaterialCategory[i] == ModAssets.Tags.BuildingFossilNodule)
						materialIndex = i;
				}

				if (materialIndex == -1)
					SgtLogger.warning("Giant Fossil does not use Fossil fragments for material. Did another mod change the construction recipe? Will not function as expected like this, will spam Livayatans instead to prevent crash.");
			}

			var fragment = GetFragment();
			var fossilVariant = Mod_Db.BigFossils.resources.Find(f => f.requiredItemId == fragment);

			if (TryGetComponent(out BuildingPreview _))
				isActivePreviewHangable = fossilVariant.hangable;

			SetVariant(fossilVariant);

			anchorMonitor.Recheck();
			renderer.UpdateAllCableLengths();
		}

		private void OnVariantSet(BigFossilVariant fossilVariant)
		{
			anchorMonitor.SetGrounding(fossilVariant.hangable ? AnchorMonitor.Grounding.Hanging : AnchorMonitor.Grounding.Floor);

			if (!alwaysUpdate)
				SimAndRenderScheduler.instance.renderEveryTick.Remove(this);

			UpdateCables();
		}

		private Tag GetFragment()
		{
			if (materialIndex == -1)
				return Tag.Invalid;

			if (TryGetComponent(out Deconstructable deconstructable))
				return deconstructable.constructionElements.Length < materialIndex + 1
					? Tag.Invalid
					: deconstructable.constructionElements[materialIndex];

			if (TryGetComponent(out Constructable constructable))
				return constructable.SelectedElementsTags.Count < materialIndex + 1
					? Tag.Invalid
					: constructable.SelectedElementsTags[materialIndex];

			if (TryGetComponent(out BuildingPreview _)
				&& BuildTool.Instance.selectedElements.Count > materialIndex
				&& BuildTool.Instance.def?.PrefabID == GiantFossilDisplayConfig.ID)
				return BuildTool.Instance.selectedElements[materialIndex];

			return "livayatan";
		}

		private void SetVariant(BigFossilVariant variant)
		{
			var kbac = GetComponent<KBatchedAnimController>();
			kbac.SwapAnims([Assets.GetAnim(variant.animFile)]);
			kbac.Play(TryGetComponent(out BuildingComplete _) ? "idle" : "place");

			renderer.ToggleCables(variant.hangable);

			OnVariantSet(variant);
		}

		public void RenderEveryTick(float dt)
		{
			var currentPosition = Grid.PosToCell(this);
			if (currentPosition != lastPosition)
			{
				UpdateCables();
				lastPosition = currentPosition;
			}
		}

		private void UpdateCables()
		{
			anchorMonitor.Recheck();
			renderer.UpdateAllCableLengths();
		}
	}
}
