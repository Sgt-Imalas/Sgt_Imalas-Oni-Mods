using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using static STRINGS.BUILDINGS.PREFABS;

namespace BlueprintsV2.BlueprintsV2.UnityUI.Components
{
	internal class YesNoInfo : KMonoBehaviour
	{
		private bool showYes = false;

		Image Yes, No;
		GameObject YesGO, NoGO;

		static Color? YesCol, NoCol;

		public void SetInfoState(bool yes)
		{
			showYes = yes;
			RefreshIcons();
		}
		public override void OnSpawn()
		{
			base.OnSpawn();
			RefreshIcons();
		}

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			YesGO = transform.Find("Yes").gameObject;
			NoGO = transform.Find("No").gameObject;

			Yes = YesGO.GetComponent<Image>();
			No = NoGO.GetComponent<Image>();

			InitColors();
			Yes.color = YesCol.Value;
			No.color = NoCol.Value;		
		}

		void InitColors()
		{
			if (YesCol.HasValue)
				return;

			var ye = (Color)GlobalAssets.Instance.colorSet.logicOn;
			ye.a = 1;
			YesCol = ye;

			var no = (Color)GlobalAssets.Instance.colorSet.logicOff;
			no.a = 1;
			NoCol = no;
		}

		void RefreshIcons()
		{
			YesGO.SetActive(showYes);
			NoGO.SetActive(!showYes);
		}
	}
}
