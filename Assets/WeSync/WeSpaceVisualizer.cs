using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[ExecuteAlways]
public class WeSpaceVisualizer : MonoBehaviour {

    [SerializeField]
    protected Tuner tuner = new();

    protected Material material;

    #region unity
    void OnEnable() {
        material = new Material(Shader.Find("Hidden/WeSpaceVisualizer"));
    }
    void OnRenderImage(RenderTexture source, RenderTexture destination) {
        material.SetFloat(P_Wireframe_Gain, tuner.wireframeGain);
        
        Graphics.Blit(source, destination, material);
    }
    #endregion

    #region declarations
    public static readonly int P_Wireframe_Gain = Shader.PropertyToID("_Wireframe_Gain");

    [System.Serializable]
    public class Tuner {
        [Range(0f, 10f)]
        public float wireframeGain = 1.0f;
    }
    #endregion
}