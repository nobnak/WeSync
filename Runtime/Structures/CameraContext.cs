using System;
using Unity.Mathematics;
using UnityEngine;

namespace WeSyncSys {

    public struct CameraContext : IEquatable<Camera> {

        public Camera camera;
        public Matrix4x4 worldToCameraMatrix;
        public Matrix4x4 projectionMatrix;

        #region IEquatable
        public bool Equals(Camera obj) {
            return obj != null
                && camera.Equals(obj)
                && worldToCameraMatrix.Equals(obj.worldToCameraMatrix)
                && projectionMatrix.Equals(obj.projectionMatrix);
        }
        public bool Equals(CameraContext cameraContext) {
            return camera.Equals(cameraContext.camera)
                && worldToCameraMatrix.Equals(cameraContext.worldToCameraMatrix)
                && projectionMatrix.Equals(cameraContext.projectionMatrix);
        }
        #endregion

        #region object
        public override bool Equals(object obj) {
            if (obj is Camera)
                return Equals((Camera)obj);
            else if (obj is CameraContext)
                return Equals((CameraContext)obj);
            else
                return false;
        }
        public override int GetHashCode() {
            return base.GetHashCode();
        }
        #endregion

        #region static
        public static implicit operator CameraContext(Camera cam) {
            return new CameraContext() {
                camera = cam,
                worldToCameraMatrix = cam.worldToCameraMatrix,
                projectionMatrix = cam.projectionMatrix
            };
        }
        #endregion
    }
}