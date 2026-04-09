using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UtilLibs;

namespace MassMoveTo.Content.Scripts
{
	internal class MultiFetch_CancellableMove : CancellableMove
	{
		[MyCmpGet] Prioritizable prioritizable;
		[MyCmpGet] KPrefabID kPrefabId;

		Dictionary<Ref<Movable>, MultiFetch_MovePickupableChore> MultiFetch_Chores = [];
		Dictionary<Chore, Ref<Movable>> MultiFetch_Chores_ReverseLookup = [];


		List<int> handles = [];
		public override void OnSpawn()
		{
			//we redo the logic of this here for multi chores
			//base.OnSpawn();

			if (!prioritizable.IsPrioritizable())
				prioritizable.AddRef();


			if (!HasAnyMoveChoresToInit())
			{
				Util.KDestroyGameObject(this.gameObject);
				return;
			}
			else
			{
				InitNewChores();
			}

			handles.Add(Subscribe((int)GameHashes.RefreshUserMenu, OnRefreshUserMenu));
			handles.Add(Subscribe((int)GameHashes.Cancel, OnCancel));
			kPrefabId.AddTag(GameTags.HasChores);
			int cell = Grid.PosToCell(this);
			Grid.Objects[cell, (int)ObjectLayer.MovePlacer] = this.gameObject;

		}
		public override void OnCleanUp()
		{
			//base.OnCleanUp();

			foreach (var handle in handles)
				Unsubscribe(handle);

			int cell = Grid.PosToCell(this);
			Grid.Objects[cell, 44] = null;
			Prioritizable.RemoveRef(this.gameObject);
		}

		bool HasAnyMoveChoresToInit()
		{
			ValidateMovables();
			return movables.Any();
		}		
		void InitMoveChore(Ref<Movable> movable)
		{
			if (MultiFetch_Chores.ContainsKey(movable))
			{
				//SgtLogger.error(movable.Get() + " already has a chore. This shouldn't happen.");
				return;
			}

			var chore = new MultiFetch_MovePickupableChore(this, movable.Get().gameObject, new Action<Chore>(this.MultiChore_OnChoreEnd));
			MultiFetch_Chores[movable] = chore;
			MultiFetch_Chores_ReverseLookup[chore] = movable;
		}

		internal void MultiChore_OnChoreEnd(Chore chore)
		{
			SgtLogger.l("onChoreEnd " + chore);
			if (!MultiFetch_Chores_ReverseLookup.TryGetValue(chore, out var movable))
			{
				SgtLogger.error("Couldn't find chore in reverse lookup. This shouldn't happen.");
				return;
			}
			SgtLogger.l(chore + " onChoreEnd, completed?: "+ chore.isComplete);

			MultiFetch_Chores.Remove(movable);
			MultiFetch_Chores_ReverseLookup.Remove(chore);

			if (!HasAnyMoveChoresToInit())
				OnCancel();
			else
				InitNewChores();

			//SgtLogger.l("Remaining chores: " + MultiFetch_Chores.Count+", remaining movables: "+movables.Count);
		}

		void InitNewChores()
		{
			foreach (var movable in movables)
			{
				InitMoveChore(movable);
			}
		}

		internal void MultiChore_OnCancel(Movable cancel_movable = null)
		{
			SgtLogger.l("OnCancel " + cancel_movable);
			for (int num = movables.Count - 1; num >= 0; num--)
			{
				Ref<Movable> @ref = movables[num];
				if (@ref != null)
				{
					Movable movable = @ref.Get();
					if (cancel_movable == null || movable == cancel_movable)
					{
						movable.ClearMove();
						movables.RemoveAt(num);
					}
				}
			}

			var allChores = MultiFetch_Chores.ToArray();
			bool hasActiveChore = false;
			for (int i = allChores.Length - 1; i >= 0; i--)
			{
				var kvp = allChores[i];
				if (cancel_movable == null || kvp.Key.Get() == cancel_movable)
				{
					kvp.Value.Cancel("CancelMove");
				}
				else if (kvp.Value.driver != null)
				{
					hasActiveChore = true;
				}
			}

			if (!hasActiveChore && !movables.Any())
			{
				Util.KDestroyGameObject(base.gameObject);
			}
		}

		internal void MultiChore_SetMovable(Movable movable)
		{
			if (MultiFetch_Chores.Keys.Any(movRev => movRev.Get() == movable))
				return;

			var newMovableRef = new Ref<Movable>(movable);
			movables.Add(newMovableRef);
			InitMoveChore(newMovableRef);
		}
		public void MultiChore_RemoveMovable(Movable moved)
		{
			MultiChore_OnCancel(moved);
		}

		internal void CheckIfShouldSelfDestruct()
		{
			//SgtLogger.l("Remaining chores: " + MultiFetch_Chores.Count + ", remaining movables: " + movables.Count);
			if (!HasAnyMoveChoresToInit())
			{
				Util.KDestroyGameObject(this.gameObject);
				return;
			}
		}
	}
}
