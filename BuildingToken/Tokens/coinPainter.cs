using KSerialization;
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
