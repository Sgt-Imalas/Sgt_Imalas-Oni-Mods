using BlueprintsV2.Tools;
using PeterHan.PLib.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BlueprintsV2.BlueprintsV2.Tools
{
	internal class ElementTypeSecondaryParameterMenu : KMonoBehaviour
	{

		public static ElementTypeSecondaryParameterMenu Instance;

		private readonly Dictionary<Element.State, GameObject> widgets = [];
		private GameObject content;
		private GameObject widgetContainer;
		private Dictionary<Element.State, ToolParameterMenu.ToggleState> parameters;

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();

			GameObject baseContent = ToolMenu.Instance.toolParameterMenu.content;
			GameObject baseWidgetContainer = ToolMenu.Instance.toolParameterMenu.widgetContainer;

			content = Util.KInstantiateUI(baseContent, baseContent.transform.parent.gameObject);
			var pos = content.transform.position;
			pos.x -= baseContent.GetComponent<RectTransform>().rect.width + 5;
			content.transform.SetPosition(pos);
			content.transform.GetChild(1).gameObject.SetActive(false);
			var title = content.transform.GetChild(0).transform.GetComponentInChildren<LocText>();
			title?.text = global::STRINGS.UI.UISIDESCREENS.TREEFILTERABLESIDESCREEN.TITLE;

		widgetContainer = Util.KInstantiateUI(baseWidgetContainer, content, true);
			content.SetActive(false);
		}

		public void PopulateMenu()
		{
			ClearMenu();
			parameters = new()
			{
					{ Element.State.Solid, ToolParameterMenu.ToggleState.On},
					{ Element.State.Liquid, ToolParameterMenu.ToggleState.On},
					{ Element.State.Gas, ToolParameterMenu.ToggleState.On },
					{ Element.State.Vacuum, ToolParameterMenu.ToggleState.On },
			};

			foreach (var parameter in parameters)
			{
				GameObject widetPrefab = Util.KInstantiateUI(ToolMenu.Instance.toolParameterMenu.widgetPrefab, widgetContainer, true);
				widetPrefab.GetComponentInChildren<LocText>().text
					= parameter.Key switch
					{
						Element.State.Solid => global::STRINGS.UI.CODEX.CATEGORYNAMES.ELEMENTSSOLID,
						Element.State.Liquid => global::STRINGS.UI.CODEX.CATEGORYNAMES.ELEMENTSLIQUID,
						Element.State.Gas => global::STRINGS.UI.CODEX.CATEGORYNAMES.ELEMENTSGAS,
						_ => global::STRINGS.ELEMENTS.VACUUM.NAME,
					};


				MultiToggle toggle = widetPrefab.GetComponentInChildren<MultiToggle>();
				switch (parameter.Value)
				{
					case ToolParameterMenu.ToggleState.On:
						toggle.ChangeState(1);
						break;

					case ToolParameterMenu.ToggleState.Disabled:
						toggle.ChangeState(2);
						break;

					default:
						toggle.ChangeState(0);
						break;
				}

				toggle.onClick += () =>
				{
					foreach (KeyValuePair<Element.State, GameObject> widget in widgets)
					{
						if (widget.Value == toggle.transform.parent.gameObject)
						{
							if (parameters[widget.Key] == ToolParameterMenu.ToggleState.Disabled)
							{
								break;
							}

							if (parameters[widget.Key] == ToolParameterMenu.ToggleState.On)
							{
								parameters[widget.Key] = ToolParameterMenu.ToggleState.Off;
							}

							else
							{
								parameters[widget.Key] = ToolParameterMenu.ToggleState.On;
							}

							OnChange();
							break;
						}
					}
				};

				widgets.Add(parameter.Key, widetPrefab);
			}

			content.SetActive(true);
		}

		public bool AllowedByFilter(Element.State state)
		{
			return parameters.TryGetValue(state, out var toggleState) && toggleState == ToolParameterMenu.ToggleState.On;
		}


		private void OnChange()
		{
			foreach (var widget in widgets)
			{
				switch (parameters[widget.Key])
				{
					case ToolParameterMenu.ToggleState.On:
						widget.Value.GetComponentInChildren<MultiToggle>().ChangeState(1);
						continue;

					case ToolParameterMenu.ToggleState.Off:
						widget.Value.GetComponentInChildren<MultiToggle>().ChangeState(0);
						continue;

					case ToolParameterMenu.ToggleState.Disabled:
						widget.Value.GetComponentInChildren<MultiToggle>().ChangeState(2);
						continue;
				}
			}
		}

		public void ClearMenu()
		{
			content.SetActive(false);

			foreach (var widget in widgets)
			{
				Util.KDestroyGameObject(widget.Value);
			}

			widgets.Clear();
		}

		public void ShowMenu()
		{
			if (parameters == null)
				PopulateMenu();
			content.SetActive(true);
		}

		public void HideMenu()
		{
			content.SetActive(false);
		}

		public static void CreateInstance()
		{
			GameObject parameterMenu = new GameObject("", typeof(ElementTypeSecondaryParameterMenu));
			parameterMenu.transform.SetParent(ToolMenu.Instance.toolParameterMenu.transform.parent);
			parameterMenu.gameObject.SetActive(true);
			parameterMenu.gameObject.SetActive(false);

			Instance = parameterMenu.GetComponent<ElementTypeSecondaryParameterMenu>();
		}

		public static void DestroyInstance()
		{
			Instance = null;
		}

	}
}