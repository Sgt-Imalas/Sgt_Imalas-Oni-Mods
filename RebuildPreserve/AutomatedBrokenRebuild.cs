using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace RebuildPreserve
{
    internal class AutomatedBrokenRebuild : KMonoBehaviour
    {
        [MyCmpGet]
        Reconstructable reconstructable;
        [MyCmpGet]
        PrimaryElement primaryElement;

        [Serialize]
        public bool RebuildOnBreaking = false;

        private static readonly EventSystem.IntraObjectHandler<AutomatedBrokenRebuild> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<AutomatedBrokenRebuild>((component, data) => component.OnCopySettings(data));

        public override void OnSpawn()
        {
            base.OnSpawn();
            if (reconstructable == null)
            {
                this.enabled = false;
                return;
            }
            Subscribe((int)GameHashes.BuildingBroken, OnBuildingBroken);
        }
        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            Subscribe((int)GameHashes.CopySettings, OnCopySettingsDelegate);
        }
        public override void OnCleanUp()
        {
            base.OnCleanUp();
            if (reconstructable == null)
            {
                return;
            }
            Unsubscribe((int) GameHashes.CopySettings, OnCopySettingsDelegate);
            Unsubscribe((int) GameHashes.BuildingBroken, OnBuildingBroken);
        }

        public void OnBuildingBroken(object data)
        {
            if(RebuildOnBreaking && reconstructable !=null)
            {
                reconstructable.RequestReconstruct(primaryElement.Element.tag);
            }
        }

        public void OnCopySettings(object data)
        {
            GameObject sauceGameObject = data as GameObject;
            if (sauceGameObject != null && sauceGameObject.TryGetComponent<AutomatedBrokenRebuild>(out var addon))
            {
                this.RebuildOnBreaking = addon.RebuildOnBreaking;
            }
        }
    }
}
