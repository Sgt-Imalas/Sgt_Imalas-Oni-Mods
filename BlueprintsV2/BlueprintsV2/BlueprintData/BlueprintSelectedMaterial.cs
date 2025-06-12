using System;
using UtilLibs;

namespace BlueprintsV2.BlueprintData
{
	public class BlueprintSelectedMaterial : IEquatable<BlueprintSelectedMaterial>
	{
		private Tag _selectedTag, _categoryTag, _buildingIdTag = null;
		public Tag SelectedTag => _selectedTag;
		public Tag CategoryTag => _categoryTag;
		public Tag BuildingIdTag => _buildingIdTag;

		public static BlueprintSelectedMaterial GetBlueprintSelectedMaterial(Tag selected, Tag category, Tag buildingId)
		{
			if(BlueprintState.AdvancedMaterialReplacement)
			{
				if (buildingId != null)
					return new BlueprintSelectedMaterial(selected, category, buildingId);
				else
					return new BlueprintSelectedMaterial(selected, category);
			}
			else
				return new BlueprintSelectedMaterial(selected, category);
		}

		private BlueprintSelectedMaterial(Tag selected, Tag category)
		{
			_selectedTag = selected;
			_categoryTag = category;
			_buildingIdTag = null;
		}
		private BlueprintSelectedMaterial(Tag selected, Tag category, Tag buildingId)
		{
			_selectedTag = selected;
			_categoryTag = category;
			_buildingIdTag = buildingId;

		}
		public bool Equals(BlueprintSelectedMaterial other)
		{
			return other?.SelectedTag == this.SelectedTag && other?.CategoryTag == this.CategoryTag && other?.BuildingIdTag == this.BuildingIdTag;
		}
		public override bool Equals(object obj) => obj is BlueprintSelectedMaterial other && Equals(other);

		public static bool operator ==(BlueprintSelectedMaterial a, BlueprintSelectedMaterial b) => a?.CategoryTag == b?.CategoryTag && a?.SelectedTag == b?.SelectedTag && a?._buildingIdTag == b?._buildingIdTag;
		public static bool operator !=(BlueprintSelectedMaterial a, BlueprintSelectedMaterial b) => !(a == b);
		public override int GetHashCode()
		{
			var val= SelectedTag.GetHashCode() ^ CategoryTag.GetHashCode() ^ BuildingIdTag.GetHashCode();
			//SgtLogger.l("mat hash for "+SelectedTag+", "+CategoryTag+", "+BuildingIdTag+": "+val);
			return val;
		}
		//public bool Equals(BlueprintSelectedMaterial x, BlueprintSelectedMaterial y)
		//{
		//    return x.SelectedTag == y.SelectedTag && x.CategoryTag == y.CategoryTag;
		//}


		//public new bool Equals(object x, object y)
		//{
		//    if(x is  BlueprintSelectedMaterial a && y is BlueprintSelectedMaterial b)
		//    {
		//        return this.Equals(a,b);
		//    }
		//    return false;
		//}

		//public int GetHashCode(object obj)
		//{
		//    if(obj is BlueprintSelectedMaterial mat)
		//        return mat.GetHashCode();
		//    else 
		//        return 0;
		//}
	}
}
