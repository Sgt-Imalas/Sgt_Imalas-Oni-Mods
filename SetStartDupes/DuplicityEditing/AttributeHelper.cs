using Klei.AI;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetStartDupes.DuplicityEditing
{
    public static class AttributeHelper
    {
        public static List<Attribute> GetEditableAttributes()
        {

            var attributes = Db.Get().Attributes;
            var output = new List<Attribute>
            {
                attributes.Construction,
                attributes.Digging,
                attributes.Machinery,
                attributes.Athletics,
                attributes.Learning,
                attributes.Cooking,
                attributes.Caring,
                attributes.Art,
                attributes.Ranching,
                attributes.MachinerySpeed
            };
            if (DlcManager.IsExpansion1Active())
                output.Add(attributes.SpaceNavigation);
            if (ModAssets.BeachedActive)
            {
                if (attributes.TryGet("Beached_Precision")!=null)
                    output.Add(attributes.Get("Beached_Precision"));
            }

            return output;
        }
    }
}
