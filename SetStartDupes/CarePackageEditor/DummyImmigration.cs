using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace SetStartDupes.CarePackageEditor
{
	class DummyImmigration : Immigration
	{
		public override void OnPrefabInit()
		{
			SgtLogger.l("Creating DummyInstance of immigration");
		}
	}
}
