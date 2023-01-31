using Klei.AI;
using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rockets_TinyYetBig.TwitchEvents.SpaceSpice
{
    internal class SpiceEyes : KMonoBehaviour, ISim1000ms
    {
        [MyCmpReq]
        private MinionIdentity identity;
        [MyCmpGet]
        private Accessorizer accessorizer;
        [MyCmpGet]
        Effects effects;
        [MyCmpReq]
        private KBatchedAnimController kbac;

        [Serialize]
        public string currentEyes;

        [Serialize]
        public string originalEyes;

        [Serialize]
        public float SpiceEyesDuration = -100;
        public override void OnSpawn()
        {
            base.OnSpawn();

            originalEyes = Db.Get().Accessories.Get(accessorizer.bodyData.eyes).Id;
            Debug.Log("ORIGINAL EYE SYMBOL IS " + originalEyes);

            OnLoadGame();

            if (SpiceEyesDuration>0)
            {
                Apply(currentEyes);
            }
        }
        public void Remove()
        {
            if (accessorizer != null)
            {
                ReplaceAccessory(originalEyes);
                currentEyes = null;
            }

            SpiceEyesDuration = -100f;
        }

        public void OnLoadGame()
        {
            if (SpiceEyesDuration>0)
            {
                ChangeAccessorySlot(currentEyes);
            }
        }
        public void Apply(string accessory)
        {
            if (accessorizer != null)
            {
                ReplaceAccessory(accessory);
                currentEyes = accessory;
            }
        }

        public void OnSaveGame()
        {
            if (SpiceEyesDuration>0)
            {
                ChangeAccessorySlot(originalEyes);
            }
        }

        private void ReplaceAccessory(string accessory)
        {
            var eyeSlot = Db.Get().AccessorySlots.Eyes;
            var newAccessory = eyeSlot.Lookup(accessory);
            var currentAccessory = accessorizer.GetAccessory(eyeSlot);
            Debug.Log($"replacing accessory from {currentAccessory.Id} to {accessory}");

            if (newAccessory == null)
            {
                Debug.LogWarning($"Could not add accessory {accessory}, it was not found in the database.");
                return;
            }

            accessorizer.RemoveAccessory(currentAccessory);
            accessorizer.AddAccessory(newAccessory);
            accessorizer.ApplyAccessories();
        }

        private void ChangeAccessorySlot(HashedString value)
        {
            if (!value.IsValid)
            {
                Debug.Log("not valid value");
                return;
            }

            Debug.Log("Changing accessory slot to " + HashCache.Get().Get(value));

            var bodyData = accessorizer.bodyData;
            bodyData.mouth = value;

            var items = accessorizer.accessories;
            var slot = Db.Get().AccessorySlots.Mouth;
            var accessories = Db.Get().Accessories;

            for (var i = 0; i < items.Count; i++)
            {
                var item = items[i];
                var accessory = item.Get();
                if (accessory.slot == slot)
                {
                    Debug.Log("changing slot");
                    items[i] = new ResourceRef<Accessory>(accessories.Get(value));

                    // force refresh the symbol
                    var newAccessory = items[i].Get();
                    kbac.GetComponent<SymbolOverrideController>().AddSymbolOverride(newAccessory.slot.targetSymbolId, newAccessory.symbol, 0);

                    return;
                }
            }
        }

        public void Sim1000ms(float dt)
        {
            if(SpiceEyesDuration>0)
            {
                SpiceEyesDuration-= dt;
            }
            if(SpiceEyesDuration <= 0 && SpiceEyesDuration > -50) 
            {
                Remove();
            }

            //if(effects.effects.First(ef => ef.effect.Id == "PILOTING_SPICE") != null)
            //{

            //}
            //foreach (var effect in effects.effects)
            //{

            //    Debug.Log("E: " + effect.effect.Id + "; " + effect.statusItem.Id + "; " + effect.modifier.Id + "; " + effect.timeRemaining);
            //}
                
        }

        internal void AddEyeDuration(float duration)
        {
            SpiceEyesDuration = duration;
            Apply(originalEyes+"_glow");
        }
    }
}
