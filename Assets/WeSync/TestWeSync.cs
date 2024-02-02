using UnityEngine;
using WeSyncSys;

namespace IllusionSys2.Tests {

	public class TestWeSync : MonoBehaviour {

		public Links links = new Links();

		public void Listen(IWeSync we) {
			var m = links.mat;
			if (m != null) {
				var sub = we.Space.CurrSubspace;
				var share = sub.localShare;
				m.mainTextureOffset = new Vector2(share.x, share.y);
				m.mainTextureScale = new Vector2(share.width, share.height);
			}
		}

		#region declarations
		[System.Serializable]
		public class Links {
			public Material mat;
		}
		#endregion
	}
}