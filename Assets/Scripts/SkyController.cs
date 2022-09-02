using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class SkyController : MonoBehaviour {
    [Range(-1, 1)] public float offset = 0;
    public Gradient skyGradient;

    public const int MAX_GRADIENT_KEYS = 16;
    
    private void Awake() {
        UpdateSkybox();
    }

    private void OnValidate() {
        if (skyGradient == null) return;
        
        if (skyGradient.colorKeys.Length > MAX_GRADIENT_KEYS) { 
            var list = skyGradient.colorKeys.ToList();
            while (list.Count > MAX_GRADIENT_KEYS) {
                list.RemoveAt(list.Count - 1); // remove last until we reach max keys
            }
            skyGradient.colorKeys = list.ToArray();
        }
        
        UpdateSkybox();
    }

    public void UpdateSkybox() {
        var list = skyGradient.colorKeys.ToList();
        while (list.Count < MAX_GRADIENT_KEYS) {
            list.Add(new GradientColorKey(Color.black, 1));
        }

        Shader.SetGlobalFloat("_GradientOffset", offset);
        Shader.SetGlobalInt("_GradientKeyCount", skyGradient.colorKeys.Length);
        Shader.SetGlobalVectorArray("_GradientColors", list.Select(k => k.color.ToVector()).ToArray());
        Shader.SetGlobalFloatArray("_GradientKeys", list.Select(k => k.time).ToArray());
    }
}
