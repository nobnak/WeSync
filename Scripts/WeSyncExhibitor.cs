using ModelDrivenGUISystem;
using ModelDrivenGUISystem.Factory;
using ModelDrivenGUISystem.ValueWrapper;
using ModelDrivenGUISystem.View;
using nobnak.Gist;
using nobnak.Gist.Cameras;
using nobnak.Gist.Exhibitor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace WeSyncSys {

	[ExecuteAlways]
	public class WeSyncExhibitor : AbstractExhibitor, IReadonlyWeSpace {

		[SerializeField]
		protected Camera targetCamera = null;
		[SerializeField]
		protected Events events = new Events();
		[SerializeField]
		protected Tuner tuner = new Tuner();

		protected WeSpace space = new WeSpace();
		protected WeTime time = new WeTime();

		protected BaseView view;
		protected CameraData screen;
		protected Validator validatorValue = new Validator();

		#region unity
		private void OnEnable() {
			validatorValue.Reset();
			validatorValue.SetCheckers(() => screen.Equals(targetCamera));
			validatorValue.Validation += () => {
				screen = targetCamera;
				if (targetCamera == null)
					return;

				var uv = tuner.localUv;
				var local = new Rect(uv.x, uv.y, uv.z, uv.w);
				var rspace = space.Apply(screen.screenSize, local, tuner.globalSize);
				time.Apply();

				targetCamera.orthographicSize = Mathf.Max(1f, rspace.localField.y);
			};
			validatorValue.Validated += () => {
				events.Changed?.Invoke(this);
			};
		}
		private void OnValidate() {
			validatorValue.Invalidate();
		}
		private void Update() {
			validatorValue.Validate();
		}
		#endregion

		#region interface

		#region IReadonlySubspace
		public Structures.SubSpace CurrSubspace => space.CurrSubspace;
		#endregion

		#region Exhibitor
		public override void DeserializeFromJson(string json) {
			JsonUtility.FromJsonOverwrite(json, tuner);
			validatorValue.Invalidate();
		}
		public override object RawData() {
			return tuner;
		}
		public override string SerializeToJson() {
			validatorValue.Validate();
			return JsonUtility.ToJson(tuner);
		}
		public override void Draw() {
			GetView().Draw();
		}
		public override void ResetView() {
			if (view != null) {
				view.Dispose();
				view = null;
			}
		}
		public override void ApplyViewModelToModel() {
			validatorValue.Validate();
		}
		public override void ResetViewModelFromModel() {
			validatorValue.Invalidate();
		}
		#endregion

		public void ListenCamera(GameObject gameObject) {
			targetCamera = gameObject.GetComponent<Camera>();
			validatorValue.Invalidate();
		}

#endregion

		#region member
		protected virtual BaseView GetView() {
			if (view == null) {
				var f = new SimpleViewFactory();
				view = ClassConfigurator.GenerateClassView(new BaseValue<object>(tuner), f);
			}
			return view;
		}
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
			public class WeSyncEvent : UnityEvent<WeSyncExhibitor> { }

			public WeSyncEvent Changed = new WeSyncEvent();
		}
#endregion
	}
}
