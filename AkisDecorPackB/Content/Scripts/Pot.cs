using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkisDecorPackB.Content.Scripts
{
	public class Pot : Sculpture
	{
		[MyCmpReq] private Storage storage;
		[MyCmpReq] private KSelectable kSelectable;
		[MyCmpReq] private TreeFilterable treeFilterable;

		public const string DEFAULT = "Default";

		public bool ShouldShowSettings => CurrentStage != DEFAULT;

		private static readonly HashSet<Tag> placeHolderSet = [Tag.Invalid];

		public override void OnSpawn()
		{
			base.OnSpawn();
			UpdateStorage(CurrentStage);
			// adding an empty list prevents the weird popup
		}

		private void UpdateStorage(string stage)
		{
			if (stage == DEFAULT)
			{
				storage.DropAll();
				storage.capacityKg = 0;
				treeFilterable.showUserMenu = false;

				if (treeFilterable.AcceptedTags == null || treeFilterable.AcceptedTags.Count == 0)
					treeFilterable.UpdateFilters(placeHolderSet);
			}
			else
			{
				storage.capacityKg = Config.Instance.PotCapacity;
				treeFilterable.showUserMenu = true;

				if (treeFilterable.AcceptedTags != null && treeFilterable.AcceptedTags.SetEquals(placeHolderSet))
					treeFilterable.UpdateFilters([]);
			}

			treeFilterable.RefreshTint();

			// refreshes fetch chores
			storage.Trigger((int)GameHashes.OnlyFetchMarkedItemsSettingChanged);

			// refresh menu
			if (kSelectable.IsSelected)
			{
				DetailsScreen.Instance.Refresh(gameObject);
				Game.Instance.userMenu.Refresh(gameObject);
			}
		}

		public override void SetStage(string stage_id, bool skip_effect)
		{
			UpdateStorage(stage_id);
			base.SetStage(stage_id, skip_effect);
		}

		public void SetRandomStage()
		{
			var potentialStages = global::Db.GetArtableStages().GetPrefabStages(this.PrefabID());

			potentialStages.RemoveAll(stage => stage.statusItem.StatusType != Database.ArtableStatuses.ArtableStatusType.LookingGreat);
			var selectedStage = potentialStages.GetRandom();

			SetStage(selectedStage.id, false);
		}
	}
}
