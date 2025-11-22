using _SgtsModUpdater.Model.LocalMods;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _SgtsModUpdater.Model.ModsJsonData
{
	internal class ModsJsonWrapper
	{
		public int version = 1;
		public List<LocalModWrapper> mods;
		public ObservableCollection<LocalModWrapper> ModsObservable = [];
		public ModsJsonWrapper()
		{
			FilterItems = new()
			{
			new ItemFilter(DistributionPlatform.Local),
			new ItemFilter(DistributionPlatform.Dev),
			new ItemFilter(DistributionPlatform.Steam),
			//new ItemFilter(DistributionPlatform.Epic),
			//new ItemFilter(DistributionPlatform.Rail),
			};
			foreach(var item in FilterItems)
			{
				item.PropertyChanged += (s, e) =>
				{
					if (e.PropertyName != nameof(ItemFilter.IsAllowed)) return;

					if (item.IsAllowed && !AllowedPlatforms.Contains(item.Platform))
						AllowedPlatforms.Add(item.Platform);
					else if (!item.IsAllowed && AllowedPlatforms.Contains(item.Platform))
						AllowedPlatforms.Remove(item.Platform);
				};
			}
		}

		public bool Filter_Enabled_SO { get; set; }
		public bool Filter_Enabled_Base { get; set; }

		public HashSet<DistributionPlatform> AllowedPlatforms = new()
		{
			DistributionPlatform.Local,
			DistributionPlatform.Dev,
			DistributionPlatform.Steam,
			//DistributionPlatform.Epic,
			//DistributionPlatform.Rail,
		};

		public class ItemFilter
		{
			public ItemFilter(DistributionPlatform platform, bool isAllowed = true)
			{
				Platform = platform;
				_isAllowed = isAllowed;
			}
			public string Name => Platform.ToString();
			public DistributionPlatform Platform;
			private bool _isAllowed;

			public bool IsAllowed
			{
				get => _isAllowed;
				set
				{
					if (_isAllowed == value) return;
					_isAllowed = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsAllowed)));
				}
			}
			public event PropertyChangedEventHandler PropertyChanged;
		}

		public ObservableCollection<ItemFilter> FilterItems;


		public ModsJson UnWrap()
		{
			var modsJson = new ModsJson();
			modsJson.version = this.version;
			modsJson.mods = this.mods.Select(mw => mw.UnWrap()).ToList();
			return modsJson;
		}

		internal void Add(KleiMod mod, LocalMod localMod)
		{
			if (this.mods == null)
			{
				this.mods = new List<LocalModWrapper>();
			}
			mods.Add(new(localMod, mod));
			ModsObservable.Add(new(localMod, mod));
		}


		public bool FiltersAllow(LocalModWrapper mod)
		{
			if (!AllowedPlatforms.Contains(mod.KleiMod.label.distribution_platform))
			{
				return false;
			}
			if(Filter_Enabled_SO && !mod.Enabled_SO)
				return false;
			if(Filter_Enabled_Base && !mod.Enabled_Base)
				return false;

			return true;
		}
	}
}
