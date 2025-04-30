using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class LightingManager : MonoBehaviour
{

    [Header("Lighting Settings")]
    public Material skyboxMaterial; 
    public Color ambientColor = Color.gray;
    public AmbientMode ambientMode = AmbientMode.Flat;

    [Header("Reflection Settings")]
    public DefaultReflectionMode reflectionMode = DefaultReflectionMode.Skybox;
    public Cubemap customReflection;

    void Start()
    {
        ApplyLightingSettings();
    }

    void ApplyLightingSettings()
    {
        // 스카이박스 설정
        RenderSettings.skybox = skyboxMaterial;

        // 환경광 설정
        RenderSettings.ambientMode = ambientMode;
        RenderSettings.ambientLight = ambientColor;

        // 반사광 설정
        RenderSettings.defaultReflectionMode = reflectionMode;
        if (reflectionMode == DefaultReflectionMode.Custom && customReflection != null)
        {
            RenderSettings.customReflection = customReflection;
        }

        // GI 적용
        DynamicGI.UpdateEnvironment();
    }
}
