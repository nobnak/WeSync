using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeSyncSys.Structures;

namespace WeSyncSys {

	public interface IReadonlyWeSpace {
		SubSpace CurrSubspace { get; }

		float spacialUnits_local();
		float spacialUnits_global();

		Vector2 uv2pos_local(Vector2 uv);
		Vector2 uv2pos_global(Vector2 uv);
		Vector2 uv2pos_local_inv(Vector2 x);
		Vector2 uv2pos_global_inv(Vector2 x);

		Vector2 uv2npos_local(Vector2 uv);
		Vector2 uv2npos_global(Vector2 uv);
		Vector2 uv2npos_local_inv(Vector2 npos);
		Vector2 uv2npos_global_inv(Vector2 npos);

		Vector2 local2global_uv(Vector2 uv);
		Vector2 local2global_npos(Vector2 npos);
		Vector2 local2global_uv_inv(Vector2 uv);
	}

	public class WeSpace : IReadonlyWeSpace, IWeBase {

		public readonly static int P_We_Local2Global = Shader.PropertyToID("_We_Local2Global");
		public readonly static int P_We_Uv2Npos = Shader.PropertyToID("_We_Uv2Npos");
		public readonly static int P_We_Uv2Pos = Shader.PropertyToID("_We_Uv2Pos");

		public event System.Action<IReadonlyWeSpace> Changed;

		#region interface

		#region object
		public override string ToString() {
			return $"<{GetType().Name} : {CurrSubspace}>";
		}
		#endregion

		#region IReadonlySubspace
		public SubSpace CurrSubspace { get; protected set; }

		public float spacialUnits_local() => CurrUv2Pos[0, 1];
		public float spacialUnits_global() => CurrUv2Pos[1, 1];

		public Vector2 uv2pos_local(Vector2 uv) => TRS(CurrUv2Pos.GetRow(0), uv);
		public Vector2 uv2pos_global(Vector2 uv) => TRS(CurrUv2Pos.GetRow(1), uv);
		public Vector2 uv2pos_local_inv(Vector2 x) => TRS(CurrUv2Pos.GetRow(2), x);
		public Vector2 uv2pos_global_inv(Vector2 x) => TRS(CurrUv2Pos.GetRow(3), x);

		public Vector2 uv2npos_local(Vector2 uv) => TRS(CurrUv2Npos.GetRow(0), uv);
		public Vector2 uv2npos_global(Vector2 uv) => TRS(CurrUv2Npos.GetRow(1), uv);
		public Vector2 uv2npos_local_inv(Vector2 npos) => TRS(CurrUv2Npos.GetRow(2), npos);
		public Vector2 uv2npos_global_inv(Vector2 npos) => TRS(CurrUv2Npos.GetRow(3), npos);

		public Vector2 local2global_uv(Vector2 uv) => TRS(CurrLocal2Global.GetRow(0), uv);
		public Vector2 local2global_npos(Vector2 npos) => TRS(CurrLocal2Global.GetRow(1), npos);
		public Vector2 local2global_uv_inv(Vector2 uv) => TRS(CurrLocal2Global.GetRow(2), uv);

		protected Vector2 TRS(Vector4 m, Vector2 uv) => new Vector2(m.x * uv.x + m.z, m.y * uv.y + m.w);
		#endregion

		#region IWeBase
		public void Update() {
			Shader.SetGlobalMatrix(P_We_Local2Global, CurrLocal2Global);
			Shader.SetGlobalMatrix(P_We_Uv2Npos, CurrUv2Npos);
			Shader.SetGlobalMatrix(P_We_Uv2Pos, CurrUv2Pos);
		}
		#endregion

		public Matrix4x4 CurrLocal2Global { get; protected set; }
		public Matrix4x4 CurrUv2Npos { get; protected set; }
		public Matrix4x4 CurrUv2Pos { get; protected set; }

		public SubSpace Apply(Vector2Int localScreen, Rect localShare, float globalSize) {
			var localShareSize = localShare.size;
			var localAspect = (float)localScreen.x / localScreen.y;
			var globalAspect = GlobalAspect(localShareSize.x, localShareSize.y, localAspect);
			var globalField = globalSize * new Vector2(globalAspect, 1f);
			var localField = new Rect(globalField * localShare.min, globalField * localShare.size);

			CurrLocal2Global = LocalToGlobal(localShare.width, localShare.height, localShare.x, localShare.y, localAspect);
			CurrUv2Npos = UvToNpos(localShare.width, localShare.height, localShare.x, localShare.y, localAspect);
			CurrUv2Pos = UvToPos(localField, new Rect(Vector2.zero, globalField));

			var result = new SubSpace() {
				localShare = localShare,
				localField = localField,
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

		public static Matrix4x4 UvToPos(
			Rect localField,
			Rect globalField
			) {

			var m = default(Matrix4x4);
			m.SetRow(0, new Vector4(localField.width, localField.height, localField.x, localField.y));
			m.SetRow(1, new Vector4(globalField.width, globalField.height, globalField.x, globalField.y));
			m.SetRow(2, new Vector4(
				1f / localField.width, 1f / localField.height,
				-localField.x / localField.width, -localField.y / localField.height));
			m.SetRow(3, new Vector4(
				1f / globalField.width, 1f / globalField.height,
				-globalField.x / globalField.width, -globalField.y / globalField.height));

			return m;
		}

		public static float GlobalAspect(float localShare_x, float localShare_y, float localAspect) {
			return localAspect * localShare_y / localShare_x;
		}
		#endregion
	}
}
