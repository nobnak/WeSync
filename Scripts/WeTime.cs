using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeSyncSys.Extensions.TimeExt;

namespace WeSyncSys {

	public class WeTime {

		public readonly static int P_We_Time = Shader.PropertyToID("_We_Time");

		#region interface
		public void Apply() {
			var date = System.DateTimeOffset.Now;
			var st = date.Pack();
			Shader.SetGlobalVector(P_We_Time, st);
		}
		#endregion
	}
}
