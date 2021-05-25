using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeSyncSys {

	public class WeSpace {

		public readonly static int P_We_Local2Global = Shader.PropertyToID("_We_Local2Global");
		public readonly static int P_We_Uv2Npos = Shader.PropertyToID("_We_Uv2Npos");

		#region interface
		public void Apply(Vector2Int screen, Rect local) {
			var localAspect = (float)screen.x / screen.y;
			var mlocal = LocalToGlobal(local.width, local.height, local.x, local.y, localAspect);
			var muv = UvToNpos(local.width, local.height, local.x, local.y, localAspect);

			Shader.SetGlobalMatrix(P_We_Local2Global, mlocal);
			Shader.SetGlobalMatrix(P_We_Uv2Npos, muv);
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

		public static float GlobalAspect(float uvSize_x, float uvSize_y, float screenAspect) {
			return screenAspect * uvSize_y / uvSize_x;
		}
		#endregion
	}
}
