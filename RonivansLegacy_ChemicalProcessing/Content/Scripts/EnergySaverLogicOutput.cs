using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	internal class EnergySaverLogicOutput : KMonoBehaviour, ISim200ms
	{
		public const string PORT_ID = "RonivanAIO_EnergySaverLogicOutputPort";
		[MyCmpReq] LogicPorts logicPorts;
		RefrigeratorController.StatesInstance smi;

		bool isEnergySaverMode = false;
		bool initialized = false;
		public override void OnSpawn()
		{
			base.OnSpawn();
			smi = gameObject.GetSMI<RefrigeratorController.StatesInstance>();
			if (smi == null)
			{
				SgtLogger.error("RefrigeratorControllerSMI was null!");
				return;
			}
			StartCoroutine(DelayedRefresh());
			initialized = true;
		}
		IEnumerator DelayedRefresh()
		{
			yield return null;
			RefreshLogicPort();
		}

		public void Sim200ms(float dt)
		{
			if (!initialized) return;
			RefreshLogicPort();
		}
		void RefreshLogicPort()
		{
			bool currentlyEnergySaver = smi.IsInsideState(smi.sm.operational.steady);
			//SgtLogger.l("Energy Saver Logic Output: currentlyEnergySaver = " + currentlyEnergySaver);
			if (currentlyEnergySaver != isEnergySaverMode)
			{
				isEnergySaverMode = currentlyEnergySaver;
				logicPorts.SendSignal(PORT_ID, isEnergySaverMode ? 1 : 0);
			}
		}
	}
}
