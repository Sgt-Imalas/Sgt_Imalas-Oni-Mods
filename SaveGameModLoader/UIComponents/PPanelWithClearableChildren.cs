using PeterHan.PLib.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveGameModLoader.UIComponents
{
    internal class PPanelWithClearableChildren : PPanel
    {
        public PPanelWithClearableChildren(string name) : base(name) { }
        public void ClearChildren() => base.children.Clear();
    }
}
