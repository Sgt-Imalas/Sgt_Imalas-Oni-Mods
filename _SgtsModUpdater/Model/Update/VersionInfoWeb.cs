using _SgtsModUpdater.Model.LocalMods;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace _SgtsModUpdater.Model.Update
{
	public class VersionInfoWeb : INotifyPropertyChanged
	{
		public string staticID, version, minimumSupportedBuild, modName, modDesc;

		LocalMod localInstall = null;
		public LocalMod LocalMod => localInstall;
		public string InstalledVersion => localInstall == null ? "not installed" : localInstall.Version;

		public string GetActionString => GetActionText();
		public string Version => version;
		public string ModName => string.IsNullOrWhiteSpace(modName) ? staticID : modName;
		public string ModDesc => modDesc;

		public string FetchUrl => _fetchUrl;
		private string _fetchUrl = "";
		public void SetUrl(string releaseUrl)
		{
			_fetchUrl = releaseUrl + "/" + zipFileName;
		}
		private bool downloading = false;
		public bool Downloading { get { return downloading; } set { downloading = value; OnPropertyChanged("DownloadingVisibility"); } }
		public Visibility DownloadingVisibility => Downloading ? Visibility.Visible : Visibility.Hidden;


		public string zipFileName => staticID + ".zip";

		string GetActionText()
		{
			if (localInstall == null)
				return "Install Mod";
			if (IsNewVersionAvailable())
				return "Update Mod";

			return "Latest Installed";
		}
		public bool InstallButtonInteractable => IsNewVersionAvailable() && !Downloading;
		public bool IsNewVersionAvailable()
		{
			if (version == null)
				return false;
			if (localInstall == null)
				return true;
			var FetchableVersion = System.Version.Parse(Version);
			var LocalVersion = System.Version.Parse(localInstall.Version);

			if (FetchableVersion != null && LocalVersion != null)
			{
				return FetchableVersion > LocalVersion;
			}
			return FetchableVersion != LocalVersion;
		}
		public override string ToString()
		{
			return modName + " (" + version + ")";
		}

		internal void SetInstalledMod(LocalMod localMod)
		{
			localInstall = localMod;
		}

		internal async void TryInstallUpdate()
		{
			await ModManager.Instance.TryInstallUpdate(this);
			OnPropertyChanged("InstalledVersion"); 
			OnPropertyChanged("GetActionString");
			OnPropertyChanged("InstallButtonInteractable");

		}
		private void OnPropertyChanged(string info)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(info));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
