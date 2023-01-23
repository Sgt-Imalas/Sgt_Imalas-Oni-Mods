using KSerialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static AmbienceManager;

namespace CrittersShedFurOnBrush
{
    internal class FloofColourHolder:KMonoBehaviour

    {
        [Serialize]
        string CritterId;
        [Serialize]
        Color32 FurColor;

        public override void OnSpawn()
        {
            //if(OriginCritter!=null)
            //    FurColor = GiveFurColourForCritter(OriginCritter);
            ApplyAnimAndTint();
            base.OnSpawn();
        }
        public void SetCritterTagAndGORef(Tag id, GameObject _originalCritter)
        {
            CritterId = id.ToString();
            FurColor = GiveFurColourForCritter(_originalCritter);
        }
        public bool ShouldReplaceAnim() => CritterId != null ;
        void ApplyAnimAndTint()
        {
            if (gameObject.TryGetComponent<KBatchedAnimController>(out var animController) && ShouldReplaceAnim())
            {
                animController.SwapAnims(new KAnimFile[1] { Assets.GetAnim((HashedString)"object_furball_kanim") });
                animController.initialAnim = "object";
                animController.TintColour = FurColor;
                animController.Play("object");

                gameObject.AddOrGet<KSelectable>().SetName(STRINGS.ITEMS.FURBALL.NAME);
                //gameObject.AddOrGet<InfoDescription>().description = "Furball Description";
            }
        }


        ///allows overriding the colour for specific, multicoloured critters
        Color32 GiveFurColourForCritter(GameObject originalCritter)
        {
            return ModAssets.SheddableCritters[(Tag)CritterId].FloofColour;
        }
    }
}
