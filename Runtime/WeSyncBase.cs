using nobnak.Gist;
using nobnak.Gist.Cameras;
using nobnak.Gist.Exhibitor;
using UnityEngine;
using UnityEngine.Events;

namespace WeSyncSys {

	[ExecuteAlways]
	public class WeSyncBase : AbstractExhibitor, IWeSync {

		[SerializeField]
		protected Camera targetCamera = null;
		[SerializeField]
		protected Events events = new Events();
		[SerializeField]
		protected Tuner tuner = new Tuner();

		protected WeSpace space = new WeSpace();
		protected WeTime time = new WeTime();
		protected WeProjection proj = new WeProjection();

		protected CameraData screen;
		protected Validator validator = new Validator();

		#region unity
		protected virtual void OnEnable() {
			screen = default;
			validator.Reset();
			validator.SetCheckers(() => screen.Equals(targetCamera));
			validator.Validation += () => {
				screen = targetCamera;
				if (targetCamera == null)
					return;

				proj.Apply(screen);

				var uv = tuner.localUv;
				var local = new Rect(uv.x, uv.y, uv.z, uv.w);
				var rspace = space.Apply(screen.screenSize, local, tuner.globalSize);

				targetCamera.orthographicSize = Mathf.Max(1f, 0.5f * rspace.localField.height);
			};
			validator.Validated += () => {
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

		#region Exhibitor
		public override void DeserializeFromJson(string json) {
			JsonUtility.FromJsonOverwrite(json, tuner);
			validator.Invalidate();
		}
		public override object RawData() {
			return tuner;
		}
		public override string SerializeToJson() {
			validator.Validate();
			return JsonUtility.ToJson(tuner);
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
