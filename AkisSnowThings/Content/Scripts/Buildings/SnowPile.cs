using AkisSnowThings.Content.Scripts.MultiTools;
using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace AkisSnowThings.Content.Scripts.Buildings
{
	public class SnowPile : Sculpture
	{
		private const int MAX_PET = 65;
		private const int DOG_CRITICAL_THRESHOLD = 15;
		public static string SNOWDOG;

		[Serialize]
		public int petCapacity;

		private bool pettable;

		[MyCmpReq]
		private KBatchedAnimController kbac;

		[MyCmpReq]
		private SnowPile sculpture;

		[MyCmpReq]
		private GlassCaseSealable sealable;

		[MyCmpReq]
		private Rotatable rotatable;

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();

			SetOffsetTable(OffsetGroups.InvertedStandardTable);
			multitoolContext = SnowBeam.CONTEXT;
			multitoolHitEffectTag = "fx_harvest_splash";
		}

		public override void OnSpawn()
		{
			base.OnSpawn();

			Subscribe((int)GameHashes.RefreshUserMenu, OnRefreshUserMenu);
			Subscribe((int)GameHashes.Rotated, OnRotated);
			Subscribe(ModAssets.Hashes.Sealed, data => PutInCase(data as GlassCase));
			Subscribe(ModAssets.Hashes.UnSealed, data => TakeOutFromCase(data as GlassCase));


			pettable = sculpture.CurrentStage == SNOWDOG;
			UpdateDog();
			UpdateBroken();
		}

		private void OnRotated(object data)
		{
			if (sealable.glassCase != null)
			{
				sealable.glassCase.UpdateAnimation(rotatable.IsRotated);
			}
		}

		public void Pet()
		{
			petCapacity++;
			UpdateDog();
			UpdateBroken(); 
			DogPetFx();
		}

		public void PutInCase(GlassCase glassCase)
		{
			if (glassCase == null)
			{
				return;
			}

			if (pettable)
			{
				kbac.Play("cased", KAnim.PlayMode.Paused);
			}

			UpdateDog();
			UpdateBroken();
		}

		public void TakeOutFromCase(GlassCase _)
		{
			if (pettable)
			{
				kbac.Play("pile", KAnim.PlayMode.Paused);
			}

			UpdateDog();
			UpdateBroken();
		}

		private void UpdateBroken()
		{
			if (sealable.glassCase != null)
			{
				sealable.glassCase.ToggleBroken(pettable && petCapacity >= DOG_CRITICAL_THRESHOLD, rotatable.IsRotated);
			}
		}

		public override void SetStage(string stage_id, bool skip_effect)
		{
			base.SetStage(stage_id, skip_effect);

			kbac.Play("pile", KAnim.PlayMode.Paused);
			if (sealable.IsCased)
			{
				PutInCase(sealable.glassCase);
			}

			UpdateDog();
			UpdateBroken();
		}
		public void DogPetFx()
		{
			pettable = sculpture.CurrentStage == SNOWDOG;

			if (!pettable)
			{
				return;
			}

			if (sealable.glassCase != null && petCapacity < DOG_CRITICAL_THRESHOLD && petCapacity >= DOG_CRITICAL_THRESHOLD - 2)
			{
				SoundUtils.PlaySound(ModAssets.Sounds.CUICA_DRUM, transform.position, KPlayerPrefs.GetFloat("Volume_SFX"));
			}

			if (SpeedControlScreen.Instance.IsPaused)
				SpeedControlScreen.Instance.Unpause(false);
			PopFXManager.Instance.SpawnFX(Assets.GetSprite("crew_state_happy"), STRINGS.UI.PETTED, null, offset: this.transform.GetPosition(), 2);
		}

		public void UpdateDog()
		{
			pettable = sculpture.CurrentStage == SNOWDOG;

			if (!pettable)
			{
				kbac.SetPositionPercent(0);
				petCapacity = 0;
				return;
			}
			kbac.SetPositionPercent((float)petCapacity / MAX_PET);
			kbac.UpdateAnim(0);
			kbac.SetDirty();
		}

		private void OnRefreshUserMenu(object obj)
		{
			if (pettable)
			{
				var button = new KIconButtonMenu.ButtonInfo("crew_state_happy", STRINGS.UI.PETTHATDAWG, Pet, is_interactable: petCapacity < MAX_PET);
				Game.Instance.userMenu.AddButton(gameObject, button);
			}
		}
	}
}
