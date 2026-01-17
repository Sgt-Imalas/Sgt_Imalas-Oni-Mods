using BlueprintsV2.BlueprintData;
using Klei.AI;
using KSerialization;
using PeterHan.PLib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static BlueprintsV2.STRINGS.BLUEPRINTS_PLANNED_ELEMENT_PLACER;
using static STRINGS.UI.TOOLS;

namespace BlueprintsV2.BlueprintsV2.BlueprintData.PlannedElements
{
	public class ElementPlanInfo: KMonoBehaviour, IMultiSliderControl
	{
		public ElementPlanInfo()
		{
			this.sliderControls = [new MassController(this), new TemperatureController(this)];
		}

		public static string FILTERLAYER = ("BPV2_ELEMENTPLANINFO_FILTER");
		MeshRenderer renderer;
		[MyCmpReq] InfoDescription description;
		[MyCmpReq] KSelectable selectable;
		[MyCmpReq] Filterable filterable;

		[Serialize]
		public float ElementAmount, ElementTemperature;
		[Serialize]
		public SimHashes ElementId = SimHashes.Water;
		[Serialize]
		public bool SeatIndicator = false;
		public bool IsSolid => ElementLoader.GetElement(ElementId.CreateTag())?.IsSolid ?? false;
		public bool IsLiquid => ElementLoader.GetElement(ElementId.CreateTag())?.IsLiquid ?? false;
		public bool IsGas => ElementLoader.GetElement(ElementId.CreateTag())?.IsGas ?? false;

		public override void OnSpawn()
		{
			renderer = GetComponentInChildren<MeshRenderer>();
			base.OnSpawn();
			UnityEngine.Object.Destroy(GetComponent<CopyBuildingSettings>());
			Subscribe((int)GameHashes.RefreshUserMenu, OnRefreshUserMenu);
			Subscribe((int)GameHashes.Cancel, Cancel);

			filterable.SelectedTag = ElementId.CreateTag();
			filterable.onFilterChanged += OnFilterChanged;
			SetElementTint();
			if (SeatIndicator)
				Seat();
		}

		private void OnRefreshUserMenu(object data)
		{
			Game.Instance.userMenu.AddButton(this.gameObject, new KIconButtonMenu.ButtonInfo("action_cancel", CANCELINDICATOR.NAME, new System.Action(this.OnCancel), tooltipText: CANCELINDICATOR.TOOLTIP));
			if(BlueprintState.InstantBuild)
				Game.Instance.userMenu.AddButton(this.gameObject, new KIconButtonMenu.ButtonInfo("", SANDBOX_SPAWN.NAME, new System.Action(this.OnSandboxSpawnTriggered), tooltipText: SANDBOX_SPAWN.TOOLTIP));
		}
		private void Cancel(object _ = null) => OnCancel();
		private void OnCancel()
		{
			DetailsScreen.Instance.Show(false);
			this.DeleteObject();
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
		public void SetDescription()
		{
			var element = ElementLoader.GetElement(ElementId.CreateTag());
			string elementName = element.name;
			string mass = GameUtil.GetFormattedMass(ElementAmount);
			string temperature = GameUtil.GetFormattedTemperature(ElementTemperature);
			string name = string.Format(STRINGS.BLUEPRINTS_PLANNED_ELEMENT_PLACER.ELEMENT_INFO_NAME_FILLABLE, elementName, mass, temperature);
			string desc = string.Format(STRINGS.BLUEPRINTS_PLANNED_ELEMENT_PLACER.ELEMENT_INFO_DESC_FILLABLE, elementName, mass, temperature);

			selectable?.SetName(name);
			this.gameObject.name = name;
			description?.description = desc;
		}
		void Seat()
		{
			SetDescription();
			Grid.Objects[Grid.PosToCell(this), (int)ModAssets.PlannedElementLayer] = this.gameObject;
			//gameObject.SetLayerRecursively(LayerMask.NameToLayer("Default"));
		}
		void OnFilterChanged(Tag elementTag)
		{
			if(elementTag == GameTags.Void)
			{
				OnCancel();
				return;
			}

			var element = ElementLoader.GetElement(elementTag);
			if(element == null)
			{
				filterable.SelectedTag = ElementId.CreateTag();
				return;
			}

			ElementId = element.id;
			OnChange();
		}
		public void OnChange()
		{
			SetElementTint();
			SetDescription();
		}
		public void SetElementTint()
		{
			var element = ElementLoader.GetElement(ElementId.CreateTag());
			renderer.material.color = element.substance.colour;
		}


		#region sliders
		protected ISliderControl[] sliderControls;
		public string SidescreenTitleKey => "STRINGS.BLUEPRINTS_PLANNED_ELEMENT_PLACER.INFO_TITLE";
		bool IMultiSliderControl.SidescreenEnabled() => SeatIndicator;
		ISliderControl[] IMultiSliderControl.sliderControls => this.sliderControls;

		#endregion
	}
}
