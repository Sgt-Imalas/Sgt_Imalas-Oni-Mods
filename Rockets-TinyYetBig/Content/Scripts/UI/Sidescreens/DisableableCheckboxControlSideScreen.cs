using Rockets_TinyYetBig.Content.Scripts.Buildings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig.Content.Scripts.UI.Sidescreens
{
	internal class DisableableCheckboxControlSideScreen : SideScreenContent
	{
		private KToggle toggle;
		private KImage toggleCheckMark;
		private LocText label;
		private ToolTip toolTip;
		private IDisableableCheckboxControl target;

		public override void OnPrefabInit() => base.OnPrefabInit();

		bool init = false;
		void Init()
		{
			if (init)
				return;
			init = true;

			UIUtils.ListAllChildrenPath(transform);
			var tr = transform.Find("Contents/CheckboxGroup");

			toggle = tr.Find("CheckBox").gameObject.GetComponent<KToggle>();
			toggleCheckMark = toggle.transform.Find("CheckMark").gameObject.GetComponent<KImage>();
			label = tr.Find("Label").gameObject.GetComponent<LocText>();
			label.key = string.Empty;


			toolTip = toggle.transform.parent.GetComponent<ToolTip>();//.SetSimpleTooltip(this.target.CheckboxTooltip);
		}
		//IEnumerator RefreshLabel()
		//{
		//	yield return null;
		//	yield return null;
		//	label.SetText(label.text);
		//}

		public override void OnSpawn()
		{
			Init();
			base.OnSpawn();
			this.toggle.onValueChanged += new Action<bool>(this.OnValueChanged);
		}

		public override bool IsValidForTarget(GameObject target)
		{
			return target.GetComponent<IDisableableCheckboxControl>() != null || target.GetSMI<IDisableableCheckboxControl>() != null;
		}
		public override void OnActivate()
		{
			base.OnActivate();
			//StartCoroutine(RefreshLabel());
		}

		public override void SetTarget(GameObject target)
		{
			Init();
			base.SetTarget(target);
			if (target == null)
			{
				Debug.LogWarning((object)"The target object provided was null");
			}
			else
			{
				this.target = target.GetComponent<IDisableableCheckboxControl>();
				if (this.target == null)
					this.target = target.GetSMI<IDisableableCheckboxControl>();
				if (this.target == null)
				{
					Debug.LogWarning((object)"The target provided does not have an IDisableableCheckboxControl component");
				}
				else
				{
					this.label.SetText(this.target.CheckboxLabel);
					toolTip.SetSimpleTooltip(this.target.CheckboxTooltip);
					this.titleKey = this.target.CheckboxTitleKey;
					this.toggle.SetIsOnWithoutNotify(this.target.GetCheckboxValue());
					this.toggle.interactable = this.target.GetIsCheckboxInteractable();
					this.toggleCheckMark.enabled = this.toggle.isOn;
				}
			}
		}

		public override void ClearTarget()
		{
			base.ClearTarget();
			this.target = null;
		}

		private void OnValueChanged(bool value)
		{
			this.target.SetCheckboxValue(value);
			this.toggleCheckMark.enabled = value;
		}
		public override int GetSideScreenSortOrder()
		{
			return -50;
		}
	}
}
