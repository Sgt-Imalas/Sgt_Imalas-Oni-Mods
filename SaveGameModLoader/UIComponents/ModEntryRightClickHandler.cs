using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace SaveGameModLoader.UIComponents
{
    internal class ModEntryRightClickHandler: KScreen
    {
        public override void OnKeyUp(KButtonEvent e)
        {
            if (e.TryConsume(Action.MouseRight))
            {
                SgtLogger.l("rightclicked");
            }
            base.OnKeyUp(e);
        }
    }
}
