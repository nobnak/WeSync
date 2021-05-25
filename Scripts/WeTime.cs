using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeSyncSys {

	public class WeTime {

		public readonly static int P_We_Time = Shader.PropertyToID("_We_Time");

		public readonly static float NOON_IN_SEC = 12 * 3600;

		#region interface
		public void Apply() {
			var date = System.DateTimeOffset.Now;
			var st = Pack(date);
			Shader.SetGlobalVector(P_We_Time, st);
		}
		#endregion

		#region static
		public static Vector4 Pack(System.DateTimeOffset date) {
			var time = date.TimeOfDay;
			var totalSecs = (float)time.TotalSeconds;
			var st = new Vector4(
				0f,
				totalSecs - NOON_IN_SEC,
				0f,
				0f
			);
			return st;
		}
		#endregion
	}
}
