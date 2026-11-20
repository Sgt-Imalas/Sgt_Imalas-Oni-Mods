using Rockets_TinyYetBig.Content.Scripts.Buildings;
using Rockets_TinyYetBig.Content.Scripts.Buildings.RocketModules;
using Rockets_TinyYetBig.NonRocketBuildings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rockets_TinyYetBig.Content.ModDb
{
	internal static class ModComps
	{
		public static Components.Cmps<FridgeModuleHatchGrabber> FridgeModuleGrabbers = [];
		//public static Components.Cmps<LifeSupportInteriorBuilding> InteriorLifeSupports = [];
		public static Components.Cmps<RocketAttachableSocket> AttachableRocketSockets = new();
	}
}
