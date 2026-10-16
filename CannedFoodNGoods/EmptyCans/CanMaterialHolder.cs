using KSerialization;
using UnityEngine;

namespace CannedFoods.EmptyCans
{
	class CanMaterialHolder : KMonoBehaviour
	{
		[SerializeField]
		[Serialize]
		public Tag ChosenElement;
		[SerializeField]
		[Serialize]
		public bool IsFood = false;
		[MyCmpReq]
		PrimaryElement PrimaryElement;
		
		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			
		}

	}
}
