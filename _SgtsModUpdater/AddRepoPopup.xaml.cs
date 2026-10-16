using _SgtsModUpdater.Model.Update;
using System.Windows;

namespace _SgtsModUpdater
{
	/// <summary>
	/// Interaction logic for AddRepoPopup.xaml
	/// </summary>
	public partial class AddRepoPopup : Window
	{
		public bool CanCreateRepoInfo { get; set; } = false;
		public AddRepoPopup()
		{
			InitializeComponent();
			NameInput.TextChanged += (_, _) => VerifyInputs();
			URLInput.TextChanged += (_, _) => VerifyInputs();
		}
		public void VerifyInputs()
		{
			bool nameValid = NameInput.Text.Trim().Length > 0;
			Uri uriResult;
			bool urlValid = Uri.TryCreate(URLInput.Text, UriKind.Absolute, out uriResult) && uriResult.Scheme == Uri.UriSchemeHttp;
			bool fileAtUrlValid = false;
			CanCreateRepoInfo = nameValid && urlValid && fileAtUrlValid;
		}

		public FetchableRepoInfo GetFetchableResult()
		{
			return new(NameInput.Text, URLInput.Text);
		}


		private void CreateBtn_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}

		private void CancelBtn_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
		}
	}
}
