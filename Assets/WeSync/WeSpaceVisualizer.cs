using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class WeSpaceVisualizer : MonoBehaviour {

    protected Material material;

    #region unity
    void OnEnable() {
        material = new Material(Shader.Find("Hidden/WeSpaceVisualizer"));
    }
    void OnRenderImage(RenderTexture source, RenderTexture destination) {
        Graphics.Blit(source, destination, material);
    }
    #endregion
}