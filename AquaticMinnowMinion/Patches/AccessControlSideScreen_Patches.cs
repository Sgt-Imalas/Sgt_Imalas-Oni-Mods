using AquaticMinnowMinion.Content.ModDb;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace AquaticMinnowMinion.Patches
{
	/// <summary>
	/// Door control sidescreen needs a new group for custom minion model
	/// </summary>
	internal class AccessControlSideScreen_Patches
	{

		private static GameObject aquaticMinionSectionHeader;
		private static GameObject aquaticMinionSectionContent;
		private static MultiToggle CollapseToggle;
		private static HierarchyReferences aquaticMinionSectionHR;

		///Sections of header, cache them once for less getters in the RefreshContainer method.
		static LocText _CategoryLabel;
		static ToolTip _HeaderTooltip;
		static MultiToggle _ToggleLeft, _ToggleRight, _CollapseToggle;
		static RectTransform _Content, _EmptyRow;

		static void CacheReferences()
		{
			if (aquaticMinionSectionHR == null)
				return;
			_ToggleLeft = aquaticMinionSectionHR.GetReference<MultiToggle>("ToggleLeft");
			_ToggleRight = aquaticMinionSectionHR.GetReference<MultiToggle>("ToggleRight");
			_CollapseToggle = aquaticMinionSectionHR.GetReference<MultiToggle>("CollapseToggle");
			_Content = aquaticMinionSectionHR.GetReference<RectTransform>("Content");
			_CategoryLabel = aquaticMinionSectionHR.GetReference<LocText>("CategoryLabel");
			_HeaderTooltip = aquaticMinionSectionHR.GetReference<ToolTip>("HeaderTooltip");
			_EmptyRow = aquaticMinionSectionHR.GetReference<RectTransform>("EmptyRow");
		}

		[HarmonyPatch(typeof(AccessControlSideScreen), nameof(AccessControlSideScreen.SpawnContainers))]
		public class AccessControlSideScreen_SpawnContainers_Patch
		{
			public static void Prefix(AccessControlSideScreen __instance, ref bool __state)
			{
				//bool gets set at the end of the method, so it needs to be captured before the method runs
				__state = __instance.containersSpawned;
			}
			public static void Postfix(AccessControlSideScreen __instance, bool __state)
			{
				if (__state)
					return;
				//not in use bc klei used an inlined method which is not accessible in a regular way.
				AccessControlSideScreen.categoryNames[ModAssets.Tags.AquaticMinion] = STRINGS.DUPLICANTS.MODEL.AQUATIC.NAME_ADJECTIVE;

				aquaticMinionSectionHeader = Util.KInstantiateUI(__instance.entityCategoryPrefab, __instance.scrollContents, true);
				aquaticMinionSectionHeader.transform.SetSiblingIndex(__instance.robotSectionHeader.transform.GetSiblingIndex());
				aquaticMinionSectionHR = aquaticMinionSectionHeader.GetComponent<HierarchyReferences>();

				aquaticMinionSectionContent = aquaticMinionSectionHR.GetReference<RectTransform>("Content").gameObject;
				CollapseToggle = aquaticMinionSectionHR.GetReference<MultiToggle>("CollapseToggle");
				CacheReferences();
			}
		}


		[HarmonyPatch(typeof(AccessControlSideScreen), nameof(AccessControlSideScreen.Refresh))]
		public class AccessControlSideScreen_Refresh_Patch
		{
			public static void Postfix(AccessControlSideScreen __instance)
			{
				bool noAquaticMinions = aquaticMinionSectionContent.transform.childCount <= 1;

				if (noAquaticMinions)
					__instance.ToggleCategoryCollapsed(false, aquaticMinionSectionContent.rectTransform(), CollapseToggle);
				_EmptyRow.gameObject.SetActive(noAquaticMinions);
			}
		}

		[HarmonyPatch(typeof(AccessControlSideScreen), nameof(AccessControlSideScreen.RefreshContainerObjects))]
		public class AccessControlSideScreen_RefreshContainerObjects_Patch
		{
			public static void Postfix(AccessControlSideScreen __instance)
			{
				if (!Game.IsDlcActiveForCurrentSave(DlcManager.DLC5_ID))
				{
					aquaticMinionSectionHeader.SetActive(false);
					return;
				}
				//else
				aquaticMinionSectionHeader.SetActive(true);
				RefreshContainer(__instance);

			}
			/// <summary>
			/// Mirrored from RefreshContainerObjects internal method...
			/// </summary>
			static void RefreshContainer(AccessControlSideScreen instance)
			{
				var containerTag = ModAssets.Tags.AquaticMinion;

				var defaultPermission = instance.target.GetDefaultPermission(containerTag);

				_CategoryLabel.SetText(STRINGS.DUPLICANTS.MODEL.AQUATIC.NAME_ADJECTIVE);
				_HeaderTooltip.SetSimpleTooltip(global::STRINGS.UI.UISIDESCREENS.ACCESS_CONTROL_SIDE_SCREEN.CATEGORY_HEADER_TOOLTIP);

				bool goLeftAllowed = defaultPermission == AccessControl.Permission.Both || defaultPermission == AccessControl.Permission.GoLeft;
				bool goRightAllowed = defaultPermission == AccessControl.Permission.Both || defaultPermission == AccessControl.Permission.GoRight;
				_ToggleLeft.ChangeState((!goLeftAllowed) ? 1 : 0);
				_ToggleRight.ChangeState((!goRightAllowed) ? 1 : 0);
				_ToggleLeft.onClick = delegate
				{
					switch (defaultPermission)
					{
						case AccessControl.Permission.Both:
							instance.target.SetDefaultPermission(containerTag, AccessControl.Permission.GoRight);
							break;
						case AccessControl.Permission.Neither:
							instance.target.SetDefaultPermission(containerTag, AccessControl.Permission.GoLeft);
							break;
						case AccessControl.Permission.GoLeft:
							instance.target.SetDefaultPermission(containerTag, AccessControl.Permission.Neither);
							break;
						case AccessControl.Permission.GoRight:
							instance.target.SetDefaultPermission(containerTag, AccessControl.Permission.Both);
							break;
					}
					instance.RefreshContainerObjects();
				};
				_ToggleRight.onClick = delegate
				{
					switch (defaultPermission)
					{
						case AccessControl.Permission.Both:
							instance.target.SetDefaultPermission(containerTag, AccessControl.Permission.GoLeft);
							break;
						case AccessControl.Permission.Neither:
							instance.target.SetDefaultPermission(containerTag, AccessControl.Permission.GoRight);
							break;
						case AccessControl.Permission.GoLeft:
							instance.target.SetDefaultPermission(containerTag, AccessControl.Permission.Both);
							break;
						case AccessControl.Permission.GoRight:
							instance.target.SetDefaultPermission(containerTag, AccessControl.Permission.Neither);
							break;
					}

					instance.RefreshContainerObjects();
				};
				_CollapseToggle.onClick = delegate
				{
					instance.ToggleCategoryCollapsed(!_Content.gameObject.activeSelf, _Content, _CollapseToggle);
				};
			}
		}


        [HarmonyPatch(typeof(AccessControlSideScreen), nameof(AccessControlSideScreen.ConfigureRow))]
        public class AccessControlSideScreen_ConfigureRow_Patch
        {

			public static IEnumerable<CodeInstruction> Transpiler(ILGenerator _, IEnumerable<CodeInstruction> orig)
            {
				var m_InstantiateIndentityRow = AccessTools.Method(typeof(AccessControlSideScreen), nameof(AccessControlSideScreen.InstantiateIndentityRow));

				foreach (var ci in orig)
				{
					if (ci.Calls(m_InstantiateIndentityRow)) {
						yield return new CodeInstruction(OpCodes.Ldarg_1); //object entity
						yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(AccessControlSideScreen_ConfigureRow_Patch), nameof(RedirectParentTarget)));
					}
					yield return ci;
				}
			}

			///swap out the parent go for the aquatic minions
            private static GameObject RedirectParentTarget(GameObject currentParent, object entity)
            {
				if(entity == null || entity is not MinionAssignablesProxy proxy)
					return currentParent;

				var minionGo = proxy.GetTargetGameObject();
				if(minionGo == null || !minionGo.HasTag(ModAssets.Tags.AquaticMinion))
					return currentParent;

				return aquaticMinionSectionContent;
			}
        }
	}
}
