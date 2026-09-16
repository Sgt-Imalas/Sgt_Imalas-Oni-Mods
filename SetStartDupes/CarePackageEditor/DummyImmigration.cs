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
