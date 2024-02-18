using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static Rockets_TinyYetBig.STRINGS.UI_MOD.UISIDESCREENS;

namespace Rockets_TinyYetBig.SpaceStations.Construction
{
    public class SpaceConstructable : KMonoBehaviour, ICheckboxListGroupControl
    {
        [Serialize] List<PartProject> OpenParts = new List<PartProject>();
        [Serialize] List<PartProject> InProgressParts = new List<PartProject>();
        [Serialize] List<PartProject> FinishedParts = new List<PartProject>();
        [Serialize] ConstructionProjectAssembly CurrentProject = null;
        [Serialize] public bool DerelictStation =false;


        [MyCmpReq]
        public Storage buildPartStorage;
        public bool ConstructionActive => this.OpenParts.Count > 0 || this.InProgressParts.Count > 0;

        private static readonly EventSystem.IntraObjectHandler<SpaceConstructable> OnStorageChangeDelegate = new EventSystem.IntraObjectHandler<SpaceConstructable>((System.Action<SpaceConstructable, object>)((component, data) => component.OnStorageChange(data)));
        
        public override void OnSpawn()
        {
            base.OnSpawn();
            Subscribe((int)GameHashes.OnStorageChange, OnStorageChangeDelegate);
        }

        private void OnStorageChange(object data)
        {
            if (!AllPartsFinished())
                return;

            CurrentProject.OnConstructionFinishedAction.Invoke(this);
            CurrentProject = null;
        }

        

        public bool CancelCurrentProject()
        {
            if (CurrentProject == null)
            {
                return true;
            }
            if (FinishedParts.Count > 0 && InProgressParts.Count > 0)
            {
                return false;
            }
            CurrentProject = null;
            OpenParts.Clear();
            return true;
        }
        public void ForceFinishProject(ConstructionProjectAssembly project)
        {

            foreach (var part in project.Parts)
            {
                FinishedParts.Add(part);

                Element element = ElementLoader.GetElement(part.ResourceTag);
                var PrefabItem = Assets.GetPrefab(part.ResourceTag);

                if (element != null)
                {
                    buildPartStorage.AddElement(element.id, part.ResourceAmountMass, element.defaultValues.temperature, 255, 0);
                }
                else if (PrefabItem != null)
                {
                    GameObject go = Util.KInstantiate(PrefabItem, new Vector3(-1, -1));
                    go.SetActive(true);
                    buildPartStorage.Store(go, true);
                }
                else
                    SgtLogger.warning(part.ResourceTag + " was not a valid resource");
            }
            if (CancelCurrentProject())
            {
                OpenParts.AddRange(new List<PartProject>(project.Parts));
                CurrentProject = project;
            }
        }
        public void AssignProject(ConstructionProjectAssembly project)
        {
            if (CancelCurrentProject())
            {
                OpenParts.AddRange(new List<PartProject>(project.Parts));
                CurrentProject = project;
            }
        }

        private bool AllPartsFinished()
        {
            return OpenParts.Count() == 0 && InProgressParts.Count() == 0;
        }

        public bool SufficientMaterialInStorage(PartProject part, Storage sourceStorage)
        {
            return sourceStorage.FindFirstWithMass(part.ResourceTag, part.ResourceAmountMass);
        }

        public bool PullFromConstructionStorage(PartProject part, Storage TargetStorage)
        {
            if (!SufficientMaterialInStorage(part, buildPartStorage))
                return false;

            var material = buildPartStorage.FindFirstWithMass(part.ResourceTag, part.ResourceAmountMass);
            if (material.TryGetComponent<Pickupable>(out var pickupable))
            {
                var takenResource = pickupable.Take(part.ResourceAmountMass);
                TargetStorage.Store(takenResource.gameObject, true);
                return true;
            }

            return false;
        }

        public bool PutInConstructionStorage(PartProject part, Storage sourceStorage)
        {
            if (!SufficientMaterialInStorage(part, sourceStorage)) return false;
            var material = sourceStorage.FindFirstWithMass(part.ResourceTag, part.ResourceAmountMass);
            if (material.TryGetComponent<Pickupable>(out var pickupable))
            {
                var takenResource = pickupable.Take(part.ResourceAmountMass);
                buildPartStorage.Store(takenResource.gameObject, true);
                return true;
            }

            return false;
        }

