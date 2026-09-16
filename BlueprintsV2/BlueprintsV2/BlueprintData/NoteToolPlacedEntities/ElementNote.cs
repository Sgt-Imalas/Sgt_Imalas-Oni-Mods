using BlueprintsV2.BlueprintData;
using BlueprintsV2.BlueprintsV2.BlueprintData.NoteToolPlacedEntities;
using BlueprintsV2.BlueprintsV2.BlueprintData.OniTogether_Integration;
using KSerialization;
using UnityEngine;
using static BlueprintsV2.STRINGS.BLUEPRINTS_BLUEPRINTNOTE;

namespace BlueprintsV2.BlueprintsV2.BlueprintData.PlannedElements
{
	public class ElementNote : BlueprintNote, IMultiSliderControl
	{
		[MyCmpReq] protected Filterable filterable;
		static Material SolidMat, LiquidMat, GasMat, VacuumMat;

		static bool init = false;
		static void InitMaterials()
		{
			if (init)
				return;
			init = true;

			SolidMat = new Material(Assets.instance.mopPlacerAssets.material);
			SolidMat.mainTexture = ModAssets.Solid_Placer_Sprite.texture;
			LiquidMat = new Material(Assets.instance.mopPlacerAssets.material);
			LiquidMat.mainTexture = ModAssets.Liquid_Placer_Sprite.texture;
			GasMat = new Material(Assets.instance.mopPlacerAssets.material);
			GasMat.mainTexture = ModAssets.Gas_Placer_Sprite.texture;
			VacuumMat = new Material(Assets.instance.mopPlacerAssets.material);
			VacuumMat.mainTexture = ModAssets.Special_Placer_Sprite.texture;
		}
		public ElementNote()
		{
			this.sliderControls = [new MassController(this), new TemperatureController(this)];
		}


		[Serialize]
		public float ElementAmount, ElementTemperature;
		[Serialize]
		public SimHashes ElementId = SimHashes.Water;
		public bool IsSolid => ElementLoader.GetElement(ElementId.CreateTag())?.IsSolid ?? false;
		public bool IsLiquid => ElementLoader.GetElement(ElementId.CreateTag())?.IsLiquid ?? false;
		public bool IsGas => ElementLoader.GetElement(ElementId.CreateTag())?.IsGas ?? false;
		public bool IsVacuum => ElementLoader.GetElement(ElementId.CreateTag())?.IsVacuum ?? false;

		public override void OnSpawn()
		{
			UnityEngine.Object.Destroy(GetComponent<CopyBuildingSettings>());
			if (ElementLoader.FindElementByHash(ElementId) == null)
			{
				Destroy(this.gameObject);
				return;
			}
			filterable.SelectedTag = ElementId.CreateTag();
			filterable.onFilterChanged += OnFilterChanged;
			SetElementTint();
			base.OnSpawn();
		}
		private void OnRefreshUserMenu(object data)
		{
			Game.Instance.userMenu.AddButton(this.gameObject, new KIconButtonMenu.ButtonInfo("action_cancel", DELETE_NOTE.NAME, new System.Action(this.OnCancel), tooltipText: DELETE_NOTE.TOOLTIP));
			if (BlueprintState.InstantBuild)
				Game.Instance.userMenu.AddButton(this.gameObject, new KIconButtonMenu.ButtonInfo("", SANDBOX_SPAWN.NAME, new System.Action(this.OnSandboxSpawnTriggered), tooltipText: SANDBOX_SPAWN.TOOLTIP));
		}
		private void OnSandboxSpawnTriggered()
		{
			SimMessages.ReplaceElement(Grid.PosToCell(this), ElementId, CellEventLogger.Instance.SandBoxTool, ElementAmount, ElementTemperature);
			OnCancel();
		}

