using UnityEngine;

namespace WeSyncSys {

    public class WeProjection {

		public CameraContext CurrCam { get; protected set; }
		public Matrix4x4 CurrVP { get; protected set; }

		public void Apply(CameraContext screen) {
			this.CurrCam = screen;
			CurrVP = screen.projectionMatrix * screen.worldToCameraMatrix;
		}

		public Vector2 WorldPosToUV(Vector3 worldPos) {
			var clip = CurrVP * new Vector4(worldPos.x, worldPos.y, worldPos.z, 1f);
			clip /= clip.w;
			return new Vector2(0.5f * (clip.x + 1), 0.5f * (clip.y + 1));
		}
	}
}