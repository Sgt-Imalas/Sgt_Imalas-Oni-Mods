using System;

namespace BlueprintsV2.BlueprintData
{
	public class BlueprintSelectedMaterial : IEquatable<BlueprintSelectedMaterial>
	{
		private Tag _selectedTag, _categoryTag;
		public Tag SelectedTag => _selectedTag;
		public Tag CategoryTag => _categoryTag;

		public BlueprintSelectedMaterial(Tag selected, Tag category)
		{
			_selectedTag = selected;
			_categoryTag = category;
		}
		public bool Equals(BlueprintSelectedMaterial other)
		{
			return other?.SelectedTag == this.SelectedTag && other?.CategoryTag == this.CategoryTag;
		}
		public override bool Equals(object obj) => obj is BlueprintSelectedMaterial other && Equals(other);

		public static bool operator ==(BlueprintSelectedMaterial a, BlueprintSelectedMaterial b) => a?.CategoryTag == b?.CategoryTag && a?.SelectedTag == b?.SelectedTag;
		public static bool operator !=(BlueprintSelectedMaterial a, BlueprintSelectedMaterial b) => !(a == b);
		public override int GetHashCode()
		{
			return SelectedTag.GetHashCode() ^ CategoryTag.GetHashCode();
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
