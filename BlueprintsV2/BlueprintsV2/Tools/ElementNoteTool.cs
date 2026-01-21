//using FMOD.Studio;
//using FMODUnity;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;

//namespace BlueprintsV2.BlueprintsV2.Tools
//{
//	internal class ElementNoteTool : BrushTool
//	{
//		public static ElementNoteTool Instance;
//		protected HashSet<int> recentlyAffectedCells = new HashSet<int>();
//		private Dictionary<int, Color> recentAffectedCellColor = new Dictionary<int, Color>();
//		private EventInstance audioEvent;

//		public static void DestroyInstance()
//		{
//			ElementNoteTool.Instance = null;
//		}

//		public  override void OnPrefabInit()
//		{
//			base.OnPrefabInit();
//			ElementNoteTool.Instance = this;
//		}
//		public override string GetDragSound() => "";

//		public void Activate() => PlayerController.Instance.ActivateTool((InterfaceTool)this);
//		public override void OnActivateTool()
//		{
//			base.OnActivateTool();
//			SandboxToolParameterMenu.instance.gameObject.SetActive(true);
//			SandboxToolParameterMenu.instance.DisableParameters();
//			SandboxToolParameterMenu.instance.brushRadiusSlider.row.SetActive(true);
//			SandboxToolParameterMenu.instance.massSlider.row.SetActive(true);
//			SandboxToolParameterMenu.instance.temperatureSlider.row.SetActive(true);
//			SandboxToolParameterMenu.instance.elementSelector.row.SetActive(true);
//			SandboxToolParameterMenu.instance.diseaseSelector.row.SetActive(true);
//			SandboxToolParameterMenu.instance.diseaseCountSlider.row.SetActive(true);
//			SandboxToolParameterMenu.instance.elementSelector.onValueChanged += new Action<object>(this.OnElementChanged);
//		}
//		public override void OnDeactivateTool(InterfaceTool new_tool)
//		{
//			base.OnDeactivateTool(new_tool);
//			SandboxToolParameterMenu.instance.gameObject.SetActive(false);
//			int num = (int)this.audioEvent.release();
//		}
//		public override void PlaySound()
//		{
//			base.PlaySound();
//			Element element = ElementLoader.elements[this.settings.GetIntSetting("SandboxTools.SelectedElement")];
//			string path = GlobalAssets.GetSound("Brush_Rock");
//			switch (element.state & Element.State.Solid)
//			{
//				case Element.State.Vacuum:
//				case Element.State.Gas:
//					path = GlobalAssets.GetSound("SandboxTool_Brush_Gas");
//					break;
//				case Element.State.Liquid:
//					path = GlobalAssets.GetSound("SandboxTool_Brush_Liquid");
//					break;
//				case Element.State.Solid:
//					path = GlobalAssets.GetSound("Brush_" + element.substance.GetOreBumpSound()) ?? path;
//					break;
//				default:
//					break;
//			}
//			this.audioEvent = KFMOD.CreateInstance(path);
//			int num1 = (int)this.audioEvent.set3DAttributes(SoundListenerController.Instance.transform.GetPosition().To3DAttributes());
//			int num2 = (int)this.audioEvent.start();
//		}
//	}
//}
