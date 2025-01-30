using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BioluminescentDupes.Content.Scripts
{
	public class SelfImprovementWorkable2 : Workable
	{
		[MyCmpReq]
		private SelfImprovement book;

		private Chore chore;

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			workerStatusItem = Db.Get().DuplicantStatusItems.Equipping;

			overrideAnims = new[]
			{
				Assets.GetAnim("anim_eat_floor_kanim")
			};

			synchronizeAnims = false;
		}

		public override void OnSpawn()
		{
			SetWorkTime(10f); 
			book.OnAssign += RefreshChore;
		}

		private void CreateChore()
		{
			chore = new SelfImprovementChore(this);
		}

		public void CancelChore(string reason = "")
		{
			if (chore != null)
			{
				chore.Cancel(reason);
				Prioritizable.RemoveRef(book.gameObject);
				chore = null;
			}
		}

		private void RefreshChore(IAssignableIdentity target)
		{
			if (chore != null)
			{
				CancelChore("Shaker Reassigned");
			}

			if (target != null)
			{
				CreateChore();
			}
		}

		public override void OnCompleteWork(WorkerBase worker)
		{
			book.OnUse(worker);
			Util.KDestroyGameObject(gameObject);
		}

		public override void OnStopWork(WorkerBase worker)
		{
			//ToggleBook(worker, true);
			workTimeRemaining = GetWorkTime();
			base.OnStopWork(worker);
		}

		public override void OnStartWork(WorkerBase worker)
		{
			base.OnStartWork(worker);
			//ToggleBook(worker, false);
		}

		public void ToggleBook(WorkerBase worker, bool visible)
		{
			var book = worker.GetComponent<Storage>().FindFirst(gameObject.PrefabID());

			if (book != null)
			{
				Storage.MakeItemInvisible(book, !visible, false);
				book.GetComponent<KAnimControllerBase>().Offset = visible ? Vector3.zero : new Vector3(40000, 0);
			}
		}
	}
}
