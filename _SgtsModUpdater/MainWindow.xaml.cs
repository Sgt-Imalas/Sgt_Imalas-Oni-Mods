using _SgtsModUpdater.Model;
using _SgtsModUpdater.Model.Update;
using System.Diagnostics;
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

		ModManager.Instance.FetchRepos();
		ModManager.Instance.UpdateProgressbar = new Progress<float>(value => UpdateProgressBar(value));
		ModManager.Instance.GetDownloadSize = SetProgressBarMax;

		CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ModListView.ItemsSource);
		view.Filter = UserFilter;

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
		if (String.IsNullOrEmpty(txtFilter.Text))
			return true;
		else
			return ((item as VersionInfoWeb).ModName.IndexOf(txtFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0);
	}
	private void PackView_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		var item = (sender as ListView).SelectedItem;
		if (item is ModRepoListInfo info)
			ModManager.Instance.SelectRepo(info);
	}


	private async void Mod_Action_Click(object sender, RoutedEventArgs e)
	{
		var rowItem = (sender as Button).DataContext as VersionInfoWeb;
		await rowItem.TryInstallUpdate();
		//Thread thread = new Thread(() =>
		//{
		//	rowItem.TryInstallUpdate();
		//});
		//thread.Start();
	}

	private void txtFilter_TextChanged(object sender, TextChangedEventArgs e)
	{
		CollectionViewSource.GetDefaultView(ModListView.ItemsSource).Refresh();
	}

	private void Delete_Mod_Click(object sender, RoutedEventArgs e)
	{
		var rowItem = (sender as Button).DataContext as VersionInfoWeb;

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
					AppSettings.Instance.AddRepoIfNotExist([fetchable]);
		}
	}

	private void OpenFolderButton_Click(object sender, RoutedEventArgs e)
	{
		var rowItem = (sender as Button).DataContext as VersionInfoWeb;

		if (rowItem.HasLocalMod)
		{
			Console.WriteLine("opening local mod folder of " + rowItem.ModName + " in explorer");
			OpenExplorer(rowItem.LocalMod.FolderPath);
		}
	}
	static void OpenExplorer(string pathToFolder)
	{
		System.Diagnostics.Process.Start(Environment.GetEnvironmentVariable("WINDIR") +
		@"\explorer.exe", pathToFolder);
	}
}