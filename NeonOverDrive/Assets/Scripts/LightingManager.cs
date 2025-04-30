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
        // ��ī�̹ڽ� ����
        RenderSettings.skybox = skyboxMaterial;

        // ȯ�汤 ����
        RenderSettings.ambientMode = ambientMode;
        RenderSettings.ambientLight = ambientColor;

        // �ݻ籤 ����
        RenderSettings.defaultReflectionMode = reflectionMode;
        if (reflectionMode == DefaultReflectionMode.Custom && customReflection != null)
        {
            RenderSettings.customReflection = customReflection;
        }

        // GI ����
        DynamicGI.UpdateEnvironment();
    }
}
