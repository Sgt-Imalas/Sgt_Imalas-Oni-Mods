using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BuildingToken.Tokens
{
    internal class coinPainter : KMonoBehaviour
    {
        [Serialize]
        public Color32 Tint;

        public override void OnSpawn()
        {
            //if(OriginCritter!=null)
            //    FurColor = GiveFurColourForCritter(OriginCritter);
            ApplyAnimAndTint();
            base.OnSpawn();
        }
        void ApplyAnimAndTint()
        {
            if (gameObject.TryGetComponent<KBatchedAnimController>(out var animController))
            {
                animController.SetSymbolTint("object", Tint);
                animController.SetSymbolTint("ui", Tint);
            }
        }
    }
}
