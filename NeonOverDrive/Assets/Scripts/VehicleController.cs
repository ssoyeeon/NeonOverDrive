using KartGame.KartSystems;
using UnityEngine;
using UnityEngine.UI;

public class VehicleController : MonoBehaviour
{
    [Header("���� ������")]
    public VehicleData vehicleData;
    public ArcadeKart kart;

    [Header("�浹 ����")]
    public float minCollisionForce = 5f;    // �浹�� ������ �ּ� ��
    public float maxCollisionForce = 50f;   // �ִ� �浹 ��

    [Header("�浹 ���� ����")]
    public float frontAngle = 45f;        // ���� ���� ����
    public float rearAngle = 45f;         // �ĸ� ���� ����

    [Header("�ΰ��� �ջ� ���� (%)")]
    public float bodyDamage = 0f;         // �ٵ� �ջ�
    public float engineDamage = 0f;       // ���� �ջ�
    public float wheelDamage = 0f;        // �� �ջ�
    public float mirrorDamage = 0f;       // �̷� �ջ�

    [Header("�ջ� ���Ƽ ����")]
    public float maxBodyPenalty = 0.3f;   // �ٵ� �ջ� �ִ� ���Ƽ (30%)
    public float maxEnginePenalty = 0.5f; // ���� �ջ� �ִ� ���Ƽ (50%)
    public float maxWheelPenalty = 0.4f;  // �� �ջ� �ִ� ���Ƽ (40%)
    public float maxMirrorPenalty = 0.2f; // �̷� �ջ� �ִ� ���Ƽ (20%)

    [Header("�����")]
    public bool showDebugLogs = true;     // ����� �α� ǥ�� ����

    [Header("UI �浹 �̹���")]
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
        // īƮ ������Ʈ �Ҵ�
        if (kart == null)
            kart = GetComponent<ArcadeKart>();

        // ���� �����Ͱ� ������ �ɷ�ġ ����
        if (vehicleData != null)
        {
            ApplyVehicleStats();
        }

