using KartGame.KartSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static VehiclePart;
using static VehicleStats;

public class VehicleController : MonoBehaviour
{
    public PlayerVehicleData vehicleData;

    public Transform bodyModelParent;
    public Transform engineMadeParent;
    public Transform[] wheelModelParents;
    public Transform[] mirrorModelParents;

    public GameObject[] bodyDamageEffects;
    public GameObject engineDamageEffect;
    public GameObject wheelDamageEffect;
    public GameObject mirrorDamageEffect;

    private ArcadeKart kart;
    private VehicleStats baseStats;
    private Dictionary<PartType, GameObject> partModels = new Dictionary<PartType, GameObject>();

    void Start()
    {
        kart = GetComponent<ArcadeKart>();

        //차량 데이터 설정 (선택된 차량 데이터 가져오기) - > 안되는거잖아 이게ㅠ ㅠㅠ  
        //vehicleData = GameManager.Instance.PlayerData.GetCurrentVehicle();

        ApplyVehicleParts();

        UpdateDamageVisuals();
    }

    void Update()
    {
        ApplyDamageEffects();
    }

    void ApplyVehicleParts()
    {
        ClearPartModels();

        //baseStats = ScriptableObject.CreateInstance<VehicleStats>();

        ApplyPartModel(PartType.Body, vehicleData.bodyId, bodyModelParent);
        
        ApplyPartModel(PartType.Engine, vehicleData.engineId, engineMadeParent);

        for(int i = 0; i < wheelModelParents.Length; i++)
        {
            ApplyPartModel(PartType.Wheel, vehicleData.wheelId, wheelModelParents[i], true);
        }
        for (int i = 0; i < wheelModelParents.Length; i++)
        {
            ApplyPartModel(PartType.Mirror, vehicleData.mirrorId, mirrorModelParents[i], true);
        }
        UpdateKartStats();

    }

    void ApplyPartModel(PartType type, int partId, Transform parent, bool isChild = false)
    {
        VehiclePart part = PartsManager.Instance.GetPart(partId);
        if(part != null && part.partModel != null)
        {
            GameObject model = Instantiate(part.partModel, parent);

            if(!isChild)
            {
                model.transform.localPosition = Vector3.zero;
                model.transform.localRotation = Quaternion.identity;
                model.transform.localScale = Vector3.one;
            }

            if (partModels.ContainsKey(type))
                partModels[type] = model;
            else
                partModels.Add(type, model);

            baseStats = part.ApplyModifiers(baseStats);
        }
    }

    void ClearPartModels()
    {
        foreach(var model in partModels.Values)
        {
            if (model != null)
                Destroy(model);
        }
        partModels.Clear();
    }

    void UpdateKartStats()
    {
        VehicleStats finalStats = baseStats.GetModifiedStats(
            vehicleData.bodyDamage,
            vehicleData.engineDamage,
            vehicleData.wheelDamage,
            vehicleData.mirrorDamage
            );
        kart.baseStats.TopSpeed = finalStats.topSpeed;
        kart.baseStats.Acceleration = finalStats.acceleration;
        kart.baseStats.Steer = finalStats.handling;
        kart.baseStats.Braking = finalStats.braking;
    }

    void ApplyDamageEffects()
    {
        UpdateKartStats();
        UpdateDamageVisuals();
    }

    //손상 시각 효과 업데이트
    void UpdateDamageVisuals()
    {
        for(int i = 0; i < bodyDamageEffects.Length; i++)
        {
            float damageThreshold = (i + 1) * (100f / bodyDamageEffects.Length);
            bodyDamageEffects[i].SetActive(vehicleData.bodyDamage >= damageThreshold);

        }

        if (engineDamageEffect != null)
            engineDamageEffect.SetActive(vehicleData.engineDamage > 50f);

        if (wheelDamageEffect != null)
            wheelDamageEffect.SetActive(vehicleData.wheelDamage > 50f);

        if (mirrorDamageEffect != null)
            mirrorDamageEffect.SetActive(vehicleData.mirrorDamage > 50f);
    }

    //충돌 시 손상 적용
    public void ApplyCollisitionDamage(PartType partType, float damageAmount)
    {
        switch(partType)
        {
            case PartType.Body:
                vehicleData.bodyDamage = Mathf.Min(vehicleData.bodyDamage + damageAmount, 100f);
                break;
            case PartType.Engine:
                vehicleData.engineDamage = Mathf.Min(vehicleData.engineDamage + damageAmount, 100f);
                break;
            case PartType.Wheel:
                vehicleData.wheelDamage = Mathf.Min(vehicleData.wheelDamage + damageAmount, 100f);
                break;
            case PartType.Mirror:
                vehicleData.mirrorDamage = Mathf.Min(vehicleData.mirrorDamage + damageAmount, 100f);
                break;
        }
    }

    //모든 손상 수리
    //public void RepairAllDamage()
    //{
    //    vehicleData.bodyDamage = 0f;
    //    vehicleData.engineDamage = 0f;
    //    vehicleData.wheelDamage = 0f;
    //    vehicleData.mirrorDamage = 0f;
    //}    

    //특정 부품만 수리 
    //public void RepairPart(PartType partType)
    //{
    //    switch (partType)
    //    {
    //        case PartType.Body:
    //            vehicleData.bodyDamage = 0f;
    //            break;
    //        case PartType.Engine:
    //            vehicleData.engineDamage = 0f;
    //            break;
    //        case PartType.Wheel:
    //            vehicleData.wheelDamage = 0f;
    //            break;
    //        case PartType.Mirror:
    //            vehicleData.mirrorDamage = 0f;
    //            break;
    //    }
    //}
}
