using System;
using System.Collections.Generic;
using System.Text;

namespace Rockets_TinyYetBig.Content.ModDb.BuildingConfigurations.BuildingConfigInterfaces
{
	internal interface IHasOtherConfigurables
	{
		List<string> OtherConfigLabels();
	}
}
