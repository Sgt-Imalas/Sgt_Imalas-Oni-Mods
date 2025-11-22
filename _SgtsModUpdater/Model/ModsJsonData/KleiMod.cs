using _SgtsModUpdater.Model.LocalMods;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _SgtsModUpdater.Model.ModsJsonData
{
	internal class KleiMod
	{
		[JsonProperty]
		public Label label;
		[JsonProperty]
		public Status status;
		[JsonProperty]
		public bool enabled;
		[JsonProperty]
		public HashSet<string> enabledForDlc;
		[JsonProperty]
		public int crash_count;
		[JsonProperty]
		public string reinstall_path;
		[JsonProperty]
		public string staticID => label.id + "." + label.distribution_platform.ToString();


		public bool EnabledForDlc(string dlcID) => enabledForDlc.Contains(dlcID);
		public void SetEnabledForDlc(string dlcID, bool enabled)
		{
			if (enabled)
				enabledForDlc.Add(dlcID);
			else
				enabledForDlc.Remove(dlcID);
		}
		public void SetInstalledState(bool installed)
		{
			if (!installed)
				enabledForDlc.Clear();
			status = installed ? Status.Installed : Status.NotInstalled;
		}
	}
	public enum Status
	{
		NotInstalled,
		Installed,
		UninstallPending,
		ReinstallPending,
	}
}
