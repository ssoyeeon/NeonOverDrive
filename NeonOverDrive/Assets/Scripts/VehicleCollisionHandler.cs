using static VehiclePart;
using UnityEngine;

public class VehicleCollisionHandler : MonoBehaviour
{
    [Header("충돌 설정")]
    public float minCollisitionForce = 5f;
    public float maxCollisitionForce = 50f;
    public float damageMultiplier = 1f;

    [Header("충돌 방향 각도")]
    public float frontAngle = 45f;    // 정면 간주 각도
    public float rearAngle = 45f;     // 후면 간주 각도
    private float roofHeightThreshold = 1.5f; // 지붕 충돌 감지 높이 임계값

    [Header("디버그")]
    public bool showDebugLogs = true; // 디버그 로그 표시 여부

    private VehicleController vehicleController;
    private Rigidbody rb;

    void Start()
    {
        vehicleController = GetComponent<VehicleController>();
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude < minCollisitionForce)
            return;

        // 충돌 지점 (첫 번째 접촉점 사용)
        ContactPoint contact = collision.contacts[0];

        // 차량 로컬 공간에서 충돌 방향 계산
        Vector3 localImpact = transform.InverseTransformPoint(contact.point);

        // 충돌 방향 각도 계산 (XZ 평면에서)
        float angle = Mathf.Atan2(localImpact.x, localImpact.z) * Mathf.Rad2Deg;
        float absAngle = Mathf.Abs(angle); // 절대값 (0-180도)

        // 지붕 충돌 체크 (Y 값이 특정 임계값 이상)
        bool isRoofImpact = localImpact.y > roofHeightThreshold;

        float collisionForce = Mathf.Min(collision.relativeVelocity.magnitude, maxCollisitionForce);
        float damageAmount = ((collisionForce - minCollisitionForce) / (maxCollisitionForce - minCollisitionForce)) * 100f * damageMultiplier;

        string impactArea = "기타";

        // 방향에 따른 충돌 부위 결정
        if (isRoofImpact)
        {
            vehicleController.ApplyCollisitionDamage(PartType.Body, damageAmount * 1.5f);
            impactArea = "지붕";
        }
        else if (absAngle < frontAngle) // 전방 충돌
        {
            vehicleController.ApplyCollisitionDamage(PartType.Engine, damageAmount * 0.7f);
            vehicleController.ApplyCollisitionDamage(PartType.Body, damageAmount * 0.3f);
            impactArea = "전방";
        }
        else if (absAngle > 180 - rearAngle) // 후방 충돌
        {
            vehicleController.ApplyCollisitionDamage(PartType.Body, damageAmount);
            impactArea = "후방";
        }
        else // 측면 충돌
        {
            vehicleController.ApplyCollisitionDamage(PartType.Body, damageAmount * 0.5f);
            vehicleController.ApplyCollisitionDamage(PartType.Wheel, damageAmount * 0.3f);
            vehicleController.ApplyCollisitionDamage(PartType.Mirror, damageAmount * 0.2f);

            // 왼쪽/오른쪽 구분
            if (localImpact.x < 0)
                impactArea = "왼쪽";
            else
                impactArea = "오른쪽";
        }

        // 디버그 정보 로그
        if (showDebugLogs)
        {
            Debug.Log($"[충돌 감지] 부위: {impactArea}, 각도: {angle:F1}°, 위치: {localImpact}, 힘: {collisionForce:F1}, 데미지: {damageAmount:F1}%");
        }
    }
}