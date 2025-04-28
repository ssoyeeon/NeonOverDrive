using KartGame.KartSystems;
using UnityEngine;
using UnityEngine.UI;

public class VehicleController : MonoBehaviour
{
    [Header("차량 데이터")]
    public VehicleData vehicleData;
    public ArcadeKart kart;

    [Header("충돌 설정")]
    public float minCollisionForce = 5f;    // 충돌로 간주할 최소 힘
    public float maxCollisionForce = 50f;   // 최대 충돌 힘

    [Header("충돌 방향 각도")]
    public float frontAngle = 45f;        // 정면 간주 각도
    public float rearAngle = 45f;         // 후면 간주 각도

    [Header("인게임 손상 상태 (%)")]
    public float bodyDamage = 0f;         // 바디 손상
    public float engineDamage = 0f;       // 엔진 손상
    public float wheelDamage = 0f;        // 휠 손상
    public float mirrorDamage = 0f;       // 미러 손상

    [Header("손상 페널티 설정")]
    public float maxBodyPenalty = 0.3f;   // 바디 손상 최대 페널티 (30%)
    public float maxEnginePenalty = 0.5f; // 엔진 손상 최대 페널티 (50%)
    public float maxWheelPenalty = 0.4f;  // 휠 손상 최대 페널티 (40%)
    public float maxMirrorPenalty = 0.2f; // 미러 손상 최대 페널티 (20%)

    [Header("디버그")]
    public bool showDebugLogs = true;     // 디버그 로그 표시 여부

    [Header("UI 충돌 이미지")]
    public Image front;
    public Image body;
    public Image back;
    public Image leftBackWheel;
    public Image leftFrontWheel;
    public Image rightBackWheel;
    public Image rightFrontWheel;

    private Rigidbody rb;

    void Start()
    {
        // 카트 컴포넌트 할당
        if (kart == null)
            kart = GetComponent<ArcadeKart>();

        // 차량 데이터가 있으면 능력치 적용
        if (vehicleData != null)
        {
            ApplyVehicleStats();
        }

        // Rigidbody 참조
        rb = GetComponent<Rigidbody>();
    }

    // 충돌 감지
    private void OnCollisionEnter(Collision collision)
    {
        if (vehicleData == null || kart == null || rb == null)
            return;

        // 최소 충돌 힘 체크
        if (collision.relativeVelocity.magnitude < minCollisionForce)
            return;

        // 충돌 지점 (첫 번째 접촉점 사용)
        ContactPoint contact = collision.contacts[0];

        // 차량 로컬 공간에서 충돌 방향 계산
        Vector3 localImpact = transform.InverseTransformPoint(contact.point);

        // 충돌 방향 각도 계산 (XZ 평면에서)
        float angle = Mathf.Atan2(localImpact.x, localImpact.z) * Mathf.Rad2Deg;
        float absAngle = Mathf.Abs(angle); // 절대값 (0-180도)

        // 충돌 힘 계산
        float collisionForce = Mathf.Min(collision.relativeVelocity.magnitude, maxCollisionForce);
        float damagePercent = (collisionForce - minCollisionForce) / (maxCollisionForce - minCollisionForce) * 70f; // 최대 60% 손상

        string impactArea = "기타";

        // 방향에 따른 충돌 부위 결정 및 데미지 적용
        if (absAngle < frontAngle) // 전방 충돌
        {
            engineDamage = Mathf.Min(engineDamage + damagePercent * 0.7f, 100f);
            bodyDamage = Mathf.Min(bodyDamage + damagePercent * 0.3f, 100f);
            impactArea = "전방";
            if(damagePercent > 30 && front.color != Color.red)
            {
                front.color = Color.yellow;
            }
            if (damagePercent > 60 && front.color == Color.yellow)
            {
                front.color = Color.red;
            }
        }
        else if (absAngle > 180 - rearAngle) // 후방 충돌
        {
            bodyDamage = Mathf.Min(bodyDamage + damagePercent, 100f);
            impactArea = "후방";
            if (damagePercent > 30 && back.color != Color.red)
            {
                back.color = Color.yellow;
            }
            if (damagePercent > 60 && back.color == Color.yellow)
            {
                back.color = Color.red;
            }
        }
        else // 측면 충돌
        {
            bodyDamage = Mathf.Min(bodyDamage + damagePercent * 0.4f, 100f);
            wheelDamage = Mathf.Min(wheelDamage + damagePercent * 0.4f, 100f);
            mirrorDamage = Mathf.Min(mirrorDamage + damagePercent * 0.2f, 100f);

            // 왼쪽/오른쪽 구분
            if (localImpact.x < 0)
            {
                impactArea = "왼쪽";
                if (damagePercent > 30 && leftBackWheel.color != Color.red)
                {
                    body.color = Color.yellow;
                    leftBackWheel.color = Color.yellow;
                    leftFrontWheel.color = Color.yellow;
                }
                if (damagePercent > 60 && leftBackWheel.color == Color.yellow)
                {
                    body.color = Color.red;
                    leftBackWheel.color = Color.red;
                    leftFrontWheel.color = Color.red;
                }
            }
            else
            {
                impactArea = "오른쪽";
                if (damagePercent > 30 && rightBackWheel.color != Color.red)
                {
                    rightBackWheel.color = Color.yellow;
                    rightFrontWheel.color = Color.yellow;
                }
                if (damagePercent > 60 && leftBackWheel.color == Color.yellow)
                {
                    leftBackWheel.color = Color.red;
                    leftFrontWheel.color = Color.red;
                }
            }
        }

        // 디버그 정보 로그
        if (showDebugLogs)
        {
            Debug.Log($"[충돌 감지] 부위: {impactArea}, 각도: {angle:F1}°, 힘: {collisionForce:F1}, 데미지: {damagePercent:F1}%");

            Debug.Log($"[차량 상태] 바디: {bodyDamage:F1}%, " +
                     $"엔진: {engineDamage:F1}%, " +
                     $"휠: {wheelDamage:F1}%, " +
                     $"미러: {mirrorDamage:F1}%");
        }

        // 성능 업데이트
        ApplyVehicleStats();
    }

