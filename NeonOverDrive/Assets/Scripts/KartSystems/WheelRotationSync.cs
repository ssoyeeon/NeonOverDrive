using KartGame.KartSystems;
using UnityEngine;

public class WheelRotationSync : MonoBehaviour
{
    [Header("�� Transform")]
    public Transform frontLeftWheelVisual;
    public Transform frontRightWheelVisual;
    public Transform rearLeftWheelVisual;
    public Transform rearRightWheelVisual;

    [Header("����")]
    public float wheelRadius = 0.3f;  // �� ������ (���� �� ũ�⿡ �°� ����)

    private ArcadeKart kart;
    private float wheelRotation = 0f;  // ���� �� ȸ����

    void Start()
    {
        kart = GetComponent<ArcadeKart>();

        if (kart == null)
        {
            Debug.LogError("ArcadeKart ������Ʈ�� ã�� �� �����ϴ�!");
            return;
        }

        // ���� ������� ���� ��� �ڵ����� ã��
        if (frontLeftWheelVisual == null || frontRightWheelVisual == null ||
            rearLeftWheelVisual == null || rearRightWheelVisual == null)
        {
            Debug.LogWarning("�Ϻ� ���� ������� �ʾҽ��ϴ�. m_VisualWheels���� �ڵ����� �����ɴϴ�.");
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

        // ���� �ӵ� ��� (����/���� ���)
        float speed = Vector3.Dot(kart.Rigidbody.velocity, transform.forward);

        // �� ȸ�� ���� ��� (�ӵ� * �ð� / ������)
        float rotationDelta = (speed * Time.deltaTime / wheelRadius) * Mathf.Rad2Deg;
        wheelRotation += rotationDelta;

        // �� ȸ�� ���� (Z�� ȸ��)
        ApplyWheelRotation();
    }

    void ApplyWheelRotation()
    {
        // ��� �ٿ� Z�� ȸ�� ����
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
        // ���� Y�� ȸ���� �����ϰ� Z�� ȸ���� ������Ʈ
        Vector3 currentEuler = wheel.localEulerAngles;
        wheel.localEulerAngles = new Vector3(currentEuler.x, currentEuler.y, wheelRotation);
    }

    // �� �������� �ڵ����� ����ϴ� �޼��� (�ɼ�)
    public void AutoCalculateWheelRadius()
    {
        if (kart != null && kart.FrontLeftWheel != null)
        {
            wheelRadius = kart.FrontLeftWheel.radius;
            Debug.Log($"�� ������ �ڵ� ����: {wheelRadius}");
        }
    }
}