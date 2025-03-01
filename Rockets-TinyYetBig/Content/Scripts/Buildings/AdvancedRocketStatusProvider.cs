using KSerialization;
using System.Collections;
using static ProcessCondition;

namespace Rockets_TinyYetBig.NonRocketBuildings
{
	public class AdvancedRocketStatusProvider : KMonoBehaviour, ISim1000ms
	{
		[MyCmpGet] LaunchPad launchPad;
		[MyCmpGet] LogicPorts logicPorts;
		public HashedString rocketStateOutput;
		public HashedString ignoreWarningInput;
		const int RocketPath = 1, RocketConstruction = 2, RocketStorage = 4, RocketCrew = 8;

		[Serialize]
		int LaunchSignalValue = 0;
		[Serialize]
		bool[] LaunchSignalBits = null;


		public override void OnSpawn()
		{
			base.OnSpawn();

			this.Subscribe((int)GameHashes.LogicEvent, OnLogicValueChanged);
		}

		public override void OnCleanUp()
		{
			base.OnCleanUp();
			this.Unsubscribe((int)GameHashes.LogicEvent, OnLogicValueChanged);

		}

		private void OnLogicValueChanged(object data)
		{
			LogicValueChanged logicValueChanged = (LogicValueChanged)data;
			if (logicValueChanged.portID != ignoreWarningInput)
				return;
			this.UpdateSignalBitmap();
		}

		public void Sim1000ms(float dt)
		{
			RocketModuleCluster landedRocket = launchPad.LandedRocket;
			if (landedRocket != null)
			{
				int status = GetRocketProcessCondition(landedRocket.CraftInterface);
				logicPorts.SendSignal(this.rocketStateOutput, status);
			}
			else
			{
				logicPorts.SendSignal(this.rocketStateOutput, 0);
			}
		}

		public bool GetBitMaskValAtIndex(int index)
		{
			if (LaunchSignalBits == null)
				UpdateSignalBitmap();
			return LaunchSignalBits[index];
		}

		public void ConvertWarnings(ProcessConditionType processConditionType, ref ProcessCondition.Status status)
		{
			if (LaunchSignalBits == null)
				UpdateSignalBitmap();

			if (status == Status.Warning
				&& LaunchSignalBits[0]
				&& processConditionType == ProcessConditionType.RocketStorage
				&& LaunchSignalBits[1]
				)
			{
				status = Status.Ready;
			}
		}
		void UpdateSignalBitmap()
		{
			var inputBitsInt = logicPorts.GetInputValue(ignoreWarningInput);
			BitArray inputs = new BitArray(new int[] { inputBitsInt });
			LaunchSignalBits = new bool[inputs.Count];
			inputs.CopyTo(LaunchSignalBits, 0);
		}

		int GetRocketProcessCondition(CraftModuleInterface rocket)
		{
			int value = 0;
			if (rocket.EvaluateConditionSet(ProcessConditionType.RocketPrep) == ProcessCondition.Status.Ready)
				value += RocketConstruction;
			if (rocket.EvaluateConditionSet(ProcessConditionType.RocketStorage) == ProcessCondition.Status.Ready)
				value += RocketStorage;
			if (rocket.EvaluateConditionSet(ProcessConditionType.RocketBoard) == ProcessCondition.Status.Ready)
				value += RocketCrew;
			if (rocket.EvaluateConditionSet(ProcessConditionType.RocketFlight) == ProcessCondition.Status.Ready)
				value += RocketPath;
			return value;
		}
	}
}
