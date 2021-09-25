using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeSyncSys.Structures {

	public struct SubSpace : System.IEquatable<SubSpace> {
		public Rect localShare;
		public Rect localField;
		public Vector2 globalField;

		public bool Equals(SubSpace b) {
			return localShare == b.localShare
				&& localField == b.localField
				&& globalField == b.globalField;
		}

		#region interface
		#region object
		public static bool operator ==(SubSpace a, SubSpace b) => a.Equals(b);
		public static bool operator !=(SubSpace a, SubSpace b) => !a.Equals(b);

		public override bool Equals(object obj) {
			return (obj != null) && (obj is SubSpace) && (this == (SubSpace)obj);
		}
		public override int GetHashCode() {
			return localShare.GetHashCode() ^ localField.GetHashCode() ^ globalField.GetHashCode(); 
		}
		public override string ToString() {
			return $"<SubSpace: localShare={localShare}, localField={localField}, globalField={globalField}>";
		}
		#endregion
		#endregion

		#region static

		public static SubSpace Generate(Vector2 globalField) => new SubSpace() {
			localShare = new Rect(0f, 0f, 1f, 1f),
			localField = new Rect(Vector2.zero, globalField),
			globalField = globalField,
		};
		#endregion
	}
}
