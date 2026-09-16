using PeterHan.PLib.Core;
using System.Collections.Generic;

namespace UtilLibs
{
	public static class DuperyShared
	{
		const string MouthDataKey = "Dupery_CustomCheekAnims";
		public static void RegisterMouthSymbolForCustomCheek(string mouth, string anim)
		{
			var data = PRegistry.GetData<Dictionary<string, string>>(MouthDataKey);
			if(data == null) 
				data = new Dictionary<string, string>();

			data[mouth] = anim;
			PRegistry.PutData(MouthDataKey, data);	
		}

		public static bool HasCustomCheekAnimForMouth(string mouth, out KAnimFile kanim)
		{
			kanim = null;
			var data = PRegistry.GetData<Dictionary<string, string>>(MouthDataKey);
			if(data == null) return false;
			return (data.TryGetValue(mouth, out var animName) && Assets.TryGetAnim(animName, out kanim));
		}
	}
}
