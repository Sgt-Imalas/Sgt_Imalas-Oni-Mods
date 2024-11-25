using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace AkisSnowThings.Content.Scripts.Buildings
{
	public class GlassCase : KMonoBehaviour
	{
		[MyCmpReq]
		private Building building;

		[MyCmpReq]
		public KBatchedAnimController kbac;

		[Serialize]
		public bool broken;

		[Serialize]
		public bool rotated;

		private const string BROKEN = "broken";
		private const string BROKEN_MIRROR = "broken_mirror";
		private const string BROKEN_PRE = "broken_pre";
		private const string BROKEN_PRE_MIRROR = "broken_pre_mirror";

		public override void OnSpawn()
		{
			base.OnSpawn();
			UpdateSealables(s => s.Seal(this));

			UpdateAnimation(rotated);
		}

		public override void OnCleanUp()
		{
			UpdateSealables(s => s.Unseal());
			base.OnCleanUp();
		}

		private void UpdateSealables(Action<GlassCaseSealable> fn)
		{
			var pooledList = ListPool<ScenePartitionerEntry, GameScenePartitioner>.Allocate();
			GameScenePartitioner.Instance.GatherEntries(building.GetExtents(), GameScenePartitioner.Instance.completeBuildings, pooledList);

			foreach (var scenePartitionerEntry in pooledList)
			{
				if (scenePartitionerEntry.obj is KMonoBehaviour kMonoBehaviour && kMonoBehaviour.TryGetComponent(out GlassCaseSealable sealable))
				{
					fn(sealable);
				}
			}
		}

		public void UpdateAnimation(bool rotated)
		{
			if (broken)
			{
				kbac.Play(rotated ? BROKEN_MIRROR : BROKEN);
			}
			else
			{
				kbac.Play("base");
			}

			this.rotated = rotated;
		}

		public void ToggleBroken(bool broken, bool rotated)
		{
			if (this.broken == broken)
			{
				return;
			}

			if (broken)
			{
				kbac.Play(rotated ? BROKEN_PRE_MIRROR : BROKEN_PRE);
				kbac.Queue(rotated ? BROKEN_MIRROR : BROKEN);

				SoundUtils.PlaySound(ModAssets.Sounds.GLASS_SHATTER, transform.position, KPlayerPrefs.GetFloat("Volume_SFX"));
			}
			else
			{
				kbac.Play("base");
			}

			this.broken = broken;
			this.rotated = rotated;
		}
	}
}

