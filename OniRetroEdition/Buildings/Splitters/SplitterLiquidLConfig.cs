using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace OniRetroEdition.Buildings.Splitters
{
	internal class SplitterLiquidLConfig : IBuildingConfig
	{
		static readonly ConduitType Type = ConduitType.Liquid;
		static readonly bool Mirrored = true;
		static readonly string anim = "utilityliquidjoiner_kanim";
		public static readonly string ID = "retro_liquidsplitter_mirrored";

		public static ConduitPortInfo PortInfo = BaseSplitterConfig.GetPortInfo(Type, Mirrored);
		public override BuildingDef CreateBuildingDef() => BaseSplitterConfig.CreateBuildingDef(ID, anim, Type, Mirrored);

		public override void DoPostConfigureUnderConstruction(GameObject go) => BaseSplitterConfig.AttachPort(go, PortInfo);
		public override void DoPostConfigurePreview(BuildingDef def, GameObject go) => BaseSplitterConfig.AttachPort(go, PortInfo);
		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag) => BaseSplitterConfig.ConfigureBuildingTemplate(go, prefab_tag, PortInfo, Mirrored);
		public override void DoPostConfigureComplete(GameObject go) => BaseSplitterConfig.DoPostConfigureComplete(go);
	}
}