		public void SetInfo(SimHashes elementId, float Amount, float temperature = -1, bool seat = false)
		{
			ElementId = elementId;
			ElementAmount = Amount;
			SeatIndicator = seat;
			ElementTemperature = temperature > 0 ? temperature : ElementLoader.GetElement(elementId.CreateTag()).defaultValues.temperature;
		}
		public override void SetDescription()
		{
			var element = ElementLoader.GetElement(ElementId.CreateTag());
			string elementName = element.name;
			string name;
			string desc;
			if ((element.state & Element.State.Solid) == Element.State.Vacuum)
			{
				name = string.Format(STRINGS.BLUEPRINTS_BLUEPRINTNOTE.ELEMENT_NOTE_SPECIAL, elementName);
				desc = string.Format(STRINGS.BLUEPRINTS_BLUEPRINTNOTE.ELEMENT_NOTE_SPECIAL_DESC, elementName);
			}
			else
			{
				string mass = GameUtil.GetFormattedMass(ElementAmount);
				string temperature = GameUtil.GetFormattedTemperature(ElementTemperature);
				name = string.Format(STRINGS.BLUEPRINTS_BLUEPRINTNOTE.ELEMENT_NOTE, elementName, mass, temperature);
				desc = string.Format(STRINGS.BLUEPRINTS_BLUEPRINTNOTE.ELEMENT_NOTE_DESC, elementName, mass, temperature);
			}

			selectable?.SetName(name);
			this.gameObject.name = name;
			description?.description = desc;
			base.SetDescription();
		}
		void OnFilterChanged(Tag elementTag)
		{
			if (elementTag == GameTags.Void)
			{
				OnCancel();
				return;
			}
			if (elementTag == ElementId.CreateTag())
				return;
			var element = ElementLoader.GetElement(elementTag);
			if (element == null)
			{
				filterable.SelectedTag = ElementId.CreateTag();
				return;
			}
			var newId = element.id;
			ElementId = newId;
			OnChange();
			RefreshSelection();
		}
		public void OnChange()
		{
			SetElementTint();
			SetDescription();

			MP_Helpers.HandleNoteUpdate(this);
		}
		public void SetElementTint()
		{
			var element = ElementLoader.GetElement(ElementId.CreateTag());
			InitMaterials();
			bool vaccuum = false;
			switch (element.state & Element.State.Solid)
			{
				case Element.State.Gas:
					renderer?.sprite = ModAssets.Gas_Placer_Sprite;
					break;
				case Element.State.Liquid:
					renderer?.sprite = ModAssets.Liquid_Placer_Sprite;
					break;
				case Element.State.Solid:
					renderer?.sprite = ModAssets.Solid_Placer_Sprite;
					break;
				default:
				case Element.State.Vacuum:
					vaccuum = true;
					renderer?.sprite = ModAssets.Special_Placer_Sprite;
					break;
			}
			Tint = Color.white;
			if (!vaccuum)
				Tint = element.substance.colour;
			RefreshTint();
		}

		#region sliders
		protected ISliderControl[] sliderControls;
		public string SidescreenTitleKey => "STRINGS.BLUEPRINTS_BLUEPRINTNOTE.INFO_TITLE";
		bool IMultiSliderControl.SidescreenEnabled() => !IsVacuum;
		ISliderControl[] IMultiSliderControl.sliderControls => this.sliderControls;

		#endregion
		public override BlueprintNoteData GetNoteData(Vector2I? newPosition = null)
		{
			return new BlueprintNoteData()
			{
				Type = BlueprintNoteData.NoteType.Element,
				ElementId = this.ElementId,
				ElementMass = this.ElementAmount,
				ElementTemperature = this.ElementTemperature,
				Location = newPosition ?? default
			};
		}

		public static BlueprintNote Create(int cell, SimHashes elementId, float amount, float temperature = -1, bool seat = false)
		{
			var infoIndicator = Util.KInstantiate(Assets.GetPrefab(ElementNoteConfig.ID));
			Grid.Objects[cell, (int)ModAssets.BlueprintNotesLayer] = infoIndicator;
			Vector3 posCbc = Grid.CellToPosCBC(cell, MopTool.Instance.visualizerLayer);
			posCbc.z -= 0.15f;
			infoIndicator.transform.SetPosition(posCbc);
			if (infoIndicator.TryGetComponent<ElementNote>(out var info))
			{
				info.SetInfo(elementId, amount, temperature, seat);
			}
			infoIndicator.SetActive(true);
			return info;
		}
	}
}
