using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs.UIcmp;

namespace DupePrioPresetManager
{
	internal class UnityScreen_ScheduleShifterPopup : FScreen
	{
#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable CS0414 // Remove unused private members
		new bool ConsumeMouseScroll = true; // do not remove!!!!
#pragma warning restore CS0414 // Remove unused private members
#pragma warning restore IDE0051 // Remove unused private members
		public static UnityScreen_ScheduleShifterPopup Instance = null;

		public FButton CloseButton2;
		public FButton CloseButton;
		public FButton ApplyButton;
		public FButton CloneButton;
		public FButton ShiftLeftButton;
		public FButton ShiftRightButton;

		public FInputField2 ShiftAmountTextField;

		public GameObject ShiftLeftIMG;
		public GameObject ShiftNoneIMG;
		public GameObject ShiftRightIMG;

		public bool CurrentlyActive = false;
		public int ShiftIndex = 0;

		Schedule ToChangeOrClone = null;

		public static void ShowWindow(Schedule toLoadFrom, GameObject parent, System.Action onClose)
		{
			if (Instance == null)
			{
				var screen = Util.KInstantiateUI(ModAssets.ScheduleShifterPrefab, parent, true);
				Instance = screen.AddOrGet<UnityScreen_ScheduleShifterPopup>();
				Instance.Init();
			}
			Instance.ToChangeOrClone = toLoadFrom;
			Instance.ShiftIndex = 0;
			Instance.ChangeOffset();

			Instance.Show(true);
			Instance.transform.SetAsLastSibling();
			Instance.OnCloseAction = onClose;
			Instance.ShiftAmountTextField.Text = "0";
		}

		private bool init;
		private System.Action OnCloseAction;
		public static System.Action RefreshAllAction = null;


		public override void OnKeyDown(KButtonEvent e)
		{
			if (e.TryConsume(Action.MouseRight))
			{
				this.Show(false);
			}
			if (e.TryConsume(Action.Escape))
			{
				this.Show(false);
			}
			base.OnKeyDown(e);
		}


		private void Init()
		{
			//UIUtils.ListAllChildrenPath(this.transform);

			CloseButton2 = transform.Find("Title/CloseButton").FindOrAddComponent<FButton>();
			CloseButton = transform.Find("Content/Cancel").FindOrAddComponent<FButton>();
			ApplyButton = transform.Find("Content/ApplyButton").FindOrAddComponent<FButton>();
			CloneButton = transform.Find("Content/CloneButton").FindOrAddComponent<FButton>();
			ShiftLeftButton = transform.Find("Content/CloneOffset/Offset/LeftButton").FindOrAddComponent<FButton>();
			ShiftRightButton = transform.Find("Content/CloneOffset/Offset/RightButton").FindOrAddComponent<FButton>();

			ShiftAmountTextField = transform.Find("Content/CloneOffset/Offset/Input").FindOrAddComponent<FInputField2>();
			ShiftAmountTextField.OnValueChanged.AddListener(TextOffset);
			ShiftAmountTextField.Text = "0";

			ShiftLeftIMG = transform.Find("Content/CloneOffset/Offset/offset_left_img").gameObject;
			ShiftNoneIMG = transform.Find("Content/CloneOffset/Offset/offset_none_img").gameObject;
			ShiftRightIMG = transform.Find("Content/CloneOffset/Offset/offset_right").gameObject;


			ApplyButton.OnClick += () =>
			{
				ApplyOffset();
				this.OnCloseAction.Invoke();
				this.Show(false);
			};
			CloneButton.OnClick += () =>
			{
				if (ToChangeOrClone != null)
				{
					ApplyOffset(true);
				}
				this.OnCloseAction.Invoke();
				this.Show(false);
			};

			ShiftLeftButton.OnClick += () =>
			{
				ChangeOffset(-1);
			};
			ShiftRightButton.OnClick += () =>
			{
				ChangeOffset(1);
			};

			CloseButton.OnClick += () => this.Show(false);
			CloseButton2.OnClick += () => this.Show(false);

			init = true;
		}

		public void TextOffset(string filterstring = "")
		{
			if (int.TryParse(filterstring, out var newValue))
			{
				if (newValue != ShiftIndex)
				{
					newValue -= ShiftIndex;
					ChangeOffset(newValue);
				}
			}

		}
		void ApplyOffset(bool CopyToNew = false)
		{

			if (ToChangeOrClone != null)
			{
				List<ScheduleBlock> newBlocks = new List<ScheduleBlock>();

				Schedule newSchedule = null;
				if (CopyToNew)
				{
					newSchedule = ScheduleManager.Instance.AddSchedule(Db.Get().ScheduleGroups.allGroups, null, false);
				}


				for (int i = 0; i < ToChangeOrClone.blocks.Count; ++i)
				{
					int oldIndex = (i - ShiftIndex + 24) % 24;
					newBlocks.Add(ToChangeOrClone.blocks[oldIndex]);
				}

				if (CopyToNew)
				{
					newSchedule.blocks = newBlocks;
					RefreshAllAction?.Invoke();
				}
				else
				{
					ToChangeOrClone.blocks = newBlocks;
				}
			}

		}

		void ChangeOffset(int offset = 0)
		{
			if (offset != 0)
			{
				ShiftIndex += offset;
				if (ShiftIndex >= 13)
				{
					ShiftIndex -= 24;
				}
				if (ShiftIndex <= -12)
				{
					ShiftIndex += +24;
				}
			}

			if (ShiftAmountTextField.Text != ShiftIndex.ToString())
				ShiftAmountTextField.Text = ShiftIndex.ToString();

			ShiftLeftIMG.SetActive(ShiftIndex < 0);
			ShiftNoneIMG.SetActive(ShiftIndex == 0);
			ShiftRightIMG.SetActive(ShiftIndex > 0);
		}



		public override void OnShow(bool show)
		{
			base.OnShow(show);
			if (!init)
			{
				Init();
			}

			if (show)
			{
				CurrentlyActive = show;
			}
			else
			{
				DeactivateStatusWithDelay(600);
			}
		}
		async Task DeactivateStatusWithDelay(int ms)
		{
			await Task.Delay(ms);
			CurrentlyActive = false;
		}
	}
}

