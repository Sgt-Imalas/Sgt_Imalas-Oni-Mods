using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetStartDupes.DuplicityEditing.ScreenComponents
{
    internal class HeaderMain : KMonoBehaviour
    {
        public string Text;
        LocText label;
        public override void OnPrefabInit()
        {
            base.OnSpawn();
            label = transform.Find("Label").GetComponent<LocText>();
            label.SetText(Text);
        }
    }
}
