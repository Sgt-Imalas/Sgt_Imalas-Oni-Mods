using HarmonyLib;
using SetStartDupes;
using System;
using System.Collections.Generic;
using UtilLibs;

namespace Beached_ModAPI
{

	[HarmonyPatch(typeof(Db), nameof(Db.Initialize))]
	static class InitBeachedAPI
	{
		public static void Postfix()
		{
			if (Beached_API.TryInitialize())
			{
				ModAssets.InitBeached();
			}
			else
				SgtLogger.l("Beached mod not found, API is resting now, gn...zzz");
		}
	}
	public static class Beached_API
	{
		public delegate List<string> GetPossibleLifegoalTraitsDelegate(string category, bool logWarning);

		/// <summary>
		/// Get all possible life goal traits. Leaving <c>category</c> to be <c>null</c> will return all trait ids.
		/// </summary>
		/// <param name="category"></param>
		/// <param name="logWarning"></param>
		/// <returns></returns>
		public static GetPossibleLifegoalTraitsDelegate GetPossibleLifegoalTraits;

		public delegate Dictionary<string, int> GetLifeGoalPointsDelegate(MinionStartingStats minionStartingStats);

		/// <summary>
		/// Get the currently rolled life goal attributes for a duplicant before printing
		/// </summary>
		/// <param name="minionStartingStats"></param>
		/// <returns>A dictionary of trait id to attribute points.</returns>
		public static GetLifeGoalPointsDelegate GetLifeGoalPoints;

		public delegate Klei.AI.Trait GetCurrentLifeGoalDelegate(MinionStartingStats minionStartingStats);

		/// <summary>
		/// Get the currently rolled life goal attributes for a duplicant before printing
		/// </summary>
		/// <param name="minionStartingStats"></param>
		/// <returns>A dictionary of trait id to attribute points.</returns>
		public static GetCurrentLifeGoalDelegate GetCurrentLifeGoal;

		public delegate bool IsUsingLifeGoalsDelegate();

		/// <summary>
		/// Returns if the user has enabled using life goals globally, or a Beached world is being played.
		/// </summary>
		public static IsUsingLifeGoalsDelegate IsUsingLifeGoals;

		public delegate void RemoveLifeGoalDelegate(MinionStartingStats minionStartingStats);

		/// <summary>
		/// Removes a life goal, clearing up all attributes and settings about it.
		/// </summary>
		public static RemoveLifeGoalDelegate RemoveLifeGoal;

		public delegate Klei.AI.Trait GetLifeGoalFromPersonalityDelegate(Personality personality);

		/// <summary>
		/// Get the expected life goal trait for a personality
		/// </summary>
		public static GetLifeGoalFromPersonalityDelegate GetLifeGoalFromPersonality;

		public delegate void ApplyLifeGoalFromPersonalityDelegate(MinionStartingStats minionStartingStats, bool force);

		/// <summary>
		/// Applies a life goal based on the personality of the duplicant
		/// </summary>
		public static ApplyLifeGoalFromPersonalityDelegate ApplyLifeGoalFromPersonality;

		public delegate void SetLifeGoalFromIdDelegate(MinionStartingStats minionStartingStats, string traitId, bool force);

		/// <summary>
		/// Set a specific life goal
		/// </summary>
		public static SetLifeGoalFromIdDelegate SetLifeGoalFromId;

		public delegate void SetLifeGoalDelegate(MinionStartingStats minionStartingStats, Klei.AI.Trait trait, bool force);

		/// <summary>
		/// Set a specific life goal
		/// </summary>
		public static SetLifeGoalDelegate SetLifeGoal;

