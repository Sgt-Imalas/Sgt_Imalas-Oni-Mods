using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UtilLibs;

namespace _3GuBsVisualFixesNTweaks.Scripts
{
	internal class VFNT_LumaFlies : KMonoBehaviour
	{
		GameObject FlyFX;
		KBatchedAnimController FlyKbac;

		[MyCmpReq] Operational operational;

		[SerializeField]
		public Vector3 FlyOffset = Vector3.zero;
		[SerializeField]
		public float startDelayMin = 6f;
		[SerializeField]
		public float startDelayMax = 16f;
		[SerializeField]
		public float endDelayMin = 2f;
		[SerializeField]
		public float endDelayMax = 6f;

		public Color clearWhite = new Color(1, 1, 1, 0.01f);

		int _sub = -1;
		bool _isActive = false;

		float animOpacity = 0f;

		Coroutine ActiveChange;

		public override void OnSpawn()
		{
			base.OnSpawn();
			SpawnFlys();
			_sub = Subscribe((int)GameHashes.ActiveChanged, OnActiveChanged);
			OnActiveChanged(null);
		}
		void OnActiveChanged(object _)
		{
			if (FlyKbac == null)
				return;

			if (_isActive == operational.IsActive)
				return;
			_isActive = operational.IsActive;
			SgtLogger.l("LumaflyOnActiveChanged, isActive: " + _isActive);
			if(ActiveChange != null)
			{
				StopCoroutine(ActiveChange);
			}
			ActiveChange = StartCoroutine(_isActive ? EnableFlies() : DisableFlies());
		}

		IEnumerator EnableFlies()
		{
			float fadeInTime = UnityEngine.Random.Range(startDelayMin, startDelayMax);
			SgtLogger.l("Lumafly turn on delay: " + fadeInTime);
			//float startOpacity = animOpacity;
			//float elapsedTime = 0f;
			
			yield return new WaitForSeconds(fadeInTime);

			FlyKbac.gameObject.SetActive(true);
			FlyKbac.Play("swarm_pre");
			FlyKbac.Queue("swarm_loop", KAnim.PlayMode.Loop);


			//while (elapsedTime < fadeInTime)
			//{
			//	elapsedTime += Time.deltaTime;
			//	animOpacity = Mathf.Lerp(startOpacity, 1f, elapsedTime / fadeInTime);
			//	FlyKbac.TintColour = new Color(1, 1, 1, animOpacity);
			//	yield return null;
			//}
			//FlyKbac.TintColour = Color.white;
		}
		IEnumerator DisableFlies()
		{
			float fadeOutTime = UnityEngine.Random.Range(endDelayMin, endDelayMax);
			//float startOpacity = animOpacity;
			//float elapsedTime = 0f;
			SgtLogger.l("Lumafly turn off delay: " + fadeOutTime);

			//while (elapsedTime < fadeOutTime)
			//{
			//	elapsedTime += Time.deltaTime;
			//	animOpacity = Mathf.Lerp(startOpacity, 0f, elapsedTime / fadeOutTime);
			//	FlyKbac.TintColour = new Color(1, 1, 1, animOpacity);
			//	yield return null;
			//}
			//animOpacity = 0f;
			yield return new WaitForSeconds(fadeOutTime);
			FlyKbac.onAnimComplete += OnFliesFinished();
			FlyKbac.Play("swarm_pst");
		}

		KAnimControllerBase.KAnimEvent _cached = null;
		KAnimControllerBase.KAnimEvent OnFliesFinished()
		{
			if (_cached == null)
				_cached = new KAnimControllerBase.KAnimEvent(RemoveListener);
			return _cached;
		}
		private void RemoveListener(HashedString anim)
		{
			FlyKbac.gameObject.SetActive(false);
			FlyKbac.onAnimComplete -= OnFliesFinished();
		}





		void SpawnFlys()
		{
			var pos = transform.GetPosition();
			FlyKbac = FXHelpers.CreateEffect("fly_swarm_kanim", pos + FlyOffset, transform, layer: Grid.SceneLayer.BuildingFront);
			FlyFX = FlyKbac.gameObject;
			FlyKbac.TintColour = clearWhite;
			FlyKbac.Play("swarm_loop", KAnim.PlayMode.Loop);
			FlyKbac.gameObject.SetActive(true);
		}
		public override void OnCleanUp()
		{
			base.OnCleanUp();
			if(_sub != -1)
				Unsubscribe(_sub);

			if (FlyFX != null)
				UnityEngine.Object.Destroy(FlyFX);
		}

		
	}
}
