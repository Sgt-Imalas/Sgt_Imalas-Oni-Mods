using _SgtsModUpdater.Model.LocalMods;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _SgtsModUpdater.Model.ModsJsonData
{
	internal class LocalModWrapper
	{
		public LocalMod LocalMod;
		public KleiMod KleiMod;


		public string ModName => Paths.StripFormatting(KleiMod.label.title);
		public string ModDesc => LocalMod?.ModYaml.description ?? "";
		public string LocalModType => LocalMod?.ModType ?? KleiMod.label.distribution_platform.ToString();

		public string InstalledVersion => LocalMod?.Version ?? "?";

		public string Folder => LocalMod?.FolderPath ?? Path.Combine(Paths.ModsFolder,KleiMod.label.distribution_platform.ToString(), KleiMod.label.id);

		public bool Enabled_Base
		{
			get => KleiMod.EnabledForDlc("");
			set => KleiMod.SetEnabledForDlc("", value);
		}
		public bool Enabled_SO
		{
			get => KleiMod.EnabledForDlc("EXPANSION1_ID");
			set => KleiMod.SetEnabledForDlc("EXPANSION1_ID", value);
		}
		public bool Compatible_SO
		{
			get
			{
				if (LocalMod == null)
					return true;
				if (LocalMod.ModInfoYaml.supportedContent?.Contains("ALL") ?? false)
					return true;
				if (LocalMod.ModInfoYaml.requiredDlcIds?.Contains("") ?? false)
					return false;
				if (LocalMod.ModInfoYaml.forbiddenDlcIds?.Contains("EXPANSION1_ID") ?? false)
					return false;
				if (!LocalMod.ModInfoYaml.supportedContent?.Contains("EXPANSION1_ID") ?? false)
					return false;
				return true;
			}
		}
		public bool Compatible_Base
		{
			get
			{
				if (LocalMod == null)
					return true;
				if (LocalMod.ModInfoYaml.supportedContent?.Contains("ALL") ?? false)
					return true;
				if (LocalMod.ModInfoYaml.forbiddenDlcIds?.Contains("") ?? false)
					return false;
				if (LocalMod.ModInfoYaml.requiredDlcIds?.Contains("EXPANSION1_ID") ?? false)
					return false;
				if (!LocalMod.ModInfoYaml.supportedContent?.Contains("") ?? false)
					return false;
				return true;
			}
		}

		public LocalModWrapper(LocalMod localMod, KleiMod kleiMod)
		{
			LocalMod = localMod;
			KleiMod = kleiMod;
		}

		internal KleiMod UnWrap()
		{
			return KleiMod;
		}
	}
}
