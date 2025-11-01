using ONITwitchLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util_TwitchIntegrationLib;

namespace Imalas_TwitchChaosEvents.Events
{
	/// <summary>
	/// spawns googly eyed dino critters;
	/// wait for new update with implementation to use easier symbol override systems
	/// </summary>
	internal class DinoSpawnEvent// : ITwitchEventBase
	{
		public string ID => "ITCE_DinoSpawn";

		public string EventGroupID => throw new NotImplementedException();

		public string EventName => throw new NotImplementedException();

		public EventWeight EventWeight => throw new NotImplementedException();

		public Action<object> EventAction => throw new NotImplementedException();

		public Func<object, bool> Condition => throw new NotImplementedException();

		public Danger EventDanger => throw new NotImplementedException();
	}
}
