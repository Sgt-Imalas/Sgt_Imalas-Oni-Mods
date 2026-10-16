using KSerialization;
using System.Collections.Generic;

namespace Rockets_TinyYetBig.Content.Scripts.Buildings.SpaceStationConstruction
{
	internal class SpaceStationMultiPartStorage : KMonoBehaviour
	{
		[Serialize] List<StoredStationPart> StoredParts = [];
	}
}