        // Rigidbody ����
        rb = GetComponent<Rigidbody>();
    }

    // �浹 ����
    private void OnCollisionEnter(Collision collision)
    {
        if (vehicleData == null || kart == null || rb == null)
            return;

        // �ּ� �浹 �� üũ
        if (collision.relativeVelocity.magnitude < minCollisionForce)
            return;

        // �浹 ���� (ù ��° ������ ���)
        ContactPoint contact = collision.contacts[0];

        // ���� ���� �������� �浹 ���� ���
        Vector3 localImpact = transform.InverseTransformPoint(contact.point);

        // �浹 ���� ���� ��� (XZ ��鿡��)
        float angle = Mathf.Atan2(localImpact.x, localImpact.z) * Mathf.Rad2Deg;
        float absAngle = Mathf.Abs(angle); // ���밪 (0-180��)

        // �浹 �� ���
        float collisionForce = Mathf.Min(collision.relativeVelocity.magnitude, maxCollisionForce);
        float damagePercent = (collisionForce - minCollisionForce) / (maxCollisionForce - minCollisionForce) * 70f; // �ִ� 60% �ջ�

        string impactArea = "��Ÿ";

        // ���⿡ ���� �浹 ���� ���� �� ������ ����
        if (absAngle < frontAngle) // ���� �浹
        {
            engineDamage = Mathf.Min(engineDamage + damagePercent * 0.7f, 100f);
            bodyDamage = Mathf.Min(bodyDamage + damagePercent * 0.3f, 100f);
            impactArea = "����";
            if(damagePercent > 30 && front.color != Color.red)
            {
                front.color = Color.yellow;
            }
            if (damagePercent > 60 && front.color == Color.yellow)
            {
                front.color = Color.red;
            }
        }
        else if (absAngle > 180 - rearAngle) // �Ĺ� �浹
        {
            bodyDamage = Mathf.Min(bodyDamage + damagePercent, 100f);
            impactArea = "�Ĺ�";
            if (damagePercent > 30 && back.color != Color.red)
            {
                back.color = Color.yellow;
            }
            if (damagePercent > 60 && back.color == Color.yellow)
            {
                back.color = Color.red;
            }
        }
        else // ���� �浹
        {
            bodyDamage = Mathf.Min(bodyDamage + damagePercent * 0.4f, 100f);
            wheelDamage = Mathf.Min(wheelDamage + damagePercent * 0.4f, 100f);
            mirrorDamage = Mathf.Min(mirrorDamage + damagePercent * 0.2f, 100f);

            // ����/������ ����
            if (localImpact.x < 0)
            {
                impactArea = "����";
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
                impactArea = "������";
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

        // ����� ���� �α�
        if (showDebugLogs)
        {
            Debug.Log($"[�浹 ����] ����: {impactArea}, ����: {angle:F1}��, ��: {collisionForce:F1}, ������: {damagePercent:F1}%");

            Debug.Log($"[���� ����] �ٵ�: {bodyDamage:F1}%, " +
                     $"����: {engineDamage:F1}%, " +
                     $"��: {wheelDamage:F1}%, " +
                     $"�̷�: {mirrorDamage:F1}%");
        }

        // ���� ������Ʈ
        ApplyVehicleStats();
    }

    // ���� �ɷ�ġ ����
    public void ApplyVehicleStats()
    {
        if (vehicleData == null || kart == null)
            return;

        // ��ǰ ������ ����� �⺻ �ɷ�ġ ���
        float baseSpeed = CalculateSpeed();
        float baseAccel = CalculateAcceleration();
        float baseHandling = CalculateHandling();
        float baseBraking = CalculateBraking();

        // �ջ� ���� ���Ƽ ���
        float bodyPenalty = (bodyDamage / 100f) * maxBodyPenalty;
        float enginePenalty = (engineDamage / 100f) * maxEnginePenalty;
        float wheelPenalty = (wheelDamage / 100f) * maxWheelPenalty;
        float mirrorPenalty = (mirrorDamage / 100f) * maxMirrorPenalty;

        // ���� �ɷ�ġ ���� (�� ��ǰ �ջ��� ������ ��ġ�� �ɷ�ġ�� �ٸ�)
        kart.baseStats.TopSpeed = baseSpeed * (1f - bodyPenalty) * (1f - enginePenalty);
        kart.baseStats.Acceleration = baseAccel * (1f - enginePenalty);
        kart.baseStats.Steer = baseHandling * (1f - wheelPenalty) * (1f - mirrorPenalty);
        kart.baseStats.Braking = baseBraking * (1f - wheelPenalty);

        if (showDebugLogs)
        {
            Debug.Log($"���� ����: �ӵ�={kart.baseStats.TopSpeed:F1}, " +
                     $"����={kart.baseStats.Acceleration:F1}, " +
                     $"�ڵ鸵={kart.baseStats.Steer:F1}, " +
                     $"����={kart.baseStats.Braking:F1}");
        }
    }

    // ��ǰ ������ ���� �ӵ� ���
    private float CalculateSpeed()
    {
        float bodyBonus = vehicleData.speedPerBodyLevel * (vehicleData.bodyLevel - 1);
        float engineBonus = vehicleData.speedPerEngineLevel * (vehicleData.engineLevel - 1);
        return vehicleData.baseSpeed + bodyBonus + engineBonus;
    }

    // ��ǰ ������ ���� ���ӵ� ���
    private float CalculateAcceleration()
    {
        float engineBonus = vehicleData.accelPerEngineLevel * (vehicleData.engineLevel - 1);
        return vehicleData.baseAcceleration + engineBonus;
    }

    // ��ǰ ������ ���� �ڵ鸵 ���
    private float CalculateHandling()
    {
        float wheelBonus = vehicleData.handlingPerWheelLevel * (vehicleData.wheelLevel - 1);
        float mirrorBonus = vehicleData.handlingPerMirrorLevel * (vehicleData.mirrorLevel - 1);
        return vehicleData.baseHandling + wheelBonus + mirrorBonus;
    }

    // ��ǰ ������ ���� ������ ���
    private float CalculateBraking()
    {
        float wheelBonus = vehicleData.brakingPerWheelLevel * (vehicleData.wheelLevel - 1);
        return vehicleData.baseBraking + wheelBonus;
    }

    // ��ǰ ���� ����
    public void UpgradePart(int partIndex)
    {
        if (vehicleData == null)
            return;

        // ��ǰ �ε����� ���� ���� ����
        switch (partIndex)
        {
            case 0: // �ٵ�
                vehicleData.bodyLevel++;
                break;
            case 1: // ����
                vehicleData.engineLevel++;
                break;
            case 2: // ��
                vehicleData.wheelLevel++;
                break;
            case 3: // �̷�
                vehicleData.mirrorLevel++;
                break;
        }

        // �ɷ�ġ �ٽ� ����
        ApplyVehicleStats();
    }

    // �ջ� �ʱ�ȭ (�ʿ�� ȣ��)
    public void ResetDamage()
    {
        bodyDamage = 0f;
        engineDamage = 0f;
        wheelDamage = 0f;
        mirrorDamage = 0f;

        ApplyVehicleStats();
    }
}