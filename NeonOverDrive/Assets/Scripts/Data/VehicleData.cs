using UnityEngine;

// 차량 기본 데이터
[CreateAssetMenu(fileName = "NewVehicleData", menuName = "Vehicle System/Vehicle Data")]
public class VehicleData : ScriptableObject
{
    [Header("차량 정보")]
    public string vehicleName;      // 차량 이름
    public int vehicleId;           // 차량 ID (1~9)

    [Header("기본 능력치")]
    public float baseSpeed = 25f;         // 기본 속도
    public float baseAcceleration = 7f;    // 기본 가속도
    public float baseHandling = 5f;        // 기본 핸들링
    public float baseBraking = 7f;         // 기본 제동력

    [Header("부품 레벨")]
    public int bodyLevel = 1;       // 바디 레벨
    public int engineLevel = 1;     // 엔진 레벨
    public int wheelLevel = 1;      // 휠 레벨
    public int mirrorLevel = 1;     // 미러 레벨

    [Header("레벨당 성능 증가량")]
    public float speedPerBodyLevel = 0.5f;      // 바디 레벨당 속도 증가량
    public float speedPerEngineLevel = 1.0f;    // 엔진 레벨당 속도 증가량
    public float accelPerEngineLevel = 0.3f;    // 엔진 레벨당 가속도 증가량
    public float handlingPerWheelLevel = 0.25f; // 휠 레벨당 핸들링 증가량
    public float handlingPerMirrorLevel = 0.1f; // 미러 레벨당 핸들링 증가량
    public float brakingPerWheelLevel = 0.2f;   // 휠 레벨당 제동력 증가량

    // 부품 레벨을 고려한 최종 속도 계산
    public float GetFinalSpeed()
    {
        float bodyBonus = speedPerBodyLevel * (bodyLevel - 1);
        float engineBonus = speedPerEngineLevel * (engineLevel - 1);
        return baseSpeed + bodyBonus + engineBonus;
    }

    // 부품 레벨을 고려한 최종 가속도 계산
    public float GetFinalAcceleration()
    {
        float engineBonus = accelPerEngineLevel * (engineLevel - 1);
        return baseAcceleration + engineBonus;
    }

    // 부품 레벨을 고려한 최종 핸들링 계산
    public float GetFinalHandling()
    {
        float wheelBonus = handlingPerWheelLevel * (wheelLevel - 1);
        float mirrorBonus = handlingPerMirrorLevel * (mirrorLevel - 1);
        return baseHandling + wheelBonus + mirrorBonus;
    }

    // 부품 레벨을 고려한 최종 제동력 계산
    public float GetFinalBraking()
    {
        float wheelBonus = brakingPerWheelLevel * (wheelLevel - 1);
        return baseBraking + wheelBonus;
    }
}