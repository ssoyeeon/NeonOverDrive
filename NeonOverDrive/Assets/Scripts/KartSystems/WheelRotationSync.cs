using KartGame.KartSystems;
using UnityEngine;

public class WheelRotationSync : MonoBehaviour
{
    [Header("휠 Transform")]
    public Transform frontLeftWheelVisual;
    public Transform frontRightWheelVisual;
    public Transform rearLeftWheelVisual;
    public Transform rearRightWheelVisual;

    [Header("설정")]
    public float wheelRadius = 0.3f;  // 휠 반지름 (실제 휠 크기에 맞게 조정)

    private ArcadeKart kart;
    private float wheelRotation = 0f;  // 현재 휠 회전값

    void Start()
    {
        kart = GetComponent<ArcadeKart>();

        if (kart == null)
        {
            Debug.LogError("ArcadeKart 컴포넌트를 찾을 수 없습니다!");
            return;
        }

        // 휠이 연결되지 않은 경우 자동으로 찾기
        if (frontLeftWheelVisual == null || frontRightWheelVisual == null ||
            rearLeftWheelVisual == null || rearRightWheelVisual == null)
        {
            Debug.LogWarning("일부 휠이 연결되지 않았습니다. m_VisualWheels에서 자동으로 가져옵니다.");
            TryAutoAssignWheels();
        }
    }

    void TryAutoAssignWheels()
    {
        if (kart.m_VisualWheels != null && kart.m_VisualWheels.Count >= 4)
        {
            frontLeftWheelVisual = kart.m_VisualWheels[0].transform;
            frontRightWheelVisual = kart.m_VisualWheels[1].transform;
            rearLeftWheelVisual = kart.m_VisualWheels[2].transform;
            rearRightWheelVisual = kart.m_VisualWheels[3].transform;
        }
    }

    void Update()
    {
        if (kart == null) return;

        // 현재 속도 계산 (전진/후진 고려)
        float speed = Vector3.Dot(kart.Rigidbody.velocity, transform.forward);

        // 휠 회전 각도 계산 (속도 * 시간 / 반지름)
        float rotationDelta = (speed * Time.deltaTime / wheelRadius) * Mathf.Rad2Deg;
        wheelRotation += rotationDelta;

        // 휠 회전 적용 (Z축 회전)
        ApplyWheelRotation();
    }

    void ApplyWheelRotation()
    {
        // 모든 휠에 Z축 회전 적용
        if (frontLeftWheelVisual != null)
            RotateWheel(frontLeftWheelVisual);

        if (frontRightWheelVisual != null)
            RotateWheel(frontRightWheelVisual);

        if (rearLeftWheelVisual != null)
            RotateWheel(rearLeftWheelVisual);

        if (rearRightWheelVisual != null)
            RotateWheel(rearRightWheelVisual);
    }

    void RotateWheel(Transform wheel)
    {
        // 기존 Y축 회전은 유지하고 Z축 회전만 업데이트
        Vector3 currentEuler = wheel.localEulerAngles;
        wheel.localEulerAngles = new Vector3(currentEuler.x, currentEuler.y, wheelRotation);
    }

    // 휠 반지름을 자동으로 계산하는 메서드 (옵션)
    public void AutoCalculateWheelRadius()
    {
        if (kart != null && kart.FrontLeftWheel != null)
        {
            wheelRadius = kart.FrontLeftWheel.radius;
            Debug.Log($"휠 반지름 자동 설정: {wheelRadius}");
        }
    }
}