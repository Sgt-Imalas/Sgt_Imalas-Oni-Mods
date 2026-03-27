using KSerialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rockets_TinyYetBig.Content.Scripts.Buildings.SpaceStationConstruction
{
	internal class SpaceStationMultiPartStorage : KMonoBehaviour
	{
		[Serialize] List<StoredStationPart> StoredParts = [];
	}
}
