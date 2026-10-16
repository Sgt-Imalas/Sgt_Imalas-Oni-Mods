using UnityEngine;
using UtilLibs;

namespace PoisNotIncluded.Content.Scripts
{
	internal class EntitySpawner : KMonoBehaviour
	{
		[SerializeField]
		public string EntityToSpawn;
		public override void OnSpawn()
		{
			base.OnSpawn();
			if (EntityToSpawn != null)
			{
				var entity = Assets.GetPrefab(EntityToSpawn);
				if(entity==null)
				{
					SgtLogger.error("Could not spawn entity " + EntityToSpawn + " - not found");
					return;
				}
				GameUtil.KInstantiate(entity, this.transform.position, Grid.SceneLayer.Building).SetActive(true);
				UnityEngine.Object.Destroy(this.gameObject);
			}
		}
	}
}
