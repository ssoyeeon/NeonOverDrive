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

    public ArcadeKart kart;
    public VehicleStats baseStats;
    private Dictionary<PartType, GameObject> partModels = new Dictionary<PartType, GameObject>();

    void Start()
    {
        kart = GetComponent<ArcadeKart>();

        //차량 데이터 설정 (선택된 차량 데이터 가져오기) - > 안되는거같아 이게... .. ..  
        vehicleData = GameManager.Instance.playerData.GetCurrentVehicle();

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

        // baseStats 초기화
        baseStats = ScriptableObject.CreateInstance<VehicleStats>();
        if (baseStats == null)
        {
            Debug.LogWarning("VehicleStats 생성 실패. 기본값을 사용합니다.");
            baseStats = ScriptableObject.CreateInstance<VehicleStats>();
            // 기본값 직접 설정
            baseStats.topSpeed = 180f;
            baseStats.acceleration = 8f;
            baseStats.handling = 5f;
            baseStats.braking = 8f;
        }

        // vehicleData가 null인지 확인
        if (vehicleData == null)
        {
            Debug.LogWarning("vehicleData가 null입니다. 기본 차량 데이터를 사용합니다.");
            vehicleData = new PlayerVehicleData();
        }

        // 부모 Transform 확인
        if (bodyModelParent != null)
            ApplyPartModel(PartType.Body, vehicleData.bodyId, bodyModelParent);
        else
            Debug.LogWarning("bodyModelParent가 할당되지 않았습니다.");

        if (engineMadeParent != null)
            ApplyPartModel(PartType.Engine, vehicleData.engineId, engineMadeParent);
        else
            Debug.LogWarning("engineMadeParent가 할당되지 않았습니다.");

        // 휠 적용
        if (wheelModelParents != null && wheelModelParents.Length > 0)
        {
            for (int i = 0; i < wheelModelParents.Length; i++)
            {
                if (wheelModelParents[i] != null)
                    ApplyPartModel(PartType.Wheel, vehicleData.wheelId, wheelModelParents[i], true);
            }
        }
        else
            Debug.LogWarning("wheelModelParents가 할당되지 않았습니다.");

        // 미러 적용
        if (mirrorModelParents != null && mirrorModelParents.Length > 0)
        {
            for (int i = 0; i < mirrorModelParents.Length; i++)
            {
                if (mirrorModelParents[i] != null)
                    ApplyPartModel(PartType.Mirror, vehicleData.mirrorId, mirrorModelParents[i], true);
            }
        }
        else
            Debug.LogWarning("mirrorModelParents가 할당되지 않았습니다.");

        // 카트 스탯 업데이트
        UpdateKartStats();
    }

    void ApplyPartModel(PartType type, int partId, Transform parent, bool isChild = false)
    {
        VehiclePart part = PartsManager.Instance.GetPart(partId);
        if (part != null && part.partModel != null)
        {
            GameObject model = Instantiate(part.partModel, parent);

            if (!isChild)
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
        foreach (var model in partModels.Values)
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
        for (int i = 0; i < bodyDamageEffects.Length; i++)
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
        switch (partType)
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

    // 추가: 부품별 손상 상태 조회 메서드
    public float GetPartDamage(PartType partType)
    {
        switch (partType)
        {
            case PartType.Body:
                return vehicleData.bodyDamage;
            case PartType.Engine:
                return vehicleData.engineDamage;
            case PartType.Wheel:
                return vehicleData.wheelDamage;
            case PartType.Mirror:
                return vehicleData.mirrorDamage;
            default:
                return 0f;
        }
    }
}