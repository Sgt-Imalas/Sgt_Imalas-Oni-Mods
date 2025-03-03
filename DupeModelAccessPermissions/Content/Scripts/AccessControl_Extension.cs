using KSerialization;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static AccessControl;

namespace DupeModelAccessPermissions.Content.Scripts
{
	internal class AccessControl_Extension : KMonoBehaviour
	{
		// -1 is regular default, using -2 for default bionics
		public static int DefaultBionicsInstanceID = -2;

		[MyCmpReq] public AccessControl accessControl;
		[MyCmpGet] Building building;
		[MyCmpGet] OccupyArea occupyArea;
		[MyCmpGet] NavTeleporter navTeleporter;

		[Serialize]
		private AccessControl.Permission _defaultPermissionBionics = (AccessControl.Permission) (-1);
		public AccessControl.Permission DefaultPermissionBionics
		{
			get => this._defaultPermissionBionics;
			set
			{
				_defaultPermissionBionics = value;
				SetGridRestrictions_DefaultBionics(_defaultPermissionBionics);
				accessControl.SetStatusItem();
			}
		}
		public static readonly EventSystem.IntraObjectHandler<AccessControl_Extension> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<AccessControl_Extension>(delegate (AccessControl_Extension component, object data)
		{
			component.OnCopySettings(data);
		});
		public override void OnSpawn()
		{
			if(_defaultPermissionBionics == (AccessControl.Permission)(-1))
				_defaultPermissionBionics = accessControl.DefaultPermission;

			base.OnSpawn();
			if (accessControl.registered) //RestorePermissions mirror
				SetGridRestrictions_DefaultBionics(DefaultPermissionBionics);

			Subscribe(-905833192, OnCopySettingsDelegate);
		}
		public void OnCopySettings(object data)
		{
			if (data is not GameObject go || !go.TryGetComponent<AccessControl_Extension>(out var source) || source == null)
				return;

			_defaultPermissionBionics = source._defaultPermissionBionics;
			SetGridRestrictions_DefaultBionics(DefaultPermissionBionics);
		}

		private void SetGridRestrictions_DefaultBionics(AccessControl.Permission permission)
		{
			if (!accessControl.registered || !accessControl.isSpawned)
				return;
			if (occupyArea == null && building == null)
				return;
			Grid.Restriction.Directions directions = 0;
			switch (permission)
			{
				case AccessControl.Permission.Both:
					directions = 0;
					break;
				case AccessControl.Permission.GoLeft:
					directions = Grid.Restriction.Directions.Right;
					break;
				case AccessControl.Permission.GoRight:
					directions = Grid.Restriction.Directions.Left;
					break;
				case AccessControl.Permission.Neither:
					directions = Grid.Restriction.Directions.Left | Grid.Restriction.Directions.Right;
					break;
			}
			if (accessControl.isTeleporter)
				directions = directions == 0 ? 0 : Grid.Restriction.Directions.Teleport;
			if (building != null)
			{
				foreach (int registeredBuildingCell in accessControl.registeredBuildingCells)
					Grid.SetRestriction(registeredBuildingCell, DefaultBionicsInstanceID, directions);
			}
			else
			{
				foreach (CellOffset occupiedCellsOffset in occupyArea.OccupiedCellsOffsets)
					Grid.SetRestriction(Grid.OffsetCell(Grid.PosToCell((KMonoBehaviour)occupyArea), occupiedCellsOffset), DefaultBionicsInstanceID, directions);
			}
			if (accessControl.isTeleporter)
				Grid.SetRestriction(navTeleporter.GetCell(), DefaultBionicsInstanceID, directions);
		}

		///BlueprintsV2 integration
		internal static JObject TryGetData(GameObject arg)
		{
			if (arg.TryGetComponent<AccessControl_Extension>(out var component) && component.accessControl.controlEnabled)
			{
				return new JObject()
					{
						{ "DefaultPermissionBionics", (int)component.DefaultPermissionBionics}
					};
			}
			return null;
		}
		public static void TryApplyData(GameObject building, JObject jObject)
		{
			if (jObject == null)
				return;
			if (building.TryGetComponent<AccessControl_Extension>(out var targetComponent) && targetComponent.accessControl.controlEnabled)
			{
				var t1 = jObject.GetValue("DefaultPermissionBionics");
				if (t1 == null)
					return;
				var DefaultPermissionBionics = t1.Value<int>();

				//applying values
				targetComponent.DefaultPermissionBionics = (AccessControl.Permission)DefaultPermissionBionics;
			}
		}
	}
}
