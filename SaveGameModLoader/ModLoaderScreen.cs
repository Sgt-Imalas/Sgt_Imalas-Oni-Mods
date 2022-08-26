using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace SaveGameModLoader
{
    public class ModLoaderScreen : KModalScreen
    {
        [SerializeField]
        private KButton closeButtonTitle;
        [SerializeField]
        private KButton closeButton;
        [SerializeField]
        private KButton toggleAllButton;
        [SerializeField]
        private KButton workshopButton;
        [SerializeField]
        private GameObject entryPrefab;
        [SerializeField]
        private Transform entryParent;
        public ModLoaderScreen()
        {
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            this.closeButtonTitle.ClearOnClick();
            this.workshopButton.onClick += (System.Action)(() => Debug.Log("Steam Called"));
            
            this.BuildDisplay();
        }
        private void BuildDisplay()
        {
            
        }
    }
}
