using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DupePrioPresetManager.Serializables
{
	internal interface IUtilityPreset
	{
		public string IFileName { get; set; }
		public string IConfigName { get; set; }
		public string ParentFolder { get;}

		public void OpenPopUpToChangeName();
	}
}
