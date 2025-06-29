using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	class WorldElementDropper : KMonoBehaviour
	{
		public Storage TargetStorage;
		public bool DropSolids = false;
		public bool DropLiquids = false;
		public bool DropGases = false;

		public override void OnCleanUp()
		{
			base.OnCleanUp();
			this.Unsubscribe((int)GameHashes.OnStorageChange, OnStorageChanged);

		}
		public override void OnSpawn()
		{
			base.OnSpawn();
			this.Subscribe((int)GameHashes.OnStorageChange, OnStorageChanged);
		}
		void OnStorageChanged(object data)
		{
			var tagsInStorage = TargetStorage.GetAllIDsInStorage();

			foreach (var tag in tagsInStorage) 
			{
				var element = ElementLoader.GetElement(tag);
				if (element == null)
					continue;
				if (element.IsSolid && !DropSolids)
					tagsInStorage.Remove(tag);
				else if (element.IsLiquid && !DropLiquids)
					tagsInStorage.Remove(tag);
				else if (element.IsGas && !DropGases)
					tagsInStorage.Remove(tag);
			}
			foreach(var dropIt in tagsInStorage)
			{
				TargetStorage.DropSome(dropIt, 9999999999, true,true);
			}
		}
	}
}
