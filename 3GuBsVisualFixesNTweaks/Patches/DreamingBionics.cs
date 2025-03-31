//using HarmonyLib;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UtilLibs;

//namespace _3GuBsVisualFixesNTweaks.Patches
//{
//	class DreamingBionics
//	{
//		private static readonly ModHashes BionicStartMeditation = new("SweetDreams_BionicStartMeditation");
//		private static readonly ModHashes BionicStopMeditation = new("SweetDreams_BionicStopMeditation");

//		//mirroring the sleeping method without the bionic-irrelevant stuff
//		public static void BionicStartedMeditating(ClinicDreamable pajamas, object bionicChore) => HandleMeditationState(pajamas, bionicChore, true);

//		public static void BionicStoppedMeditating(ClinicDreamable pajamas, object bionicChore) => HandleMeditationState(pajamas, bionicChore, false);

//		/// <summary>
//		/// Caching Pajamas to avoid expensive getcomponent calls each working tick
//		/// </summary>
//		static Dictionary<BionicBedTimeModeChore.Instance, ClinicDreamable> CachedPajamas = new();

//		static void HandleMeditationState(ClinicDreamable pajamas, object bionicChore, bool isMeditating)
//		{
//			if (bionicChore is not BionicBedTimeModeChore.Instance smi)
//			{
//				Debug.Log("invalid smi type for bionic bed chore: " + bionicChore);
//				return;
//			}

//			var bionicWorker = smi.sm.bionic.Get<WorkerBase>(smi);

//			if (isMeditating) //startwork
//			{
//				Debug.Log(bionicWorker.name + " started dreaming");
//				pajamas.StartWork(bionicWorker);

//				//responsible for those dreamer thought bubbles,needs a Dreamer state instance on the bionic, doesnt exist by default; can be added in theory to BionicMinionConfig.RATIONAL_AI_STATE_MACHINES
//				//pajamas.StartDreamingThought(); 

//				//pajamas.progressBar.Retarget(bionicWorker.gameObject); //used by microchip production
//				pajamas.selectable.AddStatusItem(Db.Get().DuplicantStatusItems.MegaBrainTank_Pajamas_Sleeping, (object)pajamas);
//				CachedPajamas.Add(smi, pajamas);
//			}
//			else //stopwork
//			{
//				Debug.Log(bionicWorker.name + " stopped dreaming");
//				//pajamas.StopDreamingThought();
//				pajamas.DreamIsDisturbed = false;
//				if (bionicWorker != null)
//					pajamas.StopWork(bionicWorker, false);
//				pajamas.selectable.RemoveStatusItem(Db.Get().DuplicantStatusItems.MegaBrainTank_Pajamas_Sleeping);

//				CachedPajamas.Remove(smi);
//			}
//		}

//		[HarmonyPatch(typeof(ClinicDreamable), nameof(ClinicDreamable.OnEquipPajamas))]
//		public class ClinicDreamable_OnEquipPajamas_Patch
//		{
//			public static void Postfix(ClinicDreamable __instance)
//			{
//				__instance.dreamer?.Subscribe(BionicStartMeditation, (bionicMeditationChoreSmi) => BionicStartedMeditating(__instance, bionicMeditationChoreSmi));
//				__instance.dreamer?.Subscribe(BionicStopMeditation, (bionicMeditationChoreSmi) => BionicStoppedMeditating(__instance, bionicMeditationChoreSmi));
//			}
//		}

//		[HarmonyPatch(typeof(ClinicDreamable), nameof(ClinicDreamable.OnUnequipPajamas))]
//		public class ClinicDreamable_OnUnequipPajamas_Patch
//		{
//			public static void Postfix(ClinicDreamable __instance)
//			{
//				__instance.dreamer?.Unsubscribe(BionicStartMeditation, (bionicMeditationChoreSmi) => BionicStartedMeditating(__instance, bionicMeditationChoreSmi));
//				__instance.dreamer?.Unsubscribe(BionicStopMeditation, (bionicMeditationChoreSmi) => BionicStoppedMeditating(__instance, bionicMeditationChoreSmi));
//			}
//		}

//		[HarmonyPatch(typeof(BionicBedTimeModeChore.States), nameof(BionicBedTimeModeChore.States.InitializeStates))]
//		public class BionicBedTimeModeChore_States_InitializeStates_Patch
//		{
//			public static void Postfix(BionicBedTimeModeChore.States __instance)
//			{
//				__instance.defragmentingOnAssignable
//					.Enter(smi => smi.master.Trigger(BionicStartMeditation, smi))
//					.Update(UpdateDreamChore, UpdateRate.SIM_1000ms)
//					.Exit(smi => smi.master.Trigger(BionicStopMeditation, smi));

//				__instance.defragmentingWithoutAssignable
//					.Enter(smi => smi.master.Trigger(BionicStartMeditation, smi))
//					.Update(UpdateDreamChore, UpdateRate.SIM_1000ms)
//					.Exit(smi => smi.master.Trigger(BionicStopMeditation, smi));

//			}
//			//update pajama chore while defragmentation is active, 1x per second is sufficient imo
//			static void UpdateDreamChore(BionicBedTimeModeChore.Instance smi, float dt)
//			{
//				if (CachedPajamas.TryGetValue(smi, out var pajamas))
//				{
//					pajamas.WorkTick(pajamas.worker, dt);
//				}
//			}
//		}
//	}
//}
