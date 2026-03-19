using KSerialization;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Rockets_TinyYetBig.Content.Scripts.StarmapEntities
{
	internal class StationDeconstructable : KMonoBehaviour
	{
		[SerializeField]
		public bool IsDerelict = false;

		[Serialize]
		public List<StationResource> Resources = new List<StationResource>();
	}
	public struct StationResource
	{
		public StationResource () { }
		public StationResource(Tag tag, float amount) { Item = tag; Amount = amount; }
		public float Amount;
		public Tag Item;
	}
}
