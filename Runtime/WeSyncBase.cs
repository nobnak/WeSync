using Gist2.Deferred;
using Gist2.Extensions.ComponentExt;
using Gist2.Extensions.SizeExt;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

namespace WeSyncSys {

	[ExecuteAlways]
	public class WeSyncBase : MonoBehaviour, IWeSync {

		[SerializeField]
		protected Camera targetCamera = null;
		[SerializeField]
		protected Events events = new Events();
		[SerializeField]
		protected Tuner tuner = new Tuner();

		protected WeSpace space = new WeSpace();
		protected WeTime time = new WeTime();
		protected WeProjection proj = new WeProjection();

		protected CameraContext screen;
		protected Validator validator = new Validator();

		#region unity
		protected virtual void OnEnable() {
			screen = default;
			validator.Reset();
			validator.CheckValidity += () => screen.Equals(targetCamera);
			validator.OnValidate += () => {
				screen = targetCamera;
				if (targetCamera == null)
					return;

				proj.Apply(screen);

				var uv = tuner.localUv;
				var local = new Rect(uv.x, uv.y, uv.z, uv.w);
				var size = screen.camera.Size();
                var rspace = space.Apply(new Vector2Int(size.x, size.y), local, tuner.globalSize);

				targetCamera.orthographicSize = math.max(1f, 0.5f * rspace.localField.height);
			};
			validator.AfterValidate += () => {
				Notify();
			};
			Notify();
		}
		protected virtual void OnValidate() {
			validator.Invalidate();
		}
		protected virtual void Update() {
			validator.Validate();
			space.Update();
			time.Update();
		}
        #endregion

        #region properties
		public Tuner CurrTuner {
			get => tuner.DeepCopy();
			set {
				if (!value.EqualsAsJson(tuner)) {
					tuner = value.DeepCopy();
                    validator.Invalidate();
				}
			}
		}
        #endregion

        #region IWeSync
        public virtual WeSpace Space => space;
		public virtual WeTime Time => time;
		public virtual WeProjection Proj => proj;
		#endregion

		#region IReadonlySubspace
		public virtual Structures.SubSpace CurrSubspace {
			get {
				validator.Validate();
				return space.CurrSubspace;
			}
		}
		#endregion

		#region listeners
		public virtual void ListenCamera(GameObject gameObject) {
			targetCamera = gameObject.GetComponent<Camera>();
			validator.Invalidate();
		}
		#endregion

		#region member
		protected virtual void Notify() => events.Changed?.Invoke(this);
		#endregion

		#region definition
		[System.Serializable]
		public class Tuner {
			public float globalSize = 10f;
			public Vector4 localUv = new Vector4(0f, 0f, 1f, 1f);
		}
		[System.Serializable]
		public class Events {
			[System.Serializable]
			public class WeSyncEvent : UnityEvent<IWeSync> { }

			public WeSyncEvent Changed = new WeSyncEvent();
		}
#endregion
	}
}
