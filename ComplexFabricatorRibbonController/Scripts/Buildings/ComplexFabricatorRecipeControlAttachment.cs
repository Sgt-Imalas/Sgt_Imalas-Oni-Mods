using ComplexFabricatorRibbonController.Defs.Buildings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KSerialization;

namespace ComplexFabricatorRibbonController.Scripts.Buildings
{
	class ComplexFabricatorRecipeControlAttachment : KMonoBehaviour
	{
		[MyCmpReq] LogicPorts logicPorts;
		[MyCmpGet] KSelectable selectable;

		ComplexFabricator ParentFab;
		public bool HasParentFab => ParentFab != null;

		public static HashSet<ComplexFabricatorRecipeControlAttachment> AllAttachments = new HashSet<ComplexFabricatorRecipeControlAttachment>();
		public static void ClearCache() => AllAttachments.Clear();

		[Serialize]
		public string[] selectedRecipeIds = [null, null, null, null];
		bool[] RibbonBits = null;

		public int Cell => _cell;
		private int _cell = -1;

		public override void OnSpawn()
		{
			_cell = Grid.PosToCell(this);
			base.OnSpawn();
			TryReattach();
			AllAttachments.Add(this);
			this.Subscribe((int)GameHashes.LogicEvent, OnLogicValueChanged);

			RefreshRecipeOrders();
		}
		public override void OnCleanUp()
		{
			AllAttachments.Remove(this);
			base.OnCleanUp();
			this.Unsubscribe((int)GameHashes.LogicEvent, OnLogicValueChanged);
		}

		void SetStatusItem()
		{
			selectable.SetStatusItem(Db.Get().StatusItemCategories.Role, HasParentFab ? null : ModAssets.NotAttachedToFabricator);
		}

		void RefreshRecipeOrders()
		{
			SetStatusItem();
			if (!HasParentFab)
				return;

			for (int i = 0; i < selectedRecipeIds.Length; i++)
			{
				if (selectedRecipeIds[i] == null)
					continue;
				var recipe = ComplexRecipeManager.Get().GetRecipe(selectedRecipeIds[i]);
				if (recipe == null)
					continue;
				bool bitOn = GetBitState(i);
				ParentFab.SetRecipeQueueCount(recipe, bitOn ? ComplexFabricator.QUEUE_INFINITE : 0);
			}
		}

		public void Detatch()
		{
			ParentFab = null;
		}

		public void TryReattach()
		{
			ParentFab = null;
			var pos = Grid.PosToCell(this);
			var go = Grid.Objects[pos, ((int)ObjectLayer.Building)];
			if (go == null)
			{
				return;
			}
			if (!go.TryGetComponent<ComplexFabricator>(out var fab))
			{
				return;
			}
			ParentFab = fab;
			CleanupSelectedRecipes();
		}
		void CleanupSelectedRecipes()
		{
			if (ParentFab == null)
			{
				return;
			}
			var recipeIds = ParentFab.GetRecipes().Select(r => r.id).ToHashSet();

			if (selectedRecipeIds[0] != null && !recipeIds.Contains(selectedRecipeIds[0]))
				selectedRecipeIds[0] = null;
			if (selectedRecipeIds[1] != null && !recipeIds.Contains(selectedRecipeIds[1]))
				selectedRecipeIds[1] = null;
			if (selectedRecipeIds[2] != null && !recipeIds.Contains(selectedRecipeIds[2]))
				selectedRecipeIds[2] = null;
			if (selectedRecipeIds[3] != null && !recipeIds.Contains(selectedRecipeIds[3]))
				selectedRecipeIds[3] = null;
			RefreshRecipeOrders();
		}

		public List<ComplexRecipe> GetAvailableRecipes()
		{
			var list = new List<ComplexRecipe>();
			if (ParentFab == null)
				return list;

			foreach (var recipe in ParentFab.GetRecipes())
			{
				if (!recipe.IsRequiredTechUnlocked()
					|| recipe.RequiresAllIngredientsDiscovered && recipe.ingredients.Any(ingredient => !DiscoveredResources.Instance.IsDiscovered(ingredient.material))
					|| recipe.ingredients.All(ingredient => !DiscoveredResources.Instance.IsDiscovered(ingredient.material)))
				{
					continue;
				}

				bool recipeSelected = false;
				foreach (var attachment in AllAttachments)
				{
					if (attachment.ParentFab != ParentFab)
						continue;

					if (attachment.IsRecipeSelected(recipe))
					{
						recipeSelected = true;
						break;
					}
				}
				if (!recipeSelected)
					list.Add(recipe);
			}
			return list;
		}

		public bool IsRecipeSelected(ComplexRecipe recipe)
		{
			if (ParentFab == null)
				return false;
			if (recipe == null)
				return false;
			if (selectedRecipeIds.Contains(recipe.id))
				return true;
			return false;
		}
		private void OnLogicValueChanged(object data)
		{
			LogicValueChanged logicValueChanged = (LogicValueChanged)data;
			if (logicValueChanged.portID != ComplexFabricatorRecipeControlAttachmentConfig.PORT_ID)
				return;
			UpdateSignalBitmap();
			RefreshRecipeOrders();
		}
		public bool GetBitState(int index)
		{
			if (RibbonBits == null)
				UpdateSignalBitmap();
			return RibbonBits[index];
		}

		void UpdateSignalBitmap()
		{
			var inputBitsInt = logicPorts.GetInputValue(ComplexFabricatorRecipeControlAttachmentConfig.PORT_ID);
			BitArray inputs = new BitArray([inputBitsInt]);
			RibbonBits = new bool[inputs.Count];
			inputs.CopyTo(RibbonBits, 0);
		}

		public ComplexRecipe GetRecipe(int bit)
		{
			if (selectedRecipeIds[bit] == null)
				return null;

			return ComplexRecipeManager.Get().GetRecipe(selectedRecipeIds[bit]);
		}

		public void SetRecipe(int bit, ComplexRecipe recipe)
		{
			string recipeId = null;
			if (recipe != null)
			{
				recipeId = recipe.id;
			}
			selectedRecipeIds[bit] = recipeId;
			RefreshRecipeOrders();
		}
	}
}
