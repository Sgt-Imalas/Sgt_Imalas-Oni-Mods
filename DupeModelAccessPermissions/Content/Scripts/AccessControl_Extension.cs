using KSerialization;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static AccessControl;

namespace DupeModelAccessPermissions.Content.Scripts
{
	public class AccessControl_Extension : KMonoBehaviour
	{
		[MyCmpReq] public AccessControl accessControl;

		[Serialize]
		private AccessControl.Permission _defaultPermissionBionics = (AccessControl.Permission)(-1);

		/// <summary>
		/// wait 2 frames to the access control has transfered its default
		/// </summary>
		/// <returns></returns>
		IEnumerator DelayedBionicDefaultTransfer()
		{
			yield return null;
			yield return null;
			accessControl.SetDefaultPermission(GameTags.Minions.Models.Bionic, _defaultPermissionBionics);
			_defaultPermissionBionics = (AccessControl.Permission)(-1);
		}

		public override void OnSpawn()
		{
			if (_defaultPermissionBionics == (AccessControl.Permission)(-1))
				return;
			StartCoroutine(DelayedBionicDefaultTransfer());
			base.OnSpawn();
		}
	}
}
