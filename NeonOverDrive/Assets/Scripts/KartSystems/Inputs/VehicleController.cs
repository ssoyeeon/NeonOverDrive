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
    public float maxCollisionForce = 70f;   // �ִ� �浹 ��

    [Header("�浹 ���� ����")]
    public float frontAngle = 45f;        // ���� ���� ����
    public float rearAngle = 45f;         // �ĸ� ���� ����

    [Header("�ΰ��� �ջ� ���� (%)")]
    public float bodyDamage = 0f;         // �ٵ� �ջ�
    public float engineDamage = 0f;       // ���� �ջ�
    public float wheelDamage = 0f;        // �� �ջ�
    public float mirrorDamage = 0f;       // �̷� �ջ�

    [Header("�ջ� ���Ƽ ����")]
    public float maxBodyPenalty = 0.5f;   // �ٵ� �ջ� �ִ� ���Ƽ (50%)
    public float maxEnginePenalty = 0.7f; // ���� �ջ� �ִ� ���Ƽ (70%)
    public float maxWheelPenalty = 0.6f;  // �� �ջ� �ִ� ���Ƽ (60%)
    public float maxMirrorPenalty = 0.4f; // �̷� �ջ� �ִ� ���Ƽ (40%)

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
        // īƮ ��Ʈ�ѷ� ã��
        if (kart == null)
            kart = GetComponent<ArcadeKart>();

        // ���� �����Ͱ� ������ �ɷ�ġ ����
        if (vehicleData != null)
        {
            ApplyVehicleStats();
        }

        // Rigidbody ����
        rb = GetComponent<Rigidbody>();

        // GameManager�� �����Ͽ� ���׷��̵�� ������ ����
        if (GameManager.Instance != null)
        {
            GameManager.Instance.InitializeGameScene(this);
            Debug.Log("GameManager�� ���� ���� �Ϸ�");
        }
        else
        {
            Debug.LogWarning("GameManager.Instance�� null�Դϴ�!");
        }
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

            // ���� ������ �������� ���� ���� ����
            if (front != null)
                front.color = GetUnifiedDamageColor(engineDamage);
        }
        else if (absAngle > 180 - rearAngle) // �Ĺ� �浹
        {
            bodyDamage = Mathf.Min(bodyDamage + damagePercent, 100f);
            impactArea = "�Ĺ�";

            // ��ü ������ �������� �Ĺ� ���� ����
            if (back != null)
                back.color = GetUnifiedDamageColor(bodyDamage);
        }
        else // ���� �浹
        {
            bodyDamage = Mathf.Min(bodyDamage + damagePercent * 0.5f, 100f);
            wheelDamage = Mathf.Min(wheelDamage + damagePercent * 0.5f, 100f);
            mirrorDamage = Mathf.Min(mirrorDamage + damagePercent * 0.3f, 100f);

            // ����/������ �����Ͽ� ���� ����
            if (localImpact.x < 0) // ����
            {
                impactArea = "����";
                if (body != null)
                    body.color = GetUnifiedDamageColor(bodyDamage);
                if (leftBackWheel != null)
                    leftBackWheel.color = GetUnifiedDamageColor(wheelDamage);
                if (leftFrontWheel != null)
                    leftFrontWheel.color = GetUnifiedDamageColor(wheelDamage);
            }
            else // ������
            {
                impactArea = "������";
                if (body != null)
                    body.color = GetUnifiedDamageColor(bodyDamage);
                if (rightBackWheel != null)
                    rightBackWheel.color = GetUnifiedDamageColor(wheelDamage);
                if (rightFrontWheel != null)
                    rightFrontWheel.color = GetUnifiedDamageColor(wheelDamage);
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

        // GameManager�� ���� ������ ���� ����
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateVehicleDamage(bodyDamage, engineDamage, wheelDamage, mirrorDamage);
        }
    }

    // VehicleController.cs�� �� �޼��� �߰�:
    private Color GetUnifiedDamageColor(float damagePercent)
    {
        if (damagePercent >= 60f)
            return Color.red;
        else if (damagePercent >= 30f)
            return Color.yellow;
        else
            return Color.white;
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

        // �ջ� �� ArcadeKart�� �ε巯�� ���� �Է°��� ��� ����
        if (bodyDamage > 0 || engineDamage > 0 || wheelDamage > 0 || mirrorDamage > 0)
        {
            // ArcadeKart�� m_CurrentAccelInput�� ���÷������� �����Ͽ� ����
            var field = kart.GetType().GetField("m_CurrentAccelInput",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(kart, 0f);
            }

            // Rigidbody �ӵ��� �ջ� �°� ��� ����
            if (kart.Rigidbody != null)
            {
                Vector3 currentVelocity = kart.Rigidbody.velocity;
                float maxAllowedSpeed = kart.baseStats.TopSpeed;

                if (currentVelocity.magnitude > maxAllowedSpeed)
                {
                    currentVelocity = currentVelocity.normalized * maxAllowedSpeed;
                    kart.Rigidbody.velocity = currentVelocity;
                }
            }
        }

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