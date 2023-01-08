using KSerialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AmbienceManager;

namespace CrittersShedFurOnBrush
{
    internal class FloofColourHolder:KMonoBehaviour

    {
        [Serialize]
        string CritterId;

        protected override void OnSpawn()
        {
            ApplyStyleChanges();
                //this.StartCoroutine(this.DelayedGeneration());
            base.OnSpawn();
        }
        public void SetCritterTag(Tag id)
        {
            CritterId = id.ToString(); 
            ApplyStyleChanges();
        }
        public bool ShouldReplaceAnim() => CritterId != null;
        public void ApplyStyleChanges()
        {
            if(gameObject.TryGetComponent<KBatchedAnimController>(out var animController)&& ShouldReplaceAnim())
            {
                animController.SwapAnims ( new KAnimFile[1] { Assets.GetAnim((HashedString)"object_furball_kanim") });
                animController.initialAnim = "object";
                animController.TintColour = ModAssets.SheddableCritters[(Tag)CritterId].FloofColour;
                animController.Play("object");
                Debug.LogWarning(animController.AnimFiles+ "AAAAAAA "+ animController.AnimFiles.Count());
            }
        }
        private IEnumerator DelayedGeneration()
        {
            yield return (object)SequenceUtil.WaitForEndOfFrame;
            this.ApplyStyleChanges();
        }
    }
}
