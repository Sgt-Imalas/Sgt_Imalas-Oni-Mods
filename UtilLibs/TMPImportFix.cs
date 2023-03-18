using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace UtilLibs
{
    internal class TMPImportFix : KMonoBehaviour
    {
        [SerializeField]
        public TextAlignmentOptions alignment;

        [MyCmpReq]
        private LocText text;

        public override void OnSpawn()
        {
            base.OnSpawn();
            text.alignment = alignment;
            Destroy(this);
        }
    }
}