    // 차량 능력치 적용
    public void ApplyVehicleStats()
    {
        if (vehicleData == null || kart == null)
            return;

        // 부품 레벨을 고려한 기본 능력치 계산
        float baseSpeed = CalculateSpeed();
        float baseAccel = CalculateAcceleration();
        float baseHandling = CalculateHandling();
        float baseBraking = CalculateBraking();

        // 손상에 따른 페널티 계산
        float bodyPenalty = (bodyDamage / 100f) * maxBodyPenalty;
        float enginePenalty = (engineDamage / 100f) * maxEnginePenalty;
        float wheelPenalty = (wheelDamage / 100f) * maxWheelPenalty;
        float mirrorPenalty = (mirrorDamage / 100f) * maxMirrorPenalty;

        // 최종 능력치 적용 (각 부품 손상이 영향을 미치는 능력치가 다름)
        kart.baseStats.TopSpeed = baseSpeed * (1f - bodyPenalty) * (1f - enginePenalty);
        kart.baseStats.Acceleration = baseAccel * (1f - enginePenalty);
        kart.baseStats.Steer = baseHandling * (1f - wheelPenalty) * (1f - mirrorPenalty);
        kart.baseStats.Braking = baseBraking * (1f - wheelPenalty);

        if (showDebugLogs)
        {
            Debug.Log($"차량 성능: 속도={kart.baseStats.TopSpeed:F1}, " +
                     $"가속={kart.baseStats.Acceleration:F1}, " +
                     $"핸들링={kart.baseStats.Steer:F1}, " +
                     $"제동={kart.baseStats.Braking:F1}");
        }
    }

    // 부품 레벨에 따른 속도 계산
    private float CalculateSpeed()
    {
        float bodyBonus = vehicleData.speedPerBodyLevel * (vehicleData.bodyLevel - 1);
        float engineBonus = vehicleData.speedPerEngineLevel * (vehicleData.engineLevel - 1);
        return vehicleData.baseSpeed + bodyBonus + engineBonus;
    }

    // 부품 레벨에 따른 가속도 계산
    private float CalculateAcceleration()
    {
        float engineBonus = vehicleData.accelPerEngineLevel * (vehicleData.engineLevel - 1);
        return vehicleData.baseAcceleration + engineBonus;
    }

    // 부품 레벨에 따른 핸들링 계산
    private float CalculateHandling()
    {
        float wheelBonus = vehicleData.handlingPerWheelLevel * (vehicleData.wheelLevel - 1);
        float mirrorBonus = vehicleData.handlingPerMirrorLevel * (vehicleData.mirrorLevel - 1);
        return vehicleData.baseHandling + wheelBonus + mirrorBonus;
    }

    // 부품 레벨에 따른 제동력 계산
    private float CalculateBraking()
    {
        float wheelBonus = vehicleData.brakingPerWheelLevel * (vehicleData.wheelLevel - 1);
        return vehicleData.baseBraking + wheelBonus;
    }

    // 부품 레벨 증가
    public void UpgradePart(int partIndex)
    {
        if (vehicleData == null)
            return;

        // 부품 인덱스에 따라 레벨 증가
        switch (partIndex)
        {
            case 0: // 바디
                vehicleData.bodyLevel++;
                break;
            case 1: // 엔진
                vehicleData.engineLevel++;
                break;
            case 2: // 휠
                vehicleData.wheelLevel++;
                break;
            case 3: // 미러
                vehicleData.mirrorLevel++;
                break;
        }

        // 능력치 다시 적용
        ApplyVehicleStats();
    }

    // 손상 초기화 (필요시 호출)
    public void ResetDamage()
    {
        bodyDamage = 0f;
        engineDamage = 0f;
        wheelDamage = 0f;
        mirrorDamage = 0f;

        ApplyVehicleStats();
    }
}