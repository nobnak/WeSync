using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeSyncSys.Structures;

namespace WeSyncSys {

	public interface IReadonlyWeSpace {
		SubSpace CurrSubspace { get; }
	}

	public class WeSpace : IReadonlyWeSpace, IWeBase {

		public readonly static int P_We_Local2Global = Shader.PropertyToID("_We_Local2Global");
		public readonly static int P_We_Uv2Npos = Shader.PropertyToID("_We_Uv2Npos");

		public event System.Action<IReadonlyWeSpace> Changed;

		#region interface

		#region object
		public override string ToString() {
			return $"<{GetType().Name} : {CurrSubspace}>";
		}
		#endregion

		#region IReadonlySubspace
		public SubSpace CurrSubspace { get; protected set; }
		#endregion

		#region IWeBase
		public void Update() {
			Shader.SetGlobalMatrix(P_We_Local2Global, CurrLocal2Global);
			Shader.SetGlobalMatrix(P_We_Uv2Npos, CurrUv2Npos);
		}
		#endregion

		public Matrix4x4 CurrLocal2Global { get; protected set; }
		public Matrix4x4 CurrUv2Npos { get; protected set; }

		public SubSpace Apply(Vector2Int localScreen, Rect localShare, float globalSize) {
			var localShareSize = localShare.size;
			var localAspect = (float)localScreen.x / localScreen.y;
			var globalAspect = GlobalAspect(localShareSize.x, localShareSize.y, localAspect);
			CurrLocal2Global = LocalToGlobal(localShare.width, localShare.height, localShare.x, localShare.y, localAspect);
			CurrUv2Npos = UvToNpos(localShare.width, localShare.height, localShare.x, localShare.y, localAspect);

			var globalField = globalSize * new Vector2(globalAspect, 1f);
			var result = new SubSpace() {
				localShare = localShare,
				localField = new Rect(globalField * localShare.min, globalField * localShare.size),
				globalField = globalField,
			};
			CurrSubspace = result;
			Changed?.Invoke(this);
			return result;
		}
		#endregion

		#region static
		public static Matrix4x4 LocalToGlobal(
			float uvSize_x, float uvSize_y,
			float uvOffset_x, float uvOffset_y,
			float localAspect) {

			var globalAspect = GlobalAspect(uvSize_x, uvSize_y, localAspect);

			Matrix4x4 m = default;
			// local to global (uv)
			m.SetRow(0, new Vector4(
				uvSize_x, uvSize_y, uvOffset_x, uvOffset_y));
			// llocal to global (npos)
			m.SetRow(1, new Vector4(
				uvSize_y, uvSize_y, globalAspect * uvOffset_x, uvOffset_y));
			// inverse (uv)
			m.SetRow(2, new Vector4(
				1f / uvSize_x, 1f / uvSize_y, -uvOffset_x / uvSize_x, -uvOffset_y / uvSize_y));
			// inverse (npos)
			m.SetRow(3, new Vector4(
				1f / uvSize_y, 1f / uvSize_y, -globalAspect * uvOffset_x / uvSize_y, -uvOffset_y / uvSize_y));
			
			return m;
		}

		public static Matrix4x4 UvToNpos(
			float uvOffset_x, float uvOffset_y,
			float uvSize_x, float uvSize_y,
			float localAspect) {

			var globalAspect = GlobalAspect(uvSize_x, uvSize_y, localAspect);

			Matrix4x4 m = default;
			// uv to npos (local)
			m.SetRow(0, new Vector4(localAspect, 1));
			// uv to npos (global)
			m.SetRow(1, new Vector4(globalAspect, 1));
			// inverse (local)
			m.SetRow(2, new Vector4(1f / localAspect, 1));
			// inverse (global)
			m.SetRow(3, new Vector4(1f / globalAspect, 1));

			return m;
		}

		public static float GlobalAspect(float localShare_x, float localShare_y, float localAspect) {
			return localAspect * localShare_y / localShare_x;
		}
		#endregion
	}
}
