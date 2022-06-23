using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace WeSyncSys.Extensions {

	public static class TimeExtension {

		public readonly static float NOON_IN_SEC = 12 * 3600;
		public readonly static long TICK_REF_TIME;

		static TimeExtension() {
			var reftime = CurrTime;
			reftime = new System.DateTimeOffset(reftime.Year, reftime.Month, reftime.Day, 12, 0, 0, reftime.Offset);
			TICK_REF_TIME = reftime.Ticks;
		}

		public static float ToSeconds(this long tick) {
			return (float)(new System.TimeSpan(tick).TotalSeconds);
		}
		public static long ToTicks(this float seconds) {
			return (long)(System.Math.Round(System.TimeSpan.TicksPerSecond * seconds));
		}
		public static System.DateTimeOffset ToDateTime(this long tick) {
			return new System.DateTimeOffset(tick, CurrTime.Offset);
		}

		public static System.DateTimeOffset CurrTime => System.DateTimeOffset.Now;
		public static long CurrTick => CurrTime.Ticks;
		public static float CurrRelativeSeconds => CurrTick.RelativeSeconds();

		public static float RelativeSeconds(this long tick) {
			return (tick - TICK_REF_TIME).ToSeconds();
		}
		public static long TickFromRelativeSeconds(this float seconds) {
			return TICK_REF_TIME + seconds.ToTicks();
		}

		public static Vector4 Pack(this System.DateTimeOffset date) {
			var totalSecs = CurrRelativeSeconds;
			var st = new Vector4(
				0f,
				totalSecs,
				0f,
				0f
			);
			return st;
		}

		public static readonly DateTimeFormatInfo CULTURE = CultureInfo.InvariantCulture.DateTimeFormat;
		public static bool TryParseAsDateTimeOffset(this string str, 
			out System.DateTimeOffset date,
			string format, bool local = false) {

			var style = local ? DateTimeStyles.AssumeLocal : DateTimeStyles.AssumeUniversal;
			return System.DateTimeOffset.TryParseExact(
				str, new string[] { format }, CULTURE, style, out date);
        }

		public static float ToFloat(this System.TimeSpan span)
			=> (float)span.TotalSeconds;
	}
}
