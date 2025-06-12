using _SgtsModUpdater.Model;
using _SgtsModUpdater.Model.Update;
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
    public MainWindow()
    {
		InitializeComponent();
		PackView.ItemsSource = ModManager.Instance.Repos;
		ModListView.ItemsSource = ModManager.Instance.CurrentRepoMods;
		ModManager.Instance.FetchRepos();

		CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ModListView.ItemsSource);
		view.Filter = UserFilter;

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


	private void Mod_Action_Click(object sender, RoutedEventArgs e)
	{
		var rowItem = (sender as Button).DataContext as VersionInfoWeb;

		rowItem.TryInstallUpdate();
	}

	private void txtFilter_TextChanged(object sender, TextChangedEventArgs e)
	{
		CollectionViewSource.GetDefaultView(ModListView.ItemsSource).Refresh();
	}
}