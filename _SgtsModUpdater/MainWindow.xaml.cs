using _SgtsModUpdater.Model;
using _SgtsModUpdater.Model.ModsJsonData;
using _SgtsModUpdater.Model.Update;
using System.Diagnostics;
using System.Security.Policy;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _SgtsModUpdater;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
	MultiStream ConsoleHandler;

	public MainWindow()
	{
		InitializeComponent();
		PackView.ItemsSource = ModManager.Instance.Repos;
		ModListView.ItemsSource = ModManager.Instance.CurrentRepoMods;

		ConsoleHandler = new(Console.Out);
		ConsoleHandler.AddWriter(new TextBoxOutputter(ConsoleOutputTextbox));
		ConsoleHandler.AddWriter(new FileWriter());

		ConsoleOutputTextbox.TextChanged += (_, _) => ConsoleOutputTextbox.ScrollToEnd();
		Console.SetOut(ConsoleHandler);

		ModManager.Instance.LoadModsJson();
		CurrentModView.ItemsSource = ModManager.Instance.ModsJsonWrap.ModsObservable;

		ModManager.Instance.FetchRepos();
		ModManager.Instance.UpdateProgressbar = new Progress<float>(value => UpdateProgressBar(value));
		ModManager.Instance.GetDownloadSize = SetProgressBarMax;

		CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ModListView.ItemsSource);
		view.Filter = UserFilter;
		CollectionView view2 = (CollectionView)CollectionViewSource.GetDefaultView(CurrentModView.ItemsSource);
		view2.Filter = UserFilterLocalList;
		ModelFilters.ItemsSource = ModManager.Instance.ModsJsonWrap.FilterItems; 
		EnabledOnlySO.SetBinding(CheckBox.IsCheckedProperty, new Binding("Filter_Enabled_SO") { Source = ModManager.Instance.ModsJsonWrap, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged});
		EnabledOnlyBase.SetBinding(CheckBox.IsCheckedProperty, new Binding("Filter_Enabled_Base") { Source = ModManager.Instance.ModsJsonWrap, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
	}


	string _downloadProgress = "-";
	public string DownloadProgressString { get { return _downloadProgress; } set { _downloadProgress = value; } }
	//public float DownloadProgress


	long MaxBytes = 0;
	void UpdateProgressBar(float progress)
	{
		string max = Paths.GetReadableFileSize(MaxBytes);
		string current = Paths.GetReadableFileSize(progress * MaxBytes);
		DownloadProgressText.Text = current + " / " + max;
		DownloadProgressbar.Value = progress;
	}
	void SetProgressBarMax(long byteCount)
	{
		MaxBytes = byteCount;
	}

	private bool UserFilter(object item)
	{
		if (string.IsNullOrEmpty(txtFilter.Text))
			return true;
		else
			return ((item as RemoteMod).ModName.IndexOf(txtFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0);
	}
	private bool UserFilterLocalList(object item)
	{
		var local = item as LocalModWrapper;

		if (string.IsNullOrEmpty(txtFilter.Text))
			return ModManager.Instance.ModsJsonWrap.FiltersAllow(local);
		else
			return (local.ModName.IndexOf(txtFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0) && ModManager.Instance.ModsJsonWrap.FiltersAllow(local);
	}
	private void PackView_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		var item = (sender as ListView).SelectedItem;
		if (item is ModRepoListInfo info)
		{
			ModManager.Instance.SelectRepo(info);
			ToggleRepoListEnabled(true);
		}

	}


	private async void Mod_Action_Click(object sender, RoutedEventArgs e)
	{
		var rowItem = (sender as Button).DataContext as RemoteMod;
		await rowItem.TryInstallUpdate();
		//Thread thread = new Thread(() =>
		//{
		//	rowItem.TryInstallUpdate();
		//});
		//thread.Start();
	}

	private void txtFilter_TextChanged(object sender, TextChangedEventArgs e)
	{
		if (RepoViewActive)
			CollectionViewSource.GetDefaultView(ModListView.ItemsSource).Refresh();
		else
			CollectionViewSource.GetDefaultView(CurrentModView.ItemsSource).Refresh();
	}

	private void Delete_Mod_Click(object sender, RoutedEventArgs e)
	{
		var rowItem = (sender as Button).DataContext as RemoteMod;

		rowItem.TryDeleteLocal();
	}

	private async void CreateAddPopup_Click(object sender, RoutedEventArgs e)
	{
		var dialog = new AddRepoPopup();
		if (dialog.ShowDialog() == true)
		{
			var fetchable = dialog.GetFetchableResult();
			if (fetchable != null)
				if (await ModManager.Instance.FetchRepo(fetchable))
					AppSettings.Instance.AddRepoIfNotExist(fetchable);
		}
	}

	private void OpenFolderButton_Click(object sender, RoutedEventArgs e)
	{
		var rowItem = (sender as Button).DataContext as RemoteMod;

		if (rowItem.HasLocalMod)
		{
			Console.WriteLine("opening local mod folder of " + rowItem.ModName + " in explorer");
			OpenExplorer(rowItem.LocalMod.FolderPath);
		}
	}
	private void OpenFolderButtonLocal_Click(object sender, RoutedEventArgs e)
	{
		var rowItem = (sender as Button).DataContext as LocalModWrapper;

		if (rowItem.Folder != null)
		{
			Console.WriteLine("opening local mod folder of " + rowItem.ModName + " in explorer");
			OpenExplorer(rowItem.Folder);
		}
	}
	static void OpenExplorer(string pathToFolder)
	{
		System.Diagnostics.Process.Start(Environment.GetEnvironmentVariable("WINDIR") +
		@"\explorer.exe", pathToFolder);
	}

	private void Delete_Repo_Click(object sender, RoutedEventArgs e)
	{
		var rowItem = (sender as Button).DataContext as ModRepoListInfo;
		AppSettings.Instance.DeleteRepo(rowItem);
		ModManager.Instance.Repos.Remove(rowItem);
	}

	private void ShowModList_Click(object sender, RoutedEventArgs e)
	{
		ToggleRepoListEnabled(false);

	}
	bool RepoViewActive = false;
	void ToggleRepoListEnabled(bool enabled)
	{
		RepoViewActive = enabled;
		CurrentModView.Visibility = enabled ? Visibility.Hidden : Visibility.Visible;
		LocalModHeader.Visibility = enabled ? Visibility.Hidden : Visibility.Visible;
		ModListView.Visibility = enabled ? Visibility.Visible : Visibility.Hidden;
		this.UpdateLayout();
	}

	const string StartOnSteam = "steam://rungameid/457140";
	private void LaunchGame_Click(object sender, RoutedEventArgs e)
	{
		ModManager.Instance.WriteModsJson();
		Process.Start(new ProcessStartInfo
		{
			FileName = StartOnSteam,
			UseShellExecute = true
		});

	}

	private void CheckBox_Checked(object sender, RoutedEventArgs e)
	{
		if (!RepoViewActive)
			CollectionViewSource.GetDefaultView(CurrentModView.ItemsSource).Refresh();
	}
}