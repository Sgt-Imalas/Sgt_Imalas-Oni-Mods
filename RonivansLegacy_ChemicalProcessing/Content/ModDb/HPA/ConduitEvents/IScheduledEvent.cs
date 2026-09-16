using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Content.ModDb.HPA.ConduitEvents
{
	/// <summary>
	/// Used to trigger events from inside of conduit updates
	/// </summary>
	interface IScheduledEvent
	{
		GameObject Target { get; set; }
		void ExecuteEventAction();
	}
}
