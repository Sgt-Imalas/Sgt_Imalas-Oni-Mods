using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
