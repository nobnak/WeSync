using UnityEngine;
using WeSyncSys.Extensions;

namespace WeSyncSys {

	public class WeTime : IWeBase {

		public readonly static int P_We_Time = Shader.PropertyToID("_We_Time");

		#region interface

		#region object
		public override string ToString() {
			return $"<{GetType().Name} : {CurrDateTime}>";
		}
		#endregion

		public System.DateTimeOffset CurrDateTime { get; protected set; }
		public void Update() {
			CurrDateTime = System.DateTimeOffset.Now;
			var st = CurrDateTime.Pack();
			Shader.SetGlobalVector(P_We_Time, st);
		}
		#endregion
	}
}
