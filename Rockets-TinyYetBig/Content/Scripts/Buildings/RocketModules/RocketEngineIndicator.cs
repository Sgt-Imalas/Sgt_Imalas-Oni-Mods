using HarmonyLib;
using Rockets_TinyYetBig.Content.Defs.StarmapEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rockets_TinyYetBig.Content.Scripts.Buildings.RocketModules
{
	internal class RocketEngineIndicator : KMonoBehaviour
	{
		int handle = -1;
		GameObject RocketExhaustIndicator = null;
		public override void OnSpawn()
		{
			base.OnSpawn();
			handle = Game.Instance.Subscribe((int)GameHashes.SelectObject, OnSelectionChanged);
		}
		public override void OnCleanUp()
		{
			RemoveIndicatorIfActive(true);
			Game.Instance.Unsubscribe(handle);
			base.OnCleanUp();
		}
		void OnSelectionChanged(object data)
		{
			if (data == null)
				RemoveIndicatorIfActive();
			var selected = (GameObject)data;
			if (selected == gameObject)
				AdIndicatorIfNotActive();
			else
				RemoveIndicatorIfActive();
		}
		void RemoveIndicatorIfActive(bool destroy = false)
		{
			if (RocketExhaustIndicator == null)
				return;
			if (destroy)
			{
				UnityEngine.Object.Destroy(RocketExhaustIndicator);
				RocketExhaustIndicator = null;
			}
			else
				RocketExhaustIndicator.SetActive(false);
		}
		void AdIndicatorIfNotActive()
		{
			Vector3 worldPos = transform.position;
			if (RocketExhaustIndicator == null)
			{
				RocketExhaustIndicator = GameUtil.KInstantiate(Assets.GetPrefab(RocketExhaustIndicatorConfig.ID), worldPos, Grid.SceneLayer.FXFront, gameLayer: LayerMask.NameToLayer("Place"));
			}
			else
				RocketExhaustIndicator.transform.SetPosition(worldPos);
			RocketExhaustIndicator.SetActive(true);

		}
	}
}
