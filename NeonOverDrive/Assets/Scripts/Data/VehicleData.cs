using UnityEngine;

// ���� �⺻ ������
[CreateAssetMenu(fileName = "NewVehicleData", menuName = "Vehicle System/Vehicle Data")]
public class VehicleData : ScriptableObject
{
    [Header("���� ����")]
    public string vehicleName;      // ���� �̸�
    public int vehicleId;           // ���� ID (1~9)

    [Header("�⺻ �ɷ�ġ")]
    public float baseSpeed = 25f;         // �⺻ �ӵ�
    public float baseAcceleration = 7f;    // �⺻ ���ӵ�
    public float baseHandling = 5f;        // �⺻ �ڵ鸵
    public float baseBraking = 7f;         // �⺻ ������

    [Header("��ǰ ����")]
    public int bodyLevel = 1;       // �ٵ� ����
    public int engineLevel = 1;     // ���� ����
    public int wheelLevel = 1;      // �� ����
    public int mirrorLevel = 1;     // �̷� ����

    [Header("������ ���� ������")]
    public float speedPerBodyLevel = 0.5f;      // �ٵ� ������ �ӵ� ������
    public float speedPerEngineLevel = 1.0f;    // ���� ������ �ӵ� ������
    public float accelPerEngineLevel = 0.3f;    // ���� ������ ���ӵ� ������
    public float handlingPerWheelLevel = 0.25f; // �� ������ �ڵ鸵 ������
    public float handlingPerMirrorLevel = 0.1f; // �̷� ������ �ڵ鸵 ������
    public float brakingPerWheelLevel = 0.2f;   // �� ������ ������ ������

    // ��ǰ ������ ����� ���� �ӵ� ���
    public float GetFinalSpeed()
    {
        float bodyBonus = speedPerBodyLevel * (bodyLevel - 1);
        float engineBonus = speedPerEngineLevel * (engineLevel - 1);
        return baseSpeed + bodyBonus + engineBonus;
    }

    // ��ǰ ������ ����� ���� ���ӵ� ���
    public float GetFinalAcceleration()
    {
        float engineBonus = accelPerEngineLevel * (engineLevel - 1);
        return baseAcceleration + engineBonus;
    }

    // ��ǰ ������ ����� ���� �ڵ鸵 ���
    public float GetFinalHandling()
    {
        float wheelBonus = handlingPerWheelLevel * (wheelLevel - 1);
        float mirrorBonus = handlingPerMirrorLevel * (mirrorLevel - 1);
        return baseHandling + wheelBonus + mirrorBonus;
    }

    // ��ǰ ������ ����� ���� ������ ���
    public float GetFinalBraking()
    {
        float wheelBonus = brakingPerWheelLevel * (wheelLevel - 1);
        return baseBraking + wheelBonus;
    }
}