        public bool StartConstruction(PartProject part)
        {
            if (OpenParts.Contains(part)
                && !InProgressParts.Contains(part)
                && !FinishedParts.Contains(part))
            {
                part.IsConstructionProcess = true;
                OpenParts.Remove(part);
                InProgressParts.Add(part);
                this.Trigger(ModAssets.Hashes.OnStationPartConstructionStarted, part);
                return true;
            }
            return false;
        }
        public bool CancelConstruction(PartProject part)
        {
            if (!OpenParts.Contains(part)
                && InProgressParts.Contains(part)
                && !FinishedParts.Contains(part))
            {
                InProgressParts.Remove(part);
                OpenParts.Add(part);
                return true;
            }
            return false;
        }
        public bool FinishConstruction(PartProject part)
        {
            if (!OpenParts.Contains(part)
                && InProgressParts.Contains(part)
                && !FinishedParts.Contains(part))
            {
                InProgressParts.Remove(part);
                FinishedParts.Add(part);
                this.Trigger(ModAssets.Hashes.OnStationPartConstructionFinished, part);
                return true;
            }
            return false;
        }
        public bool StartDeconstruction(PartProject part)
        {
            if (FinishedParts.Contains(part)
                && !InProgressParts.Contains(part)
                && !OpenParts.Contains(part))
            {
                part.IsConstructionProcess = false;

                FinishedParts.Remove(part);
                InProgressParts.Add(part);
                this.Trigger(ModAssets.Hashes.OnStationPartConstructionStarted, part);
                return true;
            }
            return false;
        }

        public bool CancelDeconstruction(PartProject part)
        {
            if (!OpenParts.Contains(part)
                && InProgressParts.Contains(part)
                && !FinishedParts.Contains(part))
            {
                InProgressParts.Remove(part);
                FinishedParts.Add(part);

                part.IsConstructionProcess = true;
                return true;
            }
            return false;
        }
        public bool FinishDeconstruction(PartProject part)
        {
            if (!OpenParts.Contains(part)
                && InProgressParts.Contains(part)
                && !FinishedParts.Contains(part))
            {
                InProgressParts.Remove(part);
                OpenParts.Add(part);
                this.Trigger(ModAssets.Hashes.OnStationPartConstructionFinished, part);

                part.IsConstructionProcess = true;
                return true;
            }
            return false;
        }


        public string Title => SPACECONSTRUCTIONSITE.TITLE;

        public string Description => SPACECONSTRUCTIONSITE.DESC;

        public object GameScheduler { get; internal set; }

        public ICheckboxListGroupControl.ListGroup[] GetData()
        {
            var Items = new List<ICheckboxListGroupControl.ListGroup>();
            if (CurrentProject != null)
            {
                var checkboxes = new List<ICheckboxListGroupControl.CheckboxItem>();
                foreach (PartProject item in InProgressParts)
                {
                    checkboxes.Add(new ICheckboxListGroupControl.CheckboxItem()
                    {
                        text = item.ResourceTag.ToString(),
                        isOn = false
                    });
                    ;
                }
                foreach (PartProject item in FinishedParts)
                {
                    checkboxes.Add(new ICheckboxListGroupControl.CheckboxItem()
                    {
                        text = item.ResourceTag.ToString(),
                        isOn = true
                    });
                    ;
                }
                foreach (PartProject item in OpenParts)
                {
                    checkboxes.Add(new ICheckboxListGroupControl.CheckboxItem()
                    {
                        text = item.ResourceTag.ToString(),
                        isOn = false
                    });
                    ;
                }
                Items.Add(new ICheckboxListGroupControl.ListGroup("Parts", checkboxes.ToArray()));
            }
            return Items.ToArray();
        }

        public bool SidescreenEnabled() => CurrentProject != null;

        public int CheckboxSideScreenSortOrder() => 20;

        internal void TransferPartsTo(SpaceConstructable constructable)
        {
            this.buildPartStorage.Transfer(constructable.buildPartStorage);
            while(FinishedParts.Count> 0)
            {
                constructable.FinishedParts.Add(FinishedParts[0]);
                FinishedParts.RemoveAt(0);
            }
        }
    }
}
