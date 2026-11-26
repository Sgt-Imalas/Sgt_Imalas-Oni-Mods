using Database;
using HarmonyLib;
using Klei.AI;
using KleiFormatters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static GameUtil;

namespace MultiTimeDisplays
{
	internal class Patches
	{
		[HarmonyPatch(typeof(RadsFormatter), nameof(RadsFormatter.AppendValue),
			[typeof(KleiStringBuilder), typeof((float, GameUtil.TimeSlice))],
			[ArgumentType.Ref, ArgumentType.Normal])]
		public class RadsFormatter_AppendValue_Patch
		{
			static bool mirroring = false;

			public static void Prefix(ref KleiStringBuilder builder, (float rads, GameUtil.TimeSlice timeSlice) args)
			{
				if (mirroring || args.timeSlice != TimeSlice.PerCycle)
					return;

				mirroring = true;
				args.timeSlice = TimeSlice.PerSecond;
				builder.Append('(');
				builder.Append<RadsFormatter, (float, TimeSlice)>(args);
				builder.Append(')');
				builder.Append(' ');
				mirroring = false;
			}
			public static void Postfix(ref KleiStringBuilder builder, (float rads, GameUtil.TimeSlice timeSlice) args)
			{
				if (mirroring || args.timeSlice != TimeSlice.PerSecond)
					return;

				mirroring = true;
				args.timeSlice = TimeSlice.PerCycle;
				builder.Append(' ');
				builder.Append('(');
				builder.Append<RadsFormatter, (float, TimeSlice)>(args);
				builder.Append(')');
				mirroring = false;
			}
		}

		[HarmonyPatch(typeof(CaloriesFormatter), nameof(CaloriesFormatter.AppendValue),
			[typeof(KleiStringBuilder), typeof((float, bool, GameUtil.TimeSlice, bool))],
			[ArgumentType.Ref, ArgumentType.Normal])]
		public class CaloriesFormatter_AppendValue_Patch
		{
			static bool mirroring = false;
			public static void Prefix(ref KleiStringBuilder builder, (float calories, bool showSuffix, GameUtil.TimeSlice timeSlice, bool forceKcal) args)
			{
				if (mirroring || args.timeSlice != TimeSlice.PerCycle)
					return;

				mirroring = true;
				args.timeSlice = TimeSlice.PerSecond;
				builder.Append('(');
				builder.Append<CaloriesFormatter, (float, bool, TimeSlice, bool)>(args);
				builder.Append(')');
				builder.Append(' ');
				mirroring = false;
			}
			public static void Postfix(ref KleiStringBuilder builder, (float calories, bool showSuffix, GameUtil.TimeSlice timeSlice, bool forceKcal) args)
			{
				if (mirroring || args.timeSlice != TimeSlice.PerSecond)
					return;

				mirroring = true;
				args.timeSlice = TimeSlice.PerCycle;
				builder.Append(' ');
				builder.Append('(');
				builder.Append<CaloriesFormatter, (float, bool, TimeSlice, bool)>(args);
				builder.Append(')');
				mirroring = false;
			}
		}

		[HarmonyPatch(typeof(MassFormatter), nameof(MassFormatter.AppendValue),
			[typeof(KleiStringBuilder), typeof((float, GameUtil.TimeSlice, GameUtil.MetricMassFormat, bool, string))],
			[ArgumentType.Ref, ArgumentType.Normal])]
		public class MassFormatter_AppendValue_Patch
		{
			static bool mirroring = false;
			public static void Prefix(ref KleiStringBuilder builder, (float mass, GameUtil.TimeSlice timeSlice, GameUtil.MetricMassFormat massFormat, bool includeSuffix, string floatFormat) args)
			{
				if (mirroring || args.timeSlice != TimeSlice.PerCycle)
					return;

				mirroring = true;
				args.timeSlice = TimeSlice.PerSecond;
				builder.Append('(');
				builder.Append<MassFormatter, (float, TimeSlice, MetricMassFormat, bool, string)>(args);
				builder.Append(')');
				builder.Append(' ');
				mirroring = false;
			}
			public static void Postfix(ref KleiStringBuilder builder, (float mass, GameUtil.TimeSlice timeSlice, GameUtil.MetricMassFormat massFormat, bool includeSuffix, string floatFormat) args)
			{
				if (mirroring || args.timeSlice != TimeSlice.PerSecond)
					return;

				mirroring = true;
				args.timeSlice = TimeSlice.PerCycle;
				builder.Append(' ');
				builder.Append('(');
				builder.Append<MassFormatter, (float, TimeSlice, MetricMassFormat, bool, string)>(args);
				builder.Append(')');
				mirroring = false;
			}
		}

		[HarmonyPatch(typeof(UnitsFormatter), nameof(UnitsFormatter.AppendValue),
			[typeof(KleiStringBuilder), typeof((float, GameUtil.TimeSlice, bool, string))],
			[ArgumentType.Ref, ArgumentType.Normal])]
		public class UnitsFormatter_AppendValue_Patch
		{
			static bool mirroring = false;
			public static void Prefix(ref KleiStringBuilder builder, (float units, GameUtil.TimeSlice timeSlice, bool displaySuffix, string floatFormatOverride) args)
			{
				if (mirroring || args.timeSlice != TimeSlice.PerCycle)
					return;

				mirroring = true;
				args.timeSlice = TimeSlice.PerSecond;
				builder.Append('(');
				builder.Append<UnitsFormatter, (float, TimeSlice, bool, string)>(args);
				builder.Append(')');
				builder.Append(' ');
				mirroring = false;
			}
			public static void Postfix(ref KleiStringBuilder builder, (float units, GameUtil.TimeSlice timeSlice, bool displaySuffix, string floatFormatOverride) args)
			{
				if (mirroring || args.timeSlice != TimeSlice.PerSecond)
					return;

				mirroring = true;
				args.timeSlice = TimeSlice.PerCycle;
				builder.Append(' ');
				builder.Append('(');
				builder.Append<UnitsFormatter, (float, TimeSlice, bool, string)>(args);
				builder.Append(')');
				mirroring = false;
			}
		}
	}
}
