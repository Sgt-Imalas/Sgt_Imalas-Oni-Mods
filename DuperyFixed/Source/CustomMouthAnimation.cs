using Dupery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace DuperyFixed.Source
{
	internal class CustomMouthAnimation : IMultiEntityConfig
	{
		
		public const string ID_Prefix = "Dupery_CustomMouthAnimation_";

		public GameObject CreatePrefab(HashedString dupeName, string anim)
		{
			string ID = GetPrefabID(dupeName);
			GameObject gameObject = EntityTemplates.CreateEntity(ID, ID, is_selectable: false);
			var animFile = Assets.GetAnim(anim);
			gameObject.AddOrGet<KBatchedAnimController>().AnimFiles = new KAnimFile[1] { animFile };
			return gameObject;
		}

		public void OnPrefabInit(GameObject go)
		{
		}

		public void OnSpawn(GameObject go)
		{
		}

		public List<GameObject> CreatePrefabs()
		{
			var list = new List<GameObject>();
			foreach (var customMouthOverride in PersonalityManager.CustomSpeechMonitorAnims)
			{
				SgtLogger.l("Creating custom mouth prefab for " + customMouthOverride.Key + ": " + customMouthOverride.Value);
				list.Add(CreatePrefab(customMouthOverride.Key, customMouthOverride.Value));
			}
			return list;
		}
		public static string GetPrefabID(HashedString dupeName)
		{
			return ID_Prefix + dupeName;
		}
	}
}

