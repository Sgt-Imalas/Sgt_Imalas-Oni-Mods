using Imalas_TwitchChaosEvents.Elements;
using KSerialization;
using UnityEngine;

namespace Imalas_TwitchChaosEvents.Creeper
{
	internal class CreeperConverter : KMonoBehaviour, ISim33ms, ISidescreenButtonControl
	{

		[Serialize]
		bool succing = false;


		[Serialize]
		float totalMassAbsorbed = 0;

		[MyCmpGet]
		KBatchedAnimController kbac;

		int radius = 0;
		int maxRadius = 10;

		float minMultiplier = 1;
		float maxMultiplier = 5;
		float currentMultiplier = 1;

		public static CellElementEvent CreepConverter = new("ITCE_CreepConverter", "converting creeper", false);

		public string SidescreenButtonText => "Start the SUCC";

		public string SidescreenButtonTooltip => "Start removing that pesky Creeper";

		public void Sim33ms(float dt)
		{
			Size(dt);
			SUCC();
		}
		public void Size(float dt)
		{
			if (currentMultiplier < maxMultiplier && succing)
			{
				currentMultiplier += dt * 1f;
				kbac.animScale = currentMultiplier * 0.005f;
			}
			else
				if (currentMultiplier > minMultiplier && !succing)
			{
				currentMultiplier -= dt * 1f;
				kbac.animScale = currentMultiplier * 0.005f;
			}
		}


		public void StartSucking()
		{
			kbac.Play("inhale_pre");
			kbac.Queue("inhale_loop", KAnim.PlayMode.Loop);
			GameScheduler.Instance.Schedule("startSucing", 3, (s) => succing = true);
		}

		void FixedUpdate()
		{
			if (!succing) { return; }

			var updatedPosition = Camera.main.ScreenToWorldPoint(KInputManager.GetMousePos());
			updatedPosition.z = Grid.GetLayerZ(Grid.SceneLayer.Creatures);
			updatedPosition.y += 50;
			transform.SetPosition(updatedPosition);
		}

		public override void OnCleanUp()
		{
			var cell = Grid.PosToCell(this);
			SimMessages.ReplaceAndDisplaceElement(
					   Grid.PosToCell(this),
					   ModElements.Creeper.SimHash,
					   CreepConverter,
					   totalMassAbsorbed,
					   300,
					   Grid.DiseaseIdx[cell],
					   Grid.DiseaseCount[cell]);
			base.OnCleanUp();
		}

		public void SUCC()
		{
			if (!succing) { return; }
			foreach (var cellPos in ProcGen.Util.GetCircle(transform.position, radius))
			{
				var cell = Grid.PosToCell(cellPos);
				var mass = Grid.Mass[cell];
				totalMassAbsorbed += mass;
				if (Grid.Element[cell].id == ModElements.Creeper.SimHash)
				{
					SimMessages.ReplaceElement(
						cell,
						SimHashes.Vacuum,
						CreepConverter,
						mass,
						Grid.Temperature[cell],
						Grid.DiseaseIdx[cell],
						Grid.DiseaseCount[cell]);
				}
				else if (Grid.Element[cell].id == ModElements.CreeperGas.SimHash)
				{
					SimMessages.ReplaceElement(
						cell,
						SimHashes.Vacuum,
						CreepConverter,
						mass,
						Grid.Temperature[cell],
						Grid.DiseaseIdx[cell],
						Grid.DiseaseCount[cell]);
				}
			}


			radius++;
			radius %= maxRadius;

		}

		public void SetButtonTextOverride(ButtonMenuTextOverride textOverride)
		{

		}

		public bool SidescreenEnabled() => !succing;

		public bool SidescreenButtonInteractable() => !succing;
		public void OnSidescreenButtonPressed()
		=>
			StartSucking();


		public int HorizontalGroupID() => -1;

		public int ButtonSideScreenSortOrder()
		{
			return 21;
		}
	}
}
