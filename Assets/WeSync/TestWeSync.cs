using RosettaUI;
using UnityEngine;
using WeSyncSys;

namespace IllusionSys2.Tests {

	public class TestWeSync : MonoBehaviour {

		public Links links = new Links();

        #region unity
        void Awake() {
            var uiRoot = links.ui;
            var weSync = links.weSync;
            var vis = links.vis;
            if (uiRoot != null) {
                uiRoot.Build(
                    UI.Window(
                        UI.WindowLauncher(
                            "WeSync",
                            UI.Window(
                                "WeSync",
                                UI.Page(
                                    UI.Field(() => weSync.CurrTuner)
                                        .RegisterValueChangeCallback(() => weSync.Invalidate())
                                )
                            )
                        ),
                        UI.WindowLauncher(
                            "WeSpaceVisualizer",
                            UI.Window(
                                "WeSpaceVisualizer",
                                UI.Page(
                                    UI.Field(() => vis.CurrTuner)
                                        .RegisterValueChangeCallback(() => vis.Invalidate())
                                )
                            )
                        )
                    )
                );
            }
        }
        #endregion

        #region declarations
        [System.Serializable]
		public class Links {
			public RosettaUIRoot ui;
            public WeSyncBase weSync;
            public WeSpaceVisualizer vis;
        }
		#endregion
	}
}