		/// <summary>
		/// Call this at Db.Initialize. Please do not use late priority.
		/// </summary>
		/// <param name="logWarnings">Log Warnings. Recommended to set false if Beached is not a hard dependency for your mod.</param>
		/// <returns>Success of finding the Beached API</returns>
		public static bool TryInitialize(bool logWarnings = false)
		{
			var type = Type.GetType("Beached.ModAPI, Beached");

			if (type == null)
			{
				if (logWarnings)
					Debug.LogWarning("Beached ModAPI does not exist.");

				return false;
			}

			var m_GetPossibleLifegoalTraits = AccessTools.Method(type, "GetPossibleLifegoalTraits",
				new[]
				{
					typeof(string),
					typeof(bool)
				});

			if (m_GetPossibleLifegoalTraits == null)
			{
				if (logWarnings) Debug.LogWarning("GetPossibleLifegoalTraits is not a method.");
				return false;
			}

			GetPossibleLifegoalTraits = (GetPossibleLifegoalTraitsDelegate)Delegate.CreateDelegate(typeof(GetPossibleLifegoalTraitsDelegate), m_GetPossibleLifegoalTraits);

			var m_GetCurrentLifeGoal = AccessTools.Method(type, "GetCurrentLifeGoal",
				new[]
				{
					typeof(MinionStartingStats)
				});

			if (m_GetCurrentLifeGoal == null)
			{
				if (logWarnings) Debug.LogWarning("GetCurrentLifeGoal is not a method.");
				return false;
			}

			GetCurrentLifeGoal = (GetCurrentLifeGoalDelegate)Delegate.CreateDelegate(typeof(GetCurrentLifeGoalDelegate), m_GetCurrentLifeGoal);

			var m_GetLifeGoalPoints = AccessTools.Method(type, "GetLifeGoalPoints",
				new[]
				{
					typeof(MinionStartingStats)
				});

			if (m_GetLifeGoalPoints == null)
			{
				if (logWarnings) Debug.LogWarning("GetLifeGoalPoints is not a method.");
				return false;
			}

			GetLifeGoalPoints = (GetLifeGoalPointsDelegate)Delegate.CreateDelegate(typeof(GetLifeGoalPointsDelegate), m_GetLifeGoalPoints);

			var m_IsUsingLifeGoals = AccessTools.Method(type, "IsUsingLifeGoals");

			if (m_IsUsingLifeGoals == null)
			{
				if (logWarnings) Debug.LogWarning("IsUsingLifeGoals is not a method.");
				return false;
			}

			IsUsingLifeGoals = (IsUsingLifeGoalsDelegate)Delegate.CreateDelegate(typeof(IsUsingLifeGoalsDelegate), m_IsUsingLifeGoals);

			var m_RemoveLifeGoal = AccessTools.Method(type, "RemoveLifeGoal",
				new[]
				{
					typeof(MinionStartingStats)
				});

			if (m_RemoveLifeGoal == null)
			{
				if (logWarnings) Debug.LogWarning("RemoveLifeGoal is not a method.");
				return false;
			}

			RemoveLifeGoal = (RemoveLifeGoalDelegate)Delegate.CreateDelegate(typeof(RemoveLifeGoalDelegate), m_RemoveLifeGoal);

			var m_GetLifeGoalFromPersonality = AccessTools.Method(type, "GetLifeGoalFromPersonality",
				new[]
				{
					typeof(Personality)
				});

			if (m_GetLifeGoalFromPersonality == null)
			{
				if (logWarnings) Debug.LogWarning("GetLifeGoalFromPersonality is not a method.");
				return false;
			}

			GetLifeGoalFromPersonality = (GetLifeGoalFromPersonalityDelegate)Delegate.CreateDelegate(typeof(GetLifeGoalFromPersonalityDelegate), m_GetLifeGoalFromPersonality);

			var m_ApplyLifeGoalFromPersonality = AccessTools.Method(type, "ApplyLifeGoalFromPersonality",
				new[]
				{
					typeof(MinionStartingStats),
					typeof(bool)
				});

			if (m_ApplyLifeGoalFromPersonality == null)
			{
				if (logWarnings) Debug.LogWarning("ApplyLifeGoalFromPersonality is not a method.");
				return false;
			}

			ApplyLifeGoalFromPersonality = (ApplyLifeGoalFromPersonalityDelegate)Delegate.CreateDelegate(typeof(ApplyLifeGoalFromPersonalityDelegate), m_ApplyLifeGoalFromPersonality);

			var m_SetLifeGoalFromId = AccessTools.Method(type, "SetLifeGoal",
				new[]
				{
					typeof(MinionStartingStats),
					typeof(string),
					typeof(bool)
				});

			if (m_SetLifeGoalFromId == null)
			{
				if (logWarnings) Debug.LogWarning("SetLifeGoalFromId is not a method.");
				return false;
			}

			SetLifeGoalFromId = (SetLifeGoalFromIdDelegate)Delegate.CreateDelegate(typeof(SetLifeGoalFromIdDelegate), m_SetLifeGoalFromId);

			var m_SetLifeGoal = AccessTools.Method(type, "SetLifeGoal",
				new[]
				{
					typeof(MinionStartingStats),
					typeof(Klei.AI.Trait),
					typeof(bool)
				});

			if (m_SetLifeGoal == null)
			{
				if (logWarnings) Debug.LogWarning("SetLifeGoal is not a method.");
				return false;
			}

			SetLifeGoal = (SetLifeGoalDelegate)Delegate.CreateDelegate(typeof(SetLifeGoalDelegate), m_SetLifeGoal);

			return true;
		}
	}
